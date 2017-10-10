using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace ParallelMD5
{
    class ParallelFolderMD5
    {
        static void Main(string[] args)
        {
            MD5 md5 = MD5.Create();
            string folderHash = FolderMD5(md5, ".");
            Console.WriteLine("Hash of folder is {0}", folderHash);
        }

        static string FolderMD5(MD5 md5Hash, string folderPath)
        {
            string folderName = NameFromPath(folderPath);
            StringBuilder sBuilder = new StringBuilder();
            Console.WriteLine("Starting to compute hash of folder: {0}", folderName);
            
            sBuilder.Append(StringMD5(md5Hash, folderName));
            
            string[] paths = Directory.GetFiles(folderPath);
            foreach (var path in paths)
            {
                sBuilder.Append(FileMD5(md5Hash, path)); //parallel here
            }
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
            Console.WriteLine("Computed hash of file: {0}", NameFromPath(filePath));
            
            return BytesToString(data);
        }

        static string BytesToString(byte[] bytes)
        {
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sBuilder.Append(bytes[i].ToString("x2"));
            }
            return sBuilder.ToString();            
        }

        static string NameFromPath(string path)
        {
            var parts = path.Split("/");
            if (parts.Length < 2)
                return parts.Last();
            return parts.Last().Length == 0 ? parts[parts.Length - 2] : parts.Last();
        }
    }
   
}