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

using MemcachedTiny.Logging;
using MemcachedTiny.Util;

namespace MemcachedTiny.Node
{
    /// <summary>
    /// 节点选择器
    /// </summary>
    public class NodeSelecter : INodeSelecter
    {
        /// <summary>
        /// 日志
        /// </summary>
        protected virtual ILogger<NodeSelecter> Logger { get; }
        /// <summary>
        /// 节点列表
        /// </summary>
        protected virtual IReadOnlyList<INode> NodeList { get; }
        /// <summary>
        /// 缓存键哈希算法
        /// </summary>
        protected virtual IKeyHash KeyHash { get; }
        /// <summary>
        /// 一致性哈希实例
        /// </summary>
        protected virtual IConsistentHash<INode> ConsistentHash { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="setting">设定</param>
        /// <param name="nodeList">节点列表</param>
        public NodeSelecter(IMemcachedClientSetting setting, IReadOnlyList<INode> nodeList)
        {
            var loggerFactory = setting.LoggerFactory ?? LoggerEmptyFactory.Instance;
            Logger = loggerFactory.CreateLogger<NodeSelecter>();

            NodeList = nodeList ?? throw new ArgumentNullException(nameof(nodeList));
            KeyHash = CreatKeyHash();
            ConsistentHash = CreatConsistentHash();
        }

        /// <summary>
        /// 创建缓存键哈希方法
        /// </summary>
        /// <returns></returns>
        protected virtual IKeyHash CreatKeyHash()
        {
            return new KeyHashFNV1a();
        }

        /// <summary>
        /// 创建哈希一致性算法
        /// </summary>
        /// <returns></returns>
        protected virtual IConsistentHash<INode> CreatConsistentHash()
        {
            return new ConsistentHash<INode>(NodeList);
        }

        /// <inheritdoc/>
        public virtual INode SelectForKey(string key)
        {
            if (NodeList.Count == 1)
            {
                var node = NodeList[0];
                if (node.Avaliable)
                    return node;

                return NodeFack.Instance;

            }
            else if (NodeList.Count == 0)
            {
                return NodeFack.Instance;
            }
            else
            {
                var hash = KeyHash.Hash(key);
                var node = ConsistentHash.GetNode(hash);
                if (node is null)
                    return NodeFack.Instance;

                return node;
            }
        }
    }
}