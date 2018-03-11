using System.Collections.Generic;
using System.Linq;
using DIHMT.Models;

namespace DIHMT.Static
{
    public static class SearchHelpers
    {
        public static List<DisplayGame> SearchGamesInDb(string q)
        {
            var retval = new List<DisplayGame>();

            var qArray = q.Split(' ');

            var dbGames = DbAccess.GameSearch(qArray);

            if (dbGames.Any())
            {
                retval.AddRange(dbGames.Select(x => new DisplayGame(x)));
            }

            return retval;
        }

        public static List<DisplayGame> AdvancedSearch(
            string q,
            List<int> requireFlags,
            List<int> blockFlags,
            List<int> allowFlags,
            List<int> platforms,
            List<int> genres)
        {
            var qArray = q?.Split(' ');

            var rawDbValues = DbAccess.AdvancedSearch(qArray, requireFlags, blockFlags, allowFlags, platforms, genres);

            return rawDbValues.Select(x => new DisplayGame(x)).ToList();
        }
    }
}
