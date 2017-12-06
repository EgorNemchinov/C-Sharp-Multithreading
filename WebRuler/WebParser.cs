using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

namespace WebRuler
{
    public class WebParser
    {
        private const int MaxThreads = 15;

        public void Execute(String url, int depth)
        {
            string source;
            try
            {
                source = new WebClient().DownloadString(url);
            }
            catch
            {
                Console.WriteLine($"Unable to download page {url}.");
                return;
            }

            IList<String> links = ParseLinks(source);
            var length = source.Length;
            
            Console.WriteLine($"Length of page '{url}' is {length}");
            if (depth == 0)
                return;
            Console.WriteLine($"\n------------ {depth} m. above the ground ------------");
        

            foreach (String link in links)
            {
                Execute(link, depth - 1);
            }
        }
        
        public async Task ExecuteAsync(String url, int depth)
        {
            string source;
            try
            {
                source = await new WebClient().DownloadStringTaskAsync(url);                
            } 
            catch
            {
                Console.WriteLine($"Unable to download page {url}.");
                return;
            }
            
            IList<String> links = ParseLinks(source);
            var length = source.Length;
            
            Console.WriteLine($"Length of page '{url}' is {length}");
            if (depth == 0)
                return;
            Console.WriteLine($"\n------------ {depth} m. above the ground ------------");
        
            
            var tasks = new Task[links.Count];
            Parallel.For(0, links.Count, new ParallelOptions()
            {
                MaxDegreeOfParallelism = MaxThreads
            }, ind =>
            {
                var index = ind;
                tasks[index] = ExecuteAsync(links[index], depth - 1);
            });
            Task.WhenAll(tasks).Wait();
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
