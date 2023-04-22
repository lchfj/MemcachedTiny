namespace MemcachedTinyTest
{
    public abstract class BasicClass
    {
        public IMemcachedClient Memcached { get; }

        protected BasicClass()
        {
            Memcached = CreatClient();
        }

        protected virtual IMemcachedClient CreatClient()
        {
            return new MemcachedClient(new MemcachedClientSetting()
            {
                Connect = new List<string>() { "127.0.0.1:11211" }
            });
        }

        protected virtual void SyncOne()
        {
            var flag = Random.Shared.Next();
            var value = Guid.NewGuid();
            var key = value.ToString();

            var setResult = Memcached.Set(key, flag, 150, value.ToByteArray());
            Assert.IsTrue(setResult.Success);

            var getResult = Memcached.Get(key);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(flag, getResult.Flags);

            var newG = new Guid(getResult.Value);
            Assert.AreEqual(value, newG);
        }

        protected virtual async Task AsyncOne()
        {
            var flag = Random.Shared.Next();
            var value = Guid.NewGuid();
            var key = value.ToString();

            var setResult = await Memcached.SetAsync(key, flag, 150, value.ToByteArray()).ConfigureAwait(false);
            Assert.IsTrue(setResult.Success);

            var getResult = await Memcached.GetAsync(key).ConfigureAwait(false);
            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(flag, getResult.Flags);

            var newG = new Guid(getResult.Value);
            Assert.AreEqual(value, newG);
        }
    }
}
