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

using System.Security.Cryptography;

namespace MemcachedTiny.Util
{
    /// <summary>
    /// FNV1-a 哈希算法
    /// </summary>
    public class FNV1a : HashAlgorithm
    {
        private const uint OFFSETBASIS = 2166136261;
        private const uint PRIME = 16777619;

        /// <summary>
        /// UInt型哈希值
        /// </summary>
        protected virtual uint UIntHashCode { get; set; }

        /// <inheritdoc/>
        public override void Initialize()
        {
            UIntHashCode = OFFSETBASIS;
        }

        /// <inheritdoc/>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            var end = ibStart + cbSize;
            for (var index = ibStart; index < end; index++)
            {
                UIntHashCode ^= array[index];
                UIntHashCode *= PRIME;
            }
        }

        /// <inheritdoc/>
        protected override byte[] HashFinal()
        {
            return BitConverter.GetBytes(UIntHashCode);
        }
    }
}
