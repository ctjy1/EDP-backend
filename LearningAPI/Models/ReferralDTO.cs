namespace Uplay.Models
{
    public class ReferralDto
    {
        // The ID of the user who is being referred
        public int UserId { get; set; }

        // The referral code that is being used
        public string ReferralCode { get; set; }
    }
}
