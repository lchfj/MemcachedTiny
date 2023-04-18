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

namespace MemcachedTiny.Node
{
    public class TCPConnectionPool : IConnectionPool
    {
        private const int MaxPoolSize = 8;

        public bool Avaliable => AllConnectionList.Any(c => c.Avaliable);

        protected virtual IReadOnlyList<IConnection> AllConnectionList { get; }
        protected virtual ConcurrentQueue<IConnection> AvailablePool { get; }

        public TCPConnectionPool(IPEndPoint endPoint)
        {
            AllConnectionList = CreatConnection(endPoint);
            AvailablePool = new ConcurrentQueue<IConnection>(AllConnectionList);
        }

        protected virtual IReadOnlyList<IConnection> CreatConnection(IPEndPoint endPoint)
        {
            var list = new List<IConnection>(MaxPoolSize);

            for (var i = 0; i < 8; i++)
                list.Add(new TcpConnection(endPoint));

            return list.AsReadOnly();
        }

        public virtual IConnection GetOne()
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

        public void Release(IConnection connection)
        {
            if (connection is null)
                return;

            if (!AllConnectionList.Any(c => ReferenceEquals(c, connection)))
                return;

            AvailablePool.Enqueue(connection);
        }
    }
}