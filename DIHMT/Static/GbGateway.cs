using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Configuration;
using GiantBomb.Api;
using GiantBomb.Api.Model;
using RateLimiter;

namespace DIHMT.Static
{
    public static class GbGateway
    {
        private static readonly TimeLimiter RateLimiter = TimeLimiter.GetFromMaxCountByInterval(1, TimeSpan.FromMilliseconds(1050));
        private static string ApiKey => WebConfigurationManager.AppSettings["GiantBombApiKey"];
        
        public static async Task<Game> GetGameAsync(int id)
        {
            var client = new GiantBombRestClient(ApiKey);

            return await RateLimiter.Perform(() => client.GetGame(id));
        }

        public static async Task<List<Game>> SearchAsync(string q, int page)
        {
            var client = new GiantBombRestClient(ApiKey);

            var results = await RateLimiter.Perform(() => client.SearchForGames(q, page, 10));

            return results.ToList();
        }

        public static Game GetGame(int id)
        {
            var client = new GiantBombRestClient(ApiKey);

            return RateLimiter.Perform(() => client.GetGame(id)).Result;
        }
    }
}