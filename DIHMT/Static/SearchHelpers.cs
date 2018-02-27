using System;
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

            if (!dbGames.Any())
            {
                return retval;
            }

            retval.AddRange(dbGames.Select(x => x.Id).Select(GameHelpers.CreateDisplayGameObject));

            return retval;
        }

        public static List<DisplayGame> AdvancedSearch(string q, List<int> blockFlags, List<int> allowFlags, List<int> platforms, List<int> genres)
        {
            var qArray = q?.Split(' ');

            var rawDbValues = DbAccess.AdvancedSearch(qArray, blockFlags, allowFlags, platforms, genres);

            throw new NotImplementedException();
        }
    }
}
