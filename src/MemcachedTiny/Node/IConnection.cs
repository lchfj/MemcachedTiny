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
    /// 一个连接
    /// </summary>
    public interface IConnection : IDisposable
    {
        /// <summary>
        /// 连接是否可用
        /// </summary>
        bool Avaliable { get; }

        /// <summary>
        /// 执行请求
        /// </summary>
        /// <typeparam name="TC">结果类型</typeparam>
        /// <param name="request">请求</param>
        /// <returns>返回执行结果</returns>
        TC Execute<TC>(IRequest request) where TC : IResponseReader, new();
    }
}