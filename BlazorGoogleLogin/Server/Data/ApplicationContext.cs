using BlazorGoogleLogin.Server.Model;
using Microsoft.EntityFrameworkCore;

namespace BlazorGoogleLogin.Server.Data
{
    public class ApplicationContext: DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<User> User { get; set; } = default!;
        public DbSet<UserRole> UserRoles { get; set; } = default!;
    }
}
