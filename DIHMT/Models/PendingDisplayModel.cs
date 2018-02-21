using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        public List<int> Flags { get; set; }

        public RatingInputModel RatingModel => new RatingInputModel
        {
            Id = Id,
            Flags = Flags ?? new List<int>(),
            RatingExplanation = RatingExplanation
        };

        public PendingDisplayModel()
        {

        }

        public PendingDisplayModel(PendingSubmission input)
        {
            Id = input.Id;
            GameId = input.GameId;
            GameName = input.DbGame.Name;
            RatingExplanation = input.RatingExplanation;
            SubmitterIp = input.SubmitterIp;
            TimeOfSubmission = input.TimeOfSubmission;

            try
            {
                Flags = input.PendingDbGameRatings?.Select(x => x.DbRating.Id).ToList() ?? new List<int>();
            }
            catch (ObjectDisposedException)
            {
                Flags = new List<int>();
            }
        }
    }
}