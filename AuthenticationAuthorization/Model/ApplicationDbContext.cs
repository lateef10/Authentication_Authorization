using Microsoft.EntityFrameworkCore;

namespace AuthenticationAuthorization.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<UserProp> userProps { get; set; }
        public DbSet<RoleProp> rolesProps{ get; set; }
    }
}
