using AdminFullStack.DTO.Admin;
using AdminFullStack.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminFullStack.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly UserManager<User> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AdminController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [HttpGet("get-members")]
        public async Task<ActionResult<IEnumerable<MemberDto>>> GetMembers()
        {
            var Members = await userManager.Users
                .Where(x => x.UserName != SD.AdminUserName)
                .Select(member => new MemberDto
                {
                    Id = member.Id,
                    UserName = member.UserName,
                    FirstName = member.FirstName,
                    LastName = member.LastName,
                    DateCreated = member.DateCreated,
                    IsLocked = userManager.IsLockedOutAsync(member).GetAwaiter().GetResult(),
                    Role = userManager.GetRolesAsync(member).GetAwaiter().GetResult(),
                }).ToListAsync();

            return Ok(Members);
        }

        [HttpPut("lock-member/{id}")]
        public async Task<IActionResult> LockMember(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) { return NotFound(); }
            if (IsAdminUserId(user.Id))
            {
                return BadRequest(SD.SuperAdminChangeNotAllowed);
            }

            await userManager.SetLockoutEndDateAsync(user, DateTime.UtcNow.AddDays(5));
            return NoContent();
        }

        [HttpPut("unlock-member/{id}")]
        public async Task<IActionResult> UnlockMember(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) { return NotFound(); };
            if (IsAdminUserId(user.Id))
            {
                return BadRequest(SD.SuperAdminChangeNotAllowed);
            }
            await userManager.SetLockoutEndDateAsync(user, null);
            return NoContent();
        }

        [HttpDelete("delete-member/{id}")]
        public async Task<IActionResult> DeleteMember(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null) { return NotFound(); }
            if (IsAdminUserId(id))
            {
                return BadRequest(SD.SuperAdminChangeNotAllowed);
            }
            await userManager.DeleteAsync(user);
            return NoContent();
        }

        [HttpGet("get-roles")]
        public async Task<ActionResult<string[]>> GetRoles()
        {
            var roles = await roleManager.Roles.Select(role => role.Name).ToListAsync();
            return Ok(roles);
        }

        [HttpGet("get-member/{id}")]
        public async Task<ActionResult<MemberAddEditDTO>> GetMember(string id)
        {
            var member = await userManager.Users.Where(x => x.Id != SD.AdminUserName && x.Id == id)
                .Select(m => new MemberAddEditDTO
                {
                    
                    Roles = string.Join(",", userManager.GetRolesAsync(m).GetAwaiter().GetResult()),
                    UserName = m.UserName,
                    FirstName = m.FirstName,
                    LastName = m.LastName,
                    Id = m.Id
                }).FirstOrDefaultAsync();

            return Ok(member);
        }

        [HttpPost("add-edit-member")]
        public async Task<IActionResult> AddEditMember(MemberAddEditDTO model)
        {
            User user;
            if(string.IsNullOrEmpty(model.Id))
            {
                //Adding a new Member
                if(string.IsNullOrEmpty(model.Password) || model.Password.Length < 6)
                {
                    ModelState.AddModelError("errors", "Password must be at least 6 characters");
                    return BadRequest(ModelState);
                }

                user = new User
                {
                    Email = model.UserName.ToLower(),
                    UserName = model.UserName.ToLower(),
                    FirstName = model.FirstName.ToLower(),
                    LastName = model.LastName.ToLower(),
                    EmailConfirmed = true
                };
                var result = await userManager.CreateAsync(user,model.Password);
                if(!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }
            }
            else
            {
                //Editing an existing Member.
                if(string.IsNullOrEmpty(model.Password))
                {
                    if(model.Password.Length < 6)
                    {
                        ModelState.AddModelError("error", "Password must be at least 6 characters");
                        return BadRequest(ModelState);
                    }
                }
                if(IsAdminUserId(model.Id))
                {
                    return BadRequest(SD.SuperAdminChangeNotAllowed);
                }
                user = await userManager.FindByIdAsync(model.Id);
                if(user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName.ToLower();
                user.LastName = model.LastName.ToLower();
                user.UserName = model.UserName.ToLower();

                if (!string.IsNullOrEmpty(model.Password))
                {
                    await userManager.RemovePasswordAsync(user);
                    await userManager.AddPasswordAsync(user,model.Password);
                }
            }
            var roles = await userManager.GetRolesAsync(user);

            //removing user's existing roles
            await userManager.RemoveFromRolesAsync(user, roles);

            foreach(var role in model.Roles.Split(",").ToArray())
            {
                var roleToAdd = await roleManager.Roles.FirstOrDefaultAsync(r=>r.Name == role);

                if(roleToAdd != null)
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }

            if (string.IsNullOrEmpty(model.Id))
            {
                return Ok(new JsonResult(new
                {
                    title = "Member Created",
                    message = $"{model.UserName} is created successfully"
                }));
            }
            else
            {
                return Ok(new JsonResult(new
                {
                    title = "Member Edited",
                    message = $"{model.UserName} is edited successfully"
                }));
            }
            return NoContent();
        }
        private bool IsAdminUserId(string userId)
        {
            return userManager.FindByIdAsync(userId).GetAwaiter().GetResult().UserName.Equals(SD.AdminUserName);
        }
    }
}
