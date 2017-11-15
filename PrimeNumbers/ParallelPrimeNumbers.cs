using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace PrimeNumbers
{
    public enum Method
    {
        Tasks, Threads, ThreadPool
    }
    
    class Tests {
        public static void Run()
        {
            int[] counts = new int[] {1000, 100000, 1000000, 10000000};
            foreach(var count in counts)
                TimeTest(count);

            Console.WriteLine("Finished.");
        }
        
        static void TimeTest(int x) {
            Stopwatch stopWatch = new Stopwatch();
            
            Method[] methods =
            {
                Method.Tasks, Method.ThreadPool, Method.Threads
            };

            stopWatch.Start();
            IList<int> standardPrimes = PrimeNumbers.SimplePrimeNumbers(x);
            stopWatch.Stop();
            Console.WriteLine("Simple prime search for {0} values took {1} ms.",
                              x, stopWatch.ElapsedMilliseconds);
            

            foreach (var method in methods)
            {
                stopWatch.Restart();
                IList<int> numbers = PrimeNumbers.ParallelPrimeNumbers(x, method);
                stopWatch.Stop();
                
                if(!numbers.SequenceEqual(standardPrimes))
                    Console.WriteLine("Failed: prime search with {0} returns wrong result.",
                                        method);
                Console.WriteLine("Parallel prime search with {0} for {1} values took {2} ms.",
                    method, x, stopWatch.ElapsedMilliseconds);
            }
            
            Console.WriteLine();
        }
        
        static void PrintArray(IList<int> array) 
        {
            for (int i = 0; i < array.Count; i++)
            {
                Console.Write(array[i]);
                Console.Write(" ");
            }
            Console.WriteLine();
        }
    }
        
    class Range {
        public int left, right;
        
        public Range(int left, int right) {
            this.left = left;
            this.right = right;
        }
    }
    
    class PrimeNumbers {
        public const int MAX_THREADS = 15;

        public static IList<int> SimplePrimeNumbers(int x) {
            return PrimeNumbersInRange(0, x);
        }
        
        public static IList<int> ParallelPrimeNumbers(int x, Method method) {
            var ranges = GenerateRanges(x);
            switch (method)
            {
                case Method.Tasks:
                    return TaskPrimes(ranges);
                case Method.ThreadPool:
                    return ThreadPoolPrimes(ranges);
                case Method.Threads:
                    return ThreadsPrimes(ranges);
                default:
                    throw new ArgumentOutOfRangeException(nameof(method), method, null);
            }
        }

        public static IList<int> TaskPrimes(Range[] ranges)
        {
            var tasks = new Task<IList<int>>[ranges.Length];
            var res = new List<int>();

            for(int i = 0; i < tasks.Length; i++)
            {
                int from = ranges[i].left, to = ranges[i].right;
                tasks[i] = new Task<IList<int>>(() => 
                    PrimeNumbersInRange(from, to));
            }
            foreach (Task<IList<int>> task in tasks)
            {
                task.Start();
            }
            Task t = Task.WhenAll(tasks);
            try {
                t.Wait();
            } catch {}
            
            foreach (Task<IList<int>> task in tasks)
            {
                task.Wait();
                res.AddRange(task.Result);
            }
            return res.ToArray();
        }

        public static IList<int> ThreadPoolPrimes(Range[] ranges)
        {
            List<int> res = new List<int>();
            IList<int>[] lists = new IList<int>[ranges.Length];
            
            var events = new ManualResetEvent[ranges.Length];
            
            for (int i = 0; i < ranges.Length; i++)
            {
                int ind = i;
                
                events[ind] = new ManualResetEvent(false);
                ThreadPool.QueueUserWorkItem(delegate
                {
                    lists[ind] = PrimeNumbersInRange(ranges[ind].left, ranges[ind].right);
                    events[ind].Set();
                });

            }
            WaitHandle.WaitAll(events);
            
            foreach (var list in lists)
            {
                res.AddRange(list);
            }
            return res;
        }

        public static IList<int> ThreadsPrimes(Range[] ranges)
        {
            var res = new List<int>();
            var threads = new Thread[ranges.Length];
            var lists = new IList<int>[ranges.Length];

            for (int i = 0; i < ranges.Length; i++)
            {
                int ind = i;
                threads[ind] = new Thread(() =>
                    lists[ind] = PrimeNumbersInRange(ranges[ind].left, ranges[ind].right));
            }
            foreach (var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }
            
            foreach (var list in lists)
            {
                res.AddRange(list);
            }
            return res;
        }

        static IList<int> PrimeNumbersInRange(int lo, int hi) {
            // Trying to pick the perfect list size
            int listSize = AmountOfPrimesLessThan(hi) - AmountOfPrimesLessThan(lo);
            List<int> primes = new List<int>(Math.Max(0, listSize));

            for(int i = lo; i <= hi; i++) {
                if(isPrime(i))
                {
                    primes.Add(i);
                }
            }
            return primes;
        }

        //TODO: Change to more balanced way than splitting into N equal pieces.
        static Range[] GenerateRanges(int x) {
            Range[] ranges = new Range[MAX_THREADS];
            int step = x / MAX_THREADS;
            for(int i = 0; i < ranges.Length - 1; i++) {
                ranges[i] = new Range(i*step, (i+1)*step - 1);
            }
            ranges[ranges.Length - 1] = new Range((ranges.Length - 1)*step, x - 1);
            return ranges;
        }

        // Approximation
        static int AmountOfPrimesLessThan(int x) {
            if (x < 2) return 0;
            return (Int32) (x / Math.Log(x - 1, 3));
        }

        static bool isPrime(int num) {
            if(num < 2) return false;
            for(int i = 2; i <= Math.Ceiling(Math.Sqrt(num)); i++) {
                if(num % i == 0) 
                    return false;
            }
            return true;
        }
    }
}
