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

namespace MemcachedTiny.Result
{
    /// <summary>
    /// 清空缓存结果
    /// </summary>
    public class FlushResult : IFlushResult
    {
        /// <summary>
        /// 各个节点的响应结果
        /// </summary>
        public virtual IReadOnlyList<IResult> NodeResultList { get; }

        /// <inheritdoc/>
        public virtual bool Success => NodeResultList?.All(r => r.Success) ?? false;

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="nodeResultList">所有节点结果</param>
        public FlushResult(IReadOnlyList<IResult> nodeResultList)
        {
            NodeResultList = nodeResultList;
        }
    }
}
