/*
 * Copyright (C) 2023 lchfj.cn
 * 
 * This file is part of MemcachedTiny.
 * 
 * MemcachedTiny is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
 * 
 * MemcachedTiny is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public License along with MemcachedTiny. If not, see <https://www.gnu.org/licenses/>.
 */

using MemcachedTiny.Data;
using System.Collections.Concurrent;

namespace MemcachedTiny.Node
{
    /// <summary>
    /// Memcached 节点
    /// </summary>
    public partial class Node : INode
    {

        /// <inheritdoc/>
        public virtual bool Avaliable => ConnectPool?.Avaliable ?? false;

        /// <summary>
        /// 要用到的连接池
        /// </summary>
        protected virtual IConnectionPool ConnectPool { get; }

        /// <summary>
        /// 任务队列
        /// </summary>
        protected virtual ConcurrentQueue<QueueTaskInfo> TaskQueue { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="connectString">Memcached连接地址</param>
        public Node(string connectString)
        {
            var endPoint = CreatEndPoint(connectString);
            if (endPoint is null)
                return;

            ConnectPool = CreatConnectionPool(endPoint);
            TaskQueue = new ConcurrentQueue<QueueTaskInfo>();
        }

        /// <summary>
        /// 创建连接池
        /// </summary>
        protected virtual IConnectionPool CreatConnectionPool(IPEndPoint endPoint)
        {
            return new TCPConnectPool(endPoint);
        }

        /// <summary>
        /// 创建连接点
        /// </summary>
        protected virtual IPEndPoint CreatEndPoint(string connectString)
        {
            if (string.IsNullOrWhiteSpace(connectString))
                return null;

            var array = connectString.Split(':');
            if (!IPAddress.TryParse(array[0], out var ipaddress))
                throw new ArgumentException(connectString);

            if (!ushort.TryParse(array[1], out var port))
                throw new ArgumentException(connectString);

            return new IPEndPoint(ipaddress, port);
        }

        /// <inheritdoc/>
        public virtual TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            var tInfo = CreatTask<TC, TC>(request, CancellationToken.None, out var task);

            var connect = ConnectPool.GetOne();
            try
            {

                if (connect is null)
                {
                    TaskQueue.Enqueue(tInfo);
                }
                else
                {
                    tInfo.Connect = connect;
                    task.RunSynchronously();
                }

                task.Wait();
                return task.Result;
            }
            catch
            {
                throw;
            }
            finally
            {
                CheckAsync(connect);
            }
        }

        /// <inheritdoc/>
        public Task<TI> ExecuteAsync<TI, TC>(IRequest request, CancellationToken cancellation) where TC : IResponseReader, TI, new()
        {
            var tInfo = CreatTask<TI, TC>(request, cancellation, out var task);

            TaskQueue.Enqueue(tInfo);

            CheckAsync(ConnectPool.GetOne());

            return task;
        }


        /// <summary>
        /// 创建一个未执行的任务
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <typeparam name="TC"></typeparam>
        /// <returns>一个任务全部信息，同时也是传递到执行方法中的变量</returns>
        protected virtual QueueTaskInfo CreatTask<TI, TC>(IRequest request, CancellationToken cancellation, out Task<TI> task) where TC : IResponseReader, TI, new()
        {
            var info = new QueueTaskInfo()
            {
                CancellationToken = cancellation,
                Request = request,
            };

            task = new Task<TI>(ExecuteWorking<TI, TC>, info, cancellation);

            info.Task = task;
            return info;
        }

        /// <summary>
        /// 执行请求的方法
        /// </summary>
        protected virtual TI ExecuteWorking<TI, TC>(object obj) where TC : IResponseReader, TI, new()
        {
            if (obj is not QueueTaskInfo asyncObject || asyncObject.Connect is null)
                return default;

            var result = asyncObject.Connect.Execute<TC>(asyncObject.Request);
            return result;
        }

        /// <summary>
        /// 检查异步队列
        /// </summary>
        /// <param name="connect">当前的连接</param>
        protected virtual void CheckAsync(IConnection connect)
        {
            if (connect is null)
                return;

            if (TaskQueue.IsEmpty)
            {
                connect.Dispose();
                return;
            }

            // 开启一个长时间任务，用于真正按队列执行Memcache操作
            var task = new Task(QueueWorking, connect, CancellationToken.None, TaskCreationOptions.LongRunning);
            task.Start();
        }

        /// <summary>
        /// 执行队列
        /// </summary>
        /// <param name="obj">当前线程使用的连接</param>
        protected virtual void QueueWorking(object obj)
        {
            if (obj is not IConnection connect)
                return;

            while (true)
            {
                if (!TaskQueue.TryDequeue(out var taskInfo) || taskInfo is null)
                {
                    if (TaskQueue.IsEmpty)
                        break;

                    continue;
                }

                // 任务已经被终止
                if (taskInfo.CancellationToken.IsCancellationRequested)
                    continue;
                taskInfo.Connect = connect;

                try
                {
                    taskInfo.Task.RunSynchronously();
                    taskInfo.Task.Wait();
                }
                catch
                {
                }
            }

            connect.Dispose();
        }

    }
}