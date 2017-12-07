using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace RedBlackTree
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Test();
        }

        public static void Test()
        {
            ParallelTree<int> tree = new ParallelTree<int>(new Tree<int>());

            List<Task<bool>> tasks = new List<Task<bool>>();
            List<int> values = new List<int>() {0, 1, 2, -2, 10, 7, 4};

            foreach (int value in values)
            {
                tree.Insert(value);
            }

            foreach (int value in values)
            {
                tree.Find(value);
            }
            foreach (int value in values)
            {
                tree.Remove(value);
            }

            Console.WriteLine("All added.\n");
            tree.Exit(0).Wait();
            Console.WriteLine("All finished.");

            Tree<int> wantedTree = new Tree<int>();
            /*foreach (int value in values)
            {
                wantedTree.Insert(value);
            }*/

            Console.WriteLine($"Tree is empty: {tree.GetTree().root == null}");
            Console.WriteLine("Tree is the way we wanted: " +
                              $"{tree.GetTree().Equals(wantedTree)}");
        }
    }
}
