using System.ComponentModel.DataAnnotations;

namespace AdminFullStack.DTO.Account
{
    public class RegisterWithExternal
    {
        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "First name must be at least {2} characters,Maximum {1} characters")]
        public string firstName { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 3, ErrorMessage = "Last name must be at least {2} characters,Maximum {1} characters")]
        public string lastName { get; set; }

        [Required]
        public string accessToken { get; set; }

        [Required]
        public string provider { get; set; }

        [Required]
        public string userId { get; set; }
    }
}
