﻿using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MD5Hash
{
    class ParallelFolderMD5
    {
        //Whether to print in-between results like calculating MD5 of file
        private const bool Verbose = false;

        private const int MaxThreads = 15;

        public static void TimeTest(string folderPath)
        {
            Stopwatch stopWatch = new Stopwatch();
            
            stopWatch.Start();
            string folderHash = FolderMD5(MD5.Create(), folderPath, false);
            stopWatch.Stop();
            Console.WriteLine("Simple calculating MD5 of the folder {0} took {1} ms.\n" +
                              "Hash is {2}.\n", NameFromPath(folderPath),
                                stopWatch.ElapsedMilliseconds, folderHash);
            
            stopWatch.Restart();
            folderHash = FolderMD5(MD5.Create(), folderPath, false);
            stopWatch.Stop();
            Console.WriteLine("Parallel calculating MD5 of the folder {0} took {1} ms.\n" +
                              "Hash is {2}.\n", NameFromPath(folderPath),
                                stopWatch.ElapsedMilliseconds, folderHash);
        }

        static string FolderMD5(MD5 md5Hash, string folderPath, bool parallel)
        {
            if (!Directory.Exists(folderPath))
            {
                Console.WriteLine("Directory {0} doesn't exist.\n", folderPath);
                return "ERROR";
            }
            
            string folderName = NameFromPath(folderPath);
            StringBuilder sBuilder = new StringBuilder();
            if(Verbose)
                Console.WriteLine("Starting to compute hash of folder: {0}", folderName);
            
            sBuilder.Append(StringMD5(md5Hash, folderName));
            string[] paths = Directory.GetFiles(folderPath);
            
            if (parallel)
            {
                string[] hashes = new string[paths.Length];
                Parallel.For(0, paths.Length, new ParallelOptions
                    {
                        MaxDegreeOfParallelism = MaxThreads
                    },
                    index =>
                    {
                        MD5 md5 = MD5.Create();
                        hashes[index] = FileMD5(md5, paths[index]);
                        if(Verbose)
                            Console.WriteLine("Computed hash of file {0}", NameFromPath(paths[index]));
                    });
                sBuilder.Append(hashes);
            }
            else
            {
                foreach (var path in paths)
                {
                    sBuilder.Append(FileMD5(md5Hash, path)); 
                }    
            }
            if(Verbose)
                Console.WriteLine("Finished computing folder hash.\n");
            
            return StringMD5(md5Hash, sBuilder.ToString());
        }
        
        static string StringMD5(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            return BytesToString(data);
        }

        static string FileMD5(MD5 md5Hash, string filePath)
        {
            byte[] fileBytes = File.ReadAllBytes(filePath);
            byte[] data = md5Hash.ComputeHash(fileBytes);
            if(Verbose)
                Console.WriteLine("Computed hash of file: {0}", NameFromPath(filePath));
            
            return BytesToString(data);
        }

        private static string BytesToString(byte[] bytes)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sBuilder.Append(bytes[i].ToString("x2"));
            }
            return sBuilder.ToString();            
        }

        private static string NameFromPath(string path)
        {
            if (Directory.Exists(path))
                return new DirectoryInfo(path).Name;
            else
                return new FileInfo(path).Name;
        }
    }

    class Tests
    {
        public static void Run(String path = "./")
        {
            ParallelFolderMD5.TimeTest(path);
        }
    }
   
}
