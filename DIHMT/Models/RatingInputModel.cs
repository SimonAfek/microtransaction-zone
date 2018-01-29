using System.Collections.Generic;

namespace DIHMT.Models
{
    public class RatingInputModel
    {
        public int Id { get; set; }
        public List<int> Flags { get; set; }
        public string RatingExplanation { get; set; }
    }
}