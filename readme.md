### HandyCollections

A convenient set of generic collections for C#

This project is in active use by myself and I will respond rapidly to bug reports and pull requests.

#### BinaryTree.BinaryTree
A binary tree for quickly storing and retrieving items in O(Log N) time. Values are stored with a key, the same key can be used to retrieve the item (internally using a binary search on the key).

#### BinaryTree.SplayTree
A more advanced binary tree which automatically reorders itself to place recently accessed items nearer the root. If you expect your access pattern frequently use the same keys a splay tree should improve your performance (it's still O(Log N), but the constants for accessing near the root are smaller).

#### BloomFilter.BloomFilter
A set of items which can have items added and supports queries asking if an item has been added to the set. Queries are probabilistic - sometimes it will return false positives (but never false negatives). However, a bloom filter can be many times smaller and faster than a completely accurate set - see [wikipedia](http://en.wikipedia.org/wiki/Bloom_filter) for a more in depth explanation of bloom filters.

#### BloomFilter.CountingBloomFilter
A bloom filter which supports removing items.

#### BloomFilter.ScalableBloomFilter
A bloom filter which expands as more items are added to it, ensuring that the probability of a false positive never exceeds some threshold.

#### Geometry.Octree
An 8-way tree which represents a cuboid of 3 dimensional space. Items can be added with cuboid volume keys and the octree can then answer queries about which items overlap certain volumes.

#### Geometry.Octree
An 4-way tree which represents a rectangle of 2 dimensional space. Items can be added with rectangle area keys and the quadtree can then answer queries about which items overlap certain areas.

#### Heap.MinHeap
A collection of items which makes accessing the smallest item in the collection very fast, often used as a priority queue. Inserting and deleting items can be done in O(Log N) time, while finding the smallest is O(1).

#### Heap.MinMaxHeap
*Currently commented out due to some bugs and no known users!*

A heap which makes accessing both min and max O(1). Insertion and deletion remain unchanged (except slightly higher constants).

#### RandomNumber.LinearFeedbackShiftRegister16
A random number generator which shuffles all 16 bit numbers and returns them in sequence. This guarantees no repeats until the sequence begins again (and then repeats itself perfectly).

#### RandomNumber.LinearFeedbackShiftRegister32
A random number generator which shuffles all 32 bit numbers and returns them in sequence. This guarantees no repeats until the sequence begins again (and then repeats itself perfectly).