//using System;
//using System.Text;
//using System.Collections.Generic;
//using System.Linq;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using HandyCollections.Extensions;

//namespace HandyCollectionsTest
//{
//    [TestClass]
//    public class Extensions
//    {
//        [TestMethod]
//        public void TestMethod1()
//        {

//        }

//        public static void CorrectnessTest()
//        {
//            Console.WriteLine("Testing selection on an in order list");
//            TestSelection(new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 });

//            Console.WriteLine("Testing selection on a reversed list");
//            TestSelection(new int[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 });

//            Console.WriteLine("Testing selection on a random order list");
//            TestSelection(new int[] { 1, 32, 7, 45, 56, 980, 23, 29580, 3857, 1 });
//        }

//        private static void TestSelection(IList<int> values)
//        {
//            List<int> sorted = new List<int>(values);
//            sorted.Sort();

//            for (int i = 0; i < values.Count; i++)
//            {
//                Assert.IsTrue(sorted[i] == values[values.OrderSelect(i)], "Incorrect order select, (sorted = " + sorted[i] + ") != (OrderSelect = " + values.OrderSelect(i) + ")");
//            }
//        }
//    }
//}
