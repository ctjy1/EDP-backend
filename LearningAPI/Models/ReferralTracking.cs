namespace UPlay.Models
{
    public class ReferralTracking
    {
        public int Id { get; set; }
        public string? Status { get; set; }

        // Foreign key property
        public int UserId { get; set; }

        // Navigation property to represent the one-to-many relationship
        public User? User { get; set; }

        // Properties obtained from the associated user
        public string Username => User?.Username ?? string.Empty;
        public string Email => User?.Email ?? string.Empty;
        public string ReferralCode => User?.ReferralCode ?? string.Empty;
        public string ReferredCode => User?.ReferredCode ?? string.Empty;
        public DateTime CreatedAt => User?.CreatedAt ?? DateTime.MinValue;
    }
}
