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
    /// 队列中的任务信息
    /// </summary>
    public class QueueTaskInfo
    {
        /// <summary>
        /// 请求
        /// </summary>
        public virtual IRequest Request { get; set; }
        /// <summary>
        /// 执行请求的连接
        /// </summary>
        public virtual IConnection Connect { get; set; }
        /// <summary>
        /// 取消源
        /// </summary>
        public virtual CancellationTokenSource HandCancle { get; set; }
        /// <summary>
        /// 取消令牌
        /// </summary>
        public virtual CancellationToken CancellationToken { get; set; }
        /// <summary>
        /// 任务对象
        /// </summary>
        public virtual Task Task { get; set; }
    }
}