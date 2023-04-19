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

namespace MemcachedTiny.Result
{
    /// <summary>
    /// 操作结果
    /// </summary>
    public class Result : IResult, IResponseReader
    {
        /// <summary>
        /// 原始相应数据
        /// </summary>
        public virtual IPacketStructure Response { get; protected set; }

        /// <inheritdoc/>
        public virtual short Status => Response.VbucketIdOrStatus;

        /// <inheritdoc/>
        public virtual bool Success => Response.VbucketIdOrStatus == 0x0000;

        /// <inheritdoc/>
        public virtual long CAS => Response.CAS;


        /// <inheritdoc/>
        public virtual void Read(IPacketStructure response)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }
    }
}