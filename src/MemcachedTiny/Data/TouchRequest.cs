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

using MemcachedTiny.Util;

namespace MemcachedTiny.Data
{
    /// <summary>
    /// 更新过期时间请求
    /// </summary>
    public class TouchRequest : KeyRequest
    {

        /// <inheritdoc/>
        public override byte Opcode => 0x1c;

        /// <inheritdoc/>
        public override byte[] Extras => MBitConverter.GetByte(Second);

        /// <inheritdoc/>
        public override byte[] Value => Array.Empty<byte>();


        /// <summary>
        /// 新的过期时间
        /// </summary>
        public virtual uint Second { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="second">新的过期时间</param>
        public TouchRequest(string key, uint second) : base(key)
        {
            Second = second;
        }
    }
}