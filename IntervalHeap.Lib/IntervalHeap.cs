using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace IntervalHeap.Lib
{
    /// <summary>
    /// Managed implementation of Interval Heap
    /// Source of insperation: http://www.mhhe.com/engcs/compsci/sahni/enrich/c9/interval.pdf
    /// Original paper by J. van Leeuwen and D. Wood
    /// https://academic.oup.com/comjnl/article/36/3/209/311525/Interval-Heaps
    /// </summary>
    public class IntervalHeap
    {
        private readonly List<HeapNode> _heap;

        public int ElementsCount { get; private set; } = 0;

        private int LastNodeIndex => ElementsCount / 2 + ElementsCount % 2 - 1;

        public IntervalHeap()
        {
            _heap = new List<HeapNode>();
        }

        public void Enque(int value)
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

                if (value < _heap[0].LeftBound)
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
                minHeap = value < _heap[lastNode].LeftBound;
            }
            else // even, insert new node
            {
                _heap.Add(new HeapNode());
                lastNode++;

                minHeap = value <= _heap[lastNode >> 1].LeftBound;
            }

            int i = lastNode;

            if (minHeap)
            {
                while (i != 0 && value < _heap[i >> 1].LeftBound)
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
                while (i != 0 && value > _heap[i >> 1].RightBound)
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

        public int FetchMax()
        {
            return _heap[0].RightBound;
        }

        public int FetchMin()
        {
            return _heap[0].LeftBound;
        }

        public int DequeMax()
        {
            var result = this.FetchMax();

            int toReinsert = this.GetItemToReinsert();
            if (_heap.Count == 0)
            {
                return result;
            }

            int current = 0, child = 1;
            while (child <= LastNodeIndex)
            {
                if (child < LastNodeIndex && _heap[child].RightBound < _heap[child + 1].RightBound)
                {
                    child++;
                }

                if (toReinsert >= _heap[child].RightBound) break;

                _heap[current].RightBound = _heap[child].RightBound;
                if (toReinsert < _heap[child].LeftBound)
                {
                    _heap[child].LeftBound = Interlocked.Exchange(ref toReinsert, _heap[child].LeftBound);
                }
                current = child;
                child = child << 1; //*=2
            }

            _heap[current].RightBound = toReinsert;

            return result;
        }

        public int DequeMin()
        {
            var result =  this.FetchMin();

            int toReinsert = this.GetItemToReinsert();

            if (_heap.Count == 0)
            {
                return result;
            }

            int current = 0, child = 1;
            while (child <= LastNodeIndex)
            {
                if (child < LastNodeIndex && _heap[child].LeftBound > _heap[child + 1].LeftBound)
                {
                    child++;
                }

                if (toReinsert <= _heap[child].LeftBound) break;

                _heap[current].LeftBound = _heap[child].LeftBound;
                if (toReinsert > _heap[child].RightBound)
                {
                    _heap[child].RightBound = Interlocked.Exchange(ref toReinsert, _heap[child].RightBound);
                }
                current = child;
                child = child << 1; //*=2
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

        private int GetItemToReinsert()
        {
            var lastNode = LastNodeIndex;
            int toReinsert;
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
            public int LeftBound { get; set; }
            public int RightBound { get; set; }
        }
    }
}
