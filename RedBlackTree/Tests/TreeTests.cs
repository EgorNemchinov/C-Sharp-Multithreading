using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace RedBlackTree.Tests
{
    [TestFixture]
    public class TreeTests
    {
        [Test]
        public void FindThreeKeys()
        {
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(20);;
            root.isBlack = true;
    
            expectedTree.root = root;
            expectedTree.root.SetLeft(new Node<int>(15));
            expectedTree.root.SetRight(new Node<int>(25));
    
            Assert.AreEqual(expectedTree.Find(20), expectedTree.root);
            Assert.AreEqual(expectedTree.Find(15), expectedTree.root.GetLeft());
            Assert.AreEqual(expectedTree.Find(25), expectedTree.root.GetRight());
            Assert.AreEqual(expectedTree.Find(30), null);
        }
        
        
        [Test]
        public void InsertOneKey() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(20);;
            root.isBlack = true;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            actualTree.Insert(20);
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
        
        [Test]
        public void InsertTwoKeys() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(20);;
            root.isBlack = true;
    
            expectedTree.root = root;
            expectedTree.root.SetLeft(new Node<int>(15));
            expectedTree.root.SetRight(new Node<int>(25));
    
            var actualTree = new Tree<int>();
    
            actualTree.Insert(20);
            actualTree.Insert(15);
            actualTree.Insert(25);
            
            Console.WriteLine(actualTree.root);
            var iter = new BfsIterator<int>(actualTree);
            Console.WriteLine("Tree:");
            while (iter.hasNext())
            {
                Console.WriteLine(iter.next().value);
            }
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    
        [Test]
        public void InsertRightRightCase() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
    
            var node2 = new Node<int>(30);
            root.SetRight(node2);
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();

    
            foreach (var i in new List<int>{20, 25, 30})
            {
                actualTree.Insert(i);
            };
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    
        [Test]
        public void InsertRightLeftCase() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
    
            var node2 = new Node<int>(30);
            root.SetRight(node2);
            node2.parent = root;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 30, 25})
            {
                actualTree.Insert(i);
            };
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    
        [Test]
        public void InsertLeftLeftCase() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
    
            var node2 = new Node<int>(30);
            root.SetRight(node2);
            node2.parent = root;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{30, 25, 20})
            {
                actualTree.Insert(i);
            }
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    
        [Test]
        public void InsertLeftRightCase() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
    
            var node2 = new Node<int>(30);
            root.SetRight(node2);
            node2.parent = root;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{30, 20, 25})
            {
                actualTree.Insert(i);
            }
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    
        [Test]
        public void InsertRecoloringRelatives() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(30);
            root.SetRight(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            var node3 = new Node<int>(40);
            node2.SetRight(node3);
            node3.parent = node2;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 40})
            {
                actualTree.Insert(i);
            }
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    
        /* Insert according to the following scenario:;
         * 1) Insert root(20) -> recoloring root in black;
         * 2) Insert node(25), node(30, 30) -> RightRight case;
         * 3) Insert node(40) -> recoloring father, uncle, grandfather; recoloring root in black;
         * 4) Insert node(35) -> RightLeft case;
         * 5) Insert node(27) -> recoloring father, uncle, grandfather;
         * 6) Insert node(26) -> LeftLeft case;
         * 7) Insert node(10);
         * 8) Insert node(15) -> LeftRight case;
         * 9) Insert node(28) -> recoloring father, uncle, grandfather;;
         *                           RightLeft case(for grandfather);;
         * 10) Insert illegal key and varue;
         */
        [Test]
        public void InsertByScenario() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(27);
            root.isBlack = true;
    
            var node1 = new Node<int>(35);
            root.SetRight(node1);
            node1.parent = root;
    
            var node2 = new Node<int>(30);
            node1.SetLeft(node2);
            node2.parent = node1;
            node2.isBlack = true;
    
            var node3 = new Node<int>(40);
            node1.SetRight(node3);
            node3.parent = node1;
            node3.isBlack = true;
    
            var node4 = new Node<int>(28);
            node2.SetLeft(node4);
            node4.parent = node2;
    
            var node5 = new Node<int>(25);
            root.SetLeft(node5);
            node5.parent = root;
    
            var node6 = new Node<int>(26);
            node5.SetRight(node6);
            node6.parent = node5;
            node6.isBlack = true;
    
            var node7 = new Node<int>(15);
            node5.SetLeft(node7);
            node7.parent = node5;
            node7.isBlack = true;
    
            var node8 = new Node<int>(20);
            node7.SetRight(node8);
            node8.parent = node7;
    
            var node9 = new Node<int>(10);
            node7.SetLeft(node9);
            node9.parent = node7;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 40, 35, 27, 26, 10, 15, 28})
            {
                actualTree.Insert(i);
            }
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    }
}
