using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HandyCollections.BloomFilter
{
    /// <summary>
    /// A bloom filter which grows as more elements are added to maintain a certain upper error bound
    /// </summary>
    public class ScalableBloomFilter<T>
        :IBloomFilter<T>
    {
        private readonly int _initialCapacity;
        private const double SCALE = 3;     //The constant scaling rate. Larger will query faster but consume memory faster
        private readonly double _ratio;
        private readonly double _falsePositiveProbability;

        private int _lastSlice = -1;
        private readonly List<BloomFilterSlice<T>> _slices = new List<BloomFilterSlice<T>>();

        /// <summary>
        /// Gets the number of items in this filter
        /// </summary>
        public int Count
        {
            get { return _slices.Select(f => f.Count).Sum(); }
        }

        public int SizeInBytes
        {
            get { return _slices.Select(f => f.SizeInBytes).Sum(); }
        }

        /// <summary>
        /// Constructs a new Scalable bloom filter
        /// </summary>
        /// <param name="ratio">The rate at which the false probability shrinks</param>
        /// <param name="initialCapacity">The capacity of the bloom filter to start with</param>
        /// <param name="falsePositiveProbability">The initial probability of a false positive</param>
        public ScalableBloomFilter(double ratio, int initialCapacity, double falsePositiveProbability)
        {
            _ratio = ratio;
            _initialCapacity = initialCapacity;
            _falsePositiveProbability = falsePositiveProbability;
        }

        public bool Add(T item)
        {
            if (Contains(item))
                return true;

            if (IsNewFilterNeeded())
                AddNewSlice();

            GetCurrentActiveSlice().Add(item);
            return false;
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < _slices.Count; i++)
            {
                if (_slices[i].Contains(item))
                    return true;
            }
            return false;
        }

        private bool IsNewFilterNeeded()
        {
            return _lastSlice < 0 || GetCurrentActiveSlice().IsFull();
        }

        private BloomFilterSlice<T> GetCurrentActiveSlice()
        {
            return _slices[_lastSlice];
        }

        private void AddNewSlice()
        {
            _lastSlice++;
            if (_lastSlice >= _slices.Count)
            {
                _slices.Add(new BloomFilterSlice<T>((int) (_initialCapacity * Math.Pow(SCALE, _slices.Count)), _falsePositiveProbability * Math.Pow(_ratio, _slices.Count)));
                _lastSlice = _slices.Count - 1;
            }
        }

        public void Clear()
        {
            _lastSlice = -1;
            for (int i = 0; i < _slices.Count; i++)
                _slices[i].Clear();
        }
    }
}
