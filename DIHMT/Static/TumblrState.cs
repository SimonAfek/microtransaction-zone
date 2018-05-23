using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using DIHMT.Models;
using HtmlAgilityPack;

namespace DIHMT.Static
{
    public static class TumblrState
    {
        private static int NumOfPosts => 5;
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
                var reader = XmlReader.Create("https://mtxzone.tumblr.com/rss");
                var feed = SyndicationFeed.Load(reader);
                reader.Close();
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
