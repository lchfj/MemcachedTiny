namespace MemcachedTinyTest
{
    [TestClass]
    public class DeleteTest : BasicClass
    {
        [TestMethod]
        public void DeleteSync()
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


            var deleteResult = Memcached.Delete(key);
            Assert.IsTrue(deleteResult.Success);

            {
                var getResult = Memcached.Get(key);

                Assert.IsFalse(getResult.Success);
                Assert.AreEqual(getResult.Status, 0x0001);
            }
        }
        [TestMethod]
        public void DeleteAfterExpireSync()
        {
            var flag = Random.Shared.Next();
            var value = Guid.NewGuid();
            var key = value.ToString();


            var setResult = Memcached.Set(key, flag, 2, value.ToByteArray());
            Assert.IsTrue(setResult.Success);

            {
                var getResult = Memcached.Get(key);

                Assert.IsTrue(getResult.Success);
                Assert.AreEqual(flag, getResult.Flags);

                var newG = new Guid(getResult.Value);
                Assert.AreEqual(value, newG);
            }

            Thread.Sleep(3000);

            var deleteResult = Memcached.Delete(key);
            Assert.IsTrue(deleteResult.Success);
            Assert.AreEqual(deleteResult.Status, 0x0001);

            {
                var getResult = Memcached.Get(key);

                Assert.IsFalse(getResult.Success);
                Assert.AreEqual(getResult.Status, 0x0001);
            }
        }
    }
}