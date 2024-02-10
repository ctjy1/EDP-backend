using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Uplay.Models
{
    public class ResetPassword
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Token { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }



}
