using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
namespace Uplay.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MinLength(2)]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MinLength(5)]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Phone]
        [MaxLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? Address1 { get; set; } // Nullable string

        [MaxLength(100)]
        public string? Address2 { get; set; } // Nullable string

        [MaxLength(6)]
        public string ReferralCode { get; set; } = string.Empty;

        [MaxLength(6)]
        public string? ReferredCode { get; set; } // Nullable string

        [Required(AllowEmptyStrings = true)]
        [MinLength(6)]
        [MaxLength(100), JsonIgnore]
        public string Password { get; set; } = string.Empty;

        [MaxLength(50)]
        public string UserRole { get; set; } = string.Empty;

        public int points { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime UpdatedAt { get; set; }
        public string? ResetToken { get; set; }
        public DateTime? ResetTokenExpires { get; set; }

        public void GenerateReferralCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = GetRandom(); // Get a static instance of Random

            var referralCode = new StringBuilder();

            while (referralCode.Length < 6)
            {
                referralCode.Append(chars[random.Next(chars.Length)]);
            }

            ReferralCode = referralCode.ToString();
        }

        private static readonly Random _random = new Random();

        private static Random GetRandom()
        {
            lock (_random)
            {
                return _random;
            }
        }

        // Navigation property to represent the one-to-many relationship
        [JsonIgnore]
        public List<Gallery>? Galleries { get; set; }
        public List<ReferralTracking>? ReferralTrackings { get; set; }

        public ICollection<ReferralTracking> ReferredUsers { get; set; }


    }
}
