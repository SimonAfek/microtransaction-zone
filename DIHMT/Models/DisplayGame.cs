using System;
using System.Collections.Generic;
using System.Linq;

namespace DIHMT.Models
{
    public class DisplayGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string RatingExplanation { get; set; }
        public DateTime LastUpdated { get; set; }
        public string SmallImageUrl { get; set; }
        public string ThumbImageUrl { get; set; }
        public string GbSiteDetailUrl { get; set; }
        public bool IsRated { get; set; }
        public List<DisplayGameRating> Ratings { get; set; }
        public List<DisplayGamePlatform> Platforms { get; set; }
        public List<DisplayGameGenre> Genres { get; set; }

        public RatingInputModel RatingModel
        {
            get
            {
                var valueProposition = 0;
                var horseArmorTier = 0;
                if (Ratings != null && Ratings.Any())
                {
                    if (Ratings.Any(x => x.Id == (int)ValuePropositionEnum.Expansive))
                    {
                        valueProposition = (int)ValuePropositionEnum.Expansive;
                    }
                    else if (Ratings.Any(x => x.Id == (int)ValuePropositionEnum.Spotless))
                    {
                        valueProposition = (int)ValuePropositionEnum.Spotless;
                    }

                    if (Ratings.Any(x => x.Id == (int)HorseArmorTierEnum.HorseArmor + 2))
                    {
                        horseArmorTier = (int)HorseArmorTierEnum.HorseArmor;
                    }
                    else if (Ratings.Any(x => x.Id == (int)HorseArmorTierEnum.BulkOrderHorseArmor + 2))
                    {
                        horseArmorTier = (int)HorseArmorTierEnum.BulkOrderHorseArmor;
                    }
                }

                return new RatingInputModel
                {
                    Id = Id,
                    ValueProposition = valueProposition,
                    HorseArmorTier = horseArmorTier,
                    Flags = Ratings?
                            .Where(x => x.Id > (int)HorseArmorTierEnum.BulkOrderHorseArmor).Select(x => x.Id)
                            .ToList()
                            ?? new List<int>(),
                    RatingExplanation = RatingExplanation
                };
            }
        }
    }

    public class DisplayGameRating
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
    }

    public class DisplayGamePlatform
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Abbreviation { get; set; }
        public string ImageUrl { get; set; }
    }

    public class DisplayGameGenre
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}