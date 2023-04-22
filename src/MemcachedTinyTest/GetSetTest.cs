namespace MemcachedTinyTest
{
    [TestClass]
    public class GetSetTest : BasicClass
    {
        [TestMethod]
        public void TestSync()
        {
            SyncOne();
        }

        [TestMethod]
        public virtual void TestAsync()
        {
            AsyncOne().Wait();
        }
    }
}