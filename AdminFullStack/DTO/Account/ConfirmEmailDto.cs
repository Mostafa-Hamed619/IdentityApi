using System.ComponentModel.DataAnnotations;

namespace AdminFullStack.DTO.Account
{
    public class ConfirmEmailDto
    {
        [Required]
        public string token { get; set; }

        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid Email Address")]
        public string Email { get; set; }

    }
}
