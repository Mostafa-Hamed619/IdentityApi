using Microsoft.Identity.Client;

namespace AdminFullStack.DTO.Account
{
    public class LoginWithThirdParty
    {
        public string accessToken { get; set; }

        public string userId { get; set; }

        public string providers { get; set; }
    }
}
