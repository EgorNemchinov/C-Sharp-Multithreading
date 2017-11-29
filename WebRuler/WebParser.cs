using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using System.Text.RegularExpressions;

namespace WebRuler
{
    public class WebParser
    {
        private const int MAX_THREADS = 15;
        
        public void Execute(String url, int depth, bool parallel = true)
        {
            var source = DownloadPage(url);
            
            if (source == null)
                return;

            IList<String> links = ParseLinks(source);
            var length = source.Length;
            
            Console.WriteLine($"Length of page '{url}' is {length}");
            if (depth == 0)
                return;
            Console.WriteLine($"\n------------ {depth} m. above the ground ------------");
            
            if (parallel)
            {
                Parallel.ForEach(links, new ParallelOptions
                {
                    MaxDegreeOfParallelism = MAX_THREADS
                }, link =>
                {
                    Execute(link, depth - 1);
                });
            }
            else
            {
                foreach (String link in links)
                {
                    Execute(link, depth - 1, parallel=false);
                }
            }
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

        private String DownloadPage(String url)
        {
            using (WebClient client = new WebClient())
            {
                try
                {
                    return client.DownloadString(url);
                }
                catch (System.Net.WebException exception)
                {
                    Console.WriteLine($"Unable to download {url}.");
                    return null;
                }
            }
        }
    }
}
