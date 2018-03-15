using System.Collections.Generic;

namespace HandyCollections.RandomNumber
{
    /// <summary>
    /// Creates a set of 16 bit numbers which repeats after 2^16 numbers (ie. longest possible period of non repeating numbers)
    /// </summary>
    public class LinearFeedbackShiftRegister16
        :IEnumerable<ushort>
    {
        #region fields
        /// <summary>
        /// The number of numbers this sequence will go through before repeating
        /// </summary>
        public const int Period = ushort.MaxValue;

        private ushort _lfsr;
        private ushort _bit;
        #endregion

        #region constructors
        /// <summary>
        /// Initializes a new instance of the <see cref="LinearFeedbackShiftRegister16"/> class.
        /// </summary>
        /// <param name="seed">The seed to initialise the sequence with</param>
        public LinearFeedbackShiftRegister16(ushort seed)
        {
            if (seed == 0)
                seed++;
            _lfsr = seed;
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
        public ushort NextRandom()
        {
            _bit = (ushort)(((_lfsr >> 0) ^ (_lfsr >> 2) ^ (_lfsr >> 3) ^ (_lfsr >> 5)) & 1);
            _lfsr = (ushort)((_lfsr >> 1) | (_bit << 15));

            return _lfsr;
        }

        #region IEnumerable
        /// <summary>
        /// Gets the enumerator which will iterate through all the values of this instance without repeating
        /// </summary>
        /// <returns></returns>
        public IEnumerator<ushort> GetEnumerator()
        {
            for (var i = 0; i < Period; i++)
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
            return (this as IEnumerable<ushort>).GetEnumerator();
        }
        #endregion
    }
}
