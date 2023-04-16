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
        public IResult Set(string key, byte[] bytes, int second)
        {
            key = AssertKey(key);
            var node = SelectNodeForKey(key);

            var request = new SetRequest(key, bytes, second);
            return node.Execute<GetResult>(request);
        }

        /// <inheritdoc/>
        public IGetResult Get(string key)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new GetRequest(key);

            return node.Execute<GetResult>(request);
        }

        /// <inheritdoc/>
        public IResult Touch(string key, int second)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new TouchRequest(key, second);

            return node.Execute<GetResult>(request);
        }

        /// <inheritdoc/>
        public IGetResult GetAndTouch(string key, int second)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new GetAndTouchRequest(key, second);

            return node.Execute<GetResult>(request);
        }

        /// <inheritdoc/>
        public IResult Delete(string key)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new DeleteRequest(key);

            return node.Execute<GetResult>(request);
        }

        /// <inheritdoc/>
        public IResult Flush()
        {
            var request = new FlushRequest();

            var resultList = new IResult[NodeList.Count];
            for (var i = 0; i < NodeList.Count; i++)
            {
                var node = NodeList[i];

                resultList[i] = node.Execute<GetResult>(request);
            }

            return new Result.Result()
            {
                Success = resultList.All(r => r.Success)
            };
        }
    }
}