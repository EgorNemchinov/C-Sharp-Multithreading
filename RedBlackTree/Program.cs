using System;
using System.Collections.Generic;

namespace RedBlackTree
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Node<int> root = new Node<int>(20);
            Tree<int> tree = new Tree<int>();
            tree.Insert(20);
            tree.Insert(25);
            tree.Insert(30);
            Console.WriteLine(tree.root.GetRight().ToString());
        }
    }
}
