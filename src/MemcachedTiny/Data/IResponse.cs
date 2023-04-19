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
    /// Memcache 响应数据
    /// </summary>
    public interface IResponse : IPacketStructure
    {
        /// <summary>
        /// 设定头部数据
        /// </summary>
        /// <param name="header"></param>
        void SetHeader(byte[] header);

        /// <summary>
        /// 设定内容数据
        /// </summary>
        void SetBody(byte[] extras, byte[] keys, byte[] values);
    }
}