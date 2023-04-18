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
    public class TCPConnectionAdapter : IConnection
    {
        public bool Avaliable => Connection.Avaliable;

        protected virtual IConnection Connection { get; set; }
        protected virtual IConnectionPool ConnectionPool { get; }

        public TCPConnectionAdapter(IConnection connection, IConnectionPool connectionPool)
        {
            Connection = connection ?? throw new ArgumentNullException(nameof(connection));
            ConnectionPool = connectionPool ?? throw new ArgumentNullException(nameof(connectionPool));
        }

        ~TCPConnectionAdapter()
        {
            Dispose();
        }

        public virtual void Dispose()
        {
            var connection = Connection;
            if (connection is null)
                return;

            //TODO: 释放时不应当有执行的任务
            Connection = null;
            ConnectionPool.Release(connection);
        }

        public virtual TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            var c = Connection;
            if (c is not null)
                return c.Execute<TC>(request);

            throw new NotImplementedException();
        }
    }
}