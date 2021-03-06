﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using DIHMT.Models;
using DIHMT.Static;

namespace DIHMT.Controllers
{
    public class SearchController : Controller
    {
        private const int PageLimit = 10;

        public ActionResult Search(string q, int page = 1)
        {
            var games = new List<DisplayGame>();

            if (!string.IsNullOrEmpty(q))
            {
                var searchResult = SearchHelpers.SearchGamesInDb(q);

                if (searchResult.Count > (page - 1) * PageLimit)
                {
                    var searchResultRanked = new List<DisplayGame>();

                    // Move all the unrated results to the bottom of the list
                    searchResultRanked.AddRange(searchResult.Where(x => x.IsRated));
                    searchResultRanked.AddRange(searchResult.Where(x => !x.IsRated));

                    games.AddRange(searchResultRanked.Skip((page - 1) * PageLimit).Take(PageLimit));

                    // Dispatch a Task on a different thread to ask GB for games
                    // related to the query, and add any results we don't yet have
                    // to our own DB.
                    Task.Run(() => GameHelpers.SearchGbAndCacheResults(q, page));
                }


                if (!games.Any()) // Ask GB for games instead
                {
                    var rawResults = GbGateway.Search(q, page);

                    if (rawResults.Any())
                    {
                        var filteredResults = GameHelpers.FilterOutUnsupportedPlatforms(rawResults);

                        GameHelpers.SaveGamesToDb(filteredResults);

                        // Not using RefreshDisplayGame here, since having to wait for
                        // potentially up to 10 games to be pulled from GB
                        // is a super duper bad idea
                        games.AddRange(filteredResults.Select(x => GameHelpers.CreateDisplayGameObject(x.Id)));
                    }
                }
            }

            var retval = new SearchResult
            {
                Page = page,
                Results = games,
                Query = q,
                Type = SearchType.Standard
            };

            return View(retval);
        }

        public ActionResult Advanced(
            string q,
            List<int> requireFlags,
            List<int> blockFlags,
            List<int> allowFlags,
            List<int> platforms,
            List<int> genres,
            int page = 1
        )
        {
            requireFlags = requireFlags?.Where(x => x >= 0).ToList() ?? new List<int>();
            blockFlags = blockFlags?.Where(x => x >= 0).ToList() ?? new List<int>();
            allowFlags = allowFlags?.Where(x => x >= 0).ToList() ?? new List<int>();
            platforms = platforms?.Where(x => x >= 0).ToList() ?? new List<int>();
            genres = genres?.Where(x => x >= 0).ToList() ?? new List<int>();

            // Determine if we're showing results or displaying the form
            if (string.IsNullOrEmpty(q)
                && !requireFlags.Any()
                && !blockFlags.Any()
                && !allowFlags.Any()
                && !platforms.Any()
                && !genres.Any())
            {
                var viewModelGenres = GameHelpers.GetGenres();
                var viewModelPlatforms = GameHelpers.GetPlatforms();

                return View(new AdvancedSearchViewModel { Genres = viewModelGenres, Platforms = viewModelPlatforms });
            }

            var results = SearchHelpers.AdvancedSearch(q, requireFlags, blockFlags, allowFlags, platforms, genres);

            var games = results.Skip((page - 1) * PageLimit).Take(PageLimit).ToList();

            var retval = new SearchResult
            {
                Page = page,
                Results = games,
                Query = q,
                AllowFlags = allowFlags,
                BlockFlags = blockFlags,
                RequireFlags = requireFlags,
                Genres = genres,
                Platforms = platforms,
                Type = SearchType.Advanced
            };

            return View("Search", retval);
        }

        public ActionResult Recent(int page = 1)
        {
            var games = GameHelpers.GetRecentlyRatedGames(PageLimit, page).ToList();

            var retval = new SearchResult
            {
                Page = page,
                Results = games,
                Type = SearchType.Recent
            };

            return View("Search", retval);
        }
    }
}
