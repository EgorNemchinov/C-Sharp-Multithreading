using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;

namespace ParallelPrimeNumbersSearch
{
    class Test {
        static int Main(string[] args)
        {
            int[] counts = new int[] {1000, 10000, 100000, 1000000, 10000000};
            foreach(var count in counts)
                TimeTest(count);
                
            return 0;
        }
        
        static void TimeTest(int x) {
            Stopwatch stopWatch = new Stopwatch();
            
            stopWatch.Start();
            PrimeNumbers.SimplePrimeNumbers(x);
            stopWatch.Stop();
            Console.WriteLine("Simple prime search for {0} values took {1} ms.",
                              x, stopWatch.ElapsedMilliseconds);
            
            stopWatch.Restart();
            PrimeNumbers.ParallelPrimeNumbers(x);
            stopWatch.Stop();
            Console.WriteLine("Parallel prime search for {0} values took {1} ms.",
                              x, stopWatch.ElapsedMilliseconds);
            
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

        public static IList<int> ParallelPrimeNumbers(int x) {
            Range[] ranges = GenerateRanges(x);
            Task<IList<int>>[] tasks = new Task<IList<int>>[ranges.Length];
            List<int> res = new List<int>();

            for(int i = 0; i < tasks.Length; i++)
            {
                int from = ranges[i].left, to = ranges[i].right;
                tasks[i] = new Task<IList<int>>(() => 
                PrimeNumbersInRange(from, to));
            }
            for(int i = 0; i < tasks.Length; i++)
            {
                tasks[i].Start();
            }
            Task t = Task.WhenAll(tasks);
            try {
                t.Wait();
            } catch {}
            for(int i = 0; i < tasks.Length; i++)
            {  
                tasks[i].Wait(); 
                res.AddRange(tasks[i].Result);
            }
            return res.ToArray();
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