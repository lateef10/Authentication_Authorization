using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AuthenticationAuthorization.Model
{
    public class ApplicationDbContextIdentity : IdentityDbContext
    {
        public ApplicationDbContextIdentity(DbContextOptions<ApplicationDbContextIdentity> options) : base(options)
        {

        }
    }
}
