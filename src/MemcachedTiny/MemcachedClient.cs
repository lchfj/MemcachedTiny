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
using MemcachedTiny.Logging;
using MemcachedTiny.Node;
using MemcachedTiny.Util;

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
        /// 这个类使用的日志
        /// </summary>
        protected virtual ILogger<MemcachedClient> Logger { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="setting">设定</param>
        public MemcachedClient(IMemcachedClientSetting setting)
        {
            if (setting is null)
                throw new ArgumentNullException(nameof(setting));

            var loggerFactory = setting.LoggerFactory ?? LoggerEmptyFactory.Instance;
            Logger = loggerFactory.CreateLogger<MemcachedClient>();

            var endpointList = CreatEnpointList(setting);
            NodeList = CreatNodeList(setting, endpointList);
            NodeSelecter = CreatNodeSelecter(setting);
        }

        protected virtual IReadOnlyList<IConnectionEndPoint> CreatEnpointList(IMemcachedClientSetting setting)
        {
            IReadOnlyList<string> connectList = setting.Connect;
            if (connectList is not { Count: > 0 })
                throw new ArgumentNullException("setting.Connect");

            var list = new IConnectionEndPoint[connectList.Count];
            for (var i = 0; i < connectList.Count; i++)
            {
                var connect = connectList[i];
                if (string.IsNullOrWhiteSpace(connect))
                    continue;

                var endPoint = setting.CustomerFactory?.CreatConnectionEndPoint(connect);
                if (endPoint is null)
                {
                    if (ConnectionEndPoint.TryParse(connect, out var endPoint1))
                        endPoint = endPoint1;
                }

                list[i] = endPoint;
            }

            if (list.All(c => c is null))
                throw new ArgumentNullException("setting.Connect");

            return new ReadOnlyCollection<IConnectionEndPoint>(list);
        }

        /// <summary>
        /// 创建节点列表
        /// </summary>
        /// <returns>节点列表</returns>
        /// <exception cref="ArgumentNullException">连接字符串为空，或空数组，或有字符串为空</exception>
        protected virtual IReadOnlyList<INode> CreatNodeList(IMemcachedClientSetting setting, IReadOnlyList<IConnectionEndPoint> endpointList)
        {
            var nodeList = new INode[endpointList.Count];

            for (var i = 0; i < endpointList.Count; i++)
            {
                var endpoint = endpointList[i];
                if (endpoint is null)
                {
                    nodeList[i] = NodeFack.Instance;
                }
                else
                {
                    var node = setting.CustomerFactory?.CreatNode(setting, endpoint);
                    node ??= new Node.Node(setting, endpoint);

                    nodeList[i] = node;
                }
            }

            return new ReadOnlyCollection<INode>(nodeList);
        }


        /// <summary>
        /// 创建节点选择器
        /// </summary>
        /// <returns>节点选择器</returns>
        protected virtual INodeSelecter CreatNodeSelecter(IMemcachedClientSetting setting)
        {
            return setting.CustomerFactory?.CreatNodeSelecter(setting, NodeList) ?? new NodeSelecter(setting, NodeList);
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


        /// <summary>
        /// 同步执行一个自定义的请求
        /// </summary>
        /// <typeparam name="TC">结果类型</typeparam>
        /// <param name="key">请求对应的缓存键</param>
        /// <param name="request">请求</param>
        /// <returns>结果</returns>
        public virtual TC Execute<TC>(string key, IRequest request) where TC : IResponseReader, new()
        {
            key = AssertKey(key);
            var node = SelectNodeForKey(key);
            return node.Execute<TC>(request);
        }

        /// <summary>
        /// 异步执行一个自定义的请求
        /// </summary>
        /// <typeparam name="TI">特定的结果类型</typeparam>
        /// <typeparam name="TC">实现特定的结果类型的类</typeparam>
        /// <param name="key">请求对应的缓存键</param>
        /// <param name="request">请求</param>
        /// <param name="cancellation">一个标识操作取消的<see cref="CancellationToken"/></param>
        /// <returns>结果</returns>
        public virtual Task<TI> ExecuteAsync<TI, TC>(string key, IRequest request, CancellationToken cancellation) where TC : IResponseReader, TI, new()
        {
            key = AssertKey(key);
            var node = SelectNodeForKey(key);
            return node.ExecuteAsync<TI, TC>(request, cancellation);
        }
    }
}
