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

namespace MemcachedTiny.Data
{
    public class Response : IResponse
    {
        private ResponseHeader header;
        private byte[] extra;
        private byte[] key;
        private byte[] value;

        public Response(ResponseHeader header, byte[] extra, byte[] key, byte[] value)
        {
            this.header = header;
            this.extra = extra;
            this.key = key;
            this.value = value;
        }

        public byte Magic => throw new NotImplementedException();

        public byte Opcode => throw new NotImplementedException();

        public ushort KeyLength => throw new NotImplementedException();

        public byte ExtrasLength => throw new NotImplementedException();

        public byte DataType => throw new NotImplementedException();

        public uint TotalBodyLength => throw new NotImplementedException();

        public ushort VbucketIdOrStatus => throw new NotImplementedException();

        public uint Opaque => throw new NotImplementedException();

        public uint CAS => throw new NotImplementedException();

        public byte[] Extras => throw new NotImplementedException();

        public string Key => throw new NotImplementedException();

        public byte[] Value => throw new NotImplementedException();
    }
}