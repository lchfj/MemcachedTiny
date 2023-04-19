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
using MemcachedTiny.Util;
using System.Net.Sockets;

namespace MemcachedTiny.Node
{
    /// <summary>
    /// TCP连接
    /// </summary>
    public class TcpConnection : IConnection
    {
        /// <summary>
        /// 响应头长度
        /// </summary>
        protected const int ResponseHeaderLength = 24;

        /// <inheritdoc/>
        public virtual bool Avaliable => TcpClient?.Connected ?? false;

        /// <summary>
        /// 一个TPC连接
        /// </summary>
        protected virtual TcpClient TcpClient { get; }
        /// <summary>
        /// 网络流
        /// </summary>
        protected virtual NetworkStream Stream { get; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="endPoint"></param>
        public TcpConnection(IConnectionEndPoint endPoint)
        {
            TcpClient = CreatTcpClient(endPoint);
            Stream = TcpClient.GetStream();
        }

        /// <summary>
        /// 创建以一个TCP连接
        /// </summary>
        /// <param name="endPoint"></param>
        /// <returns></returns>
        protected virtual TcpClient CreatTcpClient(IConnectionEndPoint endPoint)
        {
            var tcp = new TcpClient();

            tcp.Connect(endPoint.Host, endPoint.Port);
            tcp.SendTimeout = 3000;
            tcp.ReceiveTimeout = 3000;

            return tcp;
        }

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            Stream.Close();
            Stream.Dispose();
            TcpClient.Close();
            TcpClient.Dispose();
        }

        /// <inheritdoc/>
        public TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            ClearStream();
            SendRequest(request);

            var response = ReadResponse();


            var tc = new TC();
            tc.Read(response);


            return tc;
        }

        /// <summary>
        /// 读取响应
        /// </summary>
        /// <returns></returns>
        protected virtual IResponse ReadResponse()
        {
            var headerByte = ReadLength(ResponseHeaderLength);
            var response = new Response();

            response.SetHeader(headerByte);

            var extra = ReadLength(response.ExtrasLength);
            var key = ReadLength(response.KeyLength);
            var value = ReadLength(response.ValueLength);


            response.SetBody(extra, key, value);

            return response;
        }

        /// <summary>
        /// 发送请求
        /// </summary>
        /// <param name="request">请求内容</param>
        protected virtual void SendRequest(IRequest request)
        {
            request.WriteToStream(Stream);
        }

        /// <summary>
        /// 清空流中的响应
        /// </summary>
        protected virtual void ClearStream()
        {
            while (true)
            {
                var byteInBuffer = TcpClient.Available;
                if (byteInBuffer <= 0)
                    break;

                var buffer = new byte[byteInBuffer];
                Stream.Read(buffer, 0, byteInBuffer);
            }
        }

        /// <summary>
        /// 从流中读取指定长度的数据
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns></returns>
        /// <exception cref="IOException">流已断开</exception>
        protected virtual byte[] ReadLength(int length)
        {
            if (length == 0)
                return Array.Empty<byte>();


            var buffer = new byte[length];
            var position = 0;

            while (position < length)
            {
                var size = Stream.Read(buffer, position, length - position);
                if (size == 0)
                    throw new IOException("Socket Is Close");

                position += size;
            }

            return buffer;
        }
    }
}