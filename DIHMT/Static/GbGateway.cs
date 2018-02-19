using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Configuration;
using GiantBomb.Api;
using GiantBomb.Api.Model;

namespace DIHMT.Static
{
    public static class GbGateway
    {
        private static string ApiKey => WebConfigurationManager.AppSettings["GiantBombApiKey"];
        private static readonly object Lock = new object();
        private static DateTime _lastRequest = DateTime.MinValue;
        private const int RequestIntervalInMilliseconds = 1001;

        public static Game GetGame(int id)
        {
            lock (Lock)
            {
                var client = new GiantBombRestClient(ApiKey);

                WaitToProceed();

                var retval = client.GetGame(id);

                _lastRequest = DateTime.UtcNow;

                return retval;
            }
        }

        public static List<Game> Search(string q, int page)
        {
            lock (Lock)
            {
                var client = new GiantBombRestClient(ApiKey);

                WaitToProceed();

                var retval = client.SearchForGames(q, page, 10).ToList();

                _lastRequest = DateTime.UtcNow;

                return retval;
            }
        }

        private static void WaitToProceed()
        {
            var now = DateTime.UtcNow;

            var earliestAllowableRequestTime = _lastRequest.AddMilliseconds(RequestIntervalInMilliseconds);
            var msToSleep = (earliestAllowableRequestTime - now).Milliseconds;

            if (msToSleep > 0)
            {
                Thread.Sleep(msToSleep);
            }
        }
    }
}