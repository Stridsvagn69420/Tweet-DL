using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Globalization;
using System.Text.Json;
using System.Threading;
using System;

namespace Tweet_DL
{
    public class Bookmarks
    {
        public static List<string> ReadURLs(List<JSON> input)
        {
            List<string> result = new();
            foreach (JSON response in input)
            {
                response.data.bookmark_timeline.timeline.instructions[0].entries.FindAll(x => x.entryId.StartsWith("tweet")).ForEach(tweet =>
                {
                    try {
                        List<JSON.Medium> media = tweet.content.itemContent.tweet_results.result.legacy.entities.media;
                        if (media.Count > 0) media.ForEach(media =>
                        {
                            result.Add(media.media_url_https);
                        });
                    } catch (NullReferenceException) {
                        //do nothing
                    }
                });
            }
            return result;
        }
        private const string BookmarksAPI = "https://twitter.com/i/api/graphql/ue3e00SrYvoDk3eH8Qv6wg/Bookmarks?variables=";
        private const string VariablesQuery = "%7B%22count%22%3A200%2C%22withHighlightedLabel%22%3Afalse%2C%22withTweetQuoteCount%22%3Afalse%2C%22includePromotedContent%22%3Atrue%2C%22withTweetResult%22%3Atrue%2C%22withReactions%22%3Afalse%2C%22withSuperFollowsTweetFields%22%3Afalse%2C%22withSuperFollowsUserFields%22%3Afalse%2C%22withUserResults%22%3Afalse%2C%22withBirdwatchPivots%22%3Afalse%7D";
        private const string QueryCursorTemplate = "%7B%22count%22%3A200%2C%22cursor%22%3A%22{0}%22%2C%22withHighlightedLabel%22%3Afalse%2C%22withTweetQuoteCount%22%3Afalse%2C%22includePromotedContent%22%3Atrue%2C%22withTweetResult%22%3Atrue%2C%22withReactions%22%3Afalse%2C%22withSuperFollowsTweetFields%22%3Afalse%2C%22withSuperFollowsUserFields%22%3Afalse%2C%22withUserResults%22%3Afalse%2C%22withBirdwatchPivots%22%3Afalse%7D";
        private const string Authorization = "AAAAAAAAAAAAAAAAAAAAANRILgAAAAAAnNwIzUejRCOuH5E6I8xnZz4puTs%3D1Zv7ttfk8LF81IUq16cHjhLTvJu4FA33AGWWjCpTnA";
        private readonly HttpClient _client;
        private readonly Config _config;
        public Bookmarks(HttpClient client, Config config)
        {
            _client = client;
            _config = config;
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Authorization);
            _client.DefaultRequestHeaders.Add("Cookie", _config.userAuth.cookie);
            _client.DefaultRequestHeaders.Add("X-CSRF-Token", _config.userAuth.csrfToken);
            _client.DefaultRequestHeaders.Add("X-Twitter-Active-User", "yes");
            _client.DefaultRequestHeaders.Add("X-Twitter-Auth-Type", "OAuth2Session");
            _client.DefaultRequestHeaders.Add("X-Twitter-Client-Language", CultureInfo.CurrentCulture.TwoLetterISOLanguageName);
        }
        public List<JSON> GetBookmarks()
        {
            List<JSON> result = new();
            int count = 1;
            Printer.Print(ConsoleColor.Cyan, $"[{count}] ");
            Console.WriteLine("Getting initial bookmark entry...");
            JSON resJSON = GetBookmarksJSON();
            result.Add(resJSON);
            List<string> usedCursors = new();
            string resCursor = resJSON.data.bookmark_timeline.timeline.instructions[0].entries.Find(x => x.entryId.StartsWith("cursor-bottom")).content.value;
            while (!usedCursors.Contains(resCursor))
            {
                count++;
                usedCursors.Add(resCursor);
                Printer.Print(ConsoleColor.Cyan, $"[{count}] ");
                Console.WriteLine($"Getting entry at {resCursor}...");
                resJSON = GetBookmarksJSON(resCursor);
                if (usedCursors.Contains(resJSON.data.bookmark_timeline.timeline.instructions[0].entries.Find(x => x.entryId.StartsWith("cursor-bottom")).content.value))
                {
                    Printer.PrintLine(ConsoleColor.Cyan, $"Found final entry at {resCursor}!");
                    break;
                } else
                {
                    result.Add(resJSON);
                    resCursor = resJSON.data.bookmark_timeline.timeline.instructions[0].entries.Find(x => x.entryId.StartsWith("cursor-bottom")).content.value;
                    Thread.Sleep((int) Math.Round(_config.delay * 1.4));
                }
            }
            return result;
        }
        private JSON GetBookmarksJSON()
        {
            string bookmarksJSON = _client.GetStringAsync(BookmarksAPI + VariablesQuery).Result;
            return JsonSerializer.Deserialize<JSON>(bookmarksJSON);
        }
        private JSON GetBookmarksJSON(string cursor)
        {
            string bookmarksJSON = _client.GetStringAsync(BookmarksAPI + string.Format(QueryCursorTemplate, cursor)).Result;
            return JsonSerializer.Deserialize<JSON>(bookmarksJSON);
        }
    }
    public class JSON
    {
        public Data data { get; set; }
        public class AffiliatesHighlightedLabel
        {
        }

        public class Url
        {
            public string display_url { get; set; }
            public string expanded_url { get; set; }
            public string url { get; set; }
            public List<int> indices { get; set; }
        }
        public class Description
        {
            public List<Url> urls { get; set; }
        }
        public class Url2
        {
            public List<Url> urls { get; set; }
        }

        public class Entities
        {
            public Description description { get; set; }
            public Url url { get; set; }
            public List<Medium> media { get; set; }
            public List<UserMention> user_mentions { get; set; }
            public List<Url> urls { get; set; }
            public List<Hashtag> hashtags { get; set; }
            public List<object> symbols { get; set; }
        }

        public class Rgb
        {
            public int blue { get; set; }
            public int green { get; set; }
            public int red { get; set; }
        }

        public class Palette
        {
            public double percentage { get; set; }
            public Rgb rgb { get; set; }
        }

        public class Ok
        {
            public List<Palette> palette { get; set; }
        }

        public class R
        {
            public Ok ok { get; set; }
        }

        public class MediaColor
        {
            public R r { get; set; }
        }

        public class ProfileBannerExtensions
        {
            public MediaColor mediaColor { get; set; }
        }

        public class ProfileImageExtensions
        {
            public MediaColor mediaColor { get; set; }
        }

        public class Legacy
        {
            public bool blocked_by { get; set; }
            public bool blocking { get; set; }
            public bool can_dm { get; set; }
            public bool can_media_tag { get; set; }
            public string created_at { get; set; }
            public bool default_profile { get; set; }
            public bool default_profile_image { get; set; }
            public string description { get; set; }
            public Entities entities { get; set; }
            public int fast_followers_count { get; set; }
            public int favourites_count { get; set; }
            public bool follow_request_sent { get; set; }
            public bool followed_by { get; set; }
            public int followers_count { get; set; }
            public bool following { get; set; }
            public int friends_count { get; set; }
            public bool has_custom_timelines { get; set; }
            public bool is_translator { get; set; }
            public int listed_count { get; set; }
            public string location { get; set; }
            public int media_count { get; set; }
            public bool muting { get; set; }
            public string name { get; set; }
            public int normal_followers_count { get; set; }
            public bool notifications { get; set; }
            public List<string> pinned_tweet_ids_str { get; set; }
            public ProfileBannerExtensions profile_banner_extensions { get; set; }
            public string profile_banner_url { get; set; }
            public ProfileImageExtensions profile_image_extensions { get; set; }
            public string profile_image_url_https { get; set; }
            public string profile_interstitial_type { get; set; }
            public bool @protected { get; set; }
            public string screen_name { get; set; }
            public int statuses_count { get; set; }
            public string translator_type { get; set; }
            public string url { get; set; }
            public bool verified { get; set; }
            public bool want_retweets { get; set; }
            public List<object> withheld_in_countries { get; set; }
            public string conversation_id_str { get; set; }
            public List<int> display_text_range { get; set; }
            public ExtendedEntities extended_entities { get; set; }
            public int favorite_count { get; set; }
            public bool favorited { get; set; }
            public string full_text { get; set; }
            public bool is_quote_status { get; set; }
            public string lang { get; set; }
            public bool possibly_sensitive { get; set; }
            public bool possibly_sensitive_editable { get; set; }
            public int reply_count { get; set; }
            public int retweet_count { get; set; }
            public bool retweeted { get; set; }
            public string source { get; set; }
            public string user_id_str { get; set; }
            public string id_str { get; set; }
            public SelfThread self_thread { get; set; }
            public string in_reply_to_screen_name { get; set; }
            public string in_reply_to_status_id_str { get; set; }
            public string in_reply_to_user_id_str { get; set; }
        }

        public class Result2
        {
            public string __typename { get; set; }
            public string id { get; set; }
            public string rest_id { get; set; }
            public AffiliatesHighlightedLabel affiliates_highlighted_label { get; set; }
            public Legacy legacy { get; set; }
            public Core core { get; set; }
        }

        public class UserResults
        {
            public Result result { get; set; }
        }

        public class Core
        {
            public UserResults user_results { get; set; }
        }

        public class Face
        {
            public int x { get; set; }
            public int y { get; set; }
            public int h { get; set; }
            public int w { get; set; }
        }

        public class Large
        {
            public List<Face> faces { get; set; }
            public int h { get; set; }
            public int w { get; set; }
            public string resize { get; set; }
        }

        public class Medium2
        {
            public List<Face> faces { get; set; }
            public int h { get; set; }
            public int w { get; set; }
            public string resize { get; set; }
        }

        public class Small
        {
            public List<Face> faces { get; set; }
            public int h { get; set; }
            public int w { get; set; }
            public string resize { get; set; }
        }

        public class Orig
        {
            public List<Face> faces { get; set; }
        }

        public class Features
        {
            public Large large { get; set; }
            public Medium medium { get; set; }
            public Small small { get; set; }
            public Orig orig { get; set; }
        }

        public class Thumb
        {
            public int h { get; set; }
            public int w { get; set; }
            public string resize { get; set; }
        }

        public class Sizes
        {
            public Large large { get; set; }
            public Medium medium { get; set; }
            public Small small { get; set; }
            public Thumb thumb { get; set; }
        }

        public class FocusRect
        {
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
        }

        public class OriginalInfo
        {
            public int height { get; set; }
            public int width { get; set; }
            public List<FocusRect> focus_rects { get; set; }
        }

        public class Medium
        {
            public string display_url { get; set; }
            public string expanded_url { get; set; }
            public string id_str { get; set; }
            public List<int> indices { get; set; }
            public string media_url_https { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public Features features { get; set; }
            public Sizes sizes { get; set; }
            public OriginalInfo original_info { get; set; }
            public string media_key { get; set; }
            public ExtMediaColor ext_media_color { get; set; }
            public ExtMediaAvailability ext_media_availability { get; set; }
        }

        public class UserMention
        {
            public string id_str { get; set; }
            public string name { get; set; }
            public string screen_name { get; set; }
            public List<int> indices { get; set; }
        }

        public class Hashtag
        {
            public List<int> indices { get; set; }
            public string text { get; set; }
        }

        public class ExtMediaColor
        {
            public List<Palette> palette { get; set; }
        }

        public class ExtMediaAvailability
        {
            public string status { get; set; }
        }

        public class ExtendedEntities
        {
            public List<Medium> media { get; set; }
        }

        public class SelfThread
        {
            public string id_str { get; set; }
        }

        public class TweetResults
        {
            public Result2 result { get; set; }
        }
        public class Result
        {

        }
        public class ItemContent
        {
            public string itemType { get; set; }
            public TweetResults tweet_results { get; set; }
            public string tweetDisplayType { get; set; }
        }

        public class Content
        {
            public string entryType { get; set; }
            public ItemContent itemContent { get; set; }
            public string value { get; set; }
            public string cursorType { get; set; }
            public bool? stopOnEmptyResponse { get; set; }
        }

        public class Entry
        {
            public string entryId { get; set; }
            public string sortIndex { get; set; }
            public Content content { get; set; }
        }

        public class Instruction
        {
            public string type { get; set; }
            public List<Entry> entries { get; set; }
        }

        public class ResponseObjects
        {
            public List<object> feedbackActions { get; set; }
        }
        public class Timeline
        {
            public List<Instruction> instructions { get; set; }
            public ResponseObjects responseObjects { get; set; }
        }

        public class BookmarkTimeline
        {
            public Timeline timeline { get; set; }
        }

        public class Data
        {
            public BookmarkTimeline bookmark_timeline { get; set; }
        }
    }
}
