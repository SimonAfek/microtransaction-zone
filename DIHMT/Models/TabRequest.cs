using System.Collections.Generic;

namespace DIHMT.Models
{
    public class TabRequestInput
    {
        public List<int> Platforms { get; set; }
        public int Index { get; set; }
        public int TabIndex { get; set; }
    }

    public class TabRequestView
    {
        public List<DbPlatform> Platforms { get; set; }
        public int Index { get; set; }
        public int TabIndex { get; set; }
    }
}
