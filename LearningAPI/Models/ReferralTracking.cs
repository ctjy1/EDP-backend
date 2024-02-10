using Uplay.Models;

namespace Uplay.Models
{
    public class ReferralTracking
    {
        public int Id { get; set; }
        public string? Status { get; set; }

        // Foreign key property for the referred user
        public int UserId { get; set; }
        public User User { get; set; }

        public int? ReferringUserId { get; set; }
        public User ReferringUser { get; set; }


        // Properties obtained from the associated user
        public string Username => User?.Username ?? string.Empty;
        public string Email => User?.Email ?? string.Empty;
        public string ReferralCode => User?.ReferralCode ?? string.Empty;
        public string ReferredCode => User?.ReferredCode ?? string.Empty;
        public DateTime CreatedAt => User?.CreatedAt ?? DateTime.MinValue;
    }

}
