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
using System.Text;

namespace MemcachedTiny.Data
{
    public class Response : IResponse
    {
        /// <inheritdoc/>
        public byte Magic { get; protected set; }

        /// <inheritdoc/>
        public byte Opcode { get; protected set; }

        /// <inheritdoc/>
        public short KeyLength { get; protected set; }

        /// <inheritdoc/>
        public byte ExtrasLength { get; protected set; }

        /// <inheritdoc/>
        public byte DataType { get; protected set; }

        /// <inheritdoc/>
        public int TotalBodyLength { get; protected set; }

        /// <inheritdoc/>
        public short VbucketIdOrStatus { get; protected set; }

        /// <inheritdoc/>
        public int Opaque { get; protected set; }

        /// <inheritdoc/>
        public long CAS { get; protected set; }

        /// <inheritdoc/>
        public byte[] Extras { get; protected set; }

        /// <inheritdoc/>
        public string Key { get; protected set; }

        /// <inheritdoc/>
        public byte[] Value { get; protected set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public virtual int ValueLength => TotalBodyLength - ExtrasLength - KeyLength;


        /// <inheritdoc/>
        public void SetHeader(byte[] header)
        {
            if (header.Length != 24)
                throw new ArgumentException("length", nameof(header));

            Magic = header[0];
            Opcode = header[1];
            KeyLength = MBitConverter.ReadShort(header, 2);
            ExtrasLength = header[4];
            DataType = header[5];
            VbucketIdOrStatus = MBitConverter.ReadShort(header, 6);
            TotalBodyLength = MBitConverter.ReadInt(header, 8);
            Opaque = MBitConverter.ReadInt(header, 12);
            CAS = MBitConverter.ReadLong(header, 16);
        }


        /// <inheritdoc/>
        public void SetBody(byte[] extras, byte[] keys, byte[] values)
        {
            Extras = extras;
            Key = Encoding.ASCII.GetString(keys);
            Value = values;
        }
    }
}