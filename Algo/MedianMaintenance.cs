using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MOOC
{
    class MedianMaintenance
    {
        public const string PATH = @"..\..\algo\PA6_2.in";
        
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(PATH);
            var numbers = lines.Select(i => Int32.Parse(i)).ToArray();
            var maxHeap = new PriorityQueue<int>(false);
            var minHeap = new PriorityQueue<int>(true);
            var median = new List<int>();
            foreach (var number in numbers)
            {
                //if (maxHeap.Size > 0 && minHeap.Size > 0)
                //    Console.WriteLine("===Before, maxHeap Size: {0}\tminHeapSize:{1}\tmax:{2}\tmin:{3}",
                //                        maxHeap.Size, minHeap.Size, maxHeap.Top(), minHeap.Top());
                if (maxHeap.Size == 0 || number < maxHeap.Top())
                    maxHeap.Push(number);
                else
                    minHeap.Push(number);
                while (maxHeap.Size - minHeap.Size > 1)
                {
                    minHeap.Push(maxHeap.Top());
                    maxHeap.Pop();
                }
                while (minHeap.Size - maxHeap.Size > 1)
                {
                    maxHeap.Push(minHeap.Top());
                    minHeap.Pop();
                }
                //if (maxHeap.Size > 0 && minHeap.Size > 0)
                //    Console.WriteLine("===After, maxHeap Size: {0}\tminHeapSize:{1}\tmax:{2}\tmin:{3}",
                //                        maxHeap.Size, minHeap.Size, maxHeap.Top(), minHeap.Top());
                if (maxHeap.Size >= minHeap.Size)
                    median.Add(maxHeap.Top());
                else
                    median.Add(minHeap.Top());
            }
            Console.WriteLine(median.Sum() % 10000);
            Console.ReadKey();
        }


        public class Heap<T> where T : IComparable<T>
        {
            private static void Swap(ref T[] obj, int a, int b)
            {
                T temp = obj[a];
                obj[a] = obj[b];
                obj[b] = temp;
            }
            public static void AdjustFromBottom(ref T[] obj, int n, bool isMinHeap)
            {
                int m = (n - 1) / 2;
                while (n > 0 && (obj[m].CompareTo(obj[n]) < 0 ^ isMinHeap))
                {
                    Swap(ref obj, n, m);
                    n = m;
                    m = (n - 1) / 2;
                }
            }
            public static void AdjustFromTop(ref T[] obj, int n, int len, bool isMinHeap)
            {
                while (n * 2 + 1 < len)
                {
                    int m = n * 2 + 1;
                    if (m + 1 < len && (obj[m].CompareTo(obj[m + 1]) < 0 ^ isMinHeap))
                    {
                        m = m + 1;
                    }
                    if ((obj[n].CompareTo(obj[m]) > 0 ^ isMinHeap))
                        break;
                    Swap(ref obj, n, m);
                    n = m;
                }
            }
        }
        public class PriorityQueue<T> where T : IComparable<T>
        {
            private const int DEFAULT_CAPACITY = 16;
            private int len;
            private T[] buffer;
            private bool isMinHeap;
            private void Expand()
            {
                Array.Resize<T>(ref buffer, buffer.Length * 2);
            }
            private void swap(int a, int b)
            {
                T temp = buffer[a];
                buffer[a] = buffer[b];
                buffer[b] = temp;
            }
            public int Size
            {
                get { return len; }
            }
            public PriorityQueue(bool minHeap = false)
            {
                buffer = new T[DEFAULT_CAPACITY];
                len = 0;
                isMinHeap = minHeap;
            }
            public bool Empty()
            {
                return len.Equals(0);
            }
            public T Top()
            {
                if (Empty())
                {
                    throw new OverflowException("The queue is empty, unable to return the top element. ");
                }
                return buffer[0];
            }
            public void Push(T obj)
            {
                if (len.Equals(buffer.Length))
                    Expand();
                buffer[len] = obj;
                Heap<T>.AdjustFromBottom(ref buffer, len, isMinHeap);
                len = len + 1;
            }
            public void Pop()
            {
                if (Empty())
                {
                    throw new OverflowException("The queue is empty, unable to pop elements. ");
                }
                len = len - 1;
                swap(0, len);
                Heap<T>.AdjustFromTop(ref buffer, 0, len, isMinHeap);
            }
        }
    }
}
