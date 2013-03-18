using HandyCollections;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class BloomFilterTest
    {
        [TestMethod]
        public void BasicBloomFilter()
        {
            BloomFilter<int> filter = new BloomFilter<int>(100, 2);

            //10 cannot already be in the collection, so inserting it must succeed
            Assert.IsFalse(filter.Add(10));

            //10 is in the collection
            Assert.IsTrue(filter.Contains(10));

            //check a load more numbers
            for (int i = 0; i < 100; i++)
            {
                filter.Add(i);
                Assert.IsTrue(filter.Contains(i));
            }
        }

        [TestMethod]
        public void FalsePostivieRate()
        {
            //create a new filter to test false positive rate
            var filter = new BloomFilter<int>(100, 0.1f);

            for (int i = 0; i < 99; i++)
            {
                filter.Add(i);
                Assert.IsTrue(filter.Contains(i));
            }

            Assert.IsFalse(filter.FalsePositiveRate > 0.1f);
        }

        [TestMethod]
        public void CountingBloomFilter()
        {
            CountingBloomFilter<int> filter = new CountingBloomFilter<int>(10, 2);

            Assert.IsFalse(filter.Add(10));
            Assert.IsTrue(filter.Contains(10));
            Assert.IsTrue(filter.Remove(10));
            Assert.IsFalse(filter.Contains(10));
        }
    }
}
