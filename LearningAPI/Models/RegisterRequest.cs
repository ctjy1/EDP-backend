using System.ComponentModel.DataAnnotations;

namespace UPlay.Models
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "First name is required")]
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z '-,.]+$",
            ErrorMessage = "Only allow letters, spaces and characters: ' - , .")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [MinLength(2)]
        [MaxLength(50)]
        [RegularExpression(@"^[a-zA-Z '-,.]+$",
            ErrorMessage = "Only allow letters, spaces and characters: ' - , .")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required")]
        [MinLength(2)]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;


        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [MaxLength(70)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Contact number is required")]
        [RegularExpression(@"^[89]\d{7}$",
            ErrorMessage = "Invalid Singapore mobile number")]
        [MaxLength(20)]
        public string ContactNumber { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Address1 { get; set; } = string.Empty;

        [MaxLength(100)]
        public string Address2 { get; set; } = string.Empty;

        [MaxLength(50)]
        public string ReferredCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6)]
        [MaxLength(100)]
        [RegularExpression(@"^(?=.*[a-zA-Z])(?=.*[0-9])(?=.*[^a-zA-Z0-9]).{8,}$",
    ErrorMessage = "Password must contain at least 1 letter, 1 number, and 1 special character")]
        public string Password { get; set; } = string.Empty;

    }
}
