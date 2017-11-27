using System;
using System.Collections.Generic;
using System.Linq;

namespace RedBlackTree
{
    public class Tree<V> where V: IComparable<V>
    {
        public Node<V> root = null;

        public Tree(Node<V> root = null)
        {
            this.root = root;
        }

        public Node<V> Find(V value)
        {
            if (value == null)
            {
                Console.WriteLine("Error: Value passed into Find() is null.");
                return null;
            }
            if (root == null)
                return null;
            var currentNode = root;
            while (true)
            {
                if (value.CompareTo(currentNode.value) > 0)
                {
                    if (currentNode.GetRight() == null)
                        return null;
                    currentNode = currentNode.GetRight();
                } else if (value.CompareTo(currentNode.value) < 0)
                {
                    if (currentNode.GetLeft() == null)
                        return null;
                    currentNode = currentNode.GetLeft();
                }
                else
                {
                    return currentNode;
                }
            }
        }

        public void Insert(V value)
        {
            if (root == null)
            {
                root = new Node<V>(value, isBlack: true);
                return;
            }
            var currentNode = root;
            while (true)
            {
                if (value.CompareTo(currentNode.value) > 0)
                {
                    if (currentNode.GetRight() != null)
                        currentNode = currentNode.GetRight();
                    else
                    {
                        currentNode.SetRight(new Node<V>(value));
                        currentNode = currentNode.GetRight();
                        goto Inserted;
                    }
                } else if (value.CompareTo(currentNode.value) < 0)
                { 
                    if (currentNode.GetLeft() != null)
                        currentNode = currentNode.GetLeft();
                    else
                    {
                        currentNode.SetLeft(new Node<V>(value, parent: currentNode));
                        currentNode = currentNode.GetLeft();
                        goto Inserted;
                    }
                }
                else
                {
                    Console.WriteLine("Trying to add value already contained in the tree");
                    return;
                }
            }
        Inserted:
            CorrectAfterInsertion(currentNode);
            root.isBlack = true;
        }

        public void CorrectAfterInsertion(Node<V> node)
        {
            if (node.parent != null && node.parent.isBlack)
                return;
            var currentNode = node;
            var grandparent = currentNode.GrandParent();
            if (grandparent == null)
            {
                root.isBlack = true;
                return;
            }
            var uncle = currentNode.Uncle();
            if (currentNode.parent.IsLeftChild()) {
                if (uncle != null && !uncle.isBlack) {
                    //uncle is red then recolor uncle parent and grandparent
                    currentNode.parent.isBlack = true;
                    uncle.isBlack = true;
                    grandparent.isBlack = false;
                    CorrectAfterInsertion(grandparent);
                } else {
                    //uncle is black
                    if (currentNode.IsRightChild()) {;
                        //converting left triangle to left line
                        currentNode.parent.RotateLeft();
                        currentNode = currentNode.GetLeft();
                    }
                    //left line
                    grandparent.RotateRight();
                    currentNode.parent.isBlack = true;
                    grandparent.isBlack = false;
                }
            } else if (currentNode.parent.IsRightChild()) {
                if (uncle != null && !uncle.isBlack) {
                    currentNode.parent.isBlack = true;
                    uncle.isBlack = true;
                    grandparent.isBlack = false;
                    CorrectAfterInsertion(grandparent);
                } else {
                    if (currentNode.IsLeftChild()) {
                        //converting right triangle to right line
                        currentNode.parent.RotateRight();
                        currentNode = currentNode.GetRight();
                    }
                    //right line
                    grandparent.RotateLeft();
                    currentNode.parent.isBlack = true;
//                    currentNode.parent = null;
                    grandparent.isBlack = false;
                    root = currentNode.parent;
                }
            }
            if (root.parent != null) {
                root = CalculateRoot(root) as Node<V>;
            }
            if (currentNode.parent != null && !currentNode.parent.isBlack) {
                //continue for parent if it'    s red
                CorrectAfterInsertion(currentNode.parent);
            }
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            //if(other == null) return root == null ?
            if(obj == null || GetType() != obj.GetType())
                return false;
            
            var other = (Tree<V>) obj;
            var thisIterator = new BfsIterator<V>(this);
            var otherIterator = new BfsIterator<V>(other);
            
            Console.Error.WriteLine($"Comparing trees with root {this.root.value} " +
                                    $"and {other.root.value}");
            
            while(thisIterator.hasNext() && otherIterator.hasNext())
            {
                var thisNode = thisIterator.next();
                var otherNode = otherIterator.next();
                if(!thisNode.Equals(otherNode)) {
                    Console.WriteLine($"Two nodes {thisNode.value} and {otherNode.value} differ." +
                                      $" Trees are not equal.");
                    Console.WriteLine(thisNode);
                    Console.WriteLine(otherNode);
                    Console.WriteLine();
                    return false;
                }
            }

            if (thisIterator.hasNext() != otherIterator.hasNext())
            {
                Console.WriteLine("Trees have different sizes. Not equal.");
                return false;                
            }

            return true;
        }

        private Node<V> CalculateRoot()
        {
            return CalculateRoot(root);
        }

        private Node<V> CalculateRoot(Node<V> startingNode)
        {
            var cur = startingNode;
            while(cur.parent != null)
            {
                cur = cur.parent;
            }
            return cur;
        }
    }
    
    class BfsIterator<V> where V: IComparable<V>
    {
        private Node<V> nextNode;
        private Node<V> currentNode;
        private Queue<Node<V>> queue;

        public BfsIterator(Tree<V> tree)
        {
            currentNode = tree.root;
            nextNode = currentNode;
            queue = new Queue<Node<V>>();
            if (currentNode != null)
                queue.Enqueue(currentNode);
        }

        public bool hasNext()
        {
            return queue.Any();
        }

        public Node<V> next()
        {
            currentNode = queue.Dequeue();
            if (!currentNode.isLeaf())
            {
                foreach (var node in currentNode.GetChildren().ToList())
                {
                    queue.Enqueue(node);
                }
            }
            return currentNode;
        }

    }
}
