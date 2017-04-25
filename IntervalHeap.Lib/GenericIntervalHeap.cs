using System.Collections.Generic;
using System.Threading;

namespace IntervalHeap.Lib
{
    /// <summary>
    /// Generic managed implementation of Interval Heap
    /// Source of insperation: http://www.mhhe.com/engcs/compsci/sahni/enrich/c9/interval.pdf
    /// Original paper by J. van Leeuwen and D. Wood
    /// https://academic.oup.com/comjnl/article/36/3/209/311525/Interval-Heaps
    /// </summary>
    public class IntervalHeap<T> where T : class
    {
        private readonly List<HeapNode> _heap;

        public int ElementsCount { get; private set; } = 0;

        private int LastNodeIndex => ElementsCount / 2 + ElementsCount % 2 - 1;

        private readonly IComparer<T> _comparer;

        public IntervalHeap(IComparer<T> comparator)
        {
            _heap = new List<HeapNode>();
            _comparer = comparator;
        }

        public void Enque(T value)
        {
            if (ElementsCount < 2)
            {
                if (ElementsCount == 0)
                {
                    var rootNode = new HeapNode
                    {
                        LeftBound = value,
                        RightBound = value
                    };
                    _heap.Add(rootNode);
                    ElementsCount++;
                    return;
                }

                if (_comparer.Compare(value, _heap[0].LeftBound) < 0)
                {
                    _heap[0].LeftBound = value;
                }
                else
                {
                    _heap[0].RightBound = value;
                }
                ElementsCount++;
                return;
            }

            int lastNode = LastNodeIndex;
            bool minHeap;
            if (ElementsCount % 2 == 1) //odd, last node have place to insert value
            {
                minHeap = _comparer.Compare(value, _heap[lastNode].LeftBound) < 0; // value < _heap[lastNode].LeftBound;
            }
            else // even, insert new node
            {
                _heap.Add(new HeapNode());
                lastNode++;

                minHeap = _comparer.Compare(value, _heap[lastNode >> 1].LeftBound) <= 0;
            }

            int i = lastNode;

            if (minHeap)
            {
                while (i != 0 && _comparer.Compare(value, _heap[i >> 1].LeftBound) < 0)
                {
                    _heap[i].LeftBound = _heap[i >> 1].LeftBound;
                    i = i >> 1;
                }
                _heap[i].LeftBound = value;
                ElementsCount++;

                if (ElementsCount % 2 == 1)
                {
                    _heap[lastNode].RightBound = _heap[lastNode].LeftBound;
                }
            }
            else
            {
                while (i != 0 && _comparer.Compare(value, _heap[i >> 1].RightBound) > 0)
                {
                    _heap[i].RightBound = _heap[i >> 1].RightBound;
                    i = i >> 1; // /=2
                }
                _heap[i].RightBound = value;
                ElementsCount++;

                if (ElementsCount % 2 == 1)
                {
                    _heap[lastNode].LeftBound = _heap[lastNode].RightBound;
                }
            }
        }

        public T FetchMax()
        {
            return _heap[0].RightBound;
        }

        public T FetchMin()
        {
            return _heap[0].LeftBound;
        }

        public T DequeMax()
        {
            var result = this.FetchMax();

            T toReinsert = this.GetItemToReinsert();
            if (_heap.Count == 0)
            {
                return result;
            }

            int current = 0, child = 1;
            while (child <= LastNodeIndex)
            {
                if (child < LastNodeIndex && _comparer.Compare(_heap[child].RightBound, _heap[child + 1].RightBound) <
                    0)
                {
                    child++;
                }

                if (_comparer.Compare(toReinsert, _heap[child].RightBound) >= 0) break;

                _heap[current].RightBound = _heap[child].RightBound;
                if (_comparer.Compare(toReinsert, _heap[child].LeftBound) < 0)
                {
                    _heap[child].LeftBound = Interlocked.Exchange(ref toReinsert, _heap[child].LeftBound);
                }
                current = child;
                child = child << 1;
            }

            _heap[current].RightBound = toReinsert;

            return result;
        }

        public T DequeMin()
        {
            var result = this.FetchMin();

            T toReinsert = this.GetItemToReinsert();

            if (_heap.Count == 0)
            {
                return result;
            }

            int current = 0, child = 1;
            while (child <= LastNodeIndex)
            {
                if (child < LastNodeIndex && _comparer.Compare(_heap[child].LeftBound, _heap[child + 1].LeftBound) > 0)
                {
                    child++;
                }

                if (_comparer.Compare(toReinsert, _heap[child].LeftBound) <= 0) break;

                _heap[current].LeftBound = _heap[child].LeftBound;
                if (_comparer.Compare(toReinsert, _heap[child].RightBound) > 0)
                {
                    _heap[child].RightBound = Interlocked.Exchange(ref toReinsert, _heap[child].RightBound);
                }
                current = child;
                child = child << 1;
            }

            if (current == LastNodeIndex && ElementsCount % 2 == 1)
            {
                _heap[LastNodeIndex].LeftBound = _heap[LastNodeIndex].RightBound;
            }
            else
            {
                _heap[current].LeftBound = toReinsert;
            }

            return result;
        }

        public void Clear()
        {
            ElementsCount = 0;
            _heap.Clear();
        }

        private T GetItemToReinsert()
        {
            var lastNode = LastNodeIndex;
            T toReinsert;
            if (ElementsCount % 2 == 1) // remove last node
            {
                toReinsert = _heap[lastNode].LeftBound;
                _heap.RemoveAt(lastNode);
            }
            else
            {
                toReinsert = _heap[lastNode].RightBound;
                _heap[lastNode].RightBound = _heap[lastNode].LeftBound;
            }
            ElementsCount--;
            return toReinsert;
        }

        private class HeapNode
        {
            public T LeftBound { get; set; }
            public T RightBound { get; set; }
        }
    }
}
