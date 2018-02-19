﻿using System;
using System.Collections.Generic;
using System.Linq;
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
        /// <param name="includeGenres">Indicates whether or not the DbGameGenres-table should be updated, too</param>
        private static void UpdateGameFromGb(DisplayGame dGame, bool includeGenres = false)
        {
            var gbGame = GbGateway.GetGame(dGame.Id);
            var dbGame = CreateDbGameObjectWithoutNavigation(gbGame);

            dbGame.IsRated = dGame.IsRated;
            dbGame.RatingExplanation = dGame.RatingExplanation;
            dbGame.RatingLastUpdated = dGame.RatingLastUpdated;
            dbGame.LastUpdated = DateTime.UtcNow;

            DbAccess.SaveGame(dbGame);

            if (!includeGenres || gbGame.Genres == null || !gbGame.Genres.Any()) return;

            var dbGameGenres = CreateDbGameGenresListWithoutNavigation(gbGame);
            DbAccess.SaveGameGenres(dbGameGenres);
        }

        private static List<DbGamePlatform> CreateDbGamePlatformsListWithoutNavigation(Game gbGame)
        {
            var platforms = DbAccess.GetPlatforms(gbGame.Platforms);

            return platforms?.Select(p => new DbGamePlatform { GameId = gbGame.Id, PlatformId = p.Id }).ToList();
        }

        private static List<DbGameGenre> CreateDbGameGenresListWithoutNavigation(Game gbGame)
        {
            return gbGame.Genres?.Select(g => new DbGameGenre { GameId = gbGame.Id, GenreId = g.Id} ).ToList();
        }

        /// <summary>
        /// Pulls any necessary information on the game from
        /// GiantBomb, and then returns a DisplayGame object that
        /// is as updated as it needs to be.
        /// </summary>
        /// <param name="id">Id of the game to be returned</param>
        /// <param name="includeGenres">Whether to look for genre info or not</param>
        /// <returns></returns>
        public static DisplayGame RefreshDisplayGame(int id, bool includeGenres = false)
        {
            var displayGame = CreateDisplayGameObject(id);

            if (displayGame == null)
            {
                var gbGame = GbGateway.GetGame(id);

                SaveGameToDb(gbGame);

                displayGame = CreateDisplayGameObject(id);
            }

            if (includeGenres && (displayGame.Genres == null || !displayGame.Genres.Any()))
            {
                UpdateGameFromGb(displayGame, true);
                displayGame = CreateDisplayGameObject(id);
            }

            if ((DateTime.UtcNow - displayGame.LastUpdated).Days >= 7)
            {
                UpdateGameFromGb(displayGame);
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

            if (input.Genres == null || !input.Genres.Any())
            {
                return;
            }

            var dbGameGenres = CreateDbGameGenresListWithoutNavigation(input);
            DbAccess.SaveGameGenres(dbGameGenres);
        }

        /// <summary>
        /// Queries GB's search engine for games, and adds any results
        /// to our DB that don't yet exist there.
        /// </summary>
        /// <param name="q">Query</param>
        /// <param name="page">Page number</param>
        public static void SearchGbAndCacheResults(string q, int page)
        {
            var rawResults = GbGateway.Search(q, page);

            if (!rawResults.Any())
            {
                return;
            }

            var filteredResults = FilterOutUnsupportedPlatforms(rawResults);

            foreach (var v in filteredResults)
            {
                if (!GameExistsInDb(v.Id))
                {
                    SaveGameToDb(v);
                }
            }
        }

        /// <summary>
        /// Creates a DisplayGame object based on the information stored
        /// in the database.
        /// </summary>
        /// <param name="id">Id of the game to retrieve</param>
        /// <returns></returns>
        public static DisplayGame CreateDisplayGameObject(int id)
        {
            var dbGameView = DbAccess.GetDbGameView(id);

            DisplayGame result = null;

            if (dbGameView != null)
            {
                result = DbGameToDisplayGame(dbGameView);
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
                if (i.Platforms == null || i.Platforms.Count <= 0)
                {
                    continue;
                }

                foreach (var j in i.Platforms)
                {
                    if (!platforms.Contains(j.Id))
                    {
                        continue;
                    }

                    retval.Add(i);
                    break;
                }
            }

            return retval;
        }

        public static void SubmitRating(RatingInputModel input, bool isAuthenticated)
        {
            input.Flags = input.Flags ?? new List<int>();

            input.Flags.Sort();

            if (isAuthenticated)
            {
                DbAccess.SaveGameRating(input);
            }
            else
            {
                DbAccess.SavePendingRating(input);
            }
        }

        public static List<DbRating> GetRatings()
        {
            return DbAccess.GetRatings();
        }

        public static List<DisplayGame> GetRecentlyRatedGames(int numOfGames)
        {
            var dbGames = DbAccess.GetRecentlyRatedGames(numOfGames);

            var retval = new List<DisplayGame>();

            foreach (var v in dbGames)
            {
                retval.Add(DbGameToDisplayGame(v));
            }

            return retval;
        }

        private static DisplayGame DbGameToDisplayGame(DbGame input)
        {
            return new DisplayGame
            {
                GbSiteDetailUrl = input.GbSiteDetailUrl,
                Id = input.Id,
                LastUpdated = input.LastUpdated,
                Name = input.Name,

                Platforms = input.DbGamePlatforms.Select(x => new DisplayGamePlatform
                {
                    Abbreviation = x.DbPlatform.Abbreviation,
                    Id = x.DbPlatform.Id,
                    ImageUrl = x.DbPlatform.ImageUrl,
                    Name = x.DbPlatform.Name
                }).ToList(),

                Genres = input.DbGameGenres.Select(x => new DisplayGameGenre
                {
                    Id = x.DbGenre.Id,
                    Name = x.DbGenre.Name
                }).ToList(),

                Ratings = input.DbGameRatings.Select(x => new DisplayGameRating
                {
                    Id = x.DbRating.Id,
                    Description = x.DbRating.Description,
                    ImageUrl = x.DbRating.ImageUrl,
                    Name = x.DbRating.Name,
                    ShortDescription = x.DbRating.ShortDescription
                }).ToList(),

                IsRated = input.IsRated,
                RatingExplanation = input.RatingExplanation,
                RatingLastUpdated = input.RatingLastUpdated,
                SmallImageUrl = input.SmallImageUrl,
                Summary = input.Summary,
                ThumbImageUrl = input.ThumbImageUrl
            };
        }
    }
}