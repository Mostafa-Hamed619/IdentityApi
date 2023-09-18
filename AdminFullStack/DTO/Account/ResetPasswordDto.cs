using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace AdminFullStack.DTO.Account
{
    public class ResetPasswordDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6, ErrorMessage = "password must be at least {2},Maximum {1} characters")]
        public string newPassword { get; set; }
    }
}
