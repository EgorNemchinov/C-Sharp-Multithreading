using System;
using System.Threading.Tasks;

namespace Snapshots
{
    internal class Program
    {
        //Tests taken from @SergeySkoret
        //Just so that we all have similar interface
        public static void Main(string[] args)
        {
            var bsw = new BoundedWriter();
            var rd = new Random();
            var tasks = new Task[2];

            for (var i = 0; i < 22; i++)
            {
                var id = i % 2;
                var value = rd.Next(1000);
                tasks[id] = Task.Run(() =>
                {
                    Console.WriteLine("Write value {0} to #{1} register", value, id);
                    bsw.Update(id, value);
                });

                if (i % 3 == 0)
                {
                    var count = i;
                    Task.Run(() =>
                    {
                        Console.WriteLine("Read from {0} thread on {1} iteration: ({2})",
                            id, count, string.Join(", ", bsw.Read(id)));
                    });
                }

                if (i % 2 == 1)
                {
                    Task.WaitAll(tasks);
                }
            }

            Console.WriteLine("----------------");
            bsw.PrintResults();
        }
    }
}
