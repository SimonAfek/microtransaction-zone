using System;
using System.Collections.Generic;
using DIHMT.Models;

namespace DIHMT.Static
{
    public static class DummyContent
    {
        public static DisplayGame DummyDisplayGame => new DisplayGame
        {
            GbSiteDetailUrl = "https://google.com",
            Id = 47551, // Yakuza 0, natch
            LastUpdated = DateTime.UtcNow,
            Name = "Dummy Title",

            Platforms = new List<DisplayGamePlatform>
            {
                new DisplayGamePlatform
                {
                    Abbreviation = "PS4",
                    Id = 146,
                    ImageUrl = string.Empty,
                    Name = "PlayStation 4"
                }
            },

            Rating = Rating.Green.ToString(),
            RatingExplanation = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Praesent vel erat in nibh auctor accumsan. Curabitur id rutrum ex. Nullam sit amet est aliquet, facilisis turpis sed, hendrerit orci.",
            SmallImageUrl = "https://dummyimage.com/300x3:4/551a8b/000000",
            Summary = "Aliquam eu pulvinar orci. Duis sodales, urna nec vulputate ullamcorper, ex erat luctus orci, non hendrerit sapien tortor vitae purus. Ut eu pharetra ligula, eu tincidunt nunc.",
            ThumbImageUrl = "https://dummyimage.com/150x3:4/551a8b/000000"
        };

        public static string DummyImage(int width, int height)
        {
            return $"https://dummyimage.com/{width}x{height}/ffff00/000000";
        }

        public static List<DisplayGame> DummyDisplayGameList(int count)
        {
            var retval = new List<DisplayGame>();

            for (var i = 0; i < count; i++)
            {
                retval.Add(DummyDisplayGame);
            }

            return retval;
        }
    }
}