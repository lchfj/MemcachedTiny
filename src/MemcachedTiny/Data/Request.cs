﻿/*
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
    /// <summary>
    /// 请求基类
    /// </summary>
    public abstract class Request : IRequest, IPacketStructure
    {
        /// <inheritdoc/>
        public byte Magic => 0x80;

        /// <inheritdoc/>
        public abstract byte Opcode { get; }

        /// <inheritdoc/>
        public virtual short KeyLength => Convert.ToInt16(Key?.Length ?? 0);

        /// <inheritdoc/>
        public virtual byte ExtrasLength => Convert.ToByte(Extras?.Length ?? 0);

        /// <inheritdoc/>
        public virtual byte DataType => 0;

        /// <inheritdoc/>
        public virtual int TotalBodyLength => ExtrasLength + KeyLength + (Value?.Length ?? 0);

        /// <inheritdoc/>
        public virtual short VbucketIdOrStatus => 0;

        /// <inheritdoc/>
        public virtual int Opaque { get; set; }

        /// <inheritdoc/>
        public virtual long CAS { get; }

        /// <inheritdoc/>
        public abstract byte[] Extras { get; }

        /// <inheritdoc/>
        public abstract string Key { get; }

        /// <inheritdoc/>
        public abstract byte[] Value { get; }

        /// <inheritdoc/>
        public virtual void WriteToStream(Stream stream)
        {
            using var writer = new BinaryWriter(stream, Encoding.ASCII, true);

            writer.Write(Magic);
            writer.Write(Opcode);
            writer.Write(MBitConverter.GetByte(KeyLength));
            writer.Write(ExtrasLength);
            writer.Write(DataType);
            writer.Write(MBitConverter.GetByte(VbucketIdOrStatus));
            writer.Write(MBitConverter.GetByte(TotalBodyLength));
            writer.Write(MBitConverter.GetByte(Opaque));
            writer.Write(MBitConverter.GetByte(CAS));

            if (Extras is not null)
                writer.Write(Extras);
            if (Key is not null)
                writer.Write(Encoding.ASCII.GetBytes(Key));
            if (Value is not null)
                writer.Write(Value);

        }
    }
}