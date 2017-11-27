using System.Collections.Generic;
using NUnit.Framework;

namespace RedBlackTree.Tests
{
    [TestFixture]
    public class RemoveTests
    {
        private Tree<int> createTree() {
    
            var tree = new Tree<int>();
    
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
    
            tree.root = root;
    
            return tree;
    
        }

        [Test]
        public void RemoveFromEmptyTree() {
    
            var expectedTree = new Tree<int>();
            var actualTree = new Tree<int>();
    
            actualTree.Remove(20);
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    
        [Test]
        public void RemoveNonExistentKey() {
    
            var expectedTree = createTree();
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 40, 35, 27, 26, 10, 15, 28}) {
                actualTree.Insert(i);
            }
    
            foreach (var i in new List<int>{21, 22, 31, 32, 41, 42})
            {
                actualTree.Remove(i);
            }

            Assert.AreEqual(expectedTree, actualTree);

        }
    
        [Test]
        public void RemoveRootLeaf() {
    
            var expectedTree = new Tree<int>();
    
            var actualTree = new Tree<int>();
    
            actualTree.Insert(25);
    
            actualTree.Remove(25);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Simple == deletion root with two red children
         * */
        [Test]
        public void RemoveRootNonLeafSimple() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(30);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(25);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        [Test]
        public void RemoveRightRedLeaf() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{25, 20, 30}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(30);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        [Test]
        public void RemoveLeftRedLeaf() {
    ;
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(30);
            root.SetRight(node1);
            node1.parent = root;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{25, 20, 30}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(20);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** internal node == node with one or two children
         *  Case1 == next smaller/bigger node is red leaf
         */
        [Test]
        public void RemoveinternalNodeCase1() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(40);
            root.SetRight(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            var node3 = new Node<int>(27);
            node2.SetLeft(node3);
            node3.parent = node2;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{25, 20, 30, 40, 27}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(30);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case2 == Black node with one left red child
         */
        [Test]
        public void RemoveinternalNodeCase2() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(27);
            root.SetRight(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{25, 20, 30, 27}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(30);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case3 == Black node with one right red child
         */
        [Test]
        public void RemoveinternalNodeCase3() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(20);
            root.SetLeft(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(40);
            root.SetRight(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{25, 20, 30, 40}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(30);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case1 == (node == root)
         * */
        [Test]
        public void case1() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(30);
            root.SetRight(node1);
            node1.parent = root;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 35}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(35);
            actualTree.Remove(20);
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    
        /** Case2Left == red brother with two black child;
         * when (node < parent);
         * */
        [Test]
        public void case2Left() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(30);
            root.isBlack = true;
    
            var node1 = new Node<int>(40);
            root.SetRight(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(45);
            node1.SetRight(node2);
            node2.parent = node1;
    
            var node3 = new Node<int>(25);
            root.SetLeft(node3);
            node3.parent = root;
            node3.isBlack = true;

            var node4 = new Node<int>(27);
            node3.SetRight(node4);
            node4.parent = node3;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 40, 27, 45}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(20);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case2Right == red brother with two black child
         * when (node > parent)
         * */
        [Test]
        public void case2Right() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(20);
            root.isBlack = true;
    
            var node1 = new Node<int>(25);
            root.SetRight(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(15);
            root.SetLeft(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 15}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(30);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case3 == black brother with two black child
         * */
        [Test]
        public void case3() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(35);
            root.SetRight(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(20);
            root.SetLeft(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            var node3 = new Node<int>(40);
            node1.SetRight(node3);
            node3.parent = node1;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 40, 35, 27}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(27);
            actualTree.Remove(30);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case4Left == black brother with red child
         * when (parent < child < brother)
         * */
        [Test]
        public void case4Left() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(30);
            root.isBlack = true;
    
            var node1 = new Node<int>(35);
            root.SetRight(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(25);
            root.SetLeft(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 35, 30}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(20);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case4Right == black brother with red child
         * when (parent > child > brother)
         * */
        [Test]
        public void case4Right() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(30);
            root.SetRight(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(20);
            root.SetLeft(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 30, 35, 25}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(35);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case5Left == black brother with red child
         * when (child > brother > parent)
         * */
        [Test]
        public void case5Left() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(30);
            root.isBlack = true;
    
            var node1 = new Node<int>(40);
            root.SetRight(node1);
            node1.parent = root;
            node1.isBlack = true;
    
            var node2 = new Node<int>(25);
            root.SetLeft(node2);
            node2.parent = root;
            node2.isBlack = true;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 40}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(20);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        /** Case5Right == black brother with red child
         * when (parent > brother > child)
         * */
        [Test]
        public void case5Right() {
    
            var expectedTree = new Tree<int>();
    
            var root = new Node<int>(25);
            root.isBlack = true;
    
            var node1 = new Node<int>(27);
            root.SetRight(node1);
            node1.parent = root;
    
            var node2 = new Node<int>(26);
            node1.SetLeft(node2);
            node2.parent = node1;
            node2.isBlack = true;
    
            var node3 = new Node<int>(35);
            node1.SetRight(node3);
            node3.parent = node1;
            node3.isBlack = true;
    
            var node4 = new Node<int>(30);
            node3.SetLeft(node4);
            node4.parent = node3;
    
            var node5 = new Node<int>(15);
            root.SetLeft(node5);
            node5.parent = root;
            node5.isBlack = true;
    
            var node6 = new Node<int>(20);
            node5.SetRight(node6);
            node6.parent = node5;
    
            var node7 = new Node<int>(10);
            node5.SetLeft(node7);
            node7.parent = node5;
    
            expectedTree.root = root;
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 40, 35, 27, 26, 10, 15}) {
                actualTree.Insert(i);
            }
    
            actualTree.Remove(40);
    
            Assert.AreEqual(expectedTree, actualTree);
        }
    
        [Test]
        public void RemoveByScenario() {
    
            var expectedTree = new Tree<int>();
    
            var actualTree = new Tree<int>();
    
            foreach (var i in new List<int>{20, 25, 30, 40, 27, 45, 19, 18, 21, 22, 23, 17, 26}) {
                actualTree.Insert(i);
            }
    
            foreach (var i in new List<int>{17, 27, 18, 22, 23, 26, 19, 40, 25, 30, 20, 21, 45}) {
                actualTree.Remove(i);
            }
    
            Assert.AreEqual(expectedTree, actualTree);
    
        }
    }
}
