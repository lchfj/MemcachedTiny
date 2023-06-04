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

namespace MemcachedTiny.Logging
{
    /// <summary>
    /// 空白日志
    /// </summary>
    public sealed class LoggerEmptyFactory : ILoggerFactory
    {

        private class Logger<T> : ILogger<T>
        {
            public bool IsEnabled(LogLevel logLevel)
            {
                return false;
            }

            public void Log(LogLevel logLevel, Exception? exception, string? message, params object[]? args)
            {
            }
        }

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static LoggerEmptyFactory Instance { get; } = new();

        private LoggerEmptyFactory()
        {
        }

        /// <inheritdoc/>
        public ILogger<T> CreateLogger<T>()
        {
            return new Logger<T>();
        }
    }
}