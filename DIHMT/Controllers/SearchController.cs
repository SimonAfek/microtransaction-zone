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

        public async Task<ActionResult> Search(string q, int page = 1)
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
                    GameHelpers.SearchGbAndCacheResults(q, page);
                }
            }

            if (!games.Any()) // Ask GB for games instead
            {
                var rawResults = await GbGateway.SearchAsync(q, page);

                if (rawResults.Any())
                {
                    var filteredResults = GameHelpers.FilterOutUnsupportedPlatforms(rawResults);

                    foreach (var v in filteredResults)
                    {
                        if (!GameHelpers.GameExistsInDb(v.Id))
                        {
                            GameHelpers.SaveGameToDb(v);
                        }

                        games.Add(await GameHelpers.RefreshDisplayGame(v.Id));
                    }
                }
            }

            var retval = new SearchResult
            {
                Page = page,
                Results = games,
                Query = q
            };

            return View(retval);
        }
    }
}