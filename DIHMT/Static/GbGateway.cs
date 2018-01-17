using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using GiantBomb.Api;
using GiantBomb.Api.Model;
using PennedObjects.RateLimiting;

namespace DIHMT.Static
{
    public static class GbGateway
    {
        private static readonly RateGate Limiter = new RateGate(1, TimeSpan.FromMilliseconds(1050));
        private static string ApiKey => WebConfigurationManager.AppSettings["GiantBombApiKey"];
        
        public static Game GetGame(int id)
        {
            var client = new GiantBombRestClient(ApiKey);

            Limiter.WaitToProceed();

            return client.GetGame(id);
        }

        public static List<Game> Search(string q, int page = 1)
        {
            var client = new GiantBombRestClient(ApiKey);

            Limiter.WaitToProceed();

            return client.SearchForGames(q, page, 10).ToList();
        }

        public static async Task<List<Game>> SearchAsync(string q, int page)
        {
            var client = new GiantBombRestClient(ApiKey);

            Limiter.WaitToProceed();

            var results = await client.SearchForGamesAsync(q, page, 10);

            return results.ToList();
        }
    }
}