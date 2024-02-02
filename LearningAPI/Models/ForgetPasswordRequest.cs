using System.ComponentModel.DataAnnotations;

namespace UPlay.Models
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }


}
