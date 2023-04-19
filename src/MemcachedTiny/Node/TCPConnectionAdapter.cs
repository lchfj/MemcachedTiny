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

namespace MemcachedTiny.Node
{
    /// <summary>
    /// 用于包装连接池中的连接
    /// </summary>
    public class TCPConnectionAdapter : IConnection
    {
        /// <inheritdoc/>
        public bool Avaliable => Connection.Avaliable;

        /// <summary>
        /// 真实的连接实例
        /// </summary>
        protected virtual IConnection Connection { get; set; }
        /// <summary>
        /// 对应的连接池
        /// </summary>
        protected virtual IConnectionPool ConnectionPool { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="connection">真实的连接</param>
        /// <param name="connectionPool">连接池</param>
        public TCPConnectionAdapter(IConnection connection, IConnectionPool connectionPool)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ConnectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
        }

        /// <summary>
        /// 析构函数
        /// </summary>
        ~TCPConnectionAdapter()
        {
            Dispose();
        }

        /// <summary>
        /// 释放连接到连接池中
        /// </summary>
        public virtual void Dispose()
        {
            var connection = Connection;
            if (connection is null)
                return;

            //TODO: 释放时不应当有执行的任务
            Connection = null;
            ConnectionPool.Release(connection);
        }

        /// <inheritdoc/>
        public virtual TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            var c = Connection;
            if (c is not null)
                return c.Execute<TC>(request);

            throw new NotImplementedException();
        }
    }
}