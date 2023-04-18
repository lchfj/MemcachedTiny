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
using System.Net.Sockets;

namespace MemcachedTiny.Node
{
    internal class TcpConnection : IConnection
    {
        protected const int ResponseHeaderLength = 24;

        public virtual bool Avaliable => TcpClient?.Connected ?? false;

        protected virtual TcpClient TcpClient { get; }
        protected virtual NetworkStream Stream { get; }

        public TcpConnection(IConnectionEndPoint endPoint)
        {
            TcpClient = CreatTcpClient(endPoint);
            Stream = TcpClient.GetStream();
        }

        protected virtual TcpClient CreatTcpClient(IConnectionEndPoint endPoint)
        {
            var tcp = new TcpClient();

            tcp.Connect(endPoint.Host, endPoint.Port);
            tcp.SendTimeout = 3000;
            tcp.ReceiveTimeout = 3000;

            return tcp;
        }

        public virtual void Dispose()
        {
            Stream.Close();
            Stream.Dispose();
            TcpClient.Close();
            TcpClient.Dispose();
        }

        public TC Execute<TC>(IRequest request) where TC : IResponseReader, new()
        {
            ClearStream();
            SendRequest(request);

            var response = ReadResponse();


            var tc = new TC();
            tc.Read(response);


            return tc;
        }


        protected virtual Response ReadResponse()
        {
            var headerByte = ReadLength(ResponseHeaderLength);
            var header = new Data.ResponseHeader(headerByte);

            var extra = ReadLength(header.ExtraLength);
            var key = ReadLength(header.KeyLength);
            var value = ReadLength(header.ValueLength);

            return new Response(header, extra, key, value);
        }

        protected virtual void SendRequest(IRequest request)
        {
            using var sendStream = request.GetStream();

            sendStream.Position = 0;
            sendStream.CopyTo(Stream);
        }

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

        protected virtual byte[] ReadLength(int length)
        {
            if (length <= 0)
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