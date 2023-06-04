# MemcachedTiny

`MemcachedTiny` 是一个简单高速的 Memcached .net 客户端，仅提供基本操作，不依赖其它任何第三方组件。

`MemcachedTiny` is a simple and high-speed Memcached dotnet client that only provides basic operations and does not depend on any other third-party components.

---

Copyright (C) 2023 lchfj.cn

`MemcachedTiny` is free software: you can redistribute it and/or modify it under the terms of the GNU Lesser General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.

`MemcachedTiny` is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public License along with `MemcachedTiny`. If not, see <https://www.gnu.org/licenses/>.

---

## 特点

&nbsp;

扩展友好：预留扩展接口，所有方法支持重写，部分类可通过设定直接替换。

连接方式：TCP（不支持 SSL 或 TLS 以及 SASL认证）

分布式：支持

协议：二进制协议

方法：SET、GET、TOUCH、GAT、DELETE、FLUSH

&nbsp;

Extension friendly: Reserved extension interfaces, all methods support rewriting, and some classes can be directly replaced by setting.

Connection Type: TCP (no SSL or TLS or SASL Authentication)

Distributed: Yes

Protocol: Binary Protocol

Methods: SET, GET, TOUCH, GAT, DELETE、FLUSH
