using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIHMT.Models;
using GiantBomb.Api.Model;

namespace DIHMT.Static
{
    public static class GameHelpers
    {
        /// <summary>
        /// Creates a DbGame object for insertion into the database.
        /// Should only be used for games that are not already in the Db,
        /// as otherwise, their ratings will be reset.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static DbGame CreateDbGameObjectWithoutNavigation(Game input)
        {
            return new DbGame
            {
                Id = input.Id,
                Name = input.Name,
                Summary = input.Deck,
                RatingId = (int)Models.Rating.Unrated,
                LastUpdated = DateTime.UtcNow,
                SmallImageUrl = input.Image.SmallUrl,
                ThumbImageUrl = input.Image.ThumbUrl,
                GbSiteDetailUrl = input.SiteDetailUrl
            };
        }

        /// <summary>
        /// Updates all non-DIHMT-specific fields of the
        /// game in the database
        /// </summary>
        /// <param name="dGame">The game to be updated</param>
        /// <param name="gbGame">The fresh GiantBomb-supplied game object</param>
        private static void UpdateGameFromGb(DisplayGame dGame, Game gbGame)
        {
            var dbGame = CreateDbGameObjectWithoutNavigation(gbGame);

            dbGame.RatingId = (int) (Models.Rating) Enum.Parse(typeof(Models.Rating), dGame.Rating);
            dbGame.RatingExplanation = dGame.RatingExplanation;

            DbAccess.SaveGame(dbGame);
        }

        private static List<DbGamePlatform> CreateDbGamePlatformsListWithoutNavigation(Game gbGame)
        {
            var platforms = DbAccess.GetPlatforms(gbGame.Platforms);

            return platforms?.Select(p => new DbGamePlatform { GameId = gbGame.Id, PlatformId = p.Id }).ToList();
        }

        /// <summary>
        /// Pulls any necessary information on the game from
        /// GiantBomb, and then returns a DisplayGame object that
        /// is as updated as it needs to be.
        /// </summary>
        /// <param name="id">Id of the game to be returned</param>
        /// <returns></returns>
        public static async Task<DisplayGame> RefreshDisplayGame(int id)
        {
            var displayGame = CreateDisplayGameObject(id);

            if (displayGame == null)
            {
                var gbGame = await GbGateway.GetGameAsync(id);

                SaveGameToDb(gbGame);

                displayGame = CreateDisplayGameObject(id);
            }

            if ((DateTime.UtcNow - displayGame.LastUpdated).Days >= 7)
            {
                var gbGame = await GbGateway.GetGameAsync(id);
                UpdateGameFromGb(displayGame, gbGame);
                displayGame = CreateDisplayGameObject(id);
            }

            return displayGame;
        }

        /// <summary>
        /// Saves a GbGame object to the database, including
        /// relevant GamePlatform-rows
        /// </summary>
        /// <param name="input">The object to save</param>
        public static void SaveGameToDb(Game input)
        {
            var dbGame = CreateDbGameObjectWithoutNavigation(input);
            var dbGamePlatforms = CreateDbGamePlatformsListWithoutNavigation(input);

            DbAccess.SaveGame(dbGame);
            DbAccess.SaveGamePlatforms(dbGamePlatforms);
        }

        /// <summary>
        /// Queries GB's search engine for games, and adds any results
        /// to our DB that don't yet exist there.
        /// </summary>
        /// <param name="q">Query</param>
        /// <param name="page">Page number</param>
        public static async void SearchGbAndCacheResults(string q, int page)
        {
            var rawResults = await GbGateway.SearchAsync(q, page);

            if (rawResults.Any())
            {
                var filteredResults = FilterOutUnsupportedPlatforms(rawResults);

                foreach (var v in filteredResults)
                {
                    if (!GameExistsInDb(v.Id))
                    {
                        SaveGameToDb(v);
                    }
                }
            }
        }

        /// <summary>
        /// Creates a DisplayGame object based on the information stored
        /// in the database.
        /// </summary>
        /// <param name="id">Id of the game to retrieve</param>
        /// <returns></returns>
        private static DisplayGame CreateDisplayGameObject(int id)
        {
            var dbGameView = DbAccess.GetDbGameView(id);

            DisplayGame result = null;

            if (dbGameView != null)
            {
                result = new DisplayGame
                {
                    GbSiteDetailUrl = dbGameView.GbSiteDetailUrl,
                    Id = dbGameView.Id,
                    LastUpdated = dbGameView.LastUpdated,
                    Name = dbGameView.Name,

                    Platforms = dbGameView.DbGamePlatforms.Select(x => new DisplayGamePlatform
                    {
                        Abbreviation = x.DbPlatform.Abbreviation,
                        Id = x.DbPlatform.Id,
                        ImageUrl = x.DbPlatform.ImageUrl,
                        Name = x.DbPlatform.Name
                    }).ToList(),

                    Rating = dbGameView.DbRating.Name,
                    RatingExplanation = dbGameView.RatingExplanation,
                    SmallImageUrl = dbGameView.SmallImageUrl,
                    Summary = dbGameView.Summary,
                    ThumbImageUrl = dbGameView.ThumbImageUrl
                };
            }

            return result;
        }

        public static bool GameExistsInDb(int id)
        {
            return DbAccess.GameExistsInDb(id);
        }

        public static List<Game> FilterOutUnsupportedPlatforms(List<Game> input)
        {
            var platforms = DbAccess.GetPlatforms().Select(x => x.Id).ToList();

            var retval = new List<Game>();

            foreach (var i in input)
            {
                // Some games don't have platforms
                if (i.Platforms != null && i.Platforms.Count > 0)
                {
                    foreach (var j in i.Platforms)
                    {
                        if (platforms.Contains(j.Id))
                        {
                            retval.Add(i);
                            break;
                        }
                    }
                }
            }

            return retval;
        }
    }
}