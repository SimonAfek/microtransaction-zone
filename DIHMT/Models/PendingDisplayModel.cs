using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace DIHMT.Models
{
    public class PendingDisplayModel
    {
        public int Id { get; set; }
        public int GameId { get; set; }
        [DisplayName("Game")]
        public string GameName { get; set; }
        public string RatingExplanation { get; set; }
        [DisplayName("Description")]
        public string ShortExplanation => RatingExplanation.Length > 50 ? $"{RatingExplanation.Substring(0, 46)}..." : RatingExplanation;
        [DisplayName("Submission timestamp")]
        public DateTime TimeOfSubmission { get; set; }
        [DisplayName("Submitter's IP")]
        public string SubmitterIp { get; set; }
    }
}