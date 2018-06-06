using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Xml;
using DIHMT.Models;
using HtmlAgilityPack;

namespace DIHMT.Static
{
    public static class TumblrState
    {
        private static int NumOfPosts => 3;
        private static DateTime _timeOfLastRequest = DateTime.MinValue;
        private static int MillisecondsBetweenRequests => 30000; // 30 seconds should be alright
        private static readonly object Lock = new object();
        private static List<TumblrPost> CurrentPosts { get; set; }

        private static List<TumblrPost> ErrorModel => new List<TumblrPost>
        {
            new TumblrPost
            {
                Title = "Unable to load recent blog posts; click here to go directly to our blog.",
                Link = "https://mtxzone.tumblr.com",
                PubDate = DateTime.Now
            }
        };

        public static List<TumblrPost> GetPosts()
        {
            lock (Lock)
            {
                if (_timeOfLastRequest.AddMilliseconds(MillisecondsBetweenRequests) < DateTime.Now)
                {
                    UpdatePosts();
                }

                return CurrentPosts;
            }
        }

        private static void UpdatePosts()
        {
            try
            {
                byte[] byteArray;
                SyndicationFeed feed;

                using (var wc = new WebClient())
                {
                    // Gotta add a cookie to get past the stupid GDPR screen
                    wc.Headers.Add(HttpRequestHeader.Cookie, "pfg=f3803a70020b2d33c2a7a8f2bc9668f4b0239870d2c8eaac0181813e0c767cd4%23%7B%22eu_resident%22%3A1%2C%22gdpr_is_acceptable_age%22%3A1%2C%22gdpr_consent_core%22%3A1%2C%22gdpr_consent_first_party_ads%22%3A1%2C%22gdpr_consent_third_party_ads%22%3A1%2C%22gdpr_consent_search_history%22%3A1%2C%22exp%22%3A1559058504%2C%22vc%22%3A%22%22%7D%230759470943");
                    wc.Headers.Add(HttpRequestHeader.Accept, "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8");
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/66.0.3359.181 Safari/537.36");

                    byteArray = wc.DownloadData("https://mtxzone.tumblr.com/rss");
                }

                using (var stream = new MemoryStream(byteArray))
                using (var reader = XmlReader.Create(stream))
                {
                    feed = SyndicationFeed.Load(reader);
                }

                var postsToLoad = Math.Min(NumOfPosts, feed.Items.Count());
                var itemsList = feed.Items.ToList();
                var newPosts = new List<TumblrPost>();

                for (var i = 0; i < postsToLoad; i++)
                {
                    var curItem = itemsList[i];

                    var htmlDoc = new HtmlDocument();
                    htmlDoc.LoadHtml(curItem.Summary.Text);
                    var strippedHtmlDescription = htmlDoc.DocumentNode.InnerText;

                    if (strippedHtmlDescription.Length > 100)
                    {
                        strippedHtmlDescription = $"{strippedHtmlDescription.Substring(0, 100)}...";
                    }

                    // This bit just removes the body text if it's near-identical to the headline. You can comment it out if you want.
                    if (strippedHtmlDescription.StartsWith(curItem.Title.Text)
                    || curItem.Title.Text.EndsWith("...") && strippedHtmlDescription.StartsWith(curItem.Title.Text.Substring(0, curItem.Title.Text.Length - 3)))
                    {
                        strippedHtmlDescription = string.Empty;
                    }

                    newPosts.Add(new TumblrPost
                    {
                        Title = curItem.Title.Text,
                        Description = strippedHtmlDescription,
                        Link = curItem.Links.FirstOrDefault()?.Uri.ToString() ?? "/",
                        PubDate = curItem.PublishDate.DateTime
                    });
                }

                CurrentPosts = newPosts;
                _timeOfLastRequest = DateTime.Now;
            }
            catch
            {
                CurrentPosts = ErrorModel;
                _timeOfLastRequest = DateTime.Now;
            }
        }
    }
}
