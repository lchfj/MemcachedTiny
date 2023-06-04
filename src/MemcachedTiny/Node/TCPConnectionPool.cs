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

using MemcachedTiny.Logging;
using MemcachedTiny.Util;

namespace MemcachedTiny.Node
{
    /// <summary>
    /// TCP 连接池
    /// </summary>
    public class TCPConnectionPool : IConnectionPool
    {
        private const int DefaultPoolSize = 8;

        /// <inheritdoc/>
        public virtual bool Avaliable => AllConnectionList.Any(c => c.Avaliable);
        /// <summary>
        /// 所有的连接
        /// </summary>
        protected virtual IReadOnlyList<IConnection> AllConnectionList { get; }
        /// <summary>
        /// 可以使用的连接
        /// </summary>
        protected virtual ConcurrentQueue<IConnection> AvailablePool { get; }
        /// <summary>
        /// 日志
        /// </summary>
        protected virtual ILogger<TCPConnectionPool> Logger { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="setting">设定</param>
        /// <param name="endPoint">连接点</param>
        public TCPConnectionPool(IMemcachedClientSetting setting, IConnectionEndPoint endPoint)
        {
            var loggerFactory = setting.LoggerFactory ?? LoggerEmptyFactory.Instance;
            Logger = loggerFactory.CreateLogger<TCPConnectionPool>();

            AllConnectionList = CreatConnection(setting, endPoint);
            AvailablePool = new ConcurrentQueue<IConnection>(AllConnectionList);
        }

        /// <summary>
        /// 创建所有连接
        /// </summary>
        /// <param name="setting">设定</param>
        /// <param name="endPoint">连接点</param>
        /// <returns></returns>
        protected virtual IReadOnlyList<IConnection> CreatConnection(IMemcachedClientSetting setting, IConnectionEndPoint endPoint)
        {
            var poolSize = setting.PoolSize.HasValue && setting.PoolSize.Value > 0 ? setting.PoolSize.Value : DefaultPoolSize;

            var list = new IConnection[poolSize];

            for (var i = 0; i < poolSize; i++)
            {
                var connection = setting.CustomerFactory?.CreatConnection(setting, endPoint);
                connection ??= new TcpConnection(setting, endPoint);

                list[i] = connection;
            }

            return new ReadOnlyCollection<IConnection>(list);
        }

        /// <summary>
        /// 获取一个可以使用的连接
        /// </summary>
        /// <returns></returns>
        public virtual IConnection? GetOne()
        {
            if (AvailablePool.IsEmpty)
                return null;

            for (var i = 0; i < AvailablePool.Count; i++)
            {
                if (!AvailablePool.TryDequeue(out var connection))
                {
                    if (AvailablePool.IsEmpty)
                        return null;
                }

                if (connection is null)
                    continue;

                if (connection.Avaliable)
                    return new TCPConnectionAdapter(connection, this);

                AvailablePool.Enqueue(connection);
            }

            return null;
        }
        /// <summary>
        /// 释放一个连接到连接池中
        /// </summary>
        /// <param name="connection">要释放的连接</param>
        public virtual void Release(IConnection connection)
        {
            if (connection is null)
                return;

            if (!AllConnectionList.Any(c => ReferenceEquals(c, connection)))
                return;

            if (AvailablePool.Any(c => ReferenceEquals(c, connection)))
                return;

            AvailablePool.Enqueue(connection);
        }
    }
}