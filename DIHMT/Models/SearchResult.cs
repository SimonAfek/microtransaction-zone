using System.Collections.Generic;
using System.Text;

namespace DIHMT.Models
{
    public class SearchResult
    {
        public int Page { get; set; }
        public List<DisplayGame> Results { get; set; }
        public string Query { get; set; }
        public SearchType Type { get; set; }
        public List<int> RequireFlags { get; set; }
        public List<int> BlockFlags { get; set; }
        public List<int> AllowFlags { get; set; }
        public List<int> Genres { get; set; }
        public List<int> Platforms { get; set; }

        public string AdvancedQueryStringWithoutPageNumber
        {
            get
            {
                if (Type != SearchType.Advanced)
                {
                    return string.Empty;
                }

                var sb = new StringBuilder();

                sb.Append($"?q={Query}");
                
                RequireFlags.ForEach(x => sb.Append($"&requireFlags={x}"));
                BlockFlags.ForEach(x => sb.Append($"&blockFlags={x}"));
                AllowFlags.ForEach(x => sb.Append($"&allowFlags={x}"));
                Genres.ForEach(x => sb.Append($"&genres={x}"));
                Platforms.ForEach(x => sb.Append($"&platforms={x}"));

                sb.Append("&page=");

                return sb.ToString();
            }
        }

        public SearchResult()
        {
            RequireFlags = new List<int>();
            BlockFlags = new List<int>();
            AllowFlags = new List<int>();
            Genres = new List<int>();
            Platforms = new List<int>();
        }
    }

    public enum SearchType
    {
        Standard,
        Advanced,
        Recent
    }
}
