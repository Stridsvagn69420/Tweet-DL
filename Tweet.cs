using System.Net.Http;
using System.Collections.Generic;
using System.Text.Json;

namespace Tweet_DL
{
    public class Tweet
    {
        private HttpClient _client;
        public Tweet(HttpClient client, Config config)
        {
            _client = client;
        }
        public JSON GetTweets(List<string> tweetIDs)
        {
            HttpRequestMessage request = new(HttpMethod.Get, "https://api.twitter.com/2/tweets?ids=" + string.Join(",", tweetIDs) + "&tweet.fields=attachments&expansions=attachments.media_keys&media.fields=url,type");
            string result = ((_client.SendAsync(request)).Result.Content.ReadAsStringAsync()).Result;
            return JsonSerializer.Deserialize<JSON>(result);
        }
        public class JSON
        {
            public List<Data> data { get; set; }
            public Includes includes { get; set; }
            public class Attachments
            {
                public List<string> media_keys { get; set; }
            }

            public class Data
            {
                public string text { get; set; }
                public string id { get; set; }
                public Attachments attachments { get; set; }
            }

            public class Medium
            {
                public string media_key { get; set; }
                public string type { get; set; }
                public string url { get; set; }
            }

            public class Includes
            {
                public List<Medium> media { get; set; }
            }
        }
    }
}