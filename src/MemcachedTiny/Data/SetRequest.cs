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
    /// 添加或替换缓存
    /// </summary>
    public class SetRequest : KeyRequest
    {
        /// <inheritdoc/>
        public override byte Opcode => 0x01;
        /// <inheritdoc/>
        public override byte[] Extras { get; }
        /// <inheritdoc/>
        public override byte[] Value { get; }

        /// <summary>
        /// 数据类型
        /// </summary>
        public uint Flags { get; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public uint Second { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="flag">数据类型</param>
        /// <param name="second">过期时间</param>
        /// <param name="bytes">数据</param>
        public SetRequest(string key, uint flag, uint second, byte[] bytes) : base(key)
        {
            Value = bytes;
            Second = second;
            Flags = flag;

            Extras = CreatExtras();
        }

        /// <summary>
        /// 创建该请求特定的扩展数据
        /// </summary>
        /// <returns></returns>
        protected virtual byte[] CreatExtras()
        {
            var bytes = new byte[8];

            var buffer = MBitConverter.GetByte(Flags);
            buffer.CopyTo(bytes, 0);

            buffer = MBitConverter.GetByte(Second);
            buffer.CopyTo(bytes, 4);

            return bytes;
        }
    }
}