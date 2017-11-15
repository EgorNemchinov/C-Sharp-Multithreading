using NUnit.Framework;

namespace RedBlackTree.Tests
{
    [TestFixture]
    public class NodeTests
    {
        [Test]
        public void Brother()
        {
            var root = new Node<int>(5);
            root.SetLeft(new Node<int>(3));
            root.GetLeft().parent = root;
            
            root.SetRight(new Node<int>(7));
            root.GetRight().parent = root;
            
            Assert.AreEqual(root.GetRight(), root.GetLeft().Brother());
            
            root.SetRight(null);
            
            Assert.AreEqual(null, root.GetLeft().Brother());
        }
        
        [Test]
        public void GrandParent()
        {
            var root = new Node<int>(5);
            var left = new Node<int>(3);
            root.SetLeft(left);
            left.parent = root;
            
            left.SetRight(new Node<int>(4));
            left.GetRight().parent = left;
            
            Assert.AreEqual(root, left.GetRight().GrandParent());
            Assert.AreEqual(null, root.GrandParent());
            Assert.AreEqual(null, left.GrandParent());

            left.GetRight().parent = null;
            
            Assert.AreEqual(null, left.GetRight().GrandParent());
        }
        
        [Test]
        public void Uncle()
        {
            var root = new Node<int>(5);
            var left = new Node<int>(3);
            var right = new Node<int>(6);
            var leaf = new Node<int>(4);
            
            root.SetLeft(left);
            left.parent = root;
            
            root.SetRight(right);
            root.parent = root;
            
            left.SetRight(leaf);
            leaf.parent = left;
            
            Assert.AreEqual(right, leaf.Uncle());
            
            root.SetRight(null);
            right.parent = null;
            
            Assert.AreEqual(null, leaf.Uncle());

            leaf.parent = null;
            
            Assert.AreEqual(null, leaf.Uncle());
        }

        [Test]
        public void RotateLeft()
        {
            var expectedTree = new Tree<int>(new Node<int>(20, isBlack:true));
            expectedTree.root.SetLeft(new Node<int>(15));
            expectedTree.root.SetRight(new Node<int>(25));

            var actualTree = new Tree<int>();
            actualTree.root = new Node<int>(15, isBlack: true);
            actualTree.root.SetRight(new Node<int>(20));
            actualTree.root.GetRight().SetRight(new Node<int>(25));

            var right = actualTree.root.GetRight();
            actualTree.root.RotateLeft();
            actualTree.root.isBlack = false;
            actualTree.root = right;
            actualTree.root.isBlack = true;
            
            Assert.AreEqual(expectedTree, actualTree);
        }
        
        [Test]
        public void RotateRight()
        {
            var expectedTree = new Tree<int>(new Node<int>(20, isBlack:true));
            expectedTree.root.SetLeft(new Node<int>(15));
            expectedTree.root.SetRight(new Node<int>(25));

            var actualTree = new Tree<int>();
            actualTree.root = new Node<int>(25, isBlack: true);
            actualTree.root.SetLeft(new Node<int>(20));
            actualTree.root.GetLeft().SetLeft(new Node<int>(15));

            var left = actualTree.root.GetLeft();
            actualTree.root.RotateRight();
            actualTree.root.isBlack = false;
            actualTree.root = left;
            actualTree.root.isBlack = true;
            
            Assert.AreEqual(expectedTree, actualTree);
        }
    }
}
