using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Algo
{
    class DivideAndConquer
    {
        
        static readonly string path = @"..\..\algo\PA1.in";
        static void Main(string[] args)
        {
            int i = 0;
            int lineNum = 0;
            List<int> dataSource = new List<int>();

            foreach (string line in File.ReadAllLines(path))
            {
                if (int.TryParse(line, out i))
                {
                    dataSource.Add(i);
                }
                else
                {
                    Console.WriteLine("Please double check the line {0} of the file {1}, can't parse the string {2} to an integer. ", 
                        lineNum, path, line);        
                }
                lineNum++;
            }
            Console.WriteLine("Size of integer read: {0}", lineNum);
            Console.WriteLine(CountInversions(ref dataSource));
            Console.ReadKey();
        }
        public static long CountInversions(ref List<int> numbers)
        {
            int mid = numbers.Count / 2;
            long countLeft = 0,
                 countRight = 0,
                 countMerge = 0;

            if (numbers.Count <= 1)
                return 0;

            List<int> left = numbers.CloneTo(0, mid);
            List<int> right = numbers.CloneTo(mid, numbers.Count);

            countLeft = CountInversions(ref left);
            countRight = CountInversions(ref right);
            countMerge = MergeAndCount(left, right, ref numbers);

            return countLeft + countRight + countMerge;
        }

        /// <summary>
        /// Merge sort and meanwhile return the count of inversions
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="merged"></param>
        /// <returns></returns>
        public static long MergeAndCount(List<int> left, List<int> right, ref List<int> merged)
        {
            int indexL = 0,
                indexR = 0;
            long count = 0;
            merged = null; // quick dereferenced. 
            merged = new List<int>();
            while (indexL < left.Count && indexR < right.Count)
            {
                if (left[indexL] <= right[indexR])
                {
                    merged.Add(left[indexL]);
                    indexL++;
                }
                else
                {
                    merged.Add(right[indexR]);
                    count += (long)(left.Count - indexL);
                    indexR++;
                }
            }
            while (indexL.Equals(left.Count) && indexR < right.Count)
            {
                merged.Add(right[indexR]);
                indexR++;
            }
            while (indexR.Equals(right.Count) && indexL < left.Count)
            {
                merged.Add(left[indexL]);
                indexL++;
            }
            return count;
        }
    }

    /// <summary>
    /// syntatic suger
    /// </summary>
    public static class Extensions
    {
        public static List<T> CloneTo<T>(this IList<T> listToClone, int index, int length)
        {
            return listToClone.Where((_, i) => (i >= index && i < index + length)).ToList();
        }
    }
}
