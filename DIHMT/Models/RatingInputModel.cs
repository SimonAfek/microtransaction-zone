using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DIHMT.Models
{
    public class RatingInputModel
    {
        public List<int> MultiValueFlags => new List<int> { 1, 2 };
        [Required]
        public int Id { get; set; }
        public string RatingExplanation { get; set; }

        private List<int> _flags;
        public List<int> Flags
        {
            get => _flags?.Where(x => x >= (int) EnumRating.HorseArmor && x <= (int) EnumRating.F2P).ToList();
            set => _flags = value?.Where(x => x >= (int)EnumRating.HorseArmor && x <= (int)EnumRating.F2P).ToList();
        }

        public bool Valid
        {
            get
            {
                if (Flags == null)
                {
                    return false;
                }

                if (Flags.Contains((int)EnumRating.Spotless) &&
                    (Flags.Contains((int)EnumRating.ExpansiveExpansions) ||
                     Flags.Contains((int)EnumRating.Lootboxes) ||
                     Flags.Contains((int)EnumRating.MoneyHole) ||
                     Flags.Contains((int)EnumRating.NotJustCosmetic) ||
                     Flags.Contains((int)EnumRating.Subscription) ||
                     Flags.Contains((int)EnumRating.SingleplayerUntouched)))
                {
                    return false;
                }

                if (Flags.Contains((int)EnumRating.HorseArmor) && Flags.Contains((int)EnumRating.BulkOrderHorseArmor))
                {
                    return false;
                }

                return true;
            }
        }
    }

    public enum EnumRating
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