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

namespace MemcachedTiny.Util
{
    /// <summary>
    /// Memcached友好的二进制转换器（大端序）
    /// </summary>
    public static class MBitConverter
    {
        /// <summary>
        /// 是否是小端序
        /// </summary>
        public static bool IsLittleEndian = BitConverter.GetBytes(1)[0] == 1;

        /// <summary>
        /// 获取字节数组
        /// </summary>
        public static byte[] GetByte(short value)
        {
            var bytes = BitConverter.GetBytes(value);

            if (IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        public static byte[] GetByte(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            if (IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }
        /// <summary>
        /// 获取字节数组
        /// </summary>
        public static byte[] GetByte(uint value)
        {
            var bytes = BitConverter.GetBytes(value);

            if (IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        /// <summary>
        /// 获取字节数组
        /// </summary>
        public static byte[] GetByte(long value)
        {
            var bytes = BitConverter.GetBytes(value);

            if (IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        private static byte[] GetByteArray(byte[] bytes, int start, int length)
        {
            var result = new byte[length];

            for (var i = 0; i < length; i++)
                result[length - 1 - i] = bytes[start + i];

            if (!IsLittleEndian)
                Array.Reverse(result);

            return result;
        }

        /// <summary>
        /// 读取数字
        /// </summary>
        public static short ReadShort(byte[] bytes, int start)
        {
            var array = GetByteArray(bytes, start, 2);
            return BitConverter.ToInt16(array, 0);
        }

        /// <summary>
        /// 读取数字
        /// </summary>
        public static int ReadInt(byte[] bytes, int start)
        {
            var array = GetByteArray(bytes, start, 4);
            return BitConverter.ToInt32(array, 0);
        }

        /// <summary>
        /// 读取数字
        /// </summary>
        public static long ReadLong(byte[] bytes, int start)
        {
            var array = GetByteArray(bytes, start, 8);
            return BitConverter.ToInt64(array, 0);
        }
    }
}