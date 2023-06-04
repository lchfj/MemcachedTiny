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
using MemcachedTiny.Logging;
using MemcachedTiny.Util;

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
        /// 日志
        /// </summary>
        protected virtual ILogger<Node> Logger { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="setting">设定</param>
        /// <param name="endPoint">Memcached连接地址</param>
        public Node(IMemcachedClientSetting setting, IConnectionEndPoint endPoint)
        {
            var loggerFactory = setting.LoggerFactory ?? LoggerEmptyFactory.Instance;
            Logger = loggerFactory.CreateLogger<Node>();

            ConnectPool = CreatConnectionPool(endPoint, setting);
            TaskQueue = new ConcurrentQueue<QueueTaskInfo>();
        }

        /// <summary>
        /// 创建连接池
        /// </summary>
        protected virtual IConnectionPool CreatConnectionPool(IConnectionEndPoint endPoint, IMemcachedClientSetting setting)
        {
            var pool = setting.CustomerFactory?.CreatConnectionPool(setting, endPoint);
            pool ??= new TCPConnectionPool(setting, endPoint);
            return pool;
        }

        /// <summary>
        /// 创建连接点
        /// </summary>
        protected virtual IConnectionEndPoint? CreatEndPoint(string connectString, ILoggerFactory loggerFactory)
        {
            ConnectionEndPoint.TryParse(connectString, out var endPoint);
            return endPoint;
        }

        /// <inheritdoc/>
        public virtual TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            if (!Avaliable)
                return Response.CreatError<TC>("NodeUnAvaliable");

            var tInfo = CreatTask<TC, TC>(request, CancellationToken.None, out var task);
            var connect = ConnectPool.GetOne();

            if (connect is null)
            {
                TaskQueue.Enqueue(tInfo);
                task.Wait();
            }
            else
            {
                tInfo.Connect = connect;
                task.RunSynchronously();
                CheckAsync(connect);
            }

            if (task.Status == TaskStatus.RanToCompletion)
                return task.Result;

            return Response.CreatError<TC>(task.Status.ToString());
        }


        /// <inheritdoc/>
        public virtual Task<TI> ExecuteAsync<TI, TC>(IRequest request, CancellationToken cancellation) where TC : IResponseReader, TI, new()
        {
            if (!Avaliable)
                return Task.FromResult<TI>(Response.CreatError<TC>("NodeUnAvaliable"));

            var tInfo = CreatTask<TI, TC>(request, cancellation, out var task);
            TaskQueue.Enqueue(tInfo);

            var connect = ConnectPool.GetOne();
            if (connect is not null)
                CheckAsync(connect);

            // 对结果二次处理
            return task.ContinueWith(r =>
            {
                if (r.Status == TaskStatus.RanToCompletion)
                    return r.Result;

                return Response.CreatError<TC>(r.Status.ToString());
            });
        }

        /// <summary>
        /// 创建一个未执行的任务
        /// </summary>
        /// <typeparam name="TI"></typeparam>
        /// <typeparam name="TC"></typeparam>
        /// <returns>一个任务全部信息，同时也是传递到执行方法中的变量</returns>
        protected virtual QueueTaskInfo CreatTask<TI, TC>(IRequest request, CancellationToken cancellation, out Task<TI> task) where TC : IResponseReader, TI, new()
        {
            var linkCancel = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
            // 任务五秒自动超时
            linkCancel.CancelAfter(5000);

            var info = new QueueTaskInfo(request, linkCancel);

            task = new Task<TI>(ExecuteWorking<TI, TC>, info, linkCancel.Token);

            info.Task = task;
            return info;
        }

        /// <summary>
        /// 执行请求的方法
        /// </summary>
        protected virtual TI ExecuteWorking<TI, TC>(object obj) where TC : IResponseReader, TI, new()
        {
            if (obj is not QueueTaskInfo asyncObject || asyncObject.Connect is null)
                return Response.CreatError<TC>("Error");

            var result = asyncObject.Connect.Execute<TC>(asyncObject.Request);
            return result;
        }

        /// <summary>
        /// 检查异步队列
        /// </summary>
        /// <param name="connect">当前的连接</param>
        protected virtual void CheckAsync(IConnection connect)
        {
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
                // 连接中断
                if (!connect.Avaliable)
                    break;

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
                taskInfo.Task?.RunSynchronously();
                taskInfo.Task?.Wait();
            }

            connect.Dispose();

            // 这里在节点不可用时清理掉节点中所有等待的任务
            while (!Avaliable && !TaskQueue.IsEmpty)
            {
                if (TaskQueue.TryDequeue(out var taskInfo))
                {
                    taskInfo.HandCancle.Cancel();
                }
            }
        }

    }
}