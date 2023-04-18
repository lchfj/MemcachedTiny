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

using MemcachedTiny.Result;

namespace MemcachedTiny
{
    /// <summary>
    /// Memcached 同步操作方法
    /// </summary>
    public interface IMemcachedClientSync
    {
        /// <summary>
        /// 添加缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="flags">数据标识</param>
        /// <param name="second">缓存时间（秒）</param>
        /// <param name="bytes">缓存数据</param>
        IResult Set(string key, uint flags, uint second, byte[] bytes);

        /// <summary>
        /// 获取一个缓存值（如果存在的话）
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存的二进制数组（可为空）</returns>
        IGetResult Get(string key);

        /// <summary>
        /// 更新一个缓存的缓存时间
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="second">缓存时间（秒）</param>
        /// <param name="cas">数据版本</param>
        IResult Touch(string key, uint second, uint cas);

        /// <summary>
        /// 获取一个缓存值并更新缓存时间（如果存在的话）
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <param name="second">缓存时间（秒）</param>
        /// <returns>缓存的二进制数组（可为空）</returns>
        IGetResult GetAndTouch(string key, uint second);

        /// <summary>
        /// 删除一个缓存
        /// </summary>
        /// <param name="key">缓存键</param>
        IResult Delete(string key);

        /// <summary>
        /// 清空所有缓存
        /// </summary>
        /// <param name="second">清空缓存的时间</param>
        IResult Flush(uint second);
    }
}
