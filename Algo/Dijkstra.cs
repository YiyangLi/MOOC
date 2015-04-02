using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace MOOC
{
    class Dijkstra
    {
        public class Edge
        {
            public int Index { get; set; }
            public int Weight { get; set; }
            public Edge Next { get; set; }

        }
        private static readonly int[] default_vertexes_for_shortest_path_length = new int[] { 7, 37, 59, 82, 99, 115, 133, 165, 188, 197 };
        public const string PATH = @"..\..\algo\PA5.in";
        public const int MAX_VERTEX = 200;
        public const int LARGE_INT = 1000000;
        static void dijkstra(List<Edge> graph, out int[] distance)
        {
            var visited = new bool[MAX_VERTEX + 1];
            var unexplored = new HashSet<int>(Enumerable.Range(2, MAX_VERTEX - 1));
            distance = new int[MAX_VERTEX + 1];
            visited = Enumerable.Repeat(false, MAX_VERTEX).ToArray();
            distance = Enumerable.Repeat(LARGE_INT, MAX_VERTEX + 1).ToArray();
            visited[0] = true;
            visited[1] = true;
            distance[1] = 0;
            while (unexplored.Count > 0)
            {
                int nextVertex = 0;
                int minWeight = LARGE_INT;
                for (int i = 1; i <= MAX_VERTEX; i++)
                {
                    if (!unexplored.Contains(i))
                    {
                        Edge temp = graph[i];
                        while (temp != null)
                        {
                            if (unexplored.Contains(temp.Index) && distance[i] + temp.Weight < minWeight)
                            {
                                nextVertex = temp.Index;
                                minWeight = distance[i] + temp.Weight;
                            }
                            temp = temp.Next;
                        }
                    }
                }
                unexplored.Remove(nextVertex);
                distance[nextVertex] = minWeight;
            }
        }

        static void dijkstraWithHeap()
        {
 
        }
        static void Main(string[] args)
        {
            var lines = File.ReadAllLines(PATH);
            List<Edge> graph = new List<Edge>();
            int[] distance = new int[0];
            foreach (var i in Enumerable.Range(0, MAX_VERTEX + 1))
            {
                graph.Add(new Edge());
                
            }
            foreach (var line in lines)
            {
                var data = line.Trim().Split();
                int start = 0;
                if (int.TryParse(data[0], out start))
                {
                    for (int i = 1; i < data.Length; i++)
                    {
                        var edge = data[i].Split(',');
                        int weight = 0;
                        int index = 0;
                        if (edge.Length > 1 && int.TryParse(edge[0], out index) && int.TryParse(edge[1], out weight))
                        {
                            var Edge = new Edge() { Index = index, Weight = weight, Next = graph[start] };
                            graph[start] = Edge;
                        }
                        else
                        {
                            Console.WriteLine("Error in reading data!");
                            break;
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Error in reading data!");
                }
            }
            Console.WriteLine("Reading data is done. ");
            var path = new Edge();
            dijkstra(graph, out distance);
            foreach (var vertex in default_vertexes_for_shortest_path_length)
            {
                Console.WriteLine("{0}\t{1}", vertex, distance[vertex]);
            }
            Console.ReadKey();
        }
    }
}
