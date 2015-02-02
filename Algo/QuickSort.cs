//It reminds me of time in ZHYZ OI team, in memory of xt3000
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace MOOC
{
    public enum EnumPivot
    {
        First = 0,
        Last = 1,
        Mid = 2
    }
    class QuickSort
    {
        static long Ans;
        static readonly string path = @"..\..\algo\PA2.in";
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
            Ans = 0;
            int[] dataForQ1 = (int[])dataSource.ToArray<int>().Clone();
            QSort(ref dataForQ1, 0, dataForQ1.Length - 1, EnumPivot.First);
            Console.WriteLine(Ans);
            Ans = 0;
            int[] dataForQ2 = (int[])dataSource.ToArray<int>().Clone();
            QSort(ref dataForQ2, 0, dataForQ2.Length - 1, EnumPivot.Last);
            Console.WriteLine(Ans);
            Ans = 0;
            int[] dataForQ3 = (int[])dataSource.ToArray<int>().Clone();
            QSort(ref dataForQ3, 0, dataForQ3.Length - 1, EnumPivot.Mid);
            Console.WriteLine(Ans);
            int[] dataForQ4 = (int[])dataSource.ToArray<int>().Clone();
            StandardQSort(dataForQ4, 0, dataForQ3.Length - 1);
            Console.WriteLine(dataForQ4.SequenceEqual(dataForQ3));
            Console.ReadKey();
        }
        static void Swap(ref int[] data, int x, int y)
        {
            int temp = data[x];
            data[x] = data[y];
            data[y] = temp;
        }
        static void StandardQSort(int[] data, int left, int right)
        {
            if (right <= left)
                return;
            Swap(ref data, left, left + (right - left) / 2);
            int pivot = left + 1;
            for (int j = left + 1; j <= right; j++)
            {
                if (data[j] < data[left])
                {
                    Swap(ref data, j, pivot);
                    pivot++;
                }
            }
            pivot = pivot - 1;
            Swap(ref data, pivot, left);
            StandardQSort(data, left, pivot - 1);
            StandardQSort(data, pivot + 1, right);
        }
        static void QSort(ref int[] data, int left, int right, EnumPivot pivotType)
        {
            if (right <= left)
                return;
            if (pivotType == EnumPivot.Last)
                //Q2
                Swap(ref data, left, right);
            if (pivotType == EnumPivot.Mid)
            {
                //Q3
                int mid = left + (right - left) / 2;
                if (data[left] <= data[right] && data[left] <= data[mid])
                {
                    if (data[mid] < data[right])
                        Swap(ref data, mid, left);
                    else
                        Swap(ref data, right, left);
                }
                else
                    if (data[mid] <= data[right] && data[mid] <= data[left])
                    {
                        if (data[right] < data[left])
                            Swap(ref data, right, left);
                    }
                    else if (data[right] <= data[left] && data[right] <= data[mid])
                    {
                        if (data[mid] < data[left])
                            Swap(ref data, mid, left);
                    }
            }
            int pivot = left + 1;
            Ans = Ans + right - left;
            for (int j = left + 1; j <= right; j++)
            {
                if (data[j] < data[left])
                {
                    Swap(ref data, j, pivot);
                    pivot = pivot + 1;
                }
            }
            Swap(ref data, left, pivot - 1);
            pivot = pivot - 1;
            QSort(ref data, left, pivot - 1, pivotType);
            QSort(ref data, pivot + 1, right, pivotType);
        }
    }

}
