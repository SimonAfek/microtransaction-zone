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
            var retval = new DbGame
            {
                Id = input.Id,
                Name = input.Name,
                Summary = input.Deck,
                LastUpdated = DateTime.UtcNow,
                SmallImageUrl = input.Image.SmallUrl,
                ThumbImageUrl = input.Image.ThumbUrl,
                GbSiteDetailUrl = input.SiteDetailUrl,
                Aliases = input.Aliases
            };

            var nameCharArray = input.Name.ToCharArray();

            if (nameCharArray.Any(x => char.IsSymbol(x) || char.IsPunctuation(x)))
            {
                var strippedName = new string(nameCharArray.Where(x => !char.IsSymbol(x) && !char.IsPunctuation(x)).ToArray());

                if (!retval.Aliases?.Contains(strippedName) ?? false)
                {
                    if (string.IsNullOrEmpty(retval.Aliases))
                    {
                        retval.Aliases = strippedName;
                    }
                    else
                    {
                        retval.Aliases += $"{Environment.NewLine}{strippedName}";
                    }
                }
            }

            if (input.OriginalReleaseDate.HasValue)
            {
                retval.ReleaseDate = input.OriginalReleaseDate.Value.Date;
            }
            else if (input.ExpectedReleaseYear.HasValue)
            {
                var releaseDate = DateTime.MinValue;

                releaseDate = releaseDate.AddYears(input.ExpectedReleaseYear.Value - 1);

                if (input.ExpectedReleaseQuarter.HasValue && !input.ExpectedReleaseMonth.HasValue)
                {
                    releaseDate = releaseDate.AddMonths((input.ExpectedReleaseQuarter.Value * 3) - 1);
                }
                else
                {
                    releaseDate = releaseDate.AddMonths((input.ExpectedReleaseMonth ?? 1) - 1);
                }

                releaseDate = releaseDate.AddDays((input.ExpectedReleaseDay ?? 1) - 1);

                retval.ReleaseDate = releaseDate;
            }

            return retval;
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
            dbGame.Basically = dGame.Basically;
            dbGame.RatingExplanation = dGame.RatingExplanation;
            dbGame.RatingLastUpdated = dGame.RatingLastUpdated;
            dbGame.LastUpdated = DateTime.UtcNow;

            DbAccess.SaveGame(dbGame);

            var dbGamePlatforms = CreateDbGamePlatformsListWithoutNavigation(gbGame);

            DbAccess.SaveGamePlatforms(dbGamePlatforms);

            if (!includeGenres || gbGame.Genres == null || !gbGame.Genres.Any())
            {
                return;
            }

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
            return gbGame.Genres?.Select(g => new DbGameGenre { GameId = gbGame.Id, GenreId = g.Id }).ToList();
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
        /// relevant GamePlatform and GameGenre-rows
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
        /// Saves multiple GbGame objects to the database, including
        /// relevaing joining rows. Ignores games that already exist in the DB.
        /// Also updates games whose info is at least 7 days old.
        /// </summary>
        /// <param name="input">A list of objects to save</param>
        public static void SaveGamesToDb(List<Game> input)
        {
            var dbGames = input.Select(CreateDbGameObjectWithoutNavigation).ToList();

            DbAccess.SaveListOfNewGames(dbGames, out var newGameIds, out var gamesToUpdate);

            var dbGamePlatforms = input
                .Where(x => newGameIds.Contains(x.Id))
                .SelectMany(CreateDbGamePlatformsListWithoutNavigation)
                .ToList();

            if (dbGamePlatforms.Any())
            {
                DbAccess.SaveGamePlatforms(dbGamePlatforms);
            }

            var dbGameGenres = input
                .Where(x => newGameIds.Contains(x.Id))
                .Where(x => x.Genres != null && x.Genres.Any())
                .SelectMany(CreateDbGameGenresListWithoutNavigation)
                .ToList();

            if (dbGameGenres.Any())
            {
                DbAccess.SaveGameGenres(dbGameGenres);
            }

            // Updating existing games with out-of-date info
            if (gamesToUpdate.Any())
            {
                var gameObjectsToUpdate = input.Where(x => gamesToUpdate.Contains(x.Id)).ToList();

                foreach (var v in gameObjectsToUpdate)
                {
                    var dGame = CreateDisplayGameObject(v.Id);

                    var dbGame = CreateDbGameObjectWithoutNavigation(v);
                    var dbGamePlatformsUpdate = CreateDbGamePlatformsListWithoutNavigation(v);

                    dbGame.IsRated = dGame.IsRated;
                    dbGame.Basically = dGame.Basically;
                    dbGame.RatingExplanation = dGame.RatingExplanation;
                    dbGame.RatingLastUpdated = dGame.RatingLastUpdated;
                    dbGame.LastUpdated = DateTime.UtcNow;

                    DbAccess.SaveGame(dbGame);
                    DbAccess.SaveGamePlatforms(dbGamePlatformsUpdate);
                }
            }
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

            SaveGamesToDb(filteredResults);
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
                result = new DisplayGame(dbGameView);
            }

            return result;
        }

        public static List<Game> FilterOutUnsupportedPlatforms(List<Game> input)
        {
            var platforms = DbAccess.GetPlatforms().Select(x => x.Id).ToList();

            var retval = new List<Game>();

            foreach (var i in input)
            {
                // Some games don't have platforms
                if (i.Platforms == null || !i.Platforms.Any())
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

        /// <summary>
        /// Returns a Tuple containing a pending rating, as well as the current entry
        /// of the game that the pending rating is for
        /// </summary>
        /// <param name="id">Id of the pending rating to get</param>
        /// <returns>
        /// A Tuple - Item1 is the entry as it exists now on the public-facing site,
        /// Item2 is the requested pending rating as a PendingDisplayModel
        /// </returns>
        internal static Tuple<DisplayGame, PendingDisplayModel> GetPendingSubmissionWithCurrentRating(int id)
        {
            var pendingRaw = DbAccess.GetPendingSubmission(id);

            if (pendingRaw == null)
            {
                return null;
            }

            var pending = new PendingDisplayModel(pendingRaw);

            var currentRating = CreateDisplayGameObject(pendingRaw.GameId);

            return new Tuple<DisplayGame, PendingDisplayModel>(currentRating, pending);
        }

        /// <summary>
        /// Retrieves all pending submissions in the database.
        /// </summary>
        /// <returns>
        /// All currently existing pending submissions, cast to PendingDisplayModels.
        /// </returns>
        internal static List<PendingDisplayModel> GetPendingSubmissionsList()
        {
            var rawDbValues = DbAccess.GetPendingSubmissionsList();

            return rawDbValues.Select(x => new PendingDisplayModel(x)).ToList();
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

        public static void HandlePostPending(PendingDisplayModel input)
        {
            if (input.SubmitAction == "Approve")
            {
                DbAccess.ApprovePendingRating(input);
            }
            else
            {
                DbAccess.RejectPendingRating(input);
            }
        }

        public static List<DbRating> GetRatings()
        {
            var rawRatings = DbAccess.GetRatings();

            // Moving "Spotless" to the top of the list...
            // Maybe making a "DisplayOrder"-column in the DB would be better.
            // For now, you must deal with my heinous sins
            var sorted = new List<DbRating> { rawRatings.First(x => x.Id == (int)EnumTag.Spotless) };
            sorted.AddRange(rawRatings.Where(x => x.Id != (int)EnumTag.Spotless));

            return sorted;
        }

        public static List<DisplayGame> GetRecentlyRatedGames(int numOfGames, int page = 1, int pageLimit = 10)
        {
            var dbGames = DbAccess.GetRecentlyRatedGames(numOfGames, page, pageLimit);

            return dbGames.Select(x => new DisplayGame(x)).ToList();
        }

        public static List<DbGenre> GetGenres()
        {
            return DbAccess.GetGenres();
        }

        public static List<DbPlatform> GetPlatforms()
        {
            return DbAccess.GetPlatforms();
        }
    }
}
