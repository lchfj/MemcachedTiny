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
    /// 设定
    /// </summary>
    public class MemcachedClientSetting : IMemcachedClientSetting
    {
        /// <inheritdoc/>
        public List<string> Connect { get; set; }

        /// <inheritdoc/>
        public int? PoolSize { get; set; }

        /// <inheritdoc/>
        public ILoggerFactory LoggerFactory { get; set; }

        /// <inheritdoc/>
        public ICustomerFactory CustomerFactory { get; set; }
    }
}