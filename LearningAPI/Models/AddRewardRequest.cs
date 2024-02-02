using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uplay.Models
{
    public class AddRewardRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public string RewardName { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Discount { get; set; }

        [Required]
        public int PointsRequired { get; set; }

        [Required, Column(TypeName = "date")]
        public DateTime ExpiryDate { get; set; }

    }
}
