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

using MemcachedTiny.Data;
using MemcachedTiny.Result;

namespace MemcachedTiny
{
    public partial class MemcachedClient : IMemcachedClientAsync
    {
        /// <inheritdoc/>
        public virtual Task<IResult> SetAsync(string key, int flags, int second, byte[] bytes, CancellationToken cancellation = default)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new SetRequest(key, flags, second, bytes);

            return node.ExecuteAsync<IResult, Result.Result>(request, cancellation);
        }

        /// <inheritdoc/>
        public virtual Task<IGetResult> GetAsync(string key, CancellationToken cancellation = default)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new GetRequest(key);

            return node.ExecuteAsync<IGetResult, GetResult>(request, cancellation);
        }

        /// <inheritdoc/>
        public virtual Task<IResult> TouchAsync(string key, uint second, uint cas, CancellationToken cancellation = default)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new TouchRequest(key, second, cas);

            return node.ExecuteAsync<IResult, Result.Result>(request, cancellation);
        }

        /// <inheritdoc/>
        public virtual Task<IGetResult> GetAndTouchAsync(string key, uint second, CancellationToken cancellation = default)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new GetAndTouchRequest(key, second);

            return node.ExecuteAsync<IGetResult, GetResult>(request, cancellation);
        }

        /// <inheritdoc/>
        public virtual Task<IResult> DeleteAsync(string key, CancellationToken cancellation = default)
        {
            key = AssertKey(key);

            var node = SelectNodeForKey(key);

            var request = new DeleteRequest(key);

            return node.ExecuteAsync<IResult, Result.Result>(request, cancellation);
        }

        /// <inheritdoc/>
        public virtual Task<IFlushResult> FlushAsync(uint second, CancellationToken cancellation = default)
        {
            var request = new FlushRequest(second);

            var resultList = new Task<IResult>[NodeList.Count];
            for (var i = 0; i < NodeList.Count; i++)
            {
                var node = NodeList[i];

                resultList[i] = node.ExecuteAsync<IResult, Result.Result>(request, cancellation);
            }

            return Task.WhenAll(resultList).ContinueWith(r =>
            {
                var result = new FlushResult(r.Result);
                return (IFlushResult)result;
            });
        }
    }
}
