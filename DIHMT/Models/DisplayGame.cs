using System;
using System.Collections.Generic;

namespace DIHMT.Models
{
    public class DisplayGame
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Summary { get; set; }
        public string Rating { get; set; }
        public string RatingExplanation { get; set; }
        public DateTime LastUpdated { get; set; }
        public string SmallImageUrl { get; set; }
        public string ThumbImageUrl { get; set; }
        public string GbSiteDetailUrl { get; set; }
        public List<DisplayGamePlatform> Platforms { get; set; }
        public List<DisplayGameGenre> Genres { get; set; }
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