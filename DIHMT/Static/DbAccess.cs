using System;
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

        public static DbGame GetDbGameView(int id)
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
                }
            }

            return results;
        }

        public static List<DbPlatform> GetPlatforms(ICollection<Platform> platforms)
        {
            List<DbPlatform> results;
            var ids = platforms.Select(x => x.Id).ToList();

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
                        game.Basically = input.Basically;
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
                return ctx.DbGames
                    .Include(x => x.DbGamePlatforms.Select(y => y.DbPlatform))
                    .Include(x => x.DbGameGenres.Select(y => y.DbGenre))
                    .Include(x => x.DbGameRatings.Select(y => y.DbRating))
                    .OrderByDescending(x => x.RatingLastUpdated)
                    .Take(numOfGames)
                    .ToList();
            }
        }

        internal static void SavePendingRating(RatingInputModel input)
        {
            using (var ctx = new DIHMTEntities())
            {
                var pendingRatingObject = new PendingSubmission
                {
                    GameId = input.Id,
                    RatingExplanation = input.RatingExplanation,
                    Basically = input.Basically,
                    TimeOfSubmission = DateTime.UtcNow,
                    SubmitterIp = input.SubmitterIp
                };

                ctx.PendingSubmissions.Add(pendingRatingObject);

                ctx.SaveChanges();

                ctx.PendingDbGameRatings.AddRange(input.Flags?.Select(x => new PendingDbGameRating { PendingSubmissionId = pendingRatingObject.Id, RatingId = x }) ?? new List<PendingDbGameRating>());

                ctx.SaveChanges();
            }
        }

        internal static PendingSubmission GetPendingSubmission(int id)
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.PendingSubmissions
                    .Where(x => x.Id == id)
                    .Include(x => x.DbGame)
                    .Include(x => x.PendingDbGameRatings.Select(y => y.DbRating))
                    .FirstOrDefault();
            }
        }

        internal static List<PendingSubmission> GetPendingSubmissionsList()
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.PendingSubmissions
                    .Include(x => x.DbGame)
                    .OrderBy(x => x.DbGame.Id).ToList();
            }
        }
    }
}
