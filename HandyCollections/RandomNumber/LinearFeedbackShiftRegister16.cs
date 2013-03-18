using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandyCollections.RandomNumber
{
    /// <summary>
    /// Creates a set of 16 bit numbers which repeats after 2^16 numbers (ie. longest possible period of non repeating numbers)
    /// </summary>
    public class LinearFeedbackShiftRegister16
        :IEnumerable<UInt16>
    {
        #region fields
        /// <summary>
        /// The number of numbers this sequence will go through before repeating
        /// </summary>
        public const int PERIOD = UInt16.MaxValue;

        private UInt16 lfsr;
        private UInt16 bit;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearFeedbackShiftRegister16"/> class.
        /// </summary>
        /// <param name="seed">The seed to initialise the sequence with</param>
        public LinearFeedbackShiftRegister16(UInt16 seed)
        {
            lfsr = seed;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearFeedbackShiftRegister16"/> class.
        /// </summary>
        public LinearFeedbackShiftRegister16()
            :this((ushort)StaticRandomNumber.Random())
        {
            
        }
        #endregion

        /// <summary>
        /// Gets the next random number in the sequence
        /// </summary>
        /// <returns></returns>
        public UInt16 NextRandom()
        {
            bit = (UInt16)(((lfsr >> 0) ^ (lfsr >> 2) ^ (lfsr >> 3) ^ (lfsr >> 5)) & 1);
            lfsr = (UInt16)((lfsr >> 1) | (bit << 15));

            return lfsr;
        }

        /// <summary>
        /// Checks if this is implemented correctly
        /// </summary>
        public static void CorrectnessTest()
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

            if (period != PERIOD)
                throw new Exception("Period is incorrect");
        }

        #region IEnumerable
        /// <summary>
        /// Gets the enumerator which will iterate through all the values of this instance without repeating
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ushort> GetEnumerator()
        {
            for (int i = 0; i < PERIOD; i++)
                yield return NextRandom();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<UInt16>).GetEnumerator();
        }
        #endregion
    }
}
