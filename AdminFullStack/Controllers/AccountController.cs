using AdminFullStack.DTO.Account;
using AdminFullStack.Models;
using AdminFullStack.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AdminFullStack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly JWTServices jwtServices;
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public AccountController(JWTServices jwtServices,SignInManager<User> signInManager,
            UserManager<User> userManager)
        {
            this.jwtServices = jwtServices;
            this.signInManager = signInManager;
            this.userManager = userManager;
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
                    UserName = model.Email.ToLower(),
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
                return Ok("Your Account is Created");
            }
          
        }


        [HttpPost("Login")]
        public async Task<ActionResult<UserDTO>> Login(LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if(user == null)
            {
                return Unauthorized("Invaliid username or password");
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
        #endregion
    }
}
