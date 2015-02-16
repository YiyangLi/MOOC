using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace MOOC
{
    public class KargerMinCut
    {
        static readonly string path = @"..\..\algo\PA3.in";
        public static void Main(string[] args)
        {
            var graph = new Dictionary<int, HashSet<int>>();
            foreach (string line in File.ReadAllLines(path))
            {
                var nodes = line.Trim().Split().Select(i => Int32.Parse(i)).ToArray();
                var node0 = nodes.FirstOrDefault();
                if (!graph.ContainsKey(node0))
                    graph.Add(node0, new HashSet<int>());
                foreach (var node in nodes)
                {
                    if (!graph.ContainsKey(node))
                        graph.Add(node, new HashSet<int>());
                    if (node != node0)
                    {
                        graph[node].Add(node0);
                        graph[node0].Add(node);
                    }
                }
            }
            
            int minCut = int.MaxValue;
            var maxTrail = (double)(graph.Count() << 1) * Math.Log(graph.Count()); // Prob of failure = 1 / n
            for (int i = 0; i < maxTrail; i++)
            {
                var rand = new Random();
                var graphClone = graph.ToDictionary(a => a.Key, a => a.Value.ToList());
                var cut = RandomContraction(ref graphClone, rand);
                if (cut < minCut)
                    minCut = cut;
            }
            Console.ReadKey();
        }

        public static int RandomContraction(ref Dictionary<int, List<int>> graph, Random rand)
        {
            int result = 0;

            if (graph.Count() <=1)
                return result;
            while (graph.Count() != 2)
            {
                int node1 = graph.ElementAt(rand.Next(0, graph.Keys.Count())).Key;
                int node2 = graph[node1][rand.Next(0, graph[node1].Count)];
                while (node2 == node1)
                    node2 = graph[node1][rand.Next(0, graph[node1].Count)];
                foreach (var connected in graph[node1])
                {
                    graph[connected].Remove(node1);
                    if (connected != node2)
                    {
                        graph[node2].Add(connected);
                        graph[connected].Add(node2);
                    }
                }
                graph.Remove(node1);
            }
            result = graph.FirstOrDefault().Value.Count();
            return result;
        }
    }
}
