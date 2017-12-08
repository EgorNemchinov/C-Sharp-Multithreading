using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace RedBlackTree.Tests
{
    [TestFixture]
    public class ParallelTreeTests
    {
        public List<int> InitValues(int amount, bool unique = false)
        {
            var rand = new Random();
            var valuesList = new List<int>();

            for (int i = 0; i < amount; i++)
            {
                valuesList.Add(rand.Next());
            }
            if (!unique)
                return valuesList;

            var uniqueValues = new HashSet<int>(valuesList);
            valuesList = new List<int>(uniqueValues);

            return valuesList;
        }

        [Test]
        public void UniqueInsertsSearchesThenEmpty()
        {
            var values = InitValues(100000, unique: true);

            var operations = new List<TreeOperation<int>>(500000);

            var tree = new ParallelTree<int>(new Tree<int>());

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
            tree.Exit(0).Wait();

            Assert.AreEqual(new Tree<int>(), tree.GetTree());

            Assert.IsTrue(operations.All((operation) => operation.result));
        }

        [Test]
        public void InsertsRemovesValidity()
        {
            var values = InitValues(100000, unique: false);
            var parallelTree = new ParallelTree<int>(new Tree<int>());
            var simpleTree = new Tree<int>();

            foreach (int value in values)
            {
                parallelTree.Insert(value);
                simpleTree.Insert(value);
            }
            for (int i = 0; i < 5000; i++)
            {
                parallelTree.Remove(values[i]);
                simpleTree.Remove(values[i]);
            }
            parallelTree.Exit(0).Wait();

            Assert.AreEqual(simpleTree, parallelTree.GetTree());
        }

        [Test]
        public void OperationsReturnValueCheck()
        {
            var map = new Dictionary<TreeOperation<int>, bool>();
            var values = InitValues(100000, unique: false);
            var parallelTree = new ParallelTree<int>(new Tree<int>());
            var simpleTree = new Tree<int>();

            foreach (int value in values)
            {
                map[parallelTree.Insert(value)] = simpleTree.Insert(value);
            }
            for (int i = 0; i < 10000; i++)
            {
                map[parallelTree.Remove(values[i])] = simpleTree.Remove(values[i]);
            }
            foreach (int value in values)
            {
                map[parallelTree.Find(value)] = simpleTree.Find(value);
            }
            parallelTree.Exit(0).Wait();

            Assert.AreEqual(simpleTree, parallelTree.GetTree());

            var equal = map.Keys.All((key) => map[key] == key.result);
            Assert.IsTrue(equal);
        }
    }
}
