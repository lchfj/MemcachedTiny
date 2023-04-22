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
    /// 日志接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ILogger<T>
    {
        /// <summary>
        /// 指定等级的日志是否开启
        /// </summary>
        /// <param name="logLevel">等级</param>
        /// <returns></returns>
        bool IsEnabled(LogLevel logLevel);
        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="logLevel">等级</param>
        /// <param name="exception">异常</param>
        /// <param name="message">消息</param>
        /// <param name="args">消息参数</param>
        void Log(LogLevel logLevel, Exception exception, string message, params object[] args);
    }
}