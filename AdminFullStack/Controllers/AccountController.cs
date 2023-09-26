using AdminFullStack.DTO.Account;
using AdminFullStack.Models;
using AdminFullStack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using Microsoft.Win32;
using System.Reflection.Metadata.Ecma335;
using Google.Apis.Auth;

namespace AdminFullStack.Controllers
{
    [Microsoft.AspNetCore.Mvc.Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTServices jwtServices;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;
        private readonly EmailServices emailServices;
        private readonly IConfiguration config;
        private readonly HttpClient _facebookHttpClient;

        public AccountController(JWTServices jwtServices,SignInManager<User> signInManager,
            UserManager<User> userManager,EmailServices emailServices,IConfiguration config)
        {
            this.jwtServices = jwtServices;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.emailServices = emailServices;
            this.config = config;
            _facebookHttpClient = new HttpClient
            {
                BaseAddress =new Uri("https://graph.facebook.com")
            };
        }
        [Authorize]
        [HttpGet("refresh-user-token")]
        public async Task<ActionResult<UserDTO>> RefreshToken()
        {
            var user = await userManager.FindByNameAsync(User.FindFirst(ClaimTypes.Email)?.Value);
            return CreateApplicationUserDto(user); 
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO model)
        {
            if(await CheckIfUserExists(model.Email))
            {
                return BadRequest($"The Email {model.Email},already used. Use another email address");
            }
            else
            {
              
                var user = new User
                {
                    FirstName = model.FirstName.ToLower(),
                    LastName = model.LastName.ToLower(),
                    Email = model.Email.ToLower(),
                    UserName = model.Email.ToLower()
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                try
                {
                    if(await SendConfirmEmailAsync(user))
                    {
                        return Ok(new JsonResult (new {title = "Account Created",message ="\"Your account has been created," +
                            "Please confirm your email address"}));
                    }
                    return BadRequest("Failed to send email. please contact the admin");
                }
                catch(Exception )
                {
                    return BadRequest("Failed to send email. please contact the admin");
                }
            }
          
        }


        [HttpPost("register-with-third-party")]
        public async Task<ActionResult<UserDTO>> RegisterWithThirdParty(RegisterWithExternal model)
        {
            if (model.provider.Equals(SD.Facebook))
            {
                try
                {
                    if (!FacebookValidationAsync(model.accessToken, model.userId).GetAwaiter().GetResult())
                    {
                        return Unauthorized("Unable to register with facebook");
                    }
                }catch(Exception)
                {
                    return Unauthorized("Unable to register with facebook");
                }
            }else if (model.provider.Equals(SD.Google))
            {
                try
                {
                    if (!GoogleValidatedAsync(model.accessToken, model.userId).GetAwaiter().GetResult())
                    {
                        return Unauthorized("Unable to register with google");
                    }
                }
                catch (Exception)
                {
                    return Unauthorized("Unable to register with google");
                }
            }
            else
            {
                return BadRequest("Invalid provider");
            }
            var user = await userManager.FindByNameAsync(model.userId);
            if(user != null) { return BadRequest(string.Format("This user name is already existing.You can Login sir", model.provider)); }

            var userToAdd = new User()
            {
                UserName = model.userId,
                FirstName = model.firstName,
                LastName = model.lastName,
                Provider = model.provider
            };

            var result = await userManager.CreateAsync(userToAdd);
            if (!result.Succeeded) return BadRequest(result.Errors);

            return CreateApplicationUserDto(userToAdd);
        }
        

        [HttpPut("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if (user == null) { return Unauthorized("this email address has not been registered yet"); }
            if (user.EmailConfirmed == true) { return BadRequest("your email is confirmed already.you can login"); }

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await userManager.ConfirmEmailAsync(user, decodedToken);

                if (result.Succeeded)
                {
                    return Ok(new JsonResult(new { title = "Email confirmed", message = "your email is confirmed.you can login now" }));
                } 
                return BadRequest("Invalid token. Please try again.");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token. Please try again.");
            }
        }

        [HttpPost("resend-email-confirmation-link/{email}")]
        public async Task<IActionResult> ResendEmailConfirmationLink(string email)
        {
            if(String.IsNullOrEmpty(email)) { return BadRequest("Please write the email address"); }
            var user = await userManager.FindByEmailAsync(email);
            if(user == null) { return Unauthorized("this email has not been registerd yet."); }
            if(user.EmailConfirmed == true) { return BadRequest("this email is already confirmed. you can login now."); }

            try
            {
                if(await SendConfirmEmailAsync(user) == true)
                {
                    return Ok(new JsonResult(new{ title="Resending Confirmation Link", message = "The confirmation link resended." +
                        "please confirm your email address" }));
                }
                return BadRequest("Invalid token.Please try another time.");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token.Please try another time.");
            }
        }

        [HttpPost("forgot-username-or-password/{email}")]
        public async Task<IActionResult> ForgotPasswordOrUsername(string email)
        {
            if (String.IsNullOrEmpty(email)) { return BadRequest("Please write the email address"); }
            var user = await userManager.FindByEmailAsync(email);
            if (user == null) { return Unauthorized("this email has not been registerd yet."); }
            if (user.EmailConfirmed == false) { return BadRequest("Please confirm your email address first."); }

            try
            {
                if (await SendForgotUsernameOrPasswordEmail(user))
                {
                    return Ok(new JsonResult(new { Title = "Forgot Username Or Password Email Sent", message = "Please check your email" }));
                }
                return BadRequest("Failed to send email.Please try another time.");
            }
            catch (Exception)
            {
                return BadRequest("Failed to send email.Please try another time.");
            }
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto model)
        {
            var user = await userManager.FindByEmailAsync(model.Email);
            if(user == null) { return Unauthorized("this email has not been register yet"); }
            if(user.EmailConfirmed == false) { return BadRequest("Please confirm your email address first"); }

            try
            {
                var decodedTokenBytes = WebEncoders.Base64UrlDecode(model.Token);
                var decodedToken = Encoding.UTF8.GetString(decodedTokenBytes);

                var result = await userManager.ResetPasswordAsync(user, decodedToken, model.newPassword);

                if(result.Succeeded)
                { 
                    return Ok(new JsonResult(new {Title="Password Reset Succedded" ,Message="Your password has been resetted"}));
                }
                return BadRequest("Invalid token. Please try another time.");
            }
            catch (Exception)
            {
                return BadRequest("Invalid token. Please try another time.");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            if(user.EmailConfirmed == false)
            {
                return Unauthorized("Please confirm your email");
            }

            var result = await signInManager.CheckPasswordSignInAsync(user, model.Password,false);
            if(!result.Succeeded)
            {
                return Unauthorized("Invalid username or password");
            }
            return CreateApplicationUserDto(user);
        }

        [HttpPost("login-with-third-party")]
        public async Task<ActionResult<UserDTO>> LoginWithThirdParty(LoginWithThirdParty model)
        {
            if (model.providers.Equals(SD.Facebook))
            {
                try
                {
                    if(!FacebookValidationAsync(model.accessToken, model.userId).GetAwaiter().GetResult())
                    {
                        return Unauthorized("Unable to login with facebook account");
                    }
                }
                catch (Exception)
                {
                    return Unauthorized("Unable to login with facebook account");
                }
            }
            else if (model.providers.Equals(SD.Google))
            {

            }
            else
            {
                return BadRequest("Invalid provider");
            }
            var user = await userManager.Users.FirstOrDefaultAsync(x => x.UserName == model.userId && x.Provider == model.providers);
            if( user == null) { return Unauthorized("Unable to find your account"); }
            return CreateApplicationUserDto(user);

        }
        #region Private Helper Methods
        private UserDTO CreateApplicationUserDto(User user)
        {
            return new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Jwt = jwtServices.CreateJWT(user),
            };
        }

        private async Task<bool> CheckIfUserExists(string email)
        {
            return await userManager.Users.AnyAsync(x=>x.Email == email.ToLower());
        }

        private async Task<bool> SendConfirmEmailAsync(User user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{config["JWT:ClientUrl"]}/{config["Email:ConfirmEmailPath"]}?token={token}&email={user.Email}";

            var body = $"<p>Hello : {user.FirstName} {user.LastName}</p>" +
                "<p>Please confirm your email address by clicking this following link</p>" +
                $"<p><a href=\"{url}\">Click here </a></p>" +
                "<p>Thank you;</p>" +
                $"<br>{config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, "Confirm your email", body);
            return await emailServices.SendEmailAsync(emailSend);
        }

        private async Task<bool> SendForgotUsernameOrPasswordEmail(User user)
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            var url = $"{config["JWT:ClientUrl"]}/{config["Email:ResetPasswordpath"]}?token={token}&email={user.Email}";


            var body = $"<p>Hello : {user.FirstName} {user.LastName}</p>" +
                $"<p>Username :{user.UserName}</p>" +
                "<p>In order to reset your password please click the following link.</p>"+
                $"<p><a href=\"{url}\">Click here </a></p>" +
                "<p>Thank you;</p>" +
                $"<br>{config["Email:ApplicationName"]}";

            var emailSend = new EmailSendDto(user.Email, " Reset your password ", body);
            return await emailServices.SendEmailAsync(emailSend);
        }

        private async Task<bool> FacebookValidationAsync(string accessToken,string userId)
        {
            var facebookKeys = config["Facebook:AppApi"] + "|" + config["Facebook:SecretKey"];
            var fbResult = await _facebookHttpClient.GetFromJsonAsync<FacebookDtoResult>($"debug_token?input_token={accessToken}&access_token={facebookKeys}");

            if(fbResult == null || fbResult.Data.Is_Valid == false || fbResult.Data.User_id !=userId)
            {
                return false;
            }
            return true;
        }

        private async Task<bool> GoogleValidatedAsync(string accessToken,string userId)
        {
            var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);

            if (!payload.Audience.Equals(config["google:ClientId"]))
            {
                return false;
            }
            if(!payload.Issuer.Equals("accounts.google.com") && !payload.Issuer.Equals("https://accounts.google.com"))
            {
                return false;
            }

            if(payload.ExpirationTimeSeconds == null)
            {
                return false; 
            }

            DateTime now = DateTime.Now.ToUniversalTime();
            DateTime expirationDate = DateTimeOffset.FromUnixTimeSeconds((long)payload.ExpirationTimeSeconds).DateTime;
            if(now > expirationDate)
            {
                return false;
            }
            userId = payload.Subject;
            if (!payload.Subject.Equals(userId))
            {
                return false;
            }

            return true;
        }
        #endregion
    }
}
