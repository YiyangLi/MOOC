using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.IO;

namespace MOOC
{
    class TwoSum
    {
        public const string PATH = @"..\..\algo\PA6_1.in";
        
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(PATH);
            var numbers = lines.Select(i => long.Parse(i));
            var hset = new HashSet<long>(numbers);
            var sums = Enumerable.Range(-10000, 20001);
            var count = 0;
            numbers = numbers.OrderBy(i => i).Take(numbers.Count() / 2);
            Parallel.ForEach<int>(sums, sum => {
                foreach (var number in numbers)
                {
                    if (hset.Contains(sum - number))
                    {
                        Console.WriteLine("{3}: {0} + {1} = {2}", number, sum - number, sum, count + 1);
                        Interlocked.Add(ref count, 1);
                        break;
                    }
                }
            });
            Console.WriteLine(count);
            Console.ReadKey();
        }
    }
}
