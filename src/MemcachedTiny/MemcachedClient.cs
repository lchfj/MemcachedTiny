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

namespace MemcachedTiny
{
    /// <summary>
    /// Memcached 缓存客户端
    /// </summary>
    public partial class MemcachedClient : IMemcachedClient
    {
        /// <summary>
        /// 所有节点列表
        /// </summary>
        protected virtual IReadOnlyList<INode> NodeList { get; }
        /// <summary>
        /// 节点选择器
        /// </summary>
        protected virtual INodeSelecter NodeSelecter { get; }
        /// <summary>
        /// 设定
        /// </summary>
        protected virtual IMemcachedClientSetting Setting { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="setting">设定</param>
        public MemcachedClient(IMemcachedClientSetting setting)
        {
            Setting = setting ?? throw new ArgumentNullException(nameof(setting));
            NodeList = CreatNodeList();
            NodeSelecter = CreatNodeSelecter();
        }

        /// <summary>
        /// 创建节点列表
        /// </summary>
        /// <returns>节点列表</returns>
        /// <exception cref="ArgumentNullException">连接字符串为空，或空数组，或有字符串为空</exception>
        protected virtual IReadOnlyList<INode> CreatNodeList()
        {
            var conent = Setting.Connect;
            if (conent is not { Count: > 0 })
                throw new ArgumentNullException(nameof(Setting) + "." + nameof(Setting.Connect));
            if (conent.Any(c => string.IsNullOrWhiteSpace(c)))
                throw new ArgumentNullException(nameof(Setting) + "." + nameof(Setting.Connect));

            var nodeList = new INode[conent.Count];
            for (var i = 0; i < conent.Count; i++)
                nodeList[i] = new Node.Node(conent[i]);

            return new ReadOnlyCollection<INode>(nodeList);
        }


        /// <summary>
        /// 创建节点选择器
        /// </summary>
        /// <returns>节点选择器</returns>
        protected virtual INodeSelecter CreatNodeSelecter()
        {
            if (NodeList.Count == 1)
                return new NodeSelecterSingle(NodeList);
            else
                return new NodeSelecterMulti(NodeList);
        }

        /// <summary>
        /// 为指定键选择缓存节点
        /// </summary>
        /// <param name="key">缓存键</param>
        /// <returns>缓存键所在节点</returns>
        protected virtual INode SelectNodeForKey(string key)
        {
            return NodeSelecter.SelectForKey(key);
        }

        /// <summary>
        /// 验证缓存键
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">缓存键为空</exception>
        /// <exception cref="ArgumentException">长度超过250</exception>
        /// <exception cref="ArgumentOutOfRangeException">有非ASCII字符或空白字符</exception>
        protected virtual string AssertKey(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException(nameof(key));

            if (key.Length > 250)
                throw new ArgumentException("长度最大为250", nameof(key));

            if (key.Any(c => c is < (char)0x21 or > (char)0x7E))
                throw new ArgumentOutOfRangeException(nameof(key), "字符必须在0x21至0x7E之间");

            return key;
        }
    }
}
