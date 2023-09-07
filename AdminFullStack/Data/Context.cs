using AdminFullStack.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AdminFullStack.Data
{
    public class Context : IdentityDbContext<User>
    {
        public Context(DbContextOptions<Context> options):base(options)
        {
            
        }

    }
}
