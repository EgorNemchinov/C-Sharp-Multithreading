using System;
using System.Collections.Generic;
using System.Linq;

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

            List<int> values = new List<int>() {0, 1, 2, -2, 10, 7, 4};
            var operations = new List<TreeOperation<int>>();

            foreach (int value in values)
            {
                operations.Add(tree.Insert(value));
            }

            foreach (int value in values)
            {
                operations.Add(tree.Find(value));
            }
            foreach (int value in values)
            {
                operations.Add(tree.Remove(value));
            }

            Console.WriteLine("All operations are added.");
            tree.Exit(0).Wait();
            Console.WriteLine("All finished.\n");

            Console.WriteLine("All operations succeded: " +
                              operations.All(operation => operation.result));

            Console.WriteLine($"Tree is empty: {tree.GetTree().root == null}");
        }
    }
}
