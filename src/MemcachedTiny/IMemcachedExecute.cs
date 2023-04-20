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

namespace MemcachedTiny
{
    /// <summary>
    /// 执行器接口
    /// </summary>
    public interface IMemcachedExecute
    {
        /// <summary>
        /// 同步执行一个请求并返回特定结果
        /// </summary>
        /// <typeparam name="TC">结果类型</typeparam>
        /// <param name="request">请求</param>
        /// <returns>结果</returns>
        TC Execute<TC>(IRequest request) where TC : IResponseReader, new();
        /// <summary>
        /// 异步执行一个请求并返回特定结果
        /// </summary>
        /// <typeparam name="TI">特定的结果类型</typeparam>
        /// <typeparam name="TC">实现特定的结果类型的类</typeparam>
        /// <param name="request">请求</param>
        /// <param name="cancellation">一个标识操作取消的<see cref="CancellationToken"/></param>
        /// <returns>异步任务</returns>
        Task<TI> ExecuteAsync<TI, TC>(IRequest request, CancellationToken cancellation) where TC : TI, IResponseReader, new();
    }
}