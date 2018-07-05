using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DIHMT.Models
{
    public class DisplayGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GameSummary { get; set; }
        public string Basically { get; set; }
        public string RatingExplanation { get; set; }
        public string[] RatingExplanationArrayOfParagraphs => RatingExplanation.Split(new[] { $"{Environment.NewLine}{Environment.NewLine}" }, StringSplitOptions.None).Where(x => !string.IsNullOrEmpty(x)).ToArray();
        public DateTime? RatingLastUpdated { get; set; }
        public DateTime LastUpdated { get; set; }
        public string SmallImageUrl { get; set; }
        public string ThumbImageUrl { get; set; }
        public string GbSiteDetailUrl { get; set; }
        public bool IsRated { get; set; }
        public List<DisplayGameRating> Ratings { get; set; }
        public List<TagSet> TagSets { get; set; }
        public List<DisplayGamePlatform> Platforms { get; set; }
        public List<DisplayGameGenre> Genres { get; set; }
        public List<string> Links { get; set; }
        public string Aliases { get; set; }

        public RatingInputModel RatingModel => new RatingInputModel
        {
            Id = Id,
            Flags = Ratings?.Select(x => x.Id).ToList() ?? new List<int>(),
            TagSets = TagSets ?? new List<TagSet>(),
            Links = Links ?? new List<string>(),
            Basically = Basically,
            RatingExplanation = RatingExplanation
        };

        public DisplayGame()
        {

        }

        public DisplayGame(DbGame input)
        {
            GbSiteDetailUrl = input.GbSiteDetailUrl;
            Id = input.Id;
            LastUpdated = input.LastUpdated;
            Name = input.Name;

            Platforms = input.DbGamePlatforms.Select(x => new DisplayGamePlatform
            {
                Abbreviation = x.DbPlatform.Abbreviation,
                Id = x.DbPlatform.Id,
                ImageUrl = x.DbPlatform.ImageUrl,
                Name = x.DbPlatform.Name
            }).ToList();

            Genres = input.DbGameGenres.Select(x => new DisplayGameGenre
            {
                Id = x.DbGenre.Id,
                Name = x.DbGenre.Name
            }).ToList();

            Ratings = input.DbGameRatings.Where(x => !x.PlatformId.HasValue).Select(x => new DisplayGameRating
            {
                Id = x.DbRating.Id,
                Description = x.DbRating.Description,
                ImageUrl = x.DbRating.ImageUrl,
                Name = x.DbRating.Name,
                ShortDescription = x.DbRating.ShortDescription
            }).ToList();

            FillTagSets(input.DbGameRatings.Where(x => x.PlatformId.HasValue).ToList());

            Links = input.DbGameLinks.Select(x => x.Link).ToList();

            IsRated = input.IsRated;
            Basically = input.Basically;
            RatingExplanation = input.RatingExplanation;
            RatingLastUpdated = input.RatingLastUpdated;
            SmallImageUrl = input.SmallImageUrl;
            GameSummary = input.Summary;
            ThumbImageUrl = input.ThumbImageUrl;

            Aliases = input.Aliases;
        }

        private void FillTagSets(List<DbGameRating> input)
        {
            TagSets = new List<TagSet>();

            while (true)
            {
                if (!input.Any() || input.Any(x => !x.PlatformId.HasValue))
                {
                    return;
                }

                var tagset = new TagSet { Flags = new List<int>(), Platforms = new List<int>() };
                var groups = input.GroupBy(x => x.PlatformId);

                // First/"target" platform
                var pId = input[0].PlatformId;
                tagset.Platforms.Add(pId ?? -1);

                var fRatings = input.Where(x => x.PlatformId == pId).Select(x => x.RatingId).ToList();
                fRatings.Sort();

                tagset.Flags.AddRange(fRatings);

                // For each platform with ratings...
                foreach (var v in groups.Where(x => x.Key != pId))
                {
                    // Grab those ratings
                    var groupRatings = v.Select(x => x.RatingId).ToList();
                    groupRatings.Sort();

                    // If they're equal to the ratings of the target, add the platformId
                    if (fRatings.SequenceEqual(groupRatings))
                    {
                        tagset.Platforms.Add(v.Key ?? -1);
                    }
                }

                tagset.Platforms.RemoveAll(x => x < 0);
                TagSets.Add(tagset);

                // Remove all matched platforms from input
                input.RemoveAll(x => x.PlatformId.HasValue && tagset.Platforms.Contains(x.PlatformId.Value));

                if (input.Any())
                {
                    // Do it again, baby
                    continue;
                }

                break;
            }
        }
    }

    public class DisplayGameRating
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public string ShortDescription { get; set; }
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
