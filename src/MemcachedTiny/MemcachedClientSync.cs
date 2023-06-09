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

using MemcachedTiny.Data;
using MemcachedTiny.Result;

namespace MemcachedTiny
{
    public partial class MemcachedClient : IMemcachedClientSync
    {

        /// <inheritdoc/>
        public virtual IResult Set(string key, int flags, uint second, byte[] bytes)
        {
            if (bytes is null)
                throw new ArgumentNullException(nameof(bytes));

            var request = new SetRequest(key, flags, second, bytes);
            return Execute<Result.Result>(key, request);
        }

        /// <inheritdoc/>
        public virtual IGetResult Get(string key)
        {
            var request = new GetRequest(key);
            return Execute<GetResult>(key, request);
        }

        /// <inheritdoc/>
        public virtual IResult Touch(string key, uint second)
        {
            var request = new TouchRequest(key, second);
            return Execute<Result.Result>(key, request);
        }

        /// <inheritdoc/>
        public virtual IGetResult GetAndTouch(string key, uint second)
        {
            var request = new GetAndTouchRequest(key, second);
            return Execute<GetResult>(key, request);
        }

        /// <inheritdoc/>
        public virtual IResult Delete(string key)
        {
            var request = new DeleteRequest(key);
            return Execute<DeleteResult>(key, request);
        }

        /// <inheritdoc/>
        public virtual IFlushResult Flush(uint second)
        {
            var request = new FlushRequest(second);

            var resultList = new IResult[NodeList.Count];
            for (var i = 0; i < NodeList.Count; i++)
            {
                var node = NodeList[i];

                resultList[i] = node.Execute<Result.Result>(request);
            }

            return new FlushResult(resultList);
        }
    }
}
