using System;

namespace IntervalHeap.App
{
    class Program
    {
        static void Main(string[] args)
        {
            var values = new int[6] {2, 20, 3, 30, 4, 25};
            var heap = new Lib.IntervalHeap();
            for (int i = 0; i < 6; i++)
            {
                Console.WriteLine($"inserting {values[i]} into heap.");
                heap.Enque(values[i]);
            }
            Console.WriteLine("Min = {0}, Max = {1}", heap.FetchMin(), heap.FetchMax());
            Console.ReadLine();

            Console.WriteLine("Removed elements {0},{1}, Min={2}, Max={3}", 
                heap.DequeMin(), heap.DequeMax(), heap.FetchMin(), heap.FetchMax());
            Console.ReadLine();
        }
    }
}
