namespace MemcachedTinyTest
{
    [TestClass]
    public class FlushTest : BasicClass
    {
        [TestMethod]
        public void FlushSync()
        {
            var flag = Random.Shared.Next();
            var value = Guid.NewGuid();
            var key = value.ToString();

            var setResult = Memcached.Set(key, flag, 1000, value.ToByteArray());
            Assert.IsTrue(setResult.Success);

            {
                var getResult = Memcached.Get(key);

                Assert.IsTrue(getResult.Success);
                Assert.AreEqual(flag, getResult.Flags);

                var newG = new Guid(getResult.Value);
                Assert.AreEqual(value, newG);
            }

            var deleteResult = Memcached.Flush(0);
            Assert.IsTrue(deleteResult.Success);

            {
                var getResult = Memcached.Get(key);

                Assert.IsFalse(getResult.Success);
                Assert.AreEqual(getResult.Status, 0x0001);
            }
        }
        [TestMethod]
        public void FlushDelaySync()
        {
            var flag = Random.Shared.Next();
            var value = Guid.NewGuid();
            var key = value.ToString();

            var setResult = Memcached.Set(key, flag, 1000, value.ToByteArray());
            Assert.IsTrue(setResult.Success);

            {
                var getResult = Memcached.Get(key);

                Assert.IsTrue(getResult.Success);
                Assert.AreEqual(flag, getResult.Flags);

                var newG = new Guid(getResult.Value);
                Assert.AreEqual(value, newG);
            }

            var deleteResult = Memcached.Flush(2);
            Assert.IsTrue(deleteResult.Success);

            {
                var getResult = Memcached.Get(key);

                Assert.IsTrue(getResult.Success);
                Assert.AreEqual(flag, getResult.Flags);

                var newG = new Guid(getResult.Value);
                Assert.AreEqual(value, newG);
            }

            Thread.Sleep(3000);

            {
                var getResult = Memcached.Get(key);

                Assert.IsFalse(getResult.Success);
                Assert.AreEqual(getResult.Status, 0x0001);
            }
        }
    }
}