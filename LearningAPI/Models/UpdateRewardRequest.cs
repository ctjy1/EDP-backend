using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LearningAPI.Models
{
    public class UpdateRewardRequest
    {
        [Required, MinLength(3), MaxLength(100)]
        public string RewardName { get; set; } = string.Empty;

        [Required, MinLength(3), MaxLength(100)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public int Discount { get; set; }

        [Required]
        public int PointsRequired { get; set; }

        [Required, Column(TypeName = "datetime")]
        public DateTime ExpiryDate { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime RedeemedAt { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime DeletedAt { get; set; }
    }
}
