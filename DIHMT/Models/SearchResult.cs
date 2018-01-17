using System.Collections.Generic;

namespace DIHMT.Models
{
    public class SearchResult
    {
        public int Page { get; set; }
        public List<DisplayGame> Results { get; set; }
        public string Query { get; set; }
    }
}