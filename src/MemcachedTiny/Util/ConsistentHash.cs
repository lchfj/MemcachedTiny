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

namespace MemcachedTiny.Util
{
    /// <summary>
    /// 一致性哈希算法
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsistentHash<T> : IConsistentHash<T> where T : IConsistentHashNode
    {
        private const long HashMax = (long)uint.MaxValue + 1;

        /// <summary>
        /// 一致性哈希节点位置信息
        /// </summary>
        /// <typeparam name="U"></typeparam>
        protected class NodePosition<U> where U : IConsistentHashNode
        {
            /// <summary>
            /// 创建实例
            /// </summary>
            /// <param name="position"></param>
            /// <param name="node"></param>
            public NodePosition(uint position, U node)
            {
                Position = position;
                Node = node;
            }

            /// <summary>
            /// 所在位置
            /// </summary>
            public virtual uint Position { get; }
            /// <summary>
            /// 对应节点
            /// </summary>
            public virtual U Node { get; }
        }


        /// <summary>
        /// 哈希节点列表
        /// </summary>
        protected virtual IReadOnlyList<NodePosition<T>> NodeList { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        public ConsistentHash(IReadOnlyList<T> nodeList)
        {
            if (nodeList is null || nodeList.Count == 0)
                throw new ArgumentNullException(nameof(nodeList));

            NodeList = CreatNode(nodeList);
        }


        /// <inheritdoc/>
        protected virtual IReadOnlyList<NodePosition<T>> CreatNode(IReadOnlyList<T> nodeList)
        {
            var nodeList3 = new List<NodePosition<T>>(nodeList.Count);
            for (var index = 0; index < nodeList.Count; index++)
            {
                var position = Convert.ToUInt32(HashMax * index / nodeList.Count);
                var pos = new NodePosition<T>(position, nodeList[index]);
                nodeList3.Add(pos);
            }

            nodeList3.Reverse();
            return nodeList3.AsReadOnly();
        }

        /// <inheritdoc/>
        public virtual T GetNode(uint hash)
        {
            if (NodeList.Count == 0)
                return default;

            if (NodeList.Count == 1)
            {
                var node = NodeList[0];
                if (node.Node.Avaliable)
                    return node.Node;

                return default;
            }

            foreach (var node in NodeList)
            {
                if (node.Position <= hash && node.Node.Avaliable)
                    return node.Node;
            }

            foreach (var node in NodeList)
            {
                if (node.Node.Avaliable)
                    return node.Node;
            }

            return default;
        }
    }
}