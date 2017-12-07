using System;
using System.IO;

namespace RedBlackTree
{
    public class Logger
    {
        private static bool debug = false;

        public void DebugOn()
        {
            debug = true;
        }

        public void DebugOff()
        {
            debug = false;
        }
        
        public static void Log(string output, StreamWriter file = null)
        {
            if (!debug)
                return;
            WriteLine(output, file);
        }
        public static void Log(StreamWriter file = null)
        {
            Log("", file);
        }

        public static void Error(string output, StreamWriter file = null)
        {
            WriteLine("Error: " + output, file);
        }

        private static void WriteLine(string line, StreamWriter file)
        {
            if(file == null)
                Console.WriteLine(line);
            else 
                file.WriteLine(line);
        }
    }
}
