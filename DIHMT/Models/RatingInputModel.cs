using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DIHMT.Models
{
    public class RatingInputModel
    {
        public int[] MonetizationFlags => new[] { 1, 2, 4, 5, 6, 7, 8, 9 };
        public bool IsSpotless => Flags.Contains((int)EnumTag.Spotless);

        [Required]
        public int Id { get; set; }

        [DisplayName("Basically, this game")]
        public string Basically { get; set; }

        [DisplayName("Detailed Info")]
        public string RatingExplanation { get; set; }

        [DisplayName("Quiet update")]
        public bool QuietUpdate { get; set; }

        public string Comment { get; set; }
        public string SubmitterIp { get; set; }

        private List<int> _flags;
        public List<int> Flags
        {
            get => _flags;
            set => _flags = value?.Where(x => x >= (int)EnumTag.HorseArmor && x <= (int)EnumTag.F2P).ToList();
        }

        private List<string> _links;
        public List<string> Links
        {
            get => _links;
            set => _links = value?.Where(x => !string.IsNullOrEmpty(x)).ToList() ?? new List<string>();
        }


        public bool Valid
        {
            get
            {
                if (Flags == null)
                {
                    return false;
                }

                if (Flags.Contains((int)EnumTag.Spotless) &&
                   (Flags.Contains((int)EnumTag.ExpansiveExpansions) ||
                    Flags.Contains((int)EnumTag.Lootboxes) ||
                    Flags.Contains((int)EnumTag.MoneyHole) ||
                    Flags.Contains((int)EnumTag.NotJustCosmetic) ||
                    Flags.Contains((int)EnumTag.Subscription) ||
                    Flags.Contains((int)EnumTag.SingleplayerUntouched) ||
                    Flags.Contains((int)EnumTag.HorseArmor) ||
                    Flags.Contains((int)EnumTag.BulkOrderHorseArmor)))
                {
                    return false;
                }

                if (Flags.Contains((int)EnumTag.HorseArmor) && Flags.Contains((int)EnumTag.BulkOrderHorseArmor))
                {
                    return false;
                }

                if (Basically != null && Basically.Length > 280)
                {
                    return false;
                }

                if (RatingExplanation != null && RatingExplanation.Length > 4000)
                {
                    return false;
                }

                if (Links.Any(x => !Uri.IsWellFormedUriString(x, UriKind.Absolute)))
                {
                    return false;
                }

                return true;
            }
        }
    }

    public enum EnumTag
    {
        HorseArmor = 1,
        BulkOrderHorseArmor = 2,
        Spotless = 3,
        ExpansiveExpansions = 4,
        Lootboxes = 5,
        MoneyHole = 6,
        NotJustCosmetic = 7,
        Subscription = 8,
        SingleplayerUntouched = 9,
        F2P = 10
    }
}
