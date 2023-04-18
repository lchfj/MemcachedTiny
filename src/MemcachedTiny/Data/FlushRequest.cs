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
    /// 清空所有请求
    /// </summary>
    public class FlushRequest : Request
    {

        /// <inheritdoc/>
        public override byte Opcode => 0x08;

        /// <inheritdoc/>
        public override byte[] Extras => MBitConverter.GetByte(Second);

        /// <inheritdoc/>
        public override string Key => string.Empty;

        /// <inheritdoc/>
        public override byte[] Value => Array.Empty<byte>();



        /// <summary>
        /// 清空时间
        /// </summary>
        public uint Second { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="second">清空时间</param>
        public FlushRequest(uint second)
        {
            Second = second;
        }
    }
}