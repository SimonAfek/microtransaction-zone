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

            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    var dbResults = ctx.DbGames.Where(q => query.All(k => q.Name.Contains(k)));

                    results = dbResults.Any() ? dbResults.ToList() : results;
                }
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

        public static List<DbGameWithRating> GetDbGameView(int id)
        {
            List<DbGameWithRating> results;

            lock (Lock)
            {
                using (var ctx = new DIHMTEntities())
                {
                    results = ctx.DbGameWithRatings.Where(x => x.GameId == id).ToList();
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
    }
}