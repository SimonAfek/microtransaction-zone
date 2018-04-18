using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Configuration;
using DIHMT.Models;
using Newtonsoft.Json;

namespace DIHMT.Static
{
    public static class TumblrState
    {
        private static string ApiKey => WebConfigurationManager.AppSettings["TumblrApiKey"];
        private static DateTime _timeOfLastRequest = DateTime.MinValue;
        private static int MillisecondsBetweenRequests => 30000; // 30 seconds should be alright
        private static List<TumblrResponseModelResponsePost> CurrentPosts { get; set; }

        public static List<TumblrResponseModelResponsePost> GetPosts()
        {
            if (_timeOfLastRequest.AddMilliseconds(MillisecondsBetweenRequests) < DateTime.Now)
            {
                UpdatePosts();
            }

            return CurrentPosts;
        }

        private static void UpdatePosts()
        {
            try
            {
                using (var wc = new WebClient())
                {
                    var rawData = wc.DownloadString($"https://api.tumblr.com/v2/blog/mtxzone.tumblr.com/posts?api_key={ApiKey}&limit=5");

                    CurrentPosts = JsonConvert.DeserializeObject<TumblrResponseModel>(rawData).Response.Posts.ToList();
                }

                _timeOfLastRequest = DateTime.Now;
            }
            catch
            {
                CurrentPosts = new List<TumblrResponseModelResponsePost>
                {
                    new TumblrResponseModelResponsePost
                    {
                        Title = "Can't load tumblr posts; try again later.",
                        PostUrl = "/",
                        Date = DateTime.Now
                    }
                };
            }
        }
    }
}
