////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SPADemoCRUD.Models.db;

namespace SPADemoCRUD.Models
{
    public class AppDataBaseContext : DbContext
    {
        public DbSet<DepartmentModel> Departments { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<FileStorageModel> FilesStorage { get; set; }
        public DbSet<СonversationModel> Сonversations { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<MessageModel> Messages { get; set; }

        public AppDataBaseContext(DbContextOptions<AppDataBaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileStorageModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<FileStorageModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<UserModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UserModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<DepartmentModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<DepartmentModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<СonversationModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<СonversationModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<MessageModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<MessageModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<NotificationModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<NotificationModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
        }
    }
}
