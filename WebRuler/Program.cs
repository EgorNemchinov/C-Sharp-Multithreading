using System;
using System.Diagnostics;

namespace WebRuler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            CompareTimeTest("https://vk.com", 2);
        }

        public static void CompareTimeTest(String url, int depth)
        {
            Stopwatch stopwatch = new Stopwatch();
            WebParser parser = new WebParser();
                
            stopwatch.Start();
            parser.Execute(url, depth);
            stopwatch.Stop();
            var simpleTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Finished simple parsing.\n");

            stopwatch.Restart();
            parser.ExecuteAsync(url, depth).Wait();
            stopwatch.Stop();
            var parallelTime = stopwatch.ElapsedMilliseconds;
            Console.WriteLine("Finished parallel parsing.\n");

            Console.WriteLine();
            Console.WriteLine($"Parallel parsing of {url} took" +
                              $" {parallelTime} ms");
            
            Console.WriteLine($"Not parallel parsing of {url} took" +
                              $" {simpleTime} ms");
        }
    }
}
