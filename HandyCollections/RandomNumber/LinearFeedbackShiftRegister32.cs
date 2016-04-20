using System;
using System.Collections.Generic;

namespace HandyCollections.RandomNumber
{
    /// <summary>
    /// Creates a set of 32 bit numbers which repeats after 2^32 numbers (ie. longest possible period of non repeating numbers)
    /// </summary>
    public class LinearFeedbackShiftRegister32
        : IEnumerable<uint>
    {
        /// <summary>
        /// The number of numbers this sequence will go through before repeating
        /// </summary>
        public const uint PERIOD = uint.MaxValue;

        readonly ushort _repeatThreshold;
        readonly LinearFeedbackShiftRegister16 _mostSignificantBits;

        ushort _lsb;
        readonly LinearFeedbackShiftRegister16 _leastSignificantBits;

        /// <summary>
        /// Initializes a new instance of the <see cref="LinearFeedbackShiftRegister32"/> class.
        /// </summary>
        /// <param name="seed">The seed to initialise the sequence with</param>
        public LinearFeedbackShiftRegister32(uint seed)
        {
            _mostSignificantBits = new LinearFeedbackShiftRegister16((ushort)(seed & ushort.MaxValue));
            _leastSignificantBits = new LinearFeedbackShiftRegister16((ushort)((seed >> 16) & ushort.MaxValue));

            _repeatThreshold = _mostSignificantBits.NextRandom();
            _lsb = _leastSignificantBits.NextRandom();
        }

        /// <summary>
        /// Gets the next random number in the sequence
        /// </summary>
        /// <returns></returns>
        public uint NextRandom()
        {
            ushort msb = _mostSignificantBits.NextRandom();

            if (msb == _repeatThreshold)
                _lsb = _leastSignificantBits.NextRandom();

            int a = msb << 16 | _lsb;

            unsafe
            {
                return *((uint*)&a);
            }
        }

        /// <summary>
        /// Gets the enumerator which will iterate through all the values of this instance without repeating
        /// </summary>
        /// <returns></returns>
        public IEnumerator<uint> GetEnumerator()
        {
            for (uint i = 0; i < PERIOD; i++)
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
            return (this as IEnumerable<uint>).GetEnumerator();
        }
    }
}
