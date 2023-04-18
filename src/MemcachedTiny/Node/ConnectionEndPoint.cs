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
    public class ConnectionEndPoint : IConnectionEndPoint
    {
        public ConnectionEndPoint(string host, int port)
        {
            if (string.IsNullOrWhiteSpace(host))
                throw new ArgumentNullException(nameof(host));

            if (port is < ushort.MinValue or > ushort.MaxValue)
                throw new ArgumentOutOfRangeException(nameof(port));

            Host = host;
            Port = port;
        }

        public virtual string Host { get; }
        public virtual int Port { get; }

        public static bool TryParse(string connectString, out ConnectionEndPoint endPoint)
        {
            endPoint = null;

            if (string.IsNullOrWhiteSpace(connectString))
                return false;

            var array = connectString.Split(':');
            if (string.IsNullOrWhiteSpace(array[0]))
                return false;

            if (!ushort.TryParse(array[1], out var port))
                return false;

            endPoint = new ConnectionEndPoint(array[0], port);
            return true;
        }
    }
}
