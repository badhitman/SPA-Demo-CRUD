////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;

namespace SPADemoCRUD.Models
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
