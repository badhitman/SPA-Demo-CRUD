using Microsoft.EntityFrameworkCore;

namespace SimpleSPA.Models
{
    public class AppDataBaseContext : DbContext
    {
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<UserModel> Users { get; set; }

        public AppDataBaseContext(DbContextOptions<AppDataBaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
