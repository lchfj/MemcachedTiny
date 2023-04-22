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

using MemcachedTiny.Node;
using MemcachedTiny.Util;

namespace MemcachedTiny
{
    /// <summary>
    /// 自定义实现工厂
    /// </summary>
    public interface ICustomerFactory
    {
        /// <summary>
        /// 创建连接点
        /// </summary>
        /// <param name="connect">连接字符串</param>
        /// <returns>连接点</returns>
        IConnectionEndPoint CreatConnectionEndPoint(string connect);
        /// <summary>
        /// 创建节点
        /// </summary>
        /// <param name="setting">设定</param>
        /// <param name="endPoint">当前节点的连接点</param>
        /// <returns></returns>
        INode CreatNode(IMemcachedClientSetting setting, IConnectionEndPoint endPoint);
        /// <summary>
        /// 创建连接池
        /// </summary>
        /// <param name="setting">设定</param>
        /// <param name="endPoint">当前节点的连接点</param>
        IConnectionPool CreatConnectionPool(IMemcachedClientSetting setting, IConnectionEndPoint endPoint);
        /// <summary>
        /// 创建连接
        /// </summary>
        /// <param name="setting">设定</param>
        /// <param name="endPoint">当前节点的连接点</param>
        IConnection CreatConnection(IMemcachedClientSetting setting, IConnectionEndPoint endPoint);
        /// <summary>
        /// 创建节点选择器
        /// </summary>
        /// <param name="setting">设定</param>
        /// <param name="nodeList">所有节点</param>
        INodeSelecter CreatNodeSelecter(IMemcachedClientSetting setting, IReadOnlyList<INode> nodeList);
    }
}