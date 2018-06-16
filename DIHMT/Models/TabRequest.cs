using System.Collections.Generic;

namespace DIHMT.Models
{
    public class TabRequestInput
    {
        public int[] Platforms { get; set; }
        public int GameId { get; set; }
        public int Index { get; set; }
    }

    public class TabRequestView
    {
        public List<DbPlatform> Platforms { get; set; }
        public int Index { get; set; }
    }
}
