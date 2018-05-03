using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Hosting;
using DIHMT.Models;

namespace DIHMT.Static
{
    public static class ThumbHelpers
    {
        private static readonly object Lock = new object();

        public static Tuple<string, string> GetThumbByGameId(int id, bool forceUpdate)
        {
            var game = DbAccess.GetDbGameView(id);

            if (game == null)
            {
                return null;
            }

            var existingThumb = DbAccess.GetThumb(id);

            var retval = existingThumb == null || forceUpdate
                ? RefreshThumb(game)
                : new Tuple<string, string>(existingThumb.ImageUrl, existingThumb.ContentType);

            return retval;
        }

        private static Tuple<string, string> RefreshThumb(DbGame game)
        {
            lock (Lock)
            {
                var uri = new Uri(game.ThumbImageUrl);

                var filename = $"/Images/Thumb/{Path.GetFileName(uri.LocalPath)}";

                string contentType;
                byte[] data;

                using (var wc = new WebClient())
                {
                    wc.Headers.Add("user-agent", "MTXZoneThumbCacher/1.0");
                    data = wc.DownloadData(game.ThumbImageUrl);
                    contentType = wc.ResponseHeaders["Content-Type"];
                }

                if (!new[] { "image/png", "image/jpeg" }.Contains(contentType))
                {
                    return null;
                }

                var path = $"{HostingEnvironment.ApplicationPhysicalPath}{filename.Substring(1).Replace("/", @"\")}";
                File.WriteAllBytes(path, data);

                DbAccess.SaveThumb(game.Id, filename, contentType);

                return new Tuple<string, string>(filename, contentType);
            }
        }
    }
}
