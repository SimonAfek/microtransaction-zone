using System;
using System.Collections.Generic;
using System.Linq;

namespace DIHMT.Models
{
    public class DisplayGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string GameSummary { get; set; }
        public string Basically { get; set; }
        public string RatingExplanation { get; set; }
        public DateTime? RatingLastUpdated { get; set; }
        public DateTime LastUpdated { get; set; }
        public string SmallImageUrl { get; set; }
        public string ThumbImageUrl { get; set; }
        public string GbSiteDetailUrl { get; set; }
        public bool IsRated { get; set; }
        public List<DisplayGameRating> Ratings { get; set; }
        public List<DisplayGamePlatform> Platforms { get; set; }
        public List<DisplayGameGenre> Genres { get; set; }
        public List<string> Links { get; set; }

        public RatingInputModel RatingModel => new RatingInputModel
        {
            Id = Id,
            Flags = Ratings?.Select(x => x.Id).ToList() ?? new List<int>(),
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

            Ratings = input.DbGameRatings.Select(x => new DisplayGameRating
            {
                Id = x.DbRating.Id,
                Description = x.DbRating.Description,
                ImageUrl = x.DbRating.ImageUrl,
                Name = x.DbRating.Name,
                ShortDescription = x.DbRating.ShortDescription
            }).ToList();

            Links = input.DbGameLinks.Select(x => x.Link).ToList();

            IsRated = input.IsRated;
            Basically = input.Basically;
            RatingExplanation = input.RatingExplanation;
            RatingLastUpdated = input.RatingLastUpdated;
            SmallImageUrl = input.SmallImageUrl;
            GameSummary = input.Summary;
            ThumbImageUrl = input.ThumbImageUrl;
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
