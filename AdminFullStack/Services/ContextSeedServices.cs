using AdminFullStack.Data;
using Microsoft.AspNetCore.Identity;
using AdminFullStack.Models;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;

namespace AdminFullStack.Services
{
    public class ContextSeedServices
    {
        private readonly Context context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ContextSeedServices(Context context, UserManager<User> userManager,RoleManager<IdentityRole> roleManager)
        {
            this.context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task InitializeContextAsync()
        {
            if (context.Database.GetPendingMigrationsAsync().GetAwaiter().GetResult().Count() > 0)
            {
                await context.Database.MigrateAsync();
            }

            if (!_roleManager.Roles.Any())
            {
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.AdminRole });
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.ManagerRole });
                await _roleManager.CreateAsync(new IdentityRole { Name = SD.PlayerRole });
            }

            if (!_userManager.Users.Any())
            {
                //Adding The Admin
                var Admin = new User
                {
                    FirstName = "Mostafa",
                    LastName = "Seffy",
                    UserName = SD.AdminUserName,
                    Email = SD.AdminUserName,
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(Admin, "Mos@21");
                await _userManager.AddToRolesAsync(Admin, new[] {SD.AdminRole, SD.ManagerRole, SD.PlayerRole});
                await _userManager.AddClaimsAsync(Admin, new Claim[]
                {
                    new Claim(ClaimTypes.Email,Admin.Email),
                    new Claim(ClaimTypes.Surname, Admin.LastName),
                    
                });

                //Adding The Manager
                var Manager = new User
                {
                    FirstName = "Ian",
                    LastName = "Nepomniachtchi",
                    UserName = "Ian@gmail.com",
                    Email = "Ian@gmail.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(Manager, "Ian@21");
                await _userManager.AddToRoleAsync(Manager, SD.ManagerRole);
                await _userManager.AddClaimsAsync(Manager, new Claim[]
                {
                    new Claim(ClaimTypes.Email,Manager.Email),
                    new Claim(ClaimTypes.Surname, Manager.LastName),

                });

                //Adding The Player
                var Player = new User
                {
                    FirstName = "Alexander",
                    LastName = "Grischuk",
                    UserName = "Alexander@gmail.com",
                    Email = "Alexander@gmail.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(Player, "Alexander@21");
                await _userManager.AddToRoleAsync(Player, SD.PlayerRole);
                await _userManager.AddClaimsAsync(Player, new Claim[]
                {
                    new Claim(ClaimTypes.Email,Player.Email),
                    new Claim(ClaimTypes.Surname, Player.LastName),

                });

                //Adding Another Player
                //Adding The Player
                var Player2 = new User
                {
                    FirstName = "Fabiano",
                    LastName = "Caruana",
                    UserName = "Fabiano@gmail.com",
                    Email = "Fabiano@gmail.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(Player2, "Fabiano@21");
                await _userManager.AddToRoleAsync(Player2, SD.PlayerRole);
                await _userManager.AddClaimsAsync(Player2, new Claim[]
                {
                    new Claim(ClaimTypes.Email,Player2.Email),
                    new Claim(ClaimTypes.Surname, Player2.LastName),

                });

                var VipPlayer = new User
                {
                    FirstName = "Qasem",
                    LastName = "Seffy",
                    UserName = "QasemSeffy@gmail.com",
                    Email = "QasemSeffy@gmail.com",
                    EmailConfirmed = true
                };
                await _userManager.CreateAsync(VipPlayer, "Qasem@21");
                await _userManager.AddToRoleAsync(VipPlayer, SD.PlayerRole);
                await _userManager.AddClaimsAsync(VipPlayer, new Claim[]
                {
                    new Claim(ClaimTypes.Email,VipPlayer.Email),
                    new Claim(ClaimTypes.Surname, VipPlayer.LastName),

                });
            }
        }
    }
}
