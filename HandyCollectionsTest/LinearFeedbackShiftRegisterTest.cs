using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HandyCollections.RandomNumber;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HandyCollectionsTest
{
    [TestClass]
    public class LinearFeedbackShiftRegisterTest
    {
        [TestMethod]
        public static void LinearFeedbackShiftRegister16LoopsAfter2Pow16Values()
        {
            LinearFeedbackShiftRegister16 r = new LinearFeedbackShiftRegister16();

            UInt16 first = r.NextRandom();
            UInt16 value;
            int period = 0;

            do
            {
                value = r.NextRandom();
                ++period;
            } while (value != first);

            Assert.AreEqual(LinearFeedbackShiftRegister16.PERIOD, period);
        }

        [TestMethod]
        public void LinearFeedbackShiftRegister16EnumerableReturnsCorrectValues()
        {
            LinearFeedbackShiftRegister16 a = new LinearFeedbackShiftRegister16(123);
            LinearFeedbackShiftRegister16 b = new LinearFeedbackShiftRegister16(123);

            foreach (var value in b)
                Assert.AreEqual(a.NextRandom(), value);
        }

        [TestMethod]
        public static void LinearFeedbackShiftRegister32LoopsAfter2Pow32Values()
        {
            LinearFeedbackShiftRegister32 r = new LinearFeedbackShiftRegister32(3452);

            HashSet<uint> values = new HashSet<uint>();

            for (int i = 0; i < UInt16.MaxValue * 5; i++)
            {
                Assert.IsTrue(values.Add(r.NextRandom()));
            }
        }

        [TestMethod]
        public void LinearFeedbackShiftRegister32EnumerableReturnsCorrectValues()
        {
            LinearFeedbackShiftRegister32 a = new LinearFeedbackShiftRegister32(123);
            LinearFeedbackShiftRegister32 b = new LinearFeedbackShiftRegister32(123);

            int i = 0;
            foreach (var value in b)
            {
                i++;
                if (i > UInt16.MaxValue * 5)
                    break;

                Assert.AreEqual(a.NextRandom(), value);
            }
        }
    }
}
