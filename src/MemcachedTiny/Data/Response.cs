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
        public virtual byte Magic { get; protected set; }

        /// <inheritdoc/>
        public virtual byte Opcode { get; protected set; }

        /// <inheritdoc/>
        public virtual short KeyLength { get; protected set; }

        /// <inheritdoc/>
        public virtual byte ExtrasLength { get; protected set; }

        /// <inheritdoc/>
        public virtual byte DataType { get; protected set; }

        /// <inheritdoc/>
        public virtual int TotalBodyLength { get; protected set; }

        /// <inheritdoc/>
        public virtual short VbucketIdOrStatus { get; protected set; }

        /// <inheritdoc/>
        public virtual int Opaque { get; protected set; }

        /// <inheritdoc/>
        public virtual long CAS { get; protected set; }

        /// <inheritdoc/>
        public virtual byte[] Extras { get; protected set; }

        /// <inheritdoc/>
        public virtual string Key { get; protected set; }

        /// <inheritdoc/>
        public virtual byte[] Value { get; protected set; }

        /// <summary>
        /// 数据长度
        /// </summary>
        public virtual int ValueLength => TotalBodyLength - ExtrasLength - KeyLength;


        /// <inheritdoc/>
        public virtual void SetHeader(byte[] header)
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
        public virtual void SetBody(byte[] extras, byte[] keys, byte[] values)
        {
            Extras = extras;
            Key = Encoding.ASCII.GetString(keys);
            Value = values;
        }

        /// <summary>
        /// 生成一个返回错误的响应
        /// </summary>
        /// <typeparam name="TC">响应类型</typeparam>
        /// <returns>响应</returns>
        public static TC CreatError<TC>(string error) where TC : IResponseReader, new()
        {
            byte[] value;
            if (string.IsNullOrWhiteSpace(error))
                value = Array.Empty<byte>();
            else
                value = Encoding.UTF8.GetBytes(error.Trim());

            var response = new Response()
            {
                CAS = 0,
                DataType = 0,
                Extras = Array.Empty<byte>(),
                ExtrasLength = 0,
                Key = string.Empty,
                KeyLength = 0,
                Magic = 0x81,
                Opaque = int.MinValue,
                Opcode = 0xFF,
                TotalBodyLength = value.Length,
                Value = value,
                VbucketIdOrStatus = 0x0086
            };

            var tc = new TC();
            tc.Read(response);
            return tc;
        }
    }
}