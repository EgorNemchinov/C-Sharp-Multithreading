using System;
using System.Threading;

namespace MultiThreading
{
    class QSort
    {
        public const int MAX_THREADS = 15; 
        public const int MIN_ELEMENTS_FOR_THREAD = 50; 
        static int threadCount = 0;
        static void swap(int[] array, int i, int j) {
            int tmp = array[i];
            array[i] = array[j];
            array[j] = tmp;
        }

        public static void ParallelQuickSort(int[] array) {
            StartQuickSort(array, 0, array.Length - 1).Join();
            Console.WriteLine("Threads was created: {0}", threadCount);
            threadCount = 0;
        }
        static void QuickSort(int[] array, int left, int right) {
            if(left >= right) 
                return;
            int p = array[(left + right) / 2];
            int i = left, j = right;
            while (i <= j) {
                while(array[i] < p) 
                    i++;
                while(array[j] > p)
                    j--;
                if(i <= j) {
                    swap(array, i, j);
                    i++;
                    j--;
                }
            };
            

            Thread leftSort = StartQuickSort(array, left, i - 1);
            Thread rightSort = StartQuickSort(array, i, right);

            if(leftSort != null) {
                leftSort.Join();
            }
            if(rightSort != null) {
                rightSort.Join();
            }
        }

        public static Thread StartQuickSort(int[] array, int left, int right) {
            int numberOfThreads = Interlocked.CompareExchange(ref threadCount, 0, 0);

            if(numberOfThreads < MAX_THREADS && (right - left) > MIN_ELEMENTS_FOR_THREAD) {
                var t = new Thread(() => QuickSort(array, left, right));
                Interlocked.Increment(ref threadCount);
                t.Start();
                return t;
            } else {
                QuickSort(array, left, right);
                return null;
            }
        }
    }

    class Testing {
        static void Main(string[] args)
        {
            Console.WriteLine("MAX_THREADS = {0} & MIN_ELEMENTS_FOR_THREAD = {1}.", 
                                QSort.MAX_THREADS, QSort.MIN_ELEMENTS_FOR_THREAD);
            int[] elemCounts = new int[] {1000, 100000, 1000000, 10000000};
            foreach (var count in elemCounts)
            {
                Console.WriteLine("For {0} elements in array QSort took {1} ms.",
                                     count, TimeRandomQSort(count));
            }
        }

        static int TimeRandomQSort(int elementsAmount) {
            var before = DateTime.UtcNow;
            QSort.ParallelQuickSort(RandomArray(elementsAmount));
            var timeTaken = DateTime.UtcNow - before;
            return timeTaken.Milliseconds;
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
