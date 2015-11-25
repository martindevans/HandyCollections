using System;
using System.Collections.Generic;

namespace HandyCollections.Heap
{
    public interface IMinHeap<T>
    {
        int Count { get; }

        T Minimum { get; }

        void Add(T item);

        void Add(IEnumerable<T> items);

        T RemoveMin();

        T RemoveAt(int index);

        int IndexOf(T item);

        int IndexOf(Predicate<T> predicate);

        void Clear();
    }
}
