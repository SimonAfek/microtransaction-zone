﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using DIHMT.Models;
using GiantBomb.Api.Model;

namespace DIHMT.Static
{
    public static class DbAccess
    {
        private static readonly object Lock = new object();

        public static List<DbGame> GameSearch(string[] query)
        {
            var results = new List<DbGame>();

            using (var ctx = new DIHMTEntities())
            {
                var dbResults = ctx.DbGames.Where(q => query.All(k => q.Name.Contains(k)));

                results = dbResults.Any() ? dbResults.ToList() : results;
            }

            return results;
        }

        public static List<DbPlatform> GetPlatforms()
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.DbPlatforms.ToList();
            }
        }

        public static bool GameExistsInDb(int id)
        {
            bool result;

            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    result = ctx.DbGames.FirstOrDefault(x => x.Id == id) != null;
                }
            }

            return result;
        }

        public static DbGame GetGame(int id)
        {
            DbGame result;

            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    result = ctx.DbGames.FirstOrDefault(x => x.Id == id);
                }
            }

            return result;
        }

        public static DbGame GetDbGameView(int id, bool publicOnly = true)
        {
            DbGame results;

            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    results = ctx.DbGames
                        .Include(x => x.DbGamePlatforms.Select(y => y.DbPlatform))
                        .Include(x => x.DbGameGenres.Select(y => y.DbGenre))
                        .Include(x => x.DbGameRatings.Select(y => y.DbRating))
                        .FirstOrDefault(x => x.Id == id);

                    if (publicOnly && results?.DbGameRatings != null && results.DbGameRatings.Any())
                    {
                        results.DbGameRatings = results.DbGameRatings.Where(x => x.Public).ToList();
                    }
                }
            }

            return results;
        }

        public static List<DbPlatform> GetPlatforms(ICollection<Platform> platforms)
        {
            List<DbPlatform> results;
            var ids = (from p in platforms select p.Id).ToList();

            using (var ctx = new DIHMTEntities())
            {
                results = ctx.DbPlatforms.Where(x => ids.Contains(x.Id)).ToList();
            }

            return results;
        }

        public static void SaveGame(DbGame input)
        {
            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    var existingRecord = ctx.DbGames.FirstOrDefault(x => x.Id == input.Id);

                    if (existingRecord != null)
                    {
                        ctx.Entry(existingRecord).CurrentValues.SetValues(input);
                        ctx.Entry(existingRecord).State = EntityState.Modified;
                    }

                    else
                    {
                        ctx.DbGames.Add(input);
                    }

                    ctx.SaveChanges();
                }
            }
        }

        public static void SaveGamePlatforms(IEnumerable<DbGamePlatform> input)
        {
            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    ctx.DbGamePlatforms.AddRange(input);

                    ctx.SaveChanges();
                }
            }
        }

        public static void SaveGameGenres(IEnumerable<DbGameGenre> input)
        {
            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    ctx.DbGameGenres.AddRange(input);

                    ctx.SaveChanges();
                }
            }
        }

        public static void SaveGameRating(RatingInputModel input)
        {
            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    var game = ctx.DbGames.FirstOrDefault(x => x.Id == input.Id);

                    if (game != null)
                    {
                        // Set IsRated + RatingLastUpdated & update explanation
                        game.IsRated = true;
                        game.RatingLastUpdated = DateTime.UtcNow;
                        game.RatingExplanation = input.RatingExplanation;

                        // Remove current ratings
                        ctx.DbGameRatings.RemoveRange(ctx.DbGameRatings.Where(x => x.GameId == input.Id));

                        // Add ratings from input
                        ctx.DbGameRatings.AddRange(input.Flags?.Select(x => new DbGameRating { GameId = input.Id, RatingId = x }) ?? new List<DbGameRating>());

                        ctx.SaveChanges();
                    }
                }
            }
        }

        internal static List<DbRating> GetRatings()
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.DbRatings.ToList();
            }
        }

        public static List<DbGame> GetRecentlyRatedGames(int numOfGames)
        {
            using (var ctx = new DIHMTEntities())
            {
                var retval = ctx.DbGames
                    .Include(x => x.DbGamePlatforms.Select(y => y.DbPlatform))
                    .Include(x => x.DbGameGenres.Select(y => y.DbGenre))
                    .Include(x => x.DbGameRatings.Select(y => y.DbRating))
                    .OrderByDescending(x => x.RatingLastUpdated)
                    .Take(numOfGames)
                    .ToList();

                foreach (var v in retval.Where(x => x.DbGameRatings != null && x.DbGameRatings.Any()))
                {
                    v.DbGameRatings = v.DbGameRatings.Where(x => x.Public).ToList();
                }

                return retval;
            }
        }
    }
}