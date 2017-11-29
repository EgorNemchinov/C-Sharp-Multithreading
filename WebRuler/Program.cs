using System;

namespace WebRuler
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            WebParser parser = new WebParser();
            parser.Execute("https://www.google.ru/", 2);
            Console.WriteLine("\nFinished.");
        }
    }
}
