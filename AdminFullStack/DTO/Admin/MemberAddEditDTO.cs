using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations;

namespace AdminFullStack.DTO.Admin
{
    public class MemberAddEditDTO
    {
        public string Id { get; set; }

        [Required]
        [RegularExpression("^\\w+@[a-zA-Z_]+?\\.[a-zA-Z]{2,3}$", ErrorMessage = "Invalid Email Address")]
        public string UserName { get; set; }
        [Required]
        
        public string FirstName { get; set; }
        [Required]
        
        public string LastName { get; set; }
      
        public string Password { get; set; }
        [Required]
        public string Roles { get; set; }
    }
}
