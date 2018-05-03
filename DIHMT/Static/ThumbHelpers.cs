using System;
using System.Net;
using DIHMT.Models;

namespace DIHMT.Static
{
    public static class ThumbHelpers
    {
        public static Tuple<byte[], string> GetThumbByGameId(int id)
        {
            var game = DbAccess.GetDbGameView(id);

            if (game == null)
            {
                return null;
            }

            var existingThumb = DbAccess.GetThumb(id);

            Tuple<byte[], string> retval;

            if (existingThumb == null || (DateTime.UtcNow - existingThumb.LastUpdated).Days > 7)
            {
                retval = RefreshThumb(game);
            }
            else
            {
                retval = new Tuple<byte[], string>(existingThumb.Data, existingThumb.ContentType);
            }

            return retval;
        }

        private static Tuple<byte[], string> RefreshThumb(DbGame game)
        {
            Tuple<byte[], string> retval;

            using (var wc = new WebClient())
            {
                wc.Headers.Add("user-agent", "MTXZoneThumbCacher/1.0");
                var data = wc.DownloadData(game.ThumbImageUrl);
                var contentType = wc.ResponseHeaders["Content-Type"];

                retval = new Tuple<byte[], string>(data, contentType);
            }

            DbAccess.SaveThumb(game.Id, retval.Item1, retval.Item2);

            return retval;
        }
    }
}
