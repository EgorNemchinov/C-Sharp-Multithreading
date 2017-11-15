
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace RedBlackTree
{
    
    public class Node<V> where V: IComparable<V>
    {
        
        public V value;
        public Node<V> parent;
        private Pair<Node<V>> children;
        public bool isBlack = false;

        public Node(object value = null, Pair<Node<V>> children = null, 
                    Node<V> parent = null, bool isBlack = false)
        {
            if(value != null)
                this.value = (V) value;
            this.parent = parent;
            this.children = children;
            this.isBlack = isBlack;
        }
        
        public void RotateLeft() {
            if(this.GetRight() == null)
                return;
//            Logger.debugInfo("rotateLeft() executed for $this")
            var rightChild = this.GetRight();
            var floatingNode = rightChild.GetLeft(); //this will later become right child of this node;
            this.SetParentsReferenceTo(rightChild); //linking node's parent to node's right child
            rightChild.SetLeft(this);
            SetRight(floatingNode);
        }
        public void RotateRight() {
            if(this.GetLeft() == null)
                return;
            var leftChild = this.GetLeft();
            var floatingNode = leftChild.GetRight();
            this.SetParentsReferenceTo(leftChild);
            leftChild.SetRight(this);
            SetLeft(floatingNode);
//            Logger.debugInfo("rotateRight() executed for $this")
        }

        public void SetParentsReferenceTo(Node<V> newChild)
        {
            if (newChild != null)
                newChild.parent = this.parent;
            if (parent == null)
                return;
            if(IsLeftChild())
                parent.SetLeft(newChild);
            else if(IsRightChild())
                parent.SetRight(newChild);
            else 
                Console.WriteLine("Incorrect SetParentsReferenceTo() call." +
                                  " Current node is not child of it's parent.");
            parent = null;
        }

        public Node<V> GetLeft()
        {
            return GetChildren().left;
        }
        
        public Node<V> GetRight()
        {
            return GetChildren().right;
        }

        public void SetLeft(Node<V> value)
        {
            GetChildren().left = value;
            if(value != null)
                value.parent = this;
        }
        
        public void SetRight(Node<V> value)
        {
            GetChildren().right = value;
            if(value != null)
                value.parent = this;
        }
        
        public Pair<Node<V>> GetChildren()
        {
            if (children == null)
                children = new Pair<Node<V>>(null, null);
            return children;
        }

        public Node<V> GrandParent()
        {
            return parent?.parent;
        }

        public Node<V> Brother()
        {
            if (parent == null)
                return null;
            return IsLeftChild() ? parent.GetRight() : parent.GetLeft();
        }

        public Node<V> Uncle()
        {
            var grandparent = GrandParent();
            if (GrandParent() == null)
                return null;
            return parent.IsLeftChild() ? grandparent.GetRight() : grandparent.GetLeft();
        }

        public void Recolor()
        {
            isBlack = !isBlack;
        }

        public bool IsLeftChild()
        {
            return parent != null && parent.GetLeft() == this;
        }
        
        public bool IsRightChild()
        {
            return parent != null && parent.GetRight() == this;
        }

        public bool isLeaf()
        {
            return GetLeft() == null && GetRight() == null;
        }

        public bool isNil()
        {
            return value == null && GetLeft() == null && GetRight() == null;
        }

        public override string ToString()
        {
            return String.Format("Node: value {0}, parent's value {1}, left child {2}, " +
                                 "right child {3}, isBlack {4}", value,
                                parent == null ? null : parent.value.ToString(),
                                GetLeft() == null ? null : GetLeft().value.ToString(),
                                GetRight() == null ? null : GetRight().value.ToString(),
                                isBlack);
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
                return true;
            if (obj == null)
                return this.value == null;
            if(obj.GetType() != GetType())
                return false;

            Node<V> other = (Node<V>) obj;

//            Console.Error.WriteLine($"Comparing nodes {this.value} and {other.value}");
            if (!other.value.Equals(value))
                return false;
            if (other.isBlack != isBlack)
                return false;
            if (other.parent == null ^ parent == null)
                return false;
            if (parent != null && !parent.value.Equals(other.parent.value))
                return false;
            
            var otherChildren = other.children.ToList();
            var thisChildren = other.children.ToList();
            if (thisChildren.Count != otherChildren.Count)
                return false;
            for (int i = 0; i < thisChildren.Count; i++)
            {
                if (!thisChildren[i].value.Equals(otherChildren[i].value))
                    return false;
            }

            return true;
        }
    }

    public class Pair<T>
    {
        public T left;
        public T right;

        public Pair(T left, T right)
        {
            this.left = left;
            this.right = right;
        }

        public List<T> ToList()
        {
            var list = new List<T>();
            if (left != null)
                list.Add(left);
            if (right != null)
                list.Add(right);
            return list;
        }
    }
}
