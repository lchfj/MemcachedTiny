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
    public static class MBitConverter
    {
        public static byte[] GetByte(short value)
        {
            var bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] GetByte(int value)
        {
            var bytes = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        internal static byte[] GetByte(long cAS)
        {
            throw new NotImplementedException();
        }

        internal static short ReadShort(byte[] header, int v)
        {
            throw new NotImplementedException();
        }

        internal static int ReadInt(byte[] header, int v)
        {
            throw new NotImplementedException();
        }

        internal static long ReadLong(byte[] header, int v)
        {
            throw new NotImplementedException();
        }
    }
}