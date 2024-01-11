using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LearningAPI.Models
{
    public class RewardDTO
    {
        public int Id { get; set; }

        public string RewardName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public int Discount { get; set; }

        public int PointsRequired { get; set; }

        public DateTime ExpiryDate { get; set; }

        public DateTime RedeemedAt { get; set; }
        public DateTime DeletedAt { get; set; }

        public int UserId { get; set; }

        public UserBasicDTO? User { get; set; }
    }
}
