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
    /// 假节点
    /// </summary>
    public sealed class NodeFack : INode
    {
        /// <summary>
        /// 唯一实例
        /// </summary>
        public static NodeFack Instance { get; } = new();
        private NodeFack()
        {
        }

        /// <inheritdoc/>
        public int NodeIndex => 0;

        /// <inheritdoc/>
        public bool Avaliable => false;

        /// <inheritdoc/>
        public TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            return Response.CreatError<TC>();
        }

        /// <inheritdoc/>
        public Task<TI> ExecuteAsync<TI, TC>(IRequest request, CancellationToken cancellation) where TC : IResponseReader, TI, new()
        {
            return Task.FromResult<TI>(Response.CreatError<TC>());
        }
    }
}