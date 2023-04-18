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
    internal class TcpConnection : IConnection
    {
        public TcpConnection(IPEndPoint endPoint)
        {
        }

        public bool Avaliable => throw new NotImplementedException();

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            throw new NotImplementedException();
        }
    }
}