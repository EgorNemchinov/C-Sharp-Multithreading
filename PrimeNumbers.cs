using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ParallelPrimeNumbersSearch
{
    class Test {
        static int Main(string[] args)
        {
            int[] counts = new int[] {1000, 10000, 100000, 1000000, 10000000};
            foreach(var count in counts) {
                TimeTest(count, false);
                TimeTest(count, true);
                Console.WriteLine();
            }
            return 0;
        
        }
        static void TimeTest(int x, bool parallel) {
            var before = DateTime.UtcNow;
            if(parallel)
                PrimeNumbers.ParallelPrimeNumbers(x);
            else
                PrimeNumbers.SimplePrimeNumbers(x);
            var timeTaken = DateTime.UtcNow - before;
            Console.WriteLine("{0} prime search for {1} values took {2} ms.",
                            (parallel ? "Parallel" : "Not parallel"), x, timeTaken.Milliseconds);
        }
        static void PrintArray(int[] array)
        {
            for (int i = 0; i < array.Length; i++)
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

        public static int[] SimplePrimeNumbers(int x) {
            return FilterZeros(PrimeNumbersInRange(0, x));
        }

        public static int[] ParallelPrimeNumbers(int x) {
            Range[] ranges = GenerateRanges(x);
            Task<int[]>[] tasks = new Task<int[]>[ranges.Length];
            List<int> res = new List<int>();

            for(int i = 0; i < tasks.Length; i++)
            {
                int from = ranges[i].left, to = ranges[i].right;
                tasks[i] = new Task<int[]>(() => 
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
                // Console.WriteLine("Adding task{0} result:", i);
                // PrintArray(tasks[i].Result);
                res.AddRange(FilterZeros(tasks[i].Result));
            }
            // PrintArray(res.ToArray());
            return res.ToArray();
        }

        static int[] PrimeNumbersInRange(int lo, int hi) {
            // Trying to pick the perfect array size
            int arraySize = 3 +  AmountOfPrimesLessThan(hi) - AmountOfPrimesLessThan(lo);
            int[] primes = new int[arraySize];
            int ptr = 0;

            for(int i = lo; i <= hi; i++) {
                if(isPrime(i)) { 
                    primes[ptr] = i;
                    ptr++;
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

        //Removes zeros from the array
        static int[] FilterZeros(int[] array) {
            int lastNumIndex = -1;
            for(int i = array.Length - 1; i >= 0; i--) {
                if(array[i] != 0) {
                    lastNumIndex = i;
                    break;
                }
            }
            if(lastNumIndex == -1)
                return array;
            int[] final = new int[lastNumIndex+1];
            Array.Copy(array, 0, final, 0, lastNumIndex+1);
            return final;
        }

        // Ceiling approximation
        static int AmountOfPrimesLessThan(int x) {
            if (x < 2) return 0;
            return (Int32) (1.1*x / Math.Log(x - 1, 3)) + 1;
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