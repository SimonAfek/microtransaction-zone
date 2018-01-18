using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DIHMT.Models;

namespace DIHMT.Static
{
    public static class SearchHelpers
    {
        public static async Task<List<DisplayGame>> SearchGamesInDb(string q)
        {
            var retval = new List<DisplayGame>();

            var qArray = q.Split(' ');

            var dbGames = DbAccess.GameSearch(qArray);

            if (!dbGames.Any())
            {
                return retval;
            }

            foreach (var i in dbGames.Select(x => x.Id))
            {
                retval.Add(await GameHelpers.RefreshDisplayGame(i));
            }

            return retval;
        }
    }
}