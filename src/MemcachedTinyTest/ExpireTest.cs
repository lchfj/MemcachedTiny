namespace MemcachedTinyTest
{
    [TestClass]
    public class ExpireTest : BasicClass
    {
        [TestMethod]
        public void ExpireSync()
        {
            var flag = Random.Shared.Next();
            var value = Guid.NewGuid();
            var key = value.ToString();

            var setResult = Memcached.Set(key, flag, 3, value.ToByteArray());

            Assert.IsTrue(setResult.Success);

            Thread.Sleep(5000);

            var getResult = Memcached.Get(key);

            Assert.IsFalse(getResult.Success);
            Assert.AreEqual(getResult.Status, 0x0001);
        }

        [TestMethod]
        public void Touch()
        {
            var flag = Random.Shared.Next();
            var value = Guid.NewGuid();
            var key = value.ToString();

            var setResult = Memcached.Set(key, flag, 1, value.ToByteArray());
            Assert.IsTrue(setResult.Success);


            var touchResult = Memcached.Touch(key, 5);
            Assert.IsTrue(touchResult.Success);

            Thread.Sleep(3000);

            var getResult = Memcached.Get(key);

            Assert.IsTrue(getResult.Success);
            Assert.AreEqual(flag, getResult.Flags);

            var newG = new Guid(getResult.Value);
            Assert.AreEqual(value, newG);
        }

        [TestMethod]
        public void GetAndTouch()
        {
            var flag = Random.Shared.Next();
            var value = Guid.NewGuid();
            var key = value.ToString();

            var setResult = Memcached.Set(key, flag, 1, value.ToByteArray());
            Assert.IsTrue(setResult.Success);


            var touchResult = Memcached.GetAndTouch(key, 5);
            {
                Assert.IsTrue(touchResult.Success);
                Assert.AreEqual(flag, touchResult.Flags);

                var newG = new Guid(touchResult.Value);
                Assert.AreEqual(value, newG);
            }


            Thread.Sleep(3000);

            var getResult = Memcached.Get(key);
            {
                Assert.IsTrue(getResult.Success);
                Assert.AreEqual(flag, getResult.Flags);

                var newG = new Guid(getResult.Value);
                Assert.AreEqual(value, newG);
            }
        }
    }
}