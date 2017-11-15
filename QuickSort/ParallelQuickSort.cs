﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace QuickSort
{
    class QSort
    {
        public const int MAX_THREADS = 15;
        public const int MIN_ELEMENTS_FOR_THREAD = 50; 
        static int threadCount = 0;

        static void Swap<T>(IList<T> list, int i, int j)
        {
            T tmp = list[i];
            list[i] = list[j];
            list[j] = tmp;
        }

        static int DoPartition<T>(IList<T> list, int left, int right) 
            where T:IComparable<T>
        {
            if(left >= right) 
                return -1;
            T p = list[(left + right) / 2];
            int i = left, j = right;
            
            while (i <= j) {
                while(list[i].CompareTo(p) < 0) 
                    i++;
                while(list[j].CompareTo(p) > 0)
                    j--;
                if(i <= j) {
                    Swap<T>(list, i, j);
                    i++;
                    j--;
                }
            };
            
            return i;
        }

        static void QuickSort<T>(IList<T> array, int left, int right, bool parallel)
                    where T : IComparable<T>
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

        public static void SimpleQuickSort<T>(IList<T> array)
                            where T : IComparable<T>
        {
            QuickSort(array, 0, array.Count - 1, false);
        }
        
        public static void ParallelQuickSort<T>(IList<T> array)
                            where T : IComparable<T>
        {
            StartQuickSort(array, 0, array.Count - 1)?.Join();
            Console.WriteLine("Threads was created: {0}", threadCount);
            threadCount = 0;
        }

        public static Thread StartQuickSort<T>(IList<T> array, int left, int right) 
                        where T : IComparable<T> {
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

    class Tests {
         public static void Run()
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

        static void PrintArray<T>(IList<T> array)
        {
            for (int i = 0; i < array.Count; i++)
            {
                Console.Write(array[i]);
                Console.Write(" ");
            }
            Console.WriteLine();
        }

    }
}