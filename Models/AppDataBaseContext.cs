////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using SPADemoCRUD.Models.db;
using SPADemoCRUD.Models.db.delivery;

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

        public DbSet<TelegramBotUpdateModel> TelegramBotUpdates { get; set; }

        public DbSet<BtcTransactionModel> BtcTransactions { get; set; }
        public DbSet<BtcTransactionOutModel> BtcTransactionOuts { get; set; }

        public DbSet<UnitGoodModel> Units { get; set; }
        public DbSet<GoodModel> Goods { get; set; }
        public DbSet<GroupGoodModel> GroupsGoods { get; set; }
        public DbSet<WarehouseGoodModel> WarehousesGoods { get; set; }
        public DbSet<InventoryGoodBalancesWarehousesModel> WarehousesGoodsInventoryBalances { get; set; }
        public DbSet<MovementGoodsWarehousesDocumentModel> MovementsGoodsWarehouses { get; set; }

        public DbSet<DeliveryMethodModel> DeliveryMethods { get; set; }
        public DbSet<DeliveryServiceModel> DeliveryServices { get; set; }

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

            modelBuilder.Entity<TelegramBotUpdateModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TelegramBotUpdateModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<BtcTransactionModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<BtcTransactionModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<BtcTransactionOutModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<BtcTransactionOutModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<MovementGoodsWarehousesDocumentModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<MovementGoodsWarehousesDocumentModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
        }
    }
}
