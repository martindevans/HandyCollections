using HandyCollections.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest.Extensions
{
    [TestClass]
    public class ArrayExtensionsTest
    {
        private readonly int[] _array = {
            1, 7, 10, 234, 12, 235, 213, 35, 475, 352, 21, 76, 987, 576, 64, 85, 253, 12, 634, 765, 45, 24, 31, 365, 5876, 457, 4, 643, 765, 64, 42, 13, 65
        };

        private void AssertPartitioned(int pivotIndex, int pivotValue)
        {
            for (var i = 0; i < _array.Length; i++)
            {
                if (i < pivotIndex)
                    Assert.IsTrue(_array[i] < pivotValue);
                else
                    Assert.IsTrue(_array[i] >= pivotValue);
            }
        }

        [TestMethod]
        public void AssertThat_PartitionIntegers_PartitionsUnorderedArray_WithElement()
        {
            var pivot = _array.Partition(0, _array.Length - 1, 23);

            AssertPartitioned(pivot, _array[pivot]);
        }

        [TestMethod]
        public void AssertThat_PartitionIntegers_PartitionsUnorderedArray_WithFunc()
        {
            var pivot = _array.Partition(a => a < 350, 0, _array.Length - 1);

            AssertPartitioned(pivot, 350);
        }
    }
}
