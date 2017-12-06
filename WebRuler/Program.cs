using System;
using System.Diagnostics;
using System.IO;

namespace WebRuler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CompareTimeTest("https://github.com/", 2);
        }

        public static void CompareTimeTest(String url, int depth)
        {
            Stopwatch stopwatch = new Stopwatch();
            WebParser parser = new WebParser();

            string parallelPath = @"out_par.txt";
            string simplePath = @"out_sim.txt";

            using (StreamWriter outputFile = new StreamWriter(simplePath))
            {
                parser.ClearVisited();
                stopwatch.Start();
                parser.Execute(url, depth, null);
                stopwatch.Stop();
                Console.WriteLine("Finished simple parsing.");    
            }
            var simpleTime = stopwatch.ElapsedMilliseconds;

            using (StreamWriter outputFile = new StreamWriter(parallelPath))
            {
                parser.ClearVisited();
                stopwatch.Restart();
                parser.ExecuteAsync(url, depth, null).Wait();
                stopwatch.Stop();
                Console.WriteLine("Finished parallel parsing.");
            }
            var parallelTime = stopwatch.ElapsedMilliseconds;

            
            Console.WriteLine();
            Console.WriteLine($"Parallel parsing of {url} took" +
                              $" {parallelTime} ms");
            
            Console.WriteLine($"Not parallel parsing of {url} took" +
                              $" {simpleTime} ms");
        }
    }
}
