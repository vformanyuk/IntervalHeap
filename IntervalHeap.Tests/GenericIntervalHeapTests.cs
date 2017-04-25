using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IntervalHeap.Tests
{
    [TestClass]
    public class GenericIntervalHeapTests
    {
        private Lib.IntervalHeap<ObjectWithOrder> _heap;
        private int[] _random100k;

        [TestInitialize]
        public void Setup()
        {
            _heap = new Lib.IntervalHeap<ObjectWithOrder>(new ObjectWithOrderComparer());
            _random100k = new int[100000];

            Random rnd = new Random((int)DateTime.Now.Ticks);
            for (int i = 0; i < _random100k.Length; i++)
            {
                var next = rnd.Next();
                _random100k[i] = next;
                _heap.Enque(new ObjectWithOrder
                {
                    Data = new object(),
                    Order = next
                });
            }
        }

        [TestMethod]
        public void TestMinHeap()
        {
            Assert.AreEqual(_random100k.Min(), _heap.FetchMin().Order);
            var length = _heap.ElementsCount;
            var min = _heap.DequeMin();
            Assert.AreEqual(length - 1, _heap.ElementsCount);
        }

        [TestMethod]
        public void TestMaxHeap()
        {
            Assert.AreEqual(_random100k.Max(), _heap.FetchMax().Order);
            var length = _heap.ElementsCount;
            var max = _heap.DequeMax();
            Assert.AreEqual(length - 1, _heap.ElementsCount);
        }

        [TestMethod]
        public void ValidateMaxHeap()
        {
            int prev = Int32.MaxValue, curr;
            for (int i = 0; i < _random100k.Length; i++)
            {
                curr = _heap.DequeMax().Order;
                Assert.AreEqual(true, curr <= prev);
                prev = curr;
            }
        }

        [TestMethod]
        public void ValidateMinHeap()
        {
            int prev = Int32.MinValue, curr;
            for (int i = 0; i < _random100k.Length; i++)
            {
                curr = _heap.DequeMin().Order;
                Assert.AreEqual(true, curr >= prev);
                prev = curr;
            }
        }

        [TestMethod]
        public void Measure100kEnques()
        {
            var heap = new Lib.IntervalHeap();
            for (int i = 0; i < _random100k.Length; i++)
            {
                heap.Enque(_random100k[i]);
            }
            Assert.AreEqual(_random100k.Length, heap.ElementsCount);
        }

        [TestMethod]
        public void Measure100kDeques()
        {
            for (int i = 0; i < _random100k.Length; i++)
            {
                _heap.DequeMax();
            }
            Assert.AreEqual(0, _heap.ElementsCount);
        }

        private class ObjectWithOrder
        {
            public object Data { get; set; }
            public int Order { get; set; }
        }

        private class ObjectWithOrderComparer : IComparer<ObjectWithOrder>
        {
            public int Compare(ObjectWithOrder x, ObjectWithOrder y)
            {
                if (x == null)
                {
                    return Int32.MinValue;
                }
                if (y == null)
                {
                    return Int32.MaxValue;
                }

                return x.Order - y.Order;
            }
        }
    }
}
