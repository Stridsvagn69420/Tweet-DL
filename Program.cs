using System;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Threading;

namespace Tweet_DL
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient client = new();
            Config config = GetConfig();
            if (args.Length > 0)
            {
                client.DefaultRequestHeaders.Authorization = new("Bearer", config.apiAuth.bearerToken);
                Regex URLregex = new ("twitter\\.com/[a-zA-Z]+/status/[0-9]+", RegexOptions.IgnoreCase);
                Regex IDregex = new("[0-9]{19}", RegexOptions.IgnoreCase);
                List<string> IDs = new();
                foreach (string tweet in args)
                {
                    if (tweet.StartsWith("http"))
                    {
                        Uri tweetURL = new(tweet);
                        URLregex.IsMatch(tweetURL.Host + tweetURL.AbsolutePath);
                        IDs.Add(tweetURL.AbsolutePath.Split("/")[3]);
                    } else if (IDregex.IsMatch(tweet)) IDs.Add(tweet);
                }
                if (IDs.Count == 0) Console.WriteLine("Not Tweet URLs or Tweet IDs set! Exiting...");
                else
                {
                    Tweet.JSON tweets = new Tweet(client, config).GetTweets(IDs);
                    List<string> URLs = new();
                    tweets.includes.media.ForEach(delegate(Tweet.JSON.Medium medium) { if (medium.type == "photo") URLs.Add(medium.url); });
                    new Downloader(client).DownloadURLs(URLs);
                }
            } else
            {
                //download Twitter bookmarks
            }
            client.Dispose();
        }
        private static Config GetConfig()
        {
            string homedir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string combinedPath = Path.Combine(homedir, ".config/tweet-dl/config.json");
            string configFile = File.ReadAllText(combinedPath);
            return JsonSerializer.Deserialize<Config>(configFile);
        }
    }
}
