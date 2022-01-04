using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;

namespace Tweet_DL
{
    class Program
    {
        private static Config GetConfig()
        {
            string homedir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string combinedPath = Path.Combine(homedir, ".config/tweet-dl/config.json");
            string configFile = File.ReadAllText(combinedPath);
            return JsonSerializer.Deserialize<Config>(configFile);
        }
        static int Main(string[] args)
        {
            HttpClient client = new();
            Config config;
            try
            {
                Console.WriteLine("Reading config.json...");
                config = GetConfig();
            }
            catch (FileNotFoundException)
            {
                Printer.PrintLine(ConsoleColor.Red, "config.json not found! Make sure you installed Tweet-DL correctly:");
                Console.WriteLine("https://github.com/Stridsvagn69420/Tweet-DL#Installation");
                return 1;
            }
            if (args.Length > 0)
            {
                client.DefaultRequestHeaders.Authorization = new("Bearer", config.apiAuth.bearerToken);
                Regex URLregex = new("twitter\\.com/[a-zA-Z]+/status/[0-9]+", RegexOptions.IgnoreCase);
                Regex IDregex = new("[0-9]{19}", RegexOptions.IgnoreCase);
                List<string> IDs = new();
                foreach (string tweet in args)
                {
                    if (tweet.ToLower().StartsWith("http"))
                    {
                        Uri tweetURL = new(tweet);
                        URLregex.IsMatch(tweetURL.Host + tweetURL.AbsolutePath);
                        IDs.Add(tweetURL.AbsolutePath.Split("/")[3]);
                    }
                    else if (IDregex.IsMatch(tweet)) IDs.Add(tweet);
                }
                if (IDs.Count == 0)
                {
                    Printer.PrintLine(ConsoleColor.Red, "Not Tweet URLs or Tweet IDs set! Exiting...");
                    return 1;
                }
                else
                {
                    Tweet.JSON tweets;
                    try
                    {
                        Console.WriteLine($"Getting images of {IDs.Count} Tweets...");
                        tweets = new Tweet(client, config).GetTweets(IDs);
                    }
                    catch (HttpRequestException e)
                    {
                        Printer.PrintLine(ConsoleColor.Red, "An error occured while making a request to Twitter's API:");
                        Printer.PrintLine(ConsoleColor.Red, e.Message);
                        return 1;
                    }
                    List<string> URLs = new();
                    tweets.includes.media.ForEach(delegate (Tweet.JSON.Medium medium)
                    {
                        if (medium.type == "photo") URLs.Add(medium.url);
                    });
                    new Downloader(client, config.downloadDir, config.delay).DownloadURLs(URLs);
                }
            }
            else
            {
                List<JSON> allJSON;
                try
                {
                    allJSON = new Bookmarks(client, config).GetBookmarks();
                }
                catch (HttpRequestException e)
                {
                    Printer.PrintLine(ConsoleColor.Red, "An error occured while making a request to Twitter's API:");
                    Printer.PrintLine(ConsoleColor.Red, e.Message);
                    return 1;
                }
                List<string> allURLS = Bookmarks.ReadURLs(allJSON);
                new Downloader(client, config.downloadDir, config.delay).DownloadURLs(allURLS);
            }
            client.Dispose();
            return 0;
        }
    }
}
