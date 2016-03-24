using HandyCollections.BloomFilter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class BloomFilterTest
    {
        [TestMethod]
        public void BasicBloomFilterCorrectlyActsAsASet()
        {
            BloomFilter<int> filter = new BloomFilter<int>(100, 2);

            //10 cannot already be in the collection, so inserting it must succeed
            Assert.IsFalse(filter.Add(10));
            Assert.IsTrue(filter.Add(10));

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
        public void FalsePostiveRateCrossesThresholdAtCorrectCount()
        {
            var filter = new BloomFilter<int>(100, 0.1f);

            for (int i = 0; i < 99; i++)
            {
                filter.Add(i);
                Assert.IsTrue(filter.Contains(i));
            }

            Assert.IsFalse(filter.FalsePositiveRate > 0.1f);

            filter.Add(1000);
            filter.Add(1001);
            filter.Add(1002);

            Assert.IsTrue(filter.FalsePositiveRate > 0.1f);
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

        [TestMethod]
        public void RollbackWhenRemovingANonExistantItem()
        {
            var a = new CountingBloomFilter<int>(1000, 0.01f);

            for (var i = 0; i < 1000; i++)
                a.Add(i);

            byte[] copy = (byte[])a.Array.Clone();

            Assert.IsFalse(a.Remove(1001));

            for (int i = 0; i < a.Array.Length; i++)
            {
                Assert.AreEqual(a.Array[i], copy[i]);
            }
        }

        [TestMethod]
        public void ScalableBloomFilterCorrectlyActsAsASet()
        {
            IBloomFilter<int> filter = new ScalableBloomFilter<int>(0.9, 10, 0.001);

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
        public void ClearingScalabeBloomFilterResetsEverything()
        {
            ScalableBloomFilter<int> filter = new ScalableBloomFilter<int>(0.9, 1000, 0.001);

            //Add a load of numbers
            for (int i = 0; i < 100; i++)
            {
                filter.Add(i);
                Assert.IsTrue(filter.Contains(i));
            }
            Assert.AreEqual(100, filter.Count);

            filter.Clear();
            Assert.AreEqual(0, filter.Count);

            //Add them again
            for (int i = 0; i < 100; i++)
            {
                filter.Add(i);
                Assert.IsTrue(filter.Contains(i));
            }
            Assert.AreEqual(100, filter.Count);
        }
    }
}
