using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DIHMT.Models
{
    public class RatingInputModel
    {
        public int[] MultiValueFlags => new[] { 1, 2, 3, 4 };
        [Required]
        public int Id { get; set; }
        [Range(0, 2)]
        public int ValueProposition { get; set; }
        [Range(0, 2)]
        public int HorseArmorTier { get; set; }
        public List<int> Flags { get; set; }
        public string RatingExplanation { get; set; }
    }

    public enum ValuePropositionEnum
    {
        None = 0,
        Expansive = 1,
        Spotless = 2
    }

    public enum HorseArmorTierEnum
    {
        None = 0,
        HorseArmor = 1,
        BulkOrderHorseArmor = 2
    }
}