using System.ComponentModel.DataAnnotations;

namespace AdminFullStack.DTO.Account
{
    public class LoginModel
    {
        [Required(ErrorMessage ="User name is Required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "password is Required asshole")]
        public string Password { get; set; }
    }
}
