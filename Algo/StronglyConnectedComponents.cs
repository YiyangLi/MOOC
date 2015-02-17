using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;

namespace MOOC
{
    public class Node
    {
        private int id;
        public int Id { get { return id; } }
        public int Leader { get; set; }
        public bool Explorered { get; set; }
        public int FinishingTime { get; set; }
        public List<int> Heads { get; set; }
        public List<int> Tails { get; set; }
        public Node(int _id)
        {
            Leader = -1;
            Explorered = false;
            FinishingTime = -1;
            Heads = new List<int>();
            Tails = new List<int>();
            id = _id;
        }
    }
    class StronglyConnectedComponents
    {
        static readonly string path = @"..\..\algo\PA4.in";
        const int SIZE = 875714;
        static int finishingTime = 0;
        static int leader = 0;
        static Node[] Graph;
        public static void Main(string[] args)
        {
            var lines = File.ReadAllLines(path);
            var graph = Enumerable.Range(0, SIZE + 1).Select(i => new Node(i)).ToArray();
            var graphRev = Enumerable.Range(0, SIZE + 1).Select(i => new Node(i)).ToArray();
            foreach (string line in lines)
            {
                var nodes = line.Trim().Split().ToArray();
                int node0 = 0, node1 = 0;
                if (nodes.Length != 2 || !int.TryParse(nodes[0], out node0) || !int.TryParse(nodes[1], out node1))
                {
                    Console.WriteLine("Please double check the source file. {0}", line);
                }
                graph[node0].Heads.Add(node1);
                graph[node1].Tails.Add(node0);
            }
            Graph = graph;
            DFS_Loop();
            for (int i = SIZE; i > 0; i--)
            {
                int id = graph[i].FinishingTime;
                graphRev[id].Heads = graph[i].Tails.Select(tail => graph[tail].FinishingTime).ToList();
                //graphRev[id].Heads = new List<int>();
                //foreach (var tail in graph[i].Tails)
                //{
                //    graphRev[id].Heads.Add(graph[tail].FinishingTime);
                //}
            }
            Graph = graphRev;
            DFS_Loop();
            var SCC = graphRev.GroupBy(i => i.Leader).OrderByDescending(i => i.Count()).ToDictionary(i => i.Key, i => i.Select(j => j.Id).ToList());
            Console.WriteLine(string.Join(",",SCC.Take(5).Select(i => i.Value.Count().ToString()).ToArray()));
            Console.ReadKey();
        }

        public static void DFS_Loop()
        {
            leader = 0;
            finishingTime = 0;
            for (int i = SIZE; i > 0; i--)
            {
                if (!Graph[i].Explorered)
                {
                    leader = i;
                    DFS(i);
                }
            }

        }
        public static void DFS2(int start)
        {
            Graph[start].Explorered = true;
            Graph[start].Leader = leader;
            foreach (var i in Graph[start].Heads)
            {
                if (!Graph[i].Explorered)
                    DFS(i);
            }
            finishingTime++;
            Graph[start].FinishingTime = finishingTime;
        }
        public static void DFS(int start)
        {
            Stack<int> stack = new Stack<int>();
            stack.Push(start);
            while (stack.Count() > 0)
            {
                var current = stack.Peek();
                var stackPushed = false;
                Graph[current].Explorered = true;
                Graph[current].Leader = leader;
                foreach (var i in Graph[current].Heads)
                {
                    if (!Graph[i].Explorered)
                    {
                        stack.Push(i);
                        stackPushed = true;
                    }
                }
                if (!stackPushed)
                {
                    stack.Pop();
                    if (Graph[current].FinishingTime < 0)
                    {
                        finishingTime++;
                        Graph[current].FinishingTime = finishingTime;
                    }
                }
            }
        }
    }
}
