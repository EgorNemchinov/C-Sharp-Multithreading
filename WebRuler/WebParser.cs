using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

namespace WebRuler
{
    public class WebParser
    {
        private const int MaxThreads = 15;

        public void Execute(String url, int depth, StreamWriter outWriter = null)
        {
            string source;
            try
            {
                source = new WebClient().DownloadString(url);
            }
            catch
            {
                WriteLog($"Unable to download page {url}.", outWriter);
                return;
            }

            IList<String> links = ParseLinks(source);
            var length = source.Length;
            
            WriteLog($"Length of page '{url}' is {length}", outWriter);
            if (depth == 0)
                return;
            WriteLog($"\n------------ {depth} m. above the ground ------------", outWriter);
        

            foreach (String link in links)
            {
                Execute(link, depth - 1, outWriter);
            }
        }
        
        public async Task ExecuteAsync(String url, int depth, StreamWriter outWriter = null)
        {
            string source;
            try
            {
                source = await new WebClient().DownloadStringTaskAsync(url);                
            } 
            catch
            {
                WriteLog($"Unable to download page {url}.", outWriter);
                return;
            }
            
            IList<String> links = ParseLinks(source);
            var length = source.Length;
            
            WriteLog($"Length of page '{url}' is {length}", outWriter);
            if (depth == 0)
                return;
            WriteLog($"\n------------ {depth} m. above the ground ------------", outWriter);
        
            
            var tasks = new Task[links.Count];
            Parallel.For(0, links.Count, new ParallelOptions()
            {
                MaxDegreeOfParallelism = MaxThreads
            }, ind =>
            {
                var index = ind;
                tasks[index] = ExecuteAsync(links[index], depth - 1, outWriter);
            });
            Task.WhenAll(tasks).Wait();
        }

        private void WriteLog(string log, StreamWriter writer)
        {
            if (writer == null)
                Console.WriteLine(log);
            else
                writer.WriteLine(log);
        }

        private IList<String> ParseLinks(String source)
        {
            String linkFormat = @"<a[^>]* href=['""](https?([\w\.:?&-_=#/]+))['""][^>]*>";
            Regex r = new Regex(linkFormat, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            var matches = r.Matches(source);
         
            var list = new List<String>();
            foreach (Match match in matches)
            {
                list.Add(match.Groups[1].ToString());
            }
            
            return list;
        }

    }
}
