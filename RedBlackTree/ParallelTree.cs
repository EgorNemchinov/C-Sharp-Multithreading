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

        private ConcurrentQueue<TreeOperation<V>> waitQueue =
            new ConcurrentQueue<TreeOperation<V>>();

        private ConcurrentDictionary<TreeOperation<V>, byte> runningTasks =
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
            Logger.Log("\nRunOperations()");

            do
            {
                if (waitQueue.IsEmpty)
                    return;
                
                if (!waitQueue.TryPeek(out curOperation))
                {
                    Logger.Log("Couldn't peek at wait queue.");
                    return;
                }

                Logger.Log($"Took element from waitqueue: {curOperation} ");

                if (runningTasks.IsEmpty)
                {
                    Logger.Log($"Running queue is empty, let's add element.");

                    if (!waitQueue.TryDequeue(out curOperation))
                    {
                        Logger.Log("Unsuccesful dequeing.");
                        return;
                    }
                    curOperation.Run();
                    runningTasks[curOperation] = 1;
                    lastRunning = curOperation;

                    Logger.Log($"Added operation: {curOperation}");
                }
                else
                {
                    Logger.Log($"Running queue is not empty, let's check.");

                    if (lastRunning?.type == TreeOperation<V>.Type.Find &&
                        curOperation.type == TreeOperation<V>.Type.Find)
                    {
                        if (!waitQueue.TryDequeue(out curOperation))
                        {
                            Logger.Log("Unsuccesful dequeing.");
                            return;
                        }
                        
                        if (runningTasks.ContainsKey(curOperation))
                            return;
                        
                        curOperation.Run();
                        runningTasks[curOperation] = 1;
                        lastRunning = curOperation;
                        Logger.Log($"Added operation: {curOperation}");
                    }
                    else
                    {
                        Logger.Log("Finish RunOperations()\n");
                        return;
                    }
                }
            } while (!waitQueue.IsEmpty && !finished);
            Logger.Log("Finish RunOperations()\n");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void FinishOperation(TreeOperation<V> operation)
        {
            Logger.Log($"Finish {operation}");

            byte val;
            if (!runningTasks.TryRemove(operation, out val))
            {
                Logger.Log($"Unsuccesful deletion of {operation}");
            }
            if (!finished)
                RunOperations();
        }
    }

    internal abstract class TreeOperation<V> where V : IComparable<V>
    {
        public enum Type
        {
            Find,
            Insert,
            Remove,
            Exit
        }

        public Type type;
        protected Action action = () => { };
        public Task task;
        protected ParallelTree<V> tree;
        protected V value;
        private volatile bool wasRun;

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

        public virtual Task Run()
        {
            if (wasRun)
                return Task.Delay(0);
            wasRun = true;
            task = Task.Run(() =>
            {
                Logger.Log($"Run {this}");
                action();
                tree.FinishOperation(this);
            });
            return task;
        }
    }

    internal class InsertOperation<V> : TreeOperation<V> where V : IComparable<V>
    {
        public InsertOperation(ParallelTree<V> tree, V value)
            : base(Type.Insert, tree, value)
        {
            Logger.Log("Created InsertOperation");
            action = () => tree.GetTree().Insert(value);
        }
    }

    internal class FindOperation<V> : TreeOperation<V> where V : IComparable<V>
    {
        public FindOperation(ParallelTree<V> tree, V value)
            : base(Type.Find, tree, value)
        {
            Logger.Log("Created FindOperation");
            action = () => tree.GetTree().Find(value);
        }
    }

    internal class RemoveOperation<V> : TreeOperation<V> where V : IComparable<V>
    {
        public RemoveOperation(ParallelTree<V> tree, V value)
            : base(Type.Remove, tree, value)
        {
            Logger.Log("Created RemoveOperation");
            action = () => tree.GetTree().Remove(value);
        }
    }


    internal class ExitOperation<V> : TreeOperation<V> where V : IComparable<V>
    {
        public ExitOperation(ParallelTree<V> tree, V value)
            : base(Type.Exit, tree, value)
        {
            task = new Task(() =>
            {
                Logger.Log("ExitTask executed");
                tree.finished = true;
            });
        }

        public override Task Run()
        {
            task.RunSynchronously();
            return task;
        }
    }
}
