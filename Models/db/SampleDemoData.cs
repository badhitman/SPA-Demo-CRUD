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
            #region единицы измерения unitsDemo х8
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
            #endregion

            #region группы номенклатуры groupGoodsDemo x3
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
            #endregion

            #region номенклатура goodsDemo x22
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
            #endregion

            #region пользователи x17 / департаменты x7
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new DepartmentObjectModel { Name = "user", isReadonly = true },
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
                    new UserObjectModel { Name = "system", DepartmentId = 1, Role = AccessLevelUserRolesEnum.Auth, Email = "*", Password = "*", LastWebVisit = DateTime.MaxValue, isReadonly = true, isGlobalFavorite = true, Information = "Системная учётная запись" },

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
            #endregion

            #region склады количественного учёта номенклатуры warehousesDemo x3            
            WarehouseObjectModel[] warehousesDemo = new WarehouseObjectModel[]
                {
                    new WarehouseObjectModel(){ Name = "Центральный склад" },
                    new WarehouseObjectModel(){ Name = "К доставке" },
                    new WarehouseObjectModel(){ Name = "Магазин" }
                };
            if (!context.Warehouses.Any())
            {
                context.Warehouses.AddRange(warehousesDemo);
                context.SaveChanges();
            }
            #endregion

            #region документы поступления на склад x14
            if (!context.ReceiptesGoodsToWarehousesDocuments.Any())
            {
                ReceiptToWarehouseDocumentModel[] warehouseReceipts = new ReceiptToWarehouseDocumentModel[]
                {
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 3, Name = "demo поступление на склад 1", Information = "demo пополнение 1. склад: " + warehousesDemo[2].Name, Warehouse = warehousesDemo[2]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 2, Name = "demo поступление на склад 2", Information = "demo пополнение 2. склад: " + warehousesDemo[0].Name, Warehouse = warehousesDemo[0]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 1, Name = "demo поступление на склад 3", Information = "demo пополнение 3. склад: " + warehousesDemo[1].Name, Warehouse = warehousesDemo[1]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 1, Name = "demo поступление на склад 4", Information = "demo пополнение 4. склад: " + warehousesDemo[2].Name, Warehouse = warehousesDemo[2]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 3, Name = "demo поступление на склад 5", Information = "demo пополнение 5. склад: " + warehousesDemo[1].Name, Warehouse = warehousesDemo[1]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 1, Name = "demo поступление на склад 6", Information = "demo пополнение 6. склад: " + warehousesDemo[0].Name, Warehouse = warehousesDemo[0]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 2, Name = "demo поступление на склад 7", Information = "demo пополнение 7. склад: " + warehousesDemo[1].Name, Warehouse = warehousesDemo[1]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 3, Name = "demo поступление на склад 8", Information = "demo пополнение 8. склад: " + warehousesDemo[2].Name, Warehouse = warehousesDemo[2]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 2, Name = "demo поступление на склад 9", Information = "demo пополнение 9. склад: " + warehousesDemo[0].Name, Warehouse = warehousesDemo[0]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 1, Name = "demo поступление на склад 10", Information = "demo пополнение 10. склад: " + warehousesDemo[0].Name, Warehouse = warehousesDemo[0]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 3, Name = "demo поступление на склад 11", Information = "demo пополнение 11. склад: " + warehousesDemo[2].Name, Warehouse = warehousesDemo[2]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 2, Name = "demo поступление на склад 12", Information = "demo пополнение 12. склад: " + warehousesDemo[2].Name, Warehouse = warehousesDemo[2]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 1, Name = "demo поступление на склад 13", Information = "demo пополнение 13. склад: " + warehousesDemo[1].Name, Warehouse = warehousesDemo[1]},
                    new ReceiptToWarehouseDocumentModel(){AuthorId = 2, Name = "demo поступление на склад 14", Information = "demo пополнение 14. склад: " + warehousesDemo[1].Name, Warehouse = warehousesDemo[1]}
                };
                context.ReceiptesGoodsToWarehousesDocuments.AddRange(warehouseReceipts);
                context.SaveChanges();
                //context.GoodMovementDocumentRows.AddRange(new RowGoodMovementRegisterModel[] // goodsDemo[0 7 19]
                //{
                RowGoodMovementRegisterModel[] rows = new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 1, GoodId = goodsDemo[0].Id, Quantity = 22, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 1, GoodId = goodsDemo[7].Id, Quantity = 28, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 1, GoodId = goodsDemo[17].Id, Quantity = 14, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 2, GoodId = goodsDemo[19].Id, Quantity = 25, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 2, GoodId = goodsDemo[0].Id, Quantity = 23, UnitId = 3 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 2, GoodId = goodsDemo[7].Id, Quantity = 25, UnitId = 1 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 3, GoodId = goodsDemo[7].Id, Quantity = 13, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 3, GoodId = goodsDemo[0].Id, Quantity = 24, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 3, GoodId = goodsDemo[19].Id, Quantity = 12, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 4, GoodId = goodsDemo[7].Id, Quantity = 23, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 4, GoodId = goodsDemo[19].Id, Quantity = 25, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 4, GoodId = goodsDemo[7].Id, Quantity = 12, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 5, GoodId = goodsDemo[0].Id, Quantity = 23, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 5, GoodId = goodsDemo[7].Id, Quantity = 25, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 5, GoodId = goodsDemo[19].Id, Quantity = 12, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 6, GoodId = goodsDemo[19].Id, Quantity = 23, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 6, GoodId = goodsDemo[0].Id, Quantity = 25, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 6, GoodId = goodsDemo[7].Id, Quantity = 12, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 7, GoodId = goodsDemo[19].Id, Quantity = 23, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 7, GoodId = goodsDemo[0].Id, Quantity = 25, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 7, GoodId = goodsDemo[7].Id, Quantity = 12, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 8, GoodId = goodsDemo[0].Id, Quantity = 23, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 8, GoodId = goodsDemo[7].Id, Quantity = 25, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 8, GoodId = goodsDemo[19].Id, Quantity = 12, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 9, GoodId = goodsDemo[7].Id, Quantity = 23, UnitId = 3 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 9, GoodId = goodsDemo[0].Id, Quantity = 25, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 9, GoodId = goodsDemo[19].Id, Quantity = 12, UnitId = 1 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 10, GoodId = goodsDemo[0].Id, Quantity = 23, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 10, GoodId = goodsDemo[19].Id, Quantity = 25, UnitId = 3 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 10, GoodId = goodsDemo[7].Id, Quantity = 12, UnitId = 2 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 11, GoodId = goodsDemo[0].Id, Quantity = 23, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 11, GoodId = goodsDemo[7].Id, Quantity = 25, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 11, GoodId = goodsDemo[19].Id, Quantity = 12, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 12, GoodId = goodsDemo[7].Id, Quantity = 23, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 12, GoodId = goodsDemo[19].Id, Quantity = 25, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 12, GoodId = goodsDemo[0].Id, Quantity = 12, UnitId = 3 },

                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 13, GoodId = goodsDemo[0].Id, Quantity = 23, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 13, GoodId = goodsDemo[19].Id, Quantity = 25, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 13, GoodId = goodsDemo[7].Id, Quantity = 12, UnitId = 3 }
                };

                warehouseReceipts[0].RowsDocument = new List<RowGoodMovementRegisterModel>(new RowGoodMovementRegisterModel[] {
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 14, GoodId = goodsDemo[7].Id, Quantity = 23, UnitId = 3 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 14, GoodId = goodsDemo[0].Id, Quantity = 25, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ BodyDocumentId = 14, GoodId = goodsDemo[19].Id, Quantity = 12, UnitId = 1 }});

                //});
                context.GoodMovementDocumentRows.AddRange(rows);
                context.SaveChanges();
            }
            #endregion

            #region документы внутреннего перемещение номенклатуры (между складами) x7
            if (!context.InternalDisplacementWarehouseDocuments.Any())
            {
                InternalDisplacementWarehouseDocumentModel[] internalDisplacements = new InternalDisplacementWarehouseDocumentModel[]
                {
                    new InternalDisplacementWarehouseDocumentModel(){ WarehouseDebitingId = warehousesDemo[0].Id, WarehouseId = warehousesDemo[1].Id, AuthorId = 1, Name = "demo перемещение 1", Information = $"demo внутреннее перемещение 1. [{warehousesDemo[0].Name}] -> [{warehousesDemo[1].Name}]" },
                    new InternalDisplacementWarehouseDocumentModel(){ WarehouseDebitingId = warehousesDemo[1].Id, WarehouseId = warehousesDemo[2].Id, AuthorId = 2, Name = "demo перемещение 2", Information = $"demo внутреннее перемещение 2. [{warehousesDemo[1].Name}] -> [{warehousesDemo[2].Name}]" },
                    new InternalDisplacementWarehouseDocumentModel(){ WarehouseDebitingId = warehousesDemo[2].Id, WarehouseId = warehousesDemo[0].Id, AuthorId = 3, Name = "demo перемещение 3", Information = $"demo внутреннее перемещение 3. [{warehousesDemo[2].Name}] -> [{warehousesDemo[0].Name}]" },
                    new InternalDisplacementWarehouseDocumentModel(){ WarehouseDebitingId = warehousesDemo[0].Id, WarehouseId = warehousesDemo[1].Id, AuthorId = 1, Name = "demo перемещение 4", Information = $"demo внутреннее перемещение 4. [{warehousesDemo[0].Name}] -> [{warehousesDemo[1].Name}]" },
                    new InternalDisplacementWarehouseDocumentModel(){ WarehouseDebitingId = warehousesDemo[1].Id, WarehouseId = warehousesDemo[2].Id, AuthorId = 3, Name = "demo перемещение 5", Information = $"demo внутреннее перемещение 5. [{warehousesDemo[1].Name}] -> [{warehousesDemo[2].Name}]" },
                    new InternalDisplacementWarehouseDocumentModel(){ WarehouseDebitingId = warehousesDemo[2].Id, WarehouseId = warehousesDemo[0].Id, AuthorId = 2, Name = "demo перемещение 6", Information = $"demo внутреннее перемещение 6. [{warehousesDemo[2].Name}] -> [{warehousesDemo[0].Name}]" },
                    new InternalDisplacementWarehouseDocumentModel(){ WarehouseDebitingId = warehousesDemo[0].Id, WarehouseId = warehousesDemo[1].Id, AuthorId = 1, Name = "demo перемещение 7", Information = $"demo внутреннее перемещение 7. [{warehousesDemo[0].Name}] -> [{warehousesDemo[1].Name}]" }
                };

                internalDisplacements[0].RowsDocument = new List<RowGoodMovementRegisterModel>(new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ GoodId = 1, Quantity = 1, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ GoodId = 2, Quantity = 2, UnitId = 2 }
                });
                internalDisplacements[1].RowsDocument = new List<RowGoodMovementRegisterModel>(new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ GoodId = 2, Quantity = 3, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ GoodId = 1, Quantity = 2, UnitId = 1 }
                });
                internalDisplacements[2].RowsDocument = new List<RowGoodMovementRegisterModel>(new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ GoodId = 1, Quantity = 4, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ GoodId = 2, Quantity = 3, UnitId = 2 }
                });
                internalDisplacements[3].RowsDocument = new List<RowGoodMovementRegisterModel>(new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ GoodId = 2, Quantity = 3, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ GoodId = 1, Quantity = 2, UnitId = 1 }
                });
                internalDisplacements[4].RowsDocument = new List<RowGoodMovementRegisterModel>(new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ GoodId = 1, Quantity = 2, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ GoodId = 2, Quantity = 5, UnitId = 2 }
                });
                internalDisplacements[5].RowsDocument = new List<RowGoodMovementRegisterModel>(new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ GoodId = 2, Quantity = 3, UnitId = 2 },
                    new RowGoodMovementRegisterModel(){ GoodId = 1, Quantity = 2, UnitId = 1 }
                });
                internalDisplacements[6].RowsDocument = new List<RowGoodMovementRegisterModel>(new RowGoodMovementRegisterModel[]
                {
                    new RowGoodMovementRegisterModel(){ GoodId = 1, Quantity = 2, UnitId = 1 },
                    new RowGoodMovementRegisterModel(){ GoodId = 2, Quantity = 3, UnitId = 2 }
                });

                context.InternalDisplacementWarehouseDocuments.AddRange(internalDisplacements);
                context.SaveChanges();

            }
            #endregion

            #region остатки номенклатуры в контексте складов x
            if (!context.InventoryGoodsBalancesWarehouses.Any())
            {
                context.InventoryGoodsBalancesWarehouses.AddRange(new InventoryBalancesWarehousesAnalyticalModel[]
                {

                });
                context.SaveChanges();
            }
            #endregion

            #region документы отгрузки номенклатуры в доставку x
            if (!context.MovementTurnoverDeliveryDocuments.Any())
            {
                context.MovementTurnoverDeliveryDocuments.AddRange(new MovementTurnoverDeliveryDocumentModel[]
                {

                });
                context.SaveChanges();
            }
            #endregion

            #region остатки номенклатуры в контексте доставки x
            if (!context.InventoryGoodsBalancesDeliveries.Any())
            {
                context.InventoryGoodsBalancesDeliveries.AddRange(new InventoryBalancesDeliveriesAnalyticalModel[]
                {

                });
                context.SaveChanges();
            }
            #endregion
        }
    }
}
