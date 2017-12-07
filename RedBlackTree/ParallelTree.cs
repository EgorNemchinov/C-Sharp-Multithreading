using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RedBlackTree
{
    public class ParallelTree<V> where V : IComparable<V>
    {
        private Tree<V> tree;
        public volatile bool finished = false;
        ConcurrentQueue<TreeOperation<V>> waitQueue = new ConcurrentQueue<TreeOperation<V>>();

        ConcurrentDictionary<TreeOperation<V>, byte> runningTasks =
            new ConcurrentDictionary<TreeOperation<V>, byte>();

        private volatile TreeOperation<V> lastRunning = null;


        public ParallelTree(Tree<V> tree)
        {
            this.tree = tree;
        }

        public Tree<V> GetTree()
        {
            return tree;
        }

        public Task Exit(V value)
        {
            var exit = new ExitOperation<V>(this, value);
            waitQueue.Enqueue(exit);
            return exit.task;
        }

        public void Insert(V value)
        {
            var operation = new InsertOperation<V>(this, value);
            waitQueue.Enqueue(operation);
            RunOperations();
        }

        public void Find(V value)
        {
            var operation = new FindOperation<V>(this, value);
            waitQueue.Enqueue(operation);
            RunOperations();
        }

        public void Remove(V value)
        {
            var operation = new RemoveOperation<V>(this, value);
            waitQueue.Enqueue(operation);
            RunOperations();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RunOperations()
        {
            if (finished)
                return;

            TreeOperation<V> curOperation;
            Console.WriteLine("\nRunOperations()");

            do
            {
                if (waitQueue.IsEmpty)
                    return;
                if (!waitQueue.TryPeek(out curOperation))
                {
                    Console.WriteLine("Couldn't peek at wait queue.");
                    return;
                }

                if (curOperation == null)
                {
                    Console.WriteLine("CurOperation is null, waitqueue is empty though.");
                }

                Console.WriteLine($"Took element from waitqueue: {curOperation} ");

                if (runningTasks.IsEmpty)
                {
                    Console.WriteLine($"Running queue is empty, let's add element.");

                    if (!waitQueue.TryDequeue(out curOperation))
                    {
                        Console.WriteLine("Unsuccesful dequeing.");
                        return;
                    }
                    curOperation.Run();
                    runningTasks[curOperation] = 1;
                    lastRunning = curOperation;

                    Console.WriteLine($"Added operation: {curOperation}");
                }
                else
                {
                    Console.WriteLine($"Running queue is not empty, let's check.");

                    if (lastRunning?.type == TreeOperation<V>.Type.Find &&
                        curOperation.type == TreeOperation<V>.Type.Find)
                    {
                        if (!waitQueue.TryDequeue(out curOperation))
                        {
                            Console.WriteLine("Unsuccesful dequeing.");
                            return;
                        }
                        if (runningTasks.ContainsKey(curOperation))
                            return;
                        curOperation.Run();
                        runningTasks[curOperation] = 1;
                        lastRunning = curOperation;
                        Console.WriteLine($"Added operation: {curOperation}");
                    }
                    else
                    {
                        Console.WriteLine("Finish RunOperations()\n");
                        return;
                    }
                }
            } while (!waitQueue.IsEmpty && !finished);
            Console.WriteLine("Finish RunOperations()\n");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void FinishOperation(TreeOperation<V> operation)
        {
            Console.WriteLine($"Finish {operation}");

            byte val;
            if (!runningTasks.TryRemove(operation, out val))
            {
                Console.WriteLine($"Unsuccesful deletion of {operation}");
            }
            if (!finished)
                RunOperations();
        }
    }

    abstract class TreeOperation<V> where V : IComparable<V>
    {
        public enum Type
        {
            Find,
            Insert,
            Remove,
            Exit
        }

        public Type type;
        public Task task;
        protected ParallelTree<V> tree;
        protected V value;
        private volatile bool wasRun = false;

        protected TreeOperation(Type type, ParallelTree<V> tree, V value)
        {
            this.type = type;
            this.tree = tree;
            this.value = value;
        }

        public override string ToString()
        {
            return $"{type} {value}";
        }

        protected abstract void Action();

        public virtual Task Run()
        {
            if (wasRun)
                return Task.Delay(0);
            wasRun = true;
            task = Task.Run(() =>
            {
                Console.WriteLine($"Run {this}");
                Action();
                tree.FinishOperation(this);
            });
            return task;
        }
    }

    class InsertOperation<V> : TreeOperation<V> where V : IComparable<V>
    {
        public InsertOperation(ParallelTree<V> tree, V value)
            : base(Type.Insert, tree, value)
        {
            Console.WriteLine("Created InsertOperation");
        }

        protected override void Action()
        {
            tree.GetTree().Insert(value);
        }
    }

    class FindOperation<V> : TreeOperation<V> where V : IComparable<V>
    {
        public FindOperation(ParallelTree<V> tree, V value)
            : base(Type.Find, tree, value)
        {
            Console.WriteLine("Created FindOperation");
        }

        protected override void Action()
        {
            tree.GetTree().Find(value);
        }
    }

    class RemoveOperation<V> : TreeOperation<V> where V : IComparable<V>
    {
        public RemoveOperation(ParallelTree<V> tree, V value)
            : base(Type.Remove, tree, value)
        {
            Console.WriteLine("Created RemoveOperation");
        }

        protected override void Action()
        {
            tree.GetTree().Remove(value);
        }
    }


    class ExitOperation<V> : TreeOperation<V> where V : IComparable<V>
    {
        public ExitOperation(ParallelTree<V> tree, V value)
            : base(Type.Exit, tree, value)
        {
            task = new Task(() =>
            {
                Console.WriteLine("ExitTask executed");
                tree.finished = true;
            });
        }

        public override Task Run()
        {
            task.RunSynchronously();
            return task;
        }

        protected override void Action()
        {
        }
    }
}
