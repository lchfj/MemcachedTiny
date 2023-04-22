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
        public virtual bool Avaliable
        {
            get
            {
                if (TcpClient?.Connected ?? false)
                    return true;

                Task.Run(TryConnect);
                return false;
            }
        }

        /// <summary>
        /// 一个TPC连接
        /// </summary>
        protected virtual TcpClient TcpClient { get; set; }
        /// <summary>
        /// 网络流
        /// </summary>
        protected virtual NetworkStream Stream { get; set; }
        /// <summary>
        /// 远端地址
        /// </summary>
        protected virtual IConnectionEndPoint EndPoint { get; }

        /// <summary>
        /// 断线后下次尝试重连的时间
        /// </summary>
        protected virtual long NextTryConnect { get; set; }
        /// <summary>
        /// 日志
        /// </summary>
        protected virtual ILogger<TcpConnection> Logger { get; }

        /// <summary>
        /// 是否正在重连
        /// </summary>
        protected virtual bool OnConnection { get; set; }

        /// <summary>
        /// 创建实例
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="loggerFactory">日志</param>
        public TcpConnection(IConnectionEndPoint endPoint, ILoggerFactory loggerFactory)
        {
            loggerFactory ??= LoggerEmptyFactory.Instance;
            Logger = loggerFactory.CreateLogger<TcpConnection>();

            OnConnection = false;
            EndPoint = endPoint;
            Task.Run(TryConnect);
        }

        /// <summary>
        /// 析构
        /// </summary>
        ~TcpConnection()
        {
            Dispose();
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
            Close();
        }

        /// <summary>
        /// 尝试重连
        /// </summary>
        protected virtual void TryConnect()
        {
            if (DateTime.UtcNow.Ticks < NextTryConnect)
                return;

            if (OnConnection)
                return;
            OnConnection = true;


            Close();
            try
            {
                TcpClient = CreatTcpClient(EndPoint);
                Stream = TcpClient.GetStream();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                Close();
            }
            finally
            {
                OnConnection = false;
                NextTryConnect = DateTime.UtcNow.Ticks + TimeSpan.TicksPerSecond * 5;
            }
        }

        /// <summary>
        /// 关闭现有连接
        /// </summary>
        protected virtual void Close()
        {
            try
            {
                if (Stream is not null)
                {
                    Stream.Close();
                    Stream.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            finally
            {
                Stream = null;
            }

            try
            {
                if (TcpClient is not null)
                {
                    if (TcpClient.Connected)
                        TcpClient.Close();
                    TcpClient.Dispose();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
            }
            finally
            {
                TcpClient = null;
            }
        }


        /// <inheritdoc/>
        public virtual TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            try
            {
                ClearStream();
                SendRequest(request);

                var response = ReadResponse();

                var tc = new TC();
                tc.Read(response);
                return tc;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex);
                return Response.CreatError<TC>(ex.Message);
            }
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