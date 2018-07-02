using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
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
                var dbResults = ctx.DbGames
                    .Where(q =>
                        query.All(k => q.Name.Contains(k))
                        || query.All(k => q.Aliases.Contains(k)))
                    .Include(x => x.DbGamePlatforms.Select(y => y.DbPlatform))
                    .Include(x => x.DbGameGenres.Select(y => y.DbGenre))
                    .Include(x => x.DbGameRatings.Select(y => y.DbRating))
                    .Include(x => x.DbGameLinks)
                    .OrderByDescending(x => x.ReleaseDate);

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

        public static List<DbGame> AdvancedSearch(
            string[] query,
            List<int> requireFlags,
            List<int> blockFlags,
            List<int> allowFlags,
            List<int> platforms,
            List<int> genres)
        {
            using (var ctx = new DIHMTEntities())
            {
                var games = ctx.DbGames
                    .Where(x => x.IsRated)
                    .Include(x => x.DbGamePlatforms.Select(y => y.DbPlatform))
                    .Include(x => x.DbGameGenres.Select(y => y.DbGenre))
                    .Include(x => x.DbGameRatings.Select(y => y.DbRating))
                    .Include(x => x.DbGameLinks);

                if (query != null && query.Any(x => !string.IsNullOrEmpty(x)))
                {
                    games = games.Where(q =>
                        query.All(k => q.Name.Contains(k))
                        || query.All(k => q.Aliases.Contains(k)));
                }

                if (genres != null && genres.Any())
                {
                    games = games.Where(x => x.DbGameGenres.Select(y => y.GenreId).Intersect(genres).Any());
                }

                if (platforms != null && platforms.Any())
                {
                    games = games.Where(x => x.DbGamePlatforms.Select(y => y.PlatformId).Intersect(platforms).Any());
                }

                if (requireFlags != null && requireFlags.Any())
                {
                    // Avoids warning about implicit closure
                    var allowFlagsSeparateList = allowFlags.Select(x => x).ToList();

                    games = games.Where(x =>
                        x.DbGameRatings.Select(y => y.RatingId).Intersect(requireFlags).Any()
                        || x.DbGameRatings.Select(y => y.RatingId).Intersect(allowFlagsSeparateList).Any()
                    );
                }

                if (blockFlags != null && blockFlags.Any())
                {
                    // Avoids warning about implicit closure
                    var allowFlagsSeparateList = allowFlags.Select(x => x).ToList();

                    games = games.Where(x =>
                        !x.DbGameRatings.Select(y => y.RatingId).Intersect(blockFlags).Any()
                        || x.DbGameRatings.Select(y => y.RatingId).Intersect(allowFlagsSeparateList).Any()
                    );
                }

                games = games.OrderByDescending(x => x.ReleaseDate);

                return games.ToList();
            }
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
                        .Include(x => x.DbGameLinks)
                        .FirstOrDefault(x => x.Id == id);
                }
            }

            if (results != null)
            {
                results.DbGameRatings = results.DbGameRatings.OrderBy(x => x.DbRating.SortOrder).ToList();
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

        public static void SaveListOfNewGames(List<DbGame> input, out List<int> newIds, out List<int> gamesToUpdate)
        {
            using (var ctx = new DIHMTEntities())
            {
                var newGames = input.Where(x => ctx.DbGames.FirstOrDefault(y => y.Id == x.Id) == null).ToList();

                if (newGames.Any())
                {
                    ctx.DbGames.AddRange(newGames);

                    ctx.SaveChanges();
                }

                newIds = newGames.Select(x => x.Id).ToList();

                // var roles = db.Roles.Where(r => user.Roles.Contains(r.RoleId));

                var inputIds = input.Select(x => x.Id).ToArray();

                gamesToUpdate = ctx.DbGames.Where(x => inputIds.Contains(x.Id))
                    .AsEnumerable()
                    .Where(x => (DateTime.UtcNow - x.LastUpdated).Days >= 7)
                    .Select(x => x.Id)
                    .ToList();
            }
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
                    var inputList = input.ToList();

                    foreach (var v in inputList.GroupBy(x => x.GameId))
                    {
                        if (ctx.DbGamePlatforms.Any(x => x.GameId == v.Key))
                        {
                            var existingPlatformIds = v.Select(x => x.PlatformId).ToList();

                            // Remove entries that are in DB but not in input
                            var toRemove = ctx.DbGamePlatforms.Where(x =>
                                x.GameId == v.Key
                                && !existingPlatformIds.Contains(x.PlatformId));

                            ctx.DbGamePlatforms.RemoveRange(toRemove);

                            // Add entries that are in input but not in DB
                            var existingEntries = ctx.DbGamePlatforms.Where(x => x.GameId == v.Key).ToList();

                            var toAdd = v.Where(x =>
                                !existingEntries.Any(y =>
                                    y.GameId == v.Key
                                    && y.PlatformId == x.PlatformId));

                            ctx.DbGamePlatforms.AddRange(toAdd);
                        }
                        else
                        {
                            ctx.DbGamePlatforms.AddRange(v);
                        }
                    }

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
                        game.RatingLastUpdated = input.QuietUpdate ? game.RatingLastUpdated : DateTime.UtcNow;
                        game.Basically = input.Basically;
                        game.RatingExplanation = input.RatingExplanation;

                        // Remove current ratings
                        ctx.DbGameRatings.RemoveRange(ctx.DbGameRatings.Where(x => x.GameId == input.Id));

                        // Add ratings from input
                        ctx.DbGameRatings.AddRange(input.Flags?.Select(x => new DbGameRating { GameId = input.Id, RatingId = x }) ?? new List<DbGameRating>());

                        // Add platform-specific ratings, if any
                        foreach (var v in input.TagSets)
                        {
                            foreach (var k in v.Platforms)
                            {
                                ctx.DbGameRatings.AddRange(v.Flags?.Select(x => new DbGameRating { GameId = input.Id, PlatformId = k, RatingId = x }) ?? new List<DbGameRating>());
                            }
                        }

                        // Remove current links
                        ctx.DbGameLinks.RemoveRange(ctx.DbGameLinks.Where(x => x.GameId == input.Id));

                        // Add links from input
                        ctx.DbGameLinks.AddRange(input.Links?.Select(x => new DbGameLink { GameId = input.Id, Link = x }) ?? new List<DbGameLink>());

                        ctx.SaveChanges();
                    }
                }
            }
        }

        internal static List<DbRating> GetRatings()
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.DbRatings.Where(x => !x.DeletedFlag).OrderBy(x => x.SortOrder).ToList();
            }
        }

        public static List<DbGame> GetRecentlyRatedGames(int numOfGames, int page, int pageLimit)
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.DbGames
                    .Include(x => x.DbGamePlatforms.Select(y => y.DbPlatform))
                    .Include(x => x.DbGameGenres.Select(y => y.DbGenre))
                    .Include(x => x.DbGameRatings.Select(y => y.DbRating))
                    .Include(x => x.DbGameLinks)
                    .OrderByDescending(x => x.RatingLastUpdated)
                    .Skip((page - 1) * pageLimit)
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
                    SubmitterIp = input.SubmitterIp,
                    Comment = input.Comment
                };

                ctx.PendingSubmissions.Add(pendingRatingObject);

                ctx.SaveChanges();

                ctx.PendingDbGameRatings.AddRange(input.Flags?.Select(x => new PendingDbGameRating { PendingSubmissionId = pendingRatingObject.Id, RatingId = x }) ?? new List<PendingDbGameRating>());

                foreach (var v in input.TagSets)
                {
                    foreach (var k in v.Platforms)
                    {
                        ctx.PendingDbGameRatings.AddRange(v.Flags?.Select(x => new PendingDbGameRating { PendingSubmissionId = pendingRatingObject.Id, PlatformId = k, RatingId = x }) ?? new List<PendingDbGameRating>());
                    }
                }
                
                ctx.PendingGameLinks.AddRange(input.Links?.Select(x => new PendingGameLink { PendingSubmissionId = pendingRatingObject.Id, Link = x }) ?? new List<PendingGameLink>());

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
                    .Include(x => x.PendingGameLinks)
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

        public static void ApprovePendingRating(PendingDisplayModel input)
        {
            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    var game = ctx.DbGames.FirstOrDefault(x => x.Id == input.GameId);

                    if (game != null)
                    {
                        // Set IsRated + RatingLastUpdated & update explanation
                        game.IsRated = true;
                        game.RatingLastUpdated = input.QuietUpdate ? game.RatingLastUpdated : DateTime.UtcNow;
                        game.Basically = input.Basically;
                        game.RatingExplanation = input.RatingExplanation;

                        // Remove current ratings
                        ctx.DbGameRatings.RemoveRange(ctx.DbGameRatings.Where(x => x.GameId == input.GameId));

                        // Remove current links
                        ctx.DbGameLinks.RemoveRange(ctx.DbGameLinks.Where(x => x.GameId == input.GameId));

                        // Add ratings from input
                        ctx.DbGameRatings.AddRange(input.RatingModel.Flags?.Select(x => new DbGameRating { GameId = input.GameId, RatingId = x }) ?? new List<DbGameRating>());

                        // Add links from input
                        ctx.DbGameLinks.AddRange(input.RatingModel.Links?.Select(x => new DbGameLink { GameId = input.GameId, Link = x }) ?? new List<DbGameLink>());

                        // Remove pending ratings
                        ctx.PendingDbGameRatings.RemoveRange(ctx.PendingDbGameRatings.Where(x => x.RatingId == input.Id));

                        // Remove pending links
                        ctx.PendingGameLinks.RemoveRange(ctx.PendingGameLinks.Where(x => x.PendingSubmissionId == input.Id));

                        // Remove pending submission
                        ctx.PendingSubmissions.Remove(ctx.PendingSubmissions.First(x => x.Id == input.Id));

                        ctx.SaveChanges();
                    }
                }
            }
        }

        public static void RejectPendingRating(PendingDisplayModel input)
        {
            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    // Remove pending ratings
                    ctx.PendingDbGameRatings.RemoveRange(ctx.PendingDbGameRatings.Where(x => x.RatingId == input.Id));

                    // Remove pending links
                    ctx.PendingGameLinks.RemoveRange(ctx.PendingGameLinks.Where(x => x.PendingSubmissionId == input.Id));

                    // Remove pending submission
                    ctx.PendingSubmissions.Remove(ctx.PendingSubmissions.First(x => x.Id == input.Id));

                    ctx.SaveChanges();
                }
            }
        }

        public static List<DbGenre> GetGenres()
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.DbGenres.ToList();
            }
        }

        public static BlockList IsBlocked(string ip)
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.BlockLists.FirstOrDefault(x => x.BlockedIp == ip);
            }
        }

        public static void BlockIp(BlockList input)
        {
            using (var ctx = new DIHMTEntities())
            {
                var curBlock = ctx.BlockLists.FirstOrDefault(x => x.BlockedIp == input.BlockedIp);

                if (curBlock == null)
                {
                    ctx.BlockLists.Add(input);
                }
                else
                {
                    curBlock.Expiration = input.Expiration;
                    curBlock.Explicit = input.Explicit;
                }

                ctx.SaveChanges();
            }
        }

        public static ThumbImage GetThumb(int gameId)
        {
            using (var ctx = new DIHMTEntities())
            {
                return ctx.ThumbImages.FirstOrDefault(x => x.GameId == gameId);
            }
        }

        public static void SaveThumb(int gameId, string imageUrl, string contentType)
        {
            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    var existingThumb = ctx.ThumbImages.FirstOrDefault(x => x.GameId == gameId);

                    if (existingThumb == null)
                    {
                        ctx.ThumbImages.Add(new ThumbImage
                        {
                            GameId = gameId,
                            ImageUrl = imageUrl,
                            ContentType = contentType,
                            LastUpdated = DateTime.UtcNow
                        });

                        ctx.SaveChanges();
                    }
                    else
                    {
                        existingThumb.ImageUrl = imageUrl;
                        existingThumb.ContentType = contentType;
                        existingThumb.LastUpdated = DateTime.UtcNow;

                        ctx.Entry(existingThumb).State = EntityState.Modified;

                        ctx.SaveChanges();
                    }
                }
            }
        }

        public static void PurgeThumbs()
        {
            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    ctx.Database.ExecuteSqlCommand("DELETE FROM [ThumbImage];");
                }
            }
        }
    }
}
