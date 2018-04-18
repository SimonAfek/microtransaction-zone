using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using DIHMT.Models;

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
                Title = "Can't load tumblr posts; try again later.",
                Link = "/",
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
                    var descriptionIsHtml = curItem.Summary.Text.StartsWith("<");

                    newPosts.Add(new TumblrPost
                    {
                        Title = curItem.Title.Text,
                        Description = descriptionIsHtml ? string.Empty : curItem.Summary.Text,
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
