using System;
using System.Collections.Generic;
using System.Linq;
using HandyCollections.RandomNumber;
using HandyCollections.Set;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest.Set
{
    [TestClass]
    public class OrderedSetTest
    {
        private readonly OrderedSet<int> _set = new OrderedSet<int>();

        [TestMethod]
        public void AssertThat_AfterAddingItems_SetContainsItem()
        {
            ((ICollection<int>)_set).Add(1);

            Assert.IsTrue(_set.Contains(1));
        }

        [TestMethod]
        public void AssertThat_AfterClear_SetDoesNotContainItems()
        {
            for (var i = 0; i < 100; i++)
                _set.Add(i);

            _set.Clear();

            for (var i = 0; i < 100; i++)
                Assert.IsFalse(_set.Contains(i));
        }

        [TestMethod]
        public void AssertThat_SetDoesNotContainNonAddedItems()
        {
            _set.Add(1);

            Assert.IsFalse(_set.Contains(2));
        }

        [TestMethod]
        public void AssertThat_SetCount_IsNumberOfItemsAddedMinusRemoved()
        {
            for (var i = 0; i < 100; i++)
            {
                _set.Add(i);
                Assert.AreEqual(i + 1, _set.Count);
            }

            for (var i = 0; i < 50; i++)
            {
                Assert.IsTrue(_set.Remove(i));
                Assert.AreEqual(100 - i - 1, _set.Count);
            }
        }

        [TestMethod]
        public void AssertThat_ItemCannotBeAddedTwice()
        {
            Assert.IsTrue(_set.Add(1));
            Assert.IsFalse(_set.Add(1));
        }

        [TestMethod]
        public void AssertThat_ExceptWith_RemovesSpecifiedItems()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            _set.ExceptWith(new int[] { 1, 2, 3 });

            Assert.IsFalse(_set.Contains(1));
            Assert.IsFalse(_set.Contains(2));
            Assert.IsFalse(_set.Contains(3));

            Assert.IsTrue(_set.Contains(4));
            Assert.IsTrue(_set.Contains(5));
            Assert.IsTrue(_set.Contains(6));
            Assert.IsTrue(_set.Contains(7));
            Assert.IsTrue(_set.Contains(8));
            Assert.IsTrue(_set.Contains(9));
        }

        [TestMethod]
        public void AssertThat_IntersectWith_Intersects()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            _set.IntersectWith(Enumerable.Range(7, 5));

            for (var i = 0; i < 6; i++)
                Assert.IsFalse(_set.Contains(i));
            for (var i = 7; i < 10; i++)
                Assert.IsTrue(_set.Contains(i));
            for (var i = 10; i < 12; i++)
                Assert.IsFalse(_set.Contains(i));
        }

        [TestMethod]
        public void AssertThat_Enumerable_IsOrderedByInsertionOrder()
        {
            var r = new LinearFeedbackShiftRegister32(1);
            for (var i = 0; i < 100; i++)
                _set.Add((int)r.NextRandom());

            var r2 = new LinearFeedbackShiftRegister32(1);
            foreach (var item in _set)
                Assert.AreEqual((int)r2.NextRandom(), item);
        }

        [TestMethod]
        public void AssertThat_CopyTo_IsOrderedByInsertionOrder()
        {
            var r = new LinearFeedbackShiftRegister32(1);
            for (var i = 0; i < 100; i++)
                _set.Add((int)r.NextRandom());

            var items = new int[100];
            _set.CopyTo(items, 0);

            var r2 = new LinearFeedbackShiftRegister32(1);
            foreach (var item in items)
                Assert.AreEqual((int)r2.NextRandom(), item);
        }

        [TestMethod]
        public void AssertThat_IsNotReadonly()
        {
            Assert.IsFalse(_set.IsReadOnly);
        }

        [TestMethod]
        public void AssertThat_SetIsProperSubSet_OfProperSuperset()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsTrue(_set.IsProperSubsetOf(Enumerable.Range(0, 100)));
        }

        [TestMethod]
        public void AssertThat_SetIsNotProperSubSet_OfSameSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsFalse(_set.IsProperSubsetOf(_set));
        }

        [TestMethod]
        public void AssertThat_SetIsNotProperSubSet_OfSubSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsFalse(_set.IsProperSubsetOf(Enumerable.Range(0, 5)));
        }

        [TestMethod]
        public void AssertThat_SetIsSubSet_OfSuperset()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsTrue(_set.IsSubsetOf(Enumerable.Range(0, 100)));
        }

        [TestMethod]
        public void AssertThat_SetIsSubSet_OfSameSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsTrue(_set.IsSubsetOf(_set));
        }

        [TestMethod]
        public void AssertThat_SetIsNotSubSet_OfSubSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsFalse(_set.IsSubsetOf(Enumerable.Range(0, 5)));
        }

        [TestMethod]
        public void AssertThat_SetIsSuperSet_OfSubSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsTrue(_set.IsSupersetOf(Enumerable.Range(0, 5)));
        }

        [TestMethod]
        public void AssertThat_SetIsSuperSet_OfSameSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsTrue(_set.IsSupersetOf(_set));
        }

        [TestMethod]
        public void AssertThat_SetIsNotSuperSet_OfSuperSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsFalse(_set.IsSupersetOf(Enumerable.Range(0, 15)));
        }

        [TestMethod]
        public void AssertThat_SetIsProperSuperSet_OfSubSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsTrue(_set.IsProperSupersetOf(Enumerable.Range(0, 5)));
        }

        [TestMethod]
        public void AssertThat_SetIsNotProperSuperSet_OfSameSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsFalse(_set.IsProperSupersetOf(_set));
        }

        [TestMethod]
        public void AssertThat_SetIsNotProperSuperSet_OfSuperSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsFalse(_set.IsProperSupersetOf(Enumerable.Range(0, 15)));
        }

        [TestMethod]
        public void AssertThat_SetEquals_EquivalentSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsTrue(_set.SetEquals(Enumerable.Range(0, 10)));
        }

        [TestMethod]
        public void AssertThat_NotSetEquals_NonEquivalentSet()
        {
            for (var i = 0; i < 10; i++)
                _set.Add(i);

            Assert.IsFalse(_set.SetEquals(Enumerable.Range(0, 11)));
        }

        [TestMethod]
        public void AssertThat_UnionWith_AddsAllItems()
        {
            _set.Add(1);

            _set.UnionWith(new[] {
                2, 3, 4
            });

            Assert.IsTrue(_set.Contains(1));
            Assert.IsTrue(_set.Contains(2));
            Assert.IsTrue(_set.Contains(3));
            Assert.IsTrue(_set.Contains(4));

            Assert.IsFalse(_set.Contains(5));
        }

        [TestMethod]
        public void AssertThat_Overlaps_WithOverlappingArray()
        {
            _set.Add(1);
            _set.Add(2);
            _set.Add(3);

            Assert.IsTrue(_set.Overlaps(new int[] {
                2
            }));
        }

        [TestMethod]
        public void AssertThat_DoesNotOverlap_WithNonOverlappingArray()
        {
            _set.Add(1);
            _set.Add(2);
            _set.Add(3);

            Assert.IsFalse(_set.Overlaps(new int[] {
                4
            }));
        }

        [TestMethod]
        public void AssertThat_SymmetricExceptWith_RemovesItemsInBothSets()
        {
            _set.Add(1);
            _set.Add(2);

            _set.SymmetricExceptWith(new[] {
                2, 3
            });

            Assert.IsFalse(_set.Contains(2));
            Assert.IsTrue(_set.Contains(1));
        }

        [TestMethod]
        public void AssertThat_SymmetricExceptWith_AddsItemsInOtherSet()
        {
            _set.Add(1);
            _set.Add(2);

            _set.SymmetricExceptWith(new[] {
                2, 3
            });

            Assert.IsTrue(_set.Contains(3));
        }
    }
}
