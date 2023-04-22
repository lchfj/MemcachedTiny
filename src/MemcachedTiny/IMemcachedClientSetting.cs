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

using MemcachedTiny.Logging;

namespace MemcachedTiny
{
    /// <summary>
    /// 设定接口 
    /// </summary>
    public interface IMemcachedClientSetting
    {
        /// <summary>
        /// 所有连接地址
        /// </summary>
        List<string> Connect { get; }
        /// <summary>
        /// 每个节点建立多少个连接
        /// </summary>
        int? PoolSize { get; }
        /// <summary>
        /// 日志工厂
        /// </summary>
        ILoggerFactory LoggerFactory { get; }
        /// <summary>
        /// 自定义实现类
        /// </summary>
        ICustomerFactory CustomerFactory { get; set; }
    }
}