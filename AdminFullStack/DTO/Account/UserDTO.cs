using Microsoft.Identity.Client;

namespace AdminFullStack.DTO.Account
{
    public class UserDTO
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Jwt { get; set; }
    }
}
