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
    /// <summary>
    /// Memcache包结构
    /// </summary>
    public interface IPacketStructure
    {
        /// <summary>
        /// Magic number identifying the package
        /// </summary>
        byte Magic { get; }
        /// <summary>
        /// Command code
        /// </summary>
        byte Opcode { get; }
        /// <summary>
        /// Length in bytes of the text key that follows the command extras
        /// </summary>
        short KeyLength { get; }
        /// <summary>
        /// Length in bytes of the command extras
        /// </summary>
        byte ExtrasLength { get; }
        /// <summary>
        /// Reserved for future use
        /// </summary>
        byte DataType { get; }
        /// <summary>
        /// Length in bytes of extra + key + value
        /// </summary>
        int TotalBodyLength { get; }
        /// <summary>
        /// vbucket id in request  or Status in response
        /// </summary>
        short VbucketIdOrStatus { get; }
        /// <summary>
        /// Will be copied back to you in the response
        /// </summary>
        int Opaque { get; }
        /// <summary>
        /// Data version check
        /// </summary>
        long CAS { get; }

        /// <summary>
        /// COMMAND-SPECIFIC EXTRAS
        /// </summary>
        byte[]? Extras { get; }

        /// <summary>
        /// Key 
        /// </summary>
        string? Key { get; }

        /// <summary>
        /// Value
        /// </summary>
        byte[]? Value { get; }
    }
}
