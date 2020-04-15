////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using MultiTool;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Демо данные (seeds)
    /// </summary>
    public static class SampleDemoData
    {
        public static void Initialize(AppDataBaseContext context)
        {
            UnitGoodObjectModel[] unitsDemo = new UnitGoodObjectModel[]
            {
                new UnitGoodObjectModel { Name = "шт.", Information = "Штука" },
                new UnitGoodObjectModel { Name = "м.", Information = "Метр" },
                new UnitGoodObjectModel { Name = "гр.", Information = "Грамм" },
                new UnitGoodObjectModel { Name = "уп.5шт.", Information = "Упаковка 5 штук" },
                new UnitGoodObjectModel { Name = "500гр.", Information = "Фасовка 500гр." },
                new UnitGoodObjectModel { Name = "1000гр.", Information = "Фасовка 1000гр." },
                new UnitGoodObjectModel { Name = "300гр.", Information = "Фасовка 300гр." },
                new UnitGoodObjectModel { Name = "кмпл.", Information = "Комплект" }
            };
            if (!context.Units.Any())
            {
                context.Units.AddRange(unitsDemo);
                context.SaveChanges();
            }

            GroupGoodsObjectModel[] groupGoodsDemo = new GroupGoodsObjectModel[]
            {
                new GroupGoodsObjectModel { Name = "Посуда" },
                new GroupGoodsObjectModel { Name = "Одежда/Обувь" },
                new GroupGoodsObjectModel { Name = "Бакалея" }
            };
            if (!context.GroupsGoods.Any())
            {
                context.GroupsGoods.AddRange(groupGoodsDemo);
                context.SaveChanges();
            }

            GoodObjectModel[] goodsDemo = new GoodObjectModel[]
            {
                new GoodObjectModel { Name = "Тарелка", Price = 200, Group = groupGoodsDemo[0], Unit = unitsDemo[3] },
                new GoodObjectModel { Name = "Кружка", Price = 0, Group = groupGoodsDemo[0], Unit = unitsDemo[0] },
                new GoodObjectModel { Name = "Чайник", Price = 2200, Group = groupGoodsDemo[0], Unit = unitsDemo[0] },

                new GoodObjectModel { Name = "Кофта", Price = 3800, Group = groupGoodsDemo[1], Unit = unitsDemo[0] },
                new GoodObjectModel { Name = "Шорты", Price = 2500, Group = groupGoodsDemo[1], Unit = unitsDemo[0] },
                new GoodObjectModel { Name = "Носки", Price = 0, Group = groupGoodsDemo[1], Unit = unitsDemo[7] },
                new GoodObjectModel { Name = "Куртка", Price = 6400, Group = groupGoodsDemo[1], Unit = unitsDemo[0] },
                new GoodObjectModel { Name = "Варежки", Price = 1350, Group = groupGoodsDemo[1], Unit = unitsDemo[7] },
                new GoodObjectModel { Name = "Лента (отрез)", Price = 50, Group = groupGoodsDemo[1], Unit = unitsDemo[1] },

                new GoodObjectModel { Name = "мука", Price = 45, Group = groupGoodsDemo[2], Unit = unitsDemo[4] },
                new GoodObjectModel { Name = "гречка", Price = 87, Group = groupGoodsDemo[2], Unit = unitsDemo[4] },
                new GoodObjectModel { Name = "овсянка", Price = 64, Group = groupGoodsDemo[2], Unit = unitsDemo[6] },
                new GoodObjectModel { Name = "рис", Price = 0, Group = groupGoodsDemo[2], Unit = unitsDemo[4] },
                new GoodObjectModel { Name = "горох", Price = 134, Group = groupGoodsDemo[2], Unit = unitsDemo[5] },
                new GoodObjectModel { Name = "фасоль", Price = 200, Group = groupGoodsDemo[2], Unit = unitsDemo[4] },
                new GoodObjectModel { Name = "чечевица", Price = 87, Group = groupGoodsDemo[2], Unit = unitsDemo[5] },
                new GoodObjectModel { Name = "лапша", Price = 100, Group = groupGoodsDemo[2], Unit = unitsDemo[5] },
                new GoodObjectModel { Name = "кетчуп", Price = 280, Group = groupGoodsDemo[2], Unit = unitsDemo[4] },
                new GoodObjectModel { Name = "майонез", Price = 0, Group = groupGoodsDemo[2], Unit = unitsDemo[6] },
                new GoodObjectModel { Name = "сахар", Price = 76, Group = groupGoodsDemo[2], Unit = unitsDemo[5] },
                new GoodObjectModel { Name = "соль", Price = 64, Group = groupGoodsDemo[2], Unit = unitsDemo[6] },
                new GoodObjectModel { Name = "сода", Price = 165, Group = groupGoodsDemo[2], Unit = unitsDemo[6] }
            };
            if (!context.Goods.Any())
            {
                context.Goods.AddRange(goodsDemo);
                context.SaveChanges();
            }

            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new DepartmentObjectModel { Name = "user", Readonly = true },
                    new DepartmentObjectModel { Name = "Хозяйственный блок" },
                    new DepartmentObjectModel { Name = "Снабженцы" },
                    new DepartmentObjectModel { Name = "Секретариат" },
                    new DepartmentObjectModel { Name = "Производство" },
                    new DepartmentObjectModel { Name = "Бухгалтерия" },
                    new DepartmentObjectModel { Name = "ИТ" });
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                string demoPass = glob_tools.GetHashString("demo");
                context.Users.AddRange(
                    new UserObjectModel { Name = "system", DepartmentId = 1, Role = AccessLevelUserRolesEnum.Auth, Email = "*", Password = "*", LastWebVisit = DateTime.MaxValue, Readonly = true, IsGlobalFavorite = true, Information = "Системная учётная запись" },

                    new UserObjectModel { Name = "Иванова И", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "verified", Password = demoPass },
                    new UserObjectModel { Name = "Петрова П", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo1", Password = demoPass },
                    new UserObjectModel { Name = "Сидоров С", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo2", Password = demoPass },
                    new UserObjectModel { Name = "Ромашкин Р", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo3", Password = demoPass },

                    new UserObjectModel { Name = "Кузнецов К", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "privileged", Password = demoPass },
                    new UserObjectModel { Name = "Зайцев З", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo4", Password = demoPass },
                    new UserObjectModel { Name = "Новиков Н", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo5", Password = demoPass },
                    new UserObjectModel { Name = "Морозов М", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo6", Password = demoPass },

                    new UserObjectModel { Name = "Волков В", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Privileged, Email = "manager", Password = demoPass },
                    new UserObjectModel { Name = "Голубев Г", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Manager, Email = "demo7", Password = demoPass },
                    new UserObjectModel { Name = "Богданов Б", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo8", Password = demoPass },
                    new UserObjectModel { Name = "Фёдоров Ф", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Manager, Email = "demo9", Password = demoPass },

                    new UserObjectModel { Name = "Тарасов Т", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "demo10", Password = demoPass },
                    new UserObjectModel { Name = "Егоров Е", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "demo11", Password = demoPass },
                    new UserObjectModel { Name = "Орлов О", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "admin", Password = demoPass },

                    new UserObjectModel { Name = "Абрамович А", DepartmentId = 7, Role = AccessLevelUserRolesEnum.ROOT, Email = "root", Password = demoPass });
                context.SaveChanges();
            }
            //
            if (!context.WarehousesGoods.Any())
            {
                context.WarehousesGoods.AddRange(new WarehouseGoodObjectModel[]
                {
                    new WarehouseGoodObjectModel(){ Name = "Центральный склад", Information = "demo склад" },
                    new WarehouseGoodObjectModel(){ Name = "К доставке", Information = "demo склад" },
                    new WarehouseGoodObjectModel(){ Name = "Магазин", Information = "demo склад" }
                });
                context.SaveChanges();
            }

            if (!context.ReceiptesGoodsToWarehousesRegisters.Any())
            {
                context.ReceiptesGoodsToWarehousesRegisters.AddRange(new ReceiptToWarehouseDocumentModel[]
                {
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 1, Name = "начальное пополнение: склад 1", Information = "demo поступление на склад", WarehouseReceiptId = 1},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 2, Name = "начальное пополнение: склад 3", Information = "demo поступление на склад", WarehouseReceiptId = 3}
                });
                context.SaveChanges();
                context.GoodMovementDocumentRows.AddRange(new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 1, GoodId = 1, Price = 204, Quantity = 2, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 1, GoodId = 1, Price = 350, Quantity = 3, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 1, GoodId = 1, Price = 410, Quantity = 4, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 1, GoodId = 1, Price = 145, Quantity = 1, UnitId = 3 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 2, GoodId = 1, Price = 676, Quantity = 3, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 2, GoodId = 1, Price = 820, Quantity = 3, UnitId = 1 }
                });
                context.SaveChanges();
            }

            if (!context.MovementTurnoverDeliveryDocuments.Any())
            {
                context.MovementTurnoverDeliveryDocuments.AddRange(new MovementTurnoverDeliveryDocumentModel[]
                {

                });
                context.SaveChanges();
            }

            if (!context.InventoryGoodsBalancesWarehouses.Any())
            {
                context.InventoryGoodsBalancesWarehouses.AddRange(new InventoryGoodBalancesWarehousesAnalyticalModel[]
                {

                });
                context.SaveChanges();
            }

            if (!context.InventoryGoodsBalancesDeliveries.Any())
            {
                context.InventoryGoodsBalancesDeliveries.AddRange(new InventoryGoodBalancesDeliveriesAnalyticalModel[]
                {

                });
                context.SaveChanges();
            }
        }
    }
}
