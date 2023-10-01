using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AdminFullStack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        #region Roles
        [HttpGet("public")]
        public IActionResult Public()
        {
            return Ok("Public");
        }

        [HttpGet("Admin-role")]
        [Authorize(Roles ="Admin")]
        public IActionResult Private()
        {
            return Ok("private");
        }
        #endregion
        #region  Policy
        [HttpGet("Admin-Policy")]
        [Authorize(policy:"AdminPolicy")]
        public IActionResult AdminPolicy()
        {
            return Ok("Admin Policy");
        }

        [HttpGet("Manager-Policy")]
        [Authorize(policy:"ManagerPolicy")]
        public IActionResult ManagerPolicy()
        {
            return Ok("Manager Policy");
        }

        [HttpGet("Player-Policy")]
        [Authorize(policy: "PlayerPolicy")]
        public IActionResult PlayerPolicy()
        {
            return Ok("Player Policy");
        }
        #endregion

        #region Claim Policy
        [HttpGet("Admin-email-policy")]
        [Authorize("AdminEmailPolicy")]
        public IActionResult AdminEmailPolicy()
        {
            return Ok("Admin email policy from Claim Types");
        }

        [HttpGet("player-surename-policy")]
        [Authorize("IanSureNamePolicy")]
        public IActionResult NepomniachtchiPolicy()
        {
            return Ok("Ian Nepomniachtchi policy from Claim Types");
        }
        #endregion
    }
}
