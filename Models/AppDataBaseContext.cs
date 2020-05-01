////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SPADemoCRUD.Models
{
    public class AppDataBaseContext : DbContext
    {
        /// <summary>
        /// Маркеры (пользовательские) объектов признака: [избранное]
        /// </summary>
        public DbSet<UserFavoriteMarkModel> UserFavoriteLocators { get; set; }

        /// <summary>
        /// документы движения номенклатуры
        /// </summary>
        public DbSet<BodyGoodMovementDocumentModel> GoodMovementDocuments { get; set; }
        /// <summary>
        /// регистры движений номенклатуры
        /// </summary>
        public DbSet<RowGoodMovementRegisterModel> GoodMovementDocumentRows { get; set; }

        /// <summary>
        /// Контекстные файловые (прикреплённых) вложения для поддерживающих такую функцию объектов
        /// </summary>
        public DbSet<ObjectFileRegisterRowModel> FileRegisteringObjectRows { get; set; }

        /// <summary>
        /// отделы/департаменты
        /// </summary>
        public DbSet<DepartmentObjectModel> Departments { get; set; }
        /// <summary>
        /// пользователи
        /// </summary>
        public DbSet<UserObjectModel> Users { get; set; }

        /// <summary>
        /// файлы, данные о которых есть в БД (хранимые файлы)
        /// </summary>
        public DbSet<FileStorageObjectModel> FilesStorage { get; set; }

        /// <summary>
        /// переписки
        /// </summary>
        public DbSet<СonversationDocumentModel> Сonversations { get; set; }
        /// <summary>
        /// уведомления
        /// </summary>
        public DbSet<NotificationObjectModel> Notifications { get; set; }
        /// <summary>
        /// сообщения
        /// </summary>
        public DbSet<MessageObjectModel> Messages { get; set; }

        /// <summary>
        /// входящие данные
        /// </summary>
        public DbSet<TelegramBotUpdateObjectModel> TelegramBotUpdates { get; set; }

        /// <summary>
        /// BTC транзакции
        /// </summary>
        public DbSet<BtcTransactionObjectModel> BtcTransactions { get; set; }
        /// <summary>
        /// Выходы из транзакций
        /// </summary>
        public DbSet<BtcTransactionOutObjectModel> BtcTransactionOuts { get; set; }

        /// <summary>
        /// Группы товаров
        /// </summary>
        public DbSet<GroupGoodsObjectModel> GroupsGoods { get; set; }
        /// <summary>
        /// Единицы измерения
        /// </summary>
        public DbSet<UnitGoodObjectModel> Units { get; set; }
        /// <summary>
        /// Номенклатура
        /// </summary>
        public DbSet<GoodObjectModel> Goods { get; set; }
        /// <summary>
        /// Склады
        /// </summary>
        public DbSet<WarehouseObjectModel> Warehouses { get; set; }
        /// <summary>
        /// Остатки в разрезах аналитики
        /// </summary>
        public DbSet<InventoryBalancesWarehousesAnalyticalModel> InventoryGoodsBalancesWarehouses { get; set; }

        /// <summary>
        /// Складские документы
        /// </summary>
        public DbSet<WarehouseDocumentsModel> WarehouseDocuments { get; set; }
        /// <summary>
        /// Документы поступления на склад
        /// </summary>
        public DbSet<ReceiptToWarehouseDocumentModel> ReceiptesGoodsToWarehousesDocuments { get; set; }
        /// <summary>
        /// Документы внутреннего перемещения со склада на склад
        /// </summary>
        public DbSet<InternalDisplacementWarehouseDocumentModel> InternalDisplacementWarehouseDocuments { get; set; }

        /// <summary>
        /// Методы доставки
        /// </summary>
        public DbSet<DeliveryMethodObjectModel> DeliveryMethods { get; set; }
        /// <summary>
        /// Службы доставки (курьерские службы)
        /// </summary>
        public DbSet<DeliveryServiceObjectModel> DeliveryServices { get; set; }

        /// <summary>
        /// документы движения номенклатуре в доставке
        /// </summary>
        public DbSet<MovementTurnoverDeliveryDocumentModel> MovementTurnoverDeliveryDocuments { get; set; }
        /// <summary>
        /// остатки номенклатуры в доставке в разрезе аналитики
        /// </summary>
        public DbSet<InventoryBalancesDeliveriesAnalyticalModel> InventoryGoodsBalancesDeliveries { get; set; }

        public AppDataBaseContext(DbContextOptions<AppDataBaseContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileStorageObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<FileStorageObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<UserObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<UserObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<DepartmentObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<DepartmentObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<СonversationDocumentModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<СonversationDocumentModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<MessageObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<MessageObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<NotificationObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<NotificationObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<TelegramBotUpdateObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<TelegramBotUpdateObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<BtcTransactionObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<BtcTransactionObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<BtcTransactionOutObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<BtcTransactionOutObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<BodyGoodMovementDocumentModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<BodyGoodMovementDocumentModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<ObjectFileRegisterRowModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<ObjectFileRegisterRowModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<GoodObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<GoodObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });

            modelBuilder.Entity<GroupGoodsObjectModel>().Property(u => u.DateCreate).HasDefaultValueSql("GETDATE()");
            modelBuilder.Entity<GroupGoodsObjectModel>(builder =>
            {
                builder.Property(e => e.DateCreate).Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
            });
        }
    }
}
