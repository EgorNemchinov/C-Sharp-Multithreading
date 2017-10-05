using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace ParallelQSort
{
    class QSort
    {
        public const int MAX_THREADS = 15;
        public const int MIN_ELEMENTS_FOR_THREAD = 50; 
        static int threadCount = 0;

        static void Swap(int[] array, int i, int j)
        {
            int tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }

        static int DoPartition(int[] array, int left, int right)
        {
            if(left >= right) 
                return -1;
            int p = array[(left + right) / 2];
            int i = left, j = right;
            
            while (i <= j) {
                while(array[i] < p) 
                    i++;
                while(array[j] > p)
                    j--;
                if(i <= j) {
                    Swap(array, i, j);
                    i++;
                    j--;
                }
            };
            
            return i;
        }

        static void QuickSort(int[] array, int left, int right, bool parallel)
        {
            int i = DoPartition(array, left, right);
            if (i == -1)
                return;

            if (parallel)
            {
                Thread leftSort = StartQuickSort(array, left, i - 1);
                Thread rightSort = StartQuickSort(array, i, right);

                leftSort?.Join();
                rightSort?.Join();
            }
            else
            {
                QuickSort(array, left, i - 1, false);
                QuickSort(array, i, right, false);
            }
        }

        public static void SimpleQuickSort(int[] array)
        {
            QuickSort(array, 0, array.Length - 1, false);
        }
        
        public static void ParallelQuickSort(int[] array) {
            StartQuickSort(array, 0, array.Length - 1)?.Join();
            Console.WriteLine("Threads was created: {0}", threadCount);
            threadCount = 0;
        }

        public static Thread StartQuickSort(int[] array, int left, int right) {
            int numberOfThreads = Interlocked.CompareExchange(ref threadCount, 0, 0);

            if(numberOfThreads < MAX_THREADS && (right - left) > MIN_ELEMENTS_FOR_THREAD) {
                var t = new Thread(() => QuickSort(array, left, right, true));
                Interlocked.Increment(ref threadCount);
                t.Start();
                return t;
            } else {
                QuickSort(array, left, right, false);
                return null;
            }
        }
    }

    class Testing {
         static void Main(string[] args)
        {
            Console.WriteLine("MAX_THREADS = {0} & MIN_ELEMENTS_FOR_THREAD = {1}.", 
                                QSort.MAX_THREADS, QSort.MIN_ELEMENTS_FOR_THREAD);
            int[] elemCounts = new int[] {100, 1000, 100000, 1000000, 10000000};
            foreach (var count in elemCounts)
            {
                TimeTestRandomQSort(count);
            }
        }

        static void TimeTestRandomQSort(int elementsAmount)
        {
            var array = RandomArray(elementsAmount);
            Stopwatch stopWatch = new Stopwatch();
            
            stopWatch.Start();
            QSort.SimpleQuickSort(array);
            stopWatch.Stop();
            Console.WriteLine("For {0} random elements simple QSort took {1} ms.",
                elementsAmount, stopWatch.ElapsedMilliseconds);
            
            stopWatch.Restart();
            QSort.ParallelQuickSort(array);
            stopWatch.Stop();
            Console.WriteLine("For {0} random elements parallel QSort took {1} ms.",
                                elementsAmount, stopWatch.ElapsedMilliseconds);
            
            Console.WriteLine();
        }

        static int[] RandomArray(int length) {
            var randomizer = new Random();
            int[] array = new int[length];
            for(int i = 0; i < array.Length; i++) {
                array[i] = randomizer.Next();
            }
            return array;
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
}
