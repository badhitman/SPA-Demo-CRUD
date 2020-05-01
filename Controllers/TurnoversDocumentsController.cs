////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelManager")]
    public class TurnoversDocumentsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<TurnoversDocumentsController> _logger;
        private readonly UserObjectModel _user;

        public TurnoversDocumentsController(AppDataBaseContext context, ILogger<TurnoversDocumentsController> logger, SessionUser session)
        {
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/TurnoversDocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTurnoversDeliveries([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<MovementTurnoverDeliveryDocumentModel> documents = _context.MovementTurnoverDeliveryDocuments.AsQueryable();

            pagingParameters.Init(await documents.CountAsync());
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            documents = documents.OrderBy(doc => doc.Id);
            if (pagingParameters.PageNum > 1)
                documents = documents.Skip(pagingParameters.Skip);

            documents = documents
                .Take(pagingParameters.PageSize)
                .Include(doc => doc.Author)
                .Include(doc => doc.Warehouse)
                .Include(doc => doc.Buyer)
                .Include(doc => doc.DeliveryMethod)
                .Include(doc => doc.DeliveryService);

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос документов отгрузки обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await documents.Select(doc => new
                {

                    doc.Id,
                    doc.DateCreate,
                    doc.Name,
                    doc.Information,
                    Author = new
                    {
                        doc.Author.Id,
                        doc.Author.Name
                    },
                    Warehouse = new
                    {
                        doc.Warehouse.Id,
                        doc.Warehouse.Name,
                        doc.Warehouse.Information
                    },
                    Buyer = new
                    {
                        doc.Buyer.Id,
                        doc.Buyer.Name,
                        doc.Buyer.Information
                    },
                    DeliveryMethod = new
                    {
                        doc.DeliveryMethod.Id,
                        doc.DeliveryMethod.Name
                    },
                    DeliveryService = new
                    {
                        doc.DeliveryService.Id,
                        doc.DeliveryService.Name
                    },
                    doc.DeliveryAddress1,
                    doc.DeliveryAddress2,
                    CountRows = _context.GoodMovementDocumentRows.Count(row => row.BodyDocumentId == doc.Id)
                }).ToListAsync()
            });
        }

        // GET: api/TurnoversDocuments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTurnoverDeliveryDocument(int id)
        {
            MovementTurnoverDeliveryDocumentModel doc = await _context.MovementTurnoverDeliveryDocuments
                .Include(x => x.Author)
                .Include(x => x.Buyer)
                .Include(x => x.DeliveryMethod)
                .Include(x => x.DeliveryService)
                .FirstOrDefaultAsync(x => x.Id == id);

            string msg = $"Документ отгрузки не найден: id={id}";
            if (doc == null)
            {
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос документа отгрузки обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    doc.Id,
                    doc.Name,
                    doc.WarehouseId,
                    doc.Information,
                    Buyer = new
                    {
                        doc.Buyer.Id,
                        doc.Buyer.Name,
                        doc.Buyer.Information
                    },
                    DeliveryMethod = new
                    {
                        doc.DeliveryMethod.Id,
                        doc.DeliveryMethod.Name
                    },
                    DeliveryService = new
                    {
                        doc.DeliveryService.Id,
                        doc.DeliveryService.Name
                    },
                    Author = new
                    {
                        doc.Author.Id,
                        doc.Author.Name,
                        doc.Author.Information
                    },
                    doc.DeliveryAddress1,
                    doc.DeliveryAddress2,

                    rows = await _context.GoodMovementDocumentRows
                    .Where(row => row.BodyDocumentId == doc.Id)
                    .Include(row => row.Good)
                    .OrderBy(sel => sel.Good.Name)
                    .Select(row => new
                    {
                        row.Id,
                        row.Quantity,
                        Good = new
                        {
                            row.Good.Id,
                            row.Good.Name,
                            row.Good.Information
                        },
                        row.UnitId
                    }).ToListAsync(),

                    Units = await _context.Units.OrderBy(x => x.Name).Select(u => new
                    {
                        u.Id,
                        u.Name,
                        u.Information
                    }).ToListAsync(),

                    GroupsGoods = await _context.GroupsGoods
                    .OrderBy(group => group.Name)
                    .Include(group => group.Goods)
                    .Where(group => group.Goods.Count > 0)
                    .Select(group => new
                    {
                        group.Id,
                        group.Name,
                        group.Information,
                        Goods = group.Goods.OrderBy(good => good.Name).Select(good => new
                        {
                            good.Id,
                            good.UnitId,
                            good.Name,
                            good.Information
                        })
                    }).ToListAsync()
                }
            });
        }

        // POST: api/TurnoversDocuments
        [HttpPost]
        public async Task<ActionResult<object>> PostTurnoverDeliveryDocument(MovementTurnoverDeliveryDocumentModel ajaxDoc)
        {
            if (ajaxDoc.DeliveryMethodId == 0
                || ajaxDoc.WarehouseId == 0
                || !await _context.DeliveryMethods.AnyAsync(x => x.Id == ajaxDoc.DeliveryMethodId)
                || !await _context.Warehouses.AnyAsync(x => x.Id == ajaxDoc.WarehouseId)
                || (ajaxDoc.BuyerId > 0 && !await _context.Users.AnyAsync(x => x.Id == ajaxDoc.BuyerId))
                || (ajaxDoc.DeliveryServiceId > 0 && !await _context.DeliveryServices.AnyAsync(x => x.Id == ajaxDoc.DeliveryServiceId)))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            WarehouseDocumentsModel parentDoc = await _context.WarehouseDocuments
                .Include(x => x.Warehouse)
                .Include(x => x.RowsDocument)
                .FirstOrDefaultAsync(x => x.Discriminator == nameof(WarehouseDocumentsModel) && x.Name == nameof(MovementTurnoverDeliveryDocumentModel) && x.AuthorId == _user.Id);

            if (parentDoc == null || parentDoc.RowsDocument.Count == 0)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Отсутствует основание для создания документа отгрузки. Укажите в документе склад списания, метод отгрузки/доставки и добавьте строки в табличную часть",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    List<RowGoodMovementRegisterModel> rows = parentDoc.RowsDocument;
                    MovementTurnoverDeliveryDocumentModel doc = new MovementTurnoverDeliveryDocumentModel()
                    {
                        WarehouseId = ajaxDoc.WarehouseId,
                        DeliveryMethodId = ajaxDoc.DeliveryMethodId,
                        //
                        DeliveryServiceId = ajaxDoc.DeliveryServiceId,
                        DeliveryAddress1 = ajaxDoc.DeliveryAddress1,
                        DeliveryAddress2 = ajaxDoc.DeliveryAddress2,
                        ////////////////////////////////////////////
                        Name = "~ отгрузка",
                        Information = ajaxDoc.Information.Trim(),
                        BuyerId = ajaxDoc.BuyerId,
                        AuthorId = _user.Id
                    };

                    _context.MovementTurnoverDeliveryDocuments.Add(doc);
                    await _context.SaveChangesAsync();
                    while (rows.Count > 0)
                    {
                        if (rows[0].Quantity == 0)
                        {
                            rows.RemoveAt(0);
                            continue;
                        }
                        rows[0].BodyDocumentId = doc.Id;

                        #region списание количества номенклатуры со склада списания

                        InventoryBalancesWarehousesAnalyticalModel balanceWarehouse = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(InventoryBalance =>
                        InventoryBalance.GoodId == rows[0].GoodId
                        && InventoryBalance.WarehouseId == doc.WarehouseId
                        && InventoryBalance.UnitId == rows[0].UnitId);

                        if (balanceWarehouse is null)
                        {
                            balanceWarehouse = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = parentDoc.WarehouseId,
                                GoodId = rows[0].GoodId,
                                Quantity = rows[0].Quantity * -1,
                                UnitId = rows[0].UnitId
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(balanceWarehouse);
                        }
                        else
                        {
                            balanceWarehouse.Quantity -= rows[0].Quantity;
                            if (balanceWarehouse.Quantity == 0)
                            {
                                _context.InventoryGoodsBalancesWarehouses.Remove(balanceWarehouse);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesWarehouses.Update(balanceWarehouse);
                            }
                        }
                        await _context.SaveChangesAsync();

                        #endregion
                        #region зачисление количества номенклатуры в отгрузку

                        InventoryBalancesDeliveriesAnalyticalModel balanceDelivering = _context.InventoryGoodsBalancesDeliveries.FirstOrDefault(InventoryBalance =>
                        InventoryBalance.GoodId == rows[0].GoodId
                        && InventoryBalance.UnitId == rows[0].UnitId
                        && InventoryBalance.DeliveryMethodId == ajaxDoc.DeliveryMethodId
                        //
                        && InventoryBalance.DeliveryServiceId == ajaxDoc.DeliveryServiceId
                        && InventoryBalance.DeliveryAddress1 == ajaxDoc.DeliveryAddress1
                        && InventoryBalance.DeliveryAddress2 == ajaxDoc.DeliveryAddress2);

                        if (balanceDelivering is null)
                        {
                            balanceDelivering = new InventoryBalancesDeliveriesAnalyticalModel()
                            {
                                Name = "*",
                                GoodId = rows[0].GoodId,
                                UnitId = rows[0].UnitId,
                                Quantity = rows[0].Quantity,
                                DeliveryMethodId = ajaxDoc.DeliveryMethodId,
                                DeliveryServiceId = ajaxDoc.DeliveryServiceId,
                                DeliveryAddress1 = ajaxDoc.DeliveryAddress1,
                                DeliveryAddress2 = ajaxDoc.DeliveryAddress2
                            };
                            _context.InventoryGoodsBalancesDeliveries.Add(balanceDelivering);
                        }
                        else
                        {
                            balanceDelivering.Quantity += rows[0].Quantity;
                            if (balanceDelivering.Quantity == 0)
                            {
                                _context.InventoryGoodsBalancesDeliveries.Remove(balanceDelivering);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesDeliveries.Update(balanceDelivering);
                            }
                        }
                        await _context.SaveChangesAsync();

                        #endregion
                    }

                    _context.WarehouseDocuments.Remove(parentDoc);
                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = true,
                        Info = "Документ создан",
                        Status = StylesMessageEnum.success.ToString(),
                        Tag = new { doc.Id }
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Ошибка во время корректировки складских остатков: {0}", ex.Message);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = $"Ошибка во время корректировки складских остатков: {ex.Message}",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
            }
        }

        // PUT: api/TurnoversDocuments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTurnoverDeliveryDocument(int id, MovementTurnoverDeliveryDocumentModel ajaxDoc)
        {
            ajaxDoc.Information = ajaxDoc.Information.Trim();

            if (!ModelState.IsValid
                || id != ajaxDoc.Id
                || id < 1
                || !await _context.Warehouses.AnyAsync(x => x.Id == ajaxDoc.WarehouseId))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            MovementTurnoverDeliveryDocumentModel doc = await _context.MovementTurnoverDeliveryDocuments
                .Where(x => x.Id == id)
                .Include(x => x.Author)
                .Include(x => x.Warehouse)
                .Include(x => x.DeliveryMethod)
                .Include(x => x.DeliveryService)
                .Include(x => x.Buyer)
                //.Include(x => x.RowsDocument).ThenInclude(x => x.Good)
                .FirstOrDefaultAsync();

            if (doc is null)
            {
                _logger.LogError("Документ перемещения не найден в БД. id=", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (doc.Information == ajaxDoc.Information
                && doc.WarehouseId == ajaxDoc.WarehouseId)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "В документе нет изменений. Сохранению подлежат [склад поступления/списания] и [комментарий/примечание]",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            var selectorRows = _context.GoodMovementDocumentRows
                .Where(x => x.BodyDocumentId == id)
                .Include(x => x.Good)
                .Join(_context.Units, row => row.UnitId, unit => unit.Id, (row, unit) => new { row, unit });

            using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    foreach (var sel in selectorRows)
                    {
                        #region при изменении склада отгрузки
                        if (doc.WarehouseId != ajaxDoc.WarehouseId)
                        {
                            #region возврат количества номенклатуры предыдущему складу

                            InventoryBalancesWarehousesAnalyticalModel ballance = await _context.InventoryGoodsBalancesWarehouses
                                .FirstOrDefaultAsync(InventoryBalance =>
                                InventoryBalance.WarehouseId == doc.WarehouseId
                                && InventoryBalance.GoodId == sel.row.GoodId
                                && InventoryBalance.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",
                                    WarehouseId = doc.WarehouseId,
                                    GoodId = sel.row.GoodId,
                                    UnitId = sel.row.UnitId,
                                    Quantity = sel.row.Quantity
                                };
                                _context.InventoryGoodsBalancesWarehouses.Add(ballance);
                            }
                            else
                            {
                                ballance.Quantity += sel.row.Quantity;
                                if (ballance.Quantity == 0)
                                {
                                    _context.InventoryGoodsBalancesWarehouses.Remove(ballance);
                                }
                                else
                                {
                                    _context.InventoryGoodsBalancesWarehouses.Update(ballance);
                                }
                            }
                            await _context.SaveChangesAsync();

                            #endregion

                            #region списание с нового склада отгрузки

                            ballance = await _context.InventoryGoodsBalancesWarehouses.FirstOrDefaultAsync(x =>
                            x.WarehouseId == ajaxDoc.WarehouseId
                            && x.GoodId == sel.row.GoodId
                            && x.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",
                                    WarehouseId = ajaxDoc.WarehouseId,
                                    GoodId = sel.row.GoodId,
                                    UnitId = sel.row.UnitId,
                                    Quantity = sel.row.Quantity * -1
                                };
                                _context.InventoryGoodsBalancesWarehouses.Add(ballance);
                            }
                            else
                            {
                                ballance.Quantity -= sel.row.Quantity;
                                if (ballance.Quantity != 0)
                                {
                                    _context.InventoryGoodsBalancesWarehouses.Update(ballance);
                                }
                                else
                                {
                                    _context.InventoryGoodsBalancesWarehouses.Remove(ballance);
                                }
                            }
                            await _context.SaveChangesAsync();

                            #endregion
                        }
                        #endregion

                        #region при изменении параметров отгрузки
                        if (doc.DeliveryMethodId != ajaxDoc.DeliveryMethodId
                            || doc.DeliveryServiceId != ajaxDoc.DeliveryServiceId
                            || doc.DeliveryAddress1 != ajaxDoc.DeliveryAddress1
                            || doc.DeliveryAddress2 != ajaxDoc.DeliveryAddress2)
                        {
                            #region списание количества номенклатуры с баланса предыдущих параметров доставки

                            InventoryBalancesDeliveriesAnalyticalModel ballance = await _context.InventoryGoodsBalancesDeliveries
                                .FirstOrDefaultAsync(InventoryBalance =>
                                InventoryBalance.DeliveryMethodId == doc.DeliveryMethodId
                                && InventoryBalance.DeliveryServiceId == doc.DeliveryServiceId
                                && InventoryBalance.DeliveryAddress1 == doc.DeliveryAddress1
                                && InventoryBalance.DeliveryAddress2 == doc.DeliveryAddress2

                                && InventoryBalance.GoodId == sel.row.GoodId
                                && InventoryBalance.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesDeliveriesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",

                                    DeliveryMethodId = doc.DeliveryMethodId,
                                    DeliveryServiceId = doc.DeliveryServiceId,
                                    DeliveryAddress1 = doc.DeliveryAddress1,
                                    DeliveryAddress2 = doc.DeliveryAddress2,

                                    GoodId = sel.row.GoodId,
                                    UnitId = sel.row.UnitId,
                                    Quantity = sel.row.Quantity * -1
                                };
                                _context.InventoryGoodsBalancesDeliveries.Add(ballance);
                            }
                            else
                            {
                                ballance.Quantity -= sel.row.Quantity;
                                if (ballance.Quantity == 0)
                                {
                                    _context.InventoryGoodsBalancesDeliveries.Remove(ballance);
                                }
                                else
                                {
                                    _context.InventoryGoodsBalancesDeliveries.Update(ballance);
                                }
                            }
                            await _context.SaveChangesAsync();

                            #endregion

                            #region начисление количественного баланса номенклатуры по новым параметров доставки

                            ballance = await _context.InventoryGoodsBalancesDeliveries
                                .FirstOrDefaultAsync(InventoryBalance =>
                                InventoryBalance.DeliveryMethodId == ajaxDoc.DeliveryMethodId
                                && InventoryBalance.DeliveryServiceId == ajaxDoc.DeliveryServiceId
                                && InventoryBalance.DeliveryAddress1 == ajaxDoc.DeliveryAddress1
                                && InventoryBalance.DeliveryAddress2 == ajaxDoc.DeliveryAddress2

                                && InventoryBalance.GoodId == sel.row.GoodId
                                && InventoryBalance.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesDeliveriesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",

                                    DeliveryMethodId = ajaxDoc.DeliveryMethodId,
                                    DeliveryServiceId = doc.DeliveryServiceId,
                                    DeliveryAddress1 = doc.DeliveryAddress1,
                                    DeliveryAddress2 = doc.DeliveryAddress2,

                                    GoodId = sel.row.GoodId,
                                    UnitId = sel.row.UnitId,
                                    Quantity = sel.row.Quantity
                                };
                                _context.InventoryGoodsBalancesDeliveries.Add(ballance);
                            }
                            else
                            {
                                ballance.Quantity += sel.row.Quantity;
                                if (ballance.Quantity != 0)
                                {
                                    _context.InventoryGoodsBalancesDeliveries.Update(ballance);
                                }
                                else
                                {
                                    _context.InventoryGoodsBalancesDeliveries.Remove(ballance);
                                }
                            }
                            await _context.SaveChangesAsync();

                            #endregion
                        }
                        #endregion
                    }

                    doc.WarehouseId = ajaxDoc.WarehouseId;
                    doc.Information = ajaxDoc.Information.Trim();
                    _context.MovementTurnoverDeliveryDocuments.Update(doc);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = true,
                        Info = "Документ сохранён",
                        Status = StylesMessageEnum.success.ToString(),
                        Tag = new { doc.Id, doc.Information, doc.WarehouseId }
                    });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _logger.LogError("Ошибка во время корректировки складских остатков: {0}", ex.Message);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = $"Ошибка во время корректировки складских остатков: {ex.Message}",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
            }
        }

        /// <summary>
        /// манипуляция строками докумета (удалить/добавить)
        /// </summary>
        /// <param name="ajaxRow">строка документа. существующая строка удаляется, а новая добалвяется</param>
        /// <param name="id">идентификатор документа</param>
        // PATCH: api/TurnoversDocuments/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<object>> PatchRowDocument([FromBody]RowGoodMovementRegisterModel ajaxRow, [FromRoute] int id)
        {
            _logger.LogInformation($"Запрос {((ajaxRow.Id > 0) ? "удаления" : "добавления")} строки. Документ отгрузки/доставки #[{id}]");
            string msg;

            if (id < 1 || id != ajaxRow.BodyDocumentId

                // проверка наличия документа-владельца
                || !await _context.MovementTurnoverDeliveryDocuments.AnyAsync(x => x.Id == id)
                // если не указан id - значит строка для добавления => указаные [good] и [unit] должны существовать в БД, а количество движения должно быть отличным от нуля
                || (ajaxRow.Id == 0 && (ajaxRow.Quantity == 0 || !await _context.Goods.AnyAsync(x => x.Id == ajaxRow.GoodId) || !await _context.Units.AnyAsync(x => x.Id == ajaxRow.UnitId)))
                // если id указан - значит строку нужно удалить => проверяем существования нужной строки
                || (ajaxRow.Id > 0 && !await _context.GoodMovementDocumentRows.AnyAsync(x => x.Id == ajaxRow.Id && x.BodyDocumentId == ajaxRow.BodyDocumentId)))
            {
                msg = $"Ошибка проверки запроса. Задание над строкой документа отклонено";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            var docDb = await _context.MovementTurnoverDeliveryDocuments
                .Where(x => x.Id == id)
                .Include(x => x.Warehouse)
                .Include(x => x.DeliveryMethod)
                .Include(x => x.DeliveryService)
                .FirstOrDefaultAsync();

            UnitGoodObjectModel unit;
            GoodObjectModel good;

            #region удаление строки
            if (ajaxRow.Id > 0)
            {
                var rowDb = await _context.GoodMovementDocumentRows
                    .Include(row => row.Good)
                    .Join(_context.Units, row => row.UnitId, unit => unit.Id, (row, unit) => new { row, unit })
                    .FirstOrDefaultAsync(sel => sel.row.Id == ajaxRow.Id);

                using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        _context.GoodMovementDocumentRows.Remove(rowDb.row);
                        //await _context.SaveChangesAsync();
                        _logger.LogInformation("документ из БД удалён. требуется отразить движение в количественном балансе...");

                        if (rowDb.row.Quantity != 0)
                        {
                            _logger.LogInformation($"Отражение удаления строки документа отгрузки номенклатуры в остатках. [со склада: ${docDb.Warehouse.Name}]->[отгрузка: ${docDb.DeliveryMethod.Name}/${docDb.DeliveryService.Name}/${docDb.DeliveryAddress1}/${docDb.DeliveryAddress2}] [номенклатура: ${rowDb.row.Good.Name} ${rowDb.row.Quantity} ${rowDb.unit.Name}]");

                            #region возврат количества складу отгрузки

                            InventoryBalancesWarehousesAnalyticalModel iBalance = _context.InventoryGoodsBalancesWarehouses
                                .FirstOrDefault(InventoryBalance =>
                                InventoryBalance.WarehouseId == docDb.WarehouseId
                                && InventoryBalance.GoodId == rowDb.row.GoodId
                                && InventoryBalance.UnitId == rowDb.row.UnitId);

                            if (iBalance is null)
                            {
                                _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры для склада отгрузки: {rowDb.row.Quantity}");
                                iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = "*",
                                    GoodId = rowDb.row.GoodId,
                                    Quantity = rowDb.row.Quantity,
                                    UnitId = rowDb.row.UnitId
                                };
                                _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                            }
                            else
                            {
                                double result = iBalance.Quantity + rowDb.row.Quantity;
                                _logger.LogInformation($"изменение измерителя: {iBalance.Quantity}+{rowDb.row.Quantity}={result}");
                                if (result == 0)
                                {
                                    _logger.LogInformation("Накопитель с нулевым балансом подлежит удалению...");
                                    _context.InventoryGoodsBalancesWarehouses.Remove(iBalance);
                                }
                                else
                                {
                                    iBalance.Quantity = result;
                                    _context.InventoryGoodsBalancesWarehouses.Update(iBalance);
                                }
                            }
                            _context.SaveChanges();

                            #endregion
                            #region списание количества из доставки по параметрам

                            InventoryBalancesDeliveriesAnalyticalModel ballance = _context.InventoryGoodsBalancesDeliveries
                                .FirstOrDefault(InventoryBalance =>
                                InventoryBalance.DeliveryMethodId == docDb.DeliveryMethodId
                                && InventoryBalance.DeliveryServiceId == docDb.DeliveryServiceId
                                && InventoryBalance.DeliveryAddress1 == docDb.DeliveryAddress1
                                && InventoryBalance.DeliveryAddress2 == docDb.DeliveryAddress2

                                && InventoryBalance.GoodId == rowDb.row.GoodId
                                && InventoryBalance.UnitId == rowDb.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesDeliveriesAnalyticalModel()
                                {
                                    Name = $"[good: {rowDb.row.Good.Name}]/[unit: {rowDb.unit.Name}]",

                                    DeliveryMethodId = docDb.DeliveryMethodId,
                                    DeliveryServiceId = docDb.DeliveryServiceId,
                                    DeliveryAddress1 = docDb.DeliveryAddress1,
                                    DeliveryAddress2 = docDb.DeliveryAddress2,

                                    GoodId = rowDb.row.GoodId,
                                    UnitId = rowDb.row.UnitId,
                                    Quantity = rowDb.row.Quantity * -1
                                };
                                _context.InventoryGoodsBalancesDeliveries.Add(ballance);
                            }
                            else
                            {
                                ballance.Quantity -= rowDb.row.Quantity;
                                if (ballance.Quantity == 0)
                                {
                                    _context.InventoryGoodsBalancesDeliveries.Remove(ballance);
                                }
                                else
                                {
                                    _context.InventoryGoodsBalancesDeliveries.Update(ballance);
                                }
                            }
                            _context.SaveChanges();

                            #endregion
                        }
                        else
                        {
                            _logger.LogWarning("Возникла ошибка во время корректировки остатков номенклатуры на балансе склада (удаление строки документа поступления). Строка имеет нулевое движение по количеству. Строк регистров с нулевым количеством быть не должно.");
                        }

                        await transaction.CommitAsync();
                        _logger.LogInformation("Транзакционная сессия для удаления строки регистра из документа поступления - успешно завершена => строка документа удалена");
                        return new ObjectResult(new ServerActionResult()
                        {
                            Success = true,
                            Info = "Строка удалена",
                            Status = StylesMessageEnum.success.ToString()
                        });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError("Откат транзакции. Ошибка во время корректировки складских остатков: {0}", ex.Message);
                        return new ObjectResult(new ServerActionResult()
                        {
                            Success = false,
                            Info = $"Ошибка/откат во время корректировки складских остатков: {ex.Message}",
                            Status = StylesMessageEnum.danger.ToString()
                        });
                    }
                }
            }
            #endregion
            #region добавление строки
            else
            {
                unit = await _context.Units.FindAsync(ajaxRow.UnitId);
                good = await _context.Goods.FindAsync(ajaxRow.GoodId);

                _logger.LogInformation("Открытие транзакционной сессии для добавления строки регистра для документа поступления");
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        RowGoodMovementRegisterModel wRowDocumentDb = _context.GoodMovementDocumentRows
                            .FirstOrDefault(row =>
                            row.BodyDocumentId == id
                            && row.GoodId == good.Id
                            && row.UnitId == unit.Id);

                        if (wRowDocumentDb is null)
                        {
                            await _context.GoodMovementDocumentRows.AddAsync(ajaxRow);
                        }
                        else
                        {
                            wRowDocumentDb.Quantity += ajaxRow.Quantity;
                            if (wRowDocumentDb.Quantity != 0)
                            {
                                _context.GoodMovementDocumentRows.Update(wRowDocumentDb);
                            }
                            else
                            {
                                _context.GoodMovementDocumentRows.Remove(wRowDocumentDb);
                            }
                        }
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Отражение добавления строки документа поступления номенклатуры в остатках. [со склада: ${docDb.Warehouse.Name}]->[отгрузка: ${docDb.DeliveryMethod.Name}/${docDb.DeliveryService.Name}/${docDb.DeliveryAddress1}/${docDb.DeliveryAddress2}] [номенклатура: ${good.Name} ${wRowDocumentDb.Quantity} ${unit.Name}]");

                        InventoryBalancesWarehousesAnalyticalModel iBalance;
                        #region списание количества со склада отгрузки

                        iBalance = _context.InventoryGoodsBalancesWarehouses
                            .FirstOrDefault(InventoryBalance =>
                            InventoryBalance.GoodId == good.Id
                            && InventoryBalance.UnitId == unit.Id);

                        if (iBalance is null)
                        {
                            _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры для склада поступления: {ajaxRow.Quantity}");
                            iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                GoodId = good.Id,
                                Quantity = ajaxRow.Quantity,
                                UnitId = unit.Id
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                        }
                        else
                        {
                            double result = iBalance.Quantity + ajaxRow.Quantity;
                            _logger.LogInformation($"изменение измерителя: {iBalance.Quantity}+{ajaxRow.Quantity}={result}");
                            if (result == 0)
                            {
                                _logger.LogInformation("Накопитель с нулевым балансом подлежит удалению...");
                                _context.InventoryGoodsBalancesWarehouses.Remove(iBalance);
                            }
                            else
                            {
                                iBalance.Quantity = result;
                                _context.InventoryGoodsBalancesWarehouses.Update(iBalance);
                            }
                        }
                        _context.SaveChanges();

                        #endregion
                        #region начисление количества в доставку/отгрузку с параметрами

                        iBalance = _context.InventoryGoodsBalancesWarehouses
                            .FirstOrDefault(balance =>
                            balance.GoodId == good.Id
                            && balance.UnitId == unit.Id
                            && balance.WarehouseId == docDb.Warehouse.Id);

                        if (iBalance is null)
                        {
                            _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры: {ajaxRow.Quantity}");
                            iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = docDb.Warehouse.Id,
                                GoodId = good.Id,
                                Quantity = ajaxRow.Quantity * -1,
                                UnitId = unit.Id
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                        }
                        else
                        {
                            double result = iBalance.Quantity - ajaxRow.Quantity;
                            _logger.LogInformation($"изменение измерителя: {iBalance.Quantity}-{ajaxRow.Quantity}={result}");
                            if (result == 0)
                            {
                                _logger.LogInformation("Накопитель с нулевым балансом подлежит удалению...");
                                _context.InventoryGoodsBalancesWarehouses.Remove(iBalance);
                            }
                            else
                            {
                                iBalance.Quantity = result;
                                _context.InventoryGoodsBalancesWarehouses.Update(iBalance);
                            }
                        }
                        _context.SaveChanges();

                        #endregion

                        await transaction.CommitAsync();
                        _logger.LogInformation("Транзакционная сессия для добавления строки регистра для документа поступления - успешно завершена => строка документа добавлена");
                        return new ObjectResult(new ServerActionResult()
                        {
                            Success = true,
                            Info = "Строка документа добавлена",
                            Status = StylesMessageEnum.success.ToString(),
                            Tag = new { ajaxRow.Id }
                        });
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        _logger.LogError("Ошибка во время корректировки складских остатков: {0}", ex.Message);
                        return new ObjectResult(new ServerActionResult()
                        {
                            Success = false,
                            Info = $"Ошибка во время корректировки складских остатков: {ex.Message}",
                            Status = StylesMessageEnum.danger.ToString()
                        });
                    }
                }
            }
            #endregion
        }

        // DELETE: api/TurnoversDocuments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteTurnoverDeliveryDocument(int id)
        {
            string msg;

            var selDoc = await _context.MovementTurnoverDeliveryDocuments
                .Where(receiptDoc => receiptDoc.Id == id)
                .Include(x => x.Warehouse)
                .Join(_context.Warehouses, doc => doc.WarehouseId, warehouse => warehouse.Id, (Document, Warehouse) => new
                {
                    Document,
                    DebitingWarehouse = new { Warehouse.Id, Warehouse.Name, Warehouse.Information }
                })
                .FirstOrDefaultAsync();

            if (selDoc == null)
            {
                msg = $"Документ отгрузки не найден в БД. id={id}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    var selRows = _context.GoodMovementDocumentRows
                        .Include(x => x.Good)
                        .Where(x => x.BodyDocumentId == id && x.Quantity != 0)
                        .Join(_context.Units, row => row.UnitId, unit => unit.Id, (row, unit) => new { row, unit });

                    (await selRows.ToListAsync()).ForEach(selRow =>
                    {
                        #region возврат количества номенклатуры на склад

                        InventoryBalancesWarehousesAnalyticalModel balanceWarehouse = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(balance =>
                        balance.GoodId == selRow.row.GoodId
                        && balance.UnitId == selRow.row.UnitId
                        && balance.WarehouseId == selDoc.DebitingWarehouse.Id);

                        if (balanceWarehouse is null)
                        {
                            balanceWarehouse = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = selDoc.DebitingWarehouse.Id,
                                GoodId = selRow.row.GoodId,
                                Quantity = selRow.row.Quantity,
                                UnitId = selRow.row.UnitId
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(balanceWarehouse);
                        }
                        else
                        {
                            balanceWarehouse.Quantity += selRow.row.Quantity;
                            if (balanceWarehouse.Quantity != 0)
                            {
                                _context.InventoryGoodsBalancesWarehouses.Update(balanceWarehouse);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesWarehouses.Remove(balanceWarehouse);
                            }
                        }

                        #endregion
                        #region списание количества номенклатцры с баланса доставки/отгрузки

                        InventoryBalancesDeliveriesAnalyticalModel ballance = _context.InventoryGoodsBalancesDeliveries
                                .FirstOrDefault(InventoryBalance =>
                                InventoryBalance.DeliveryMethodId == selDoc.Document.DeliveryMethodId
                                && InventoryBalance.DeliveryServiceId == selDoc.Document.DeliveryServiceId
                                && InventoryBalance.DeliveryAddress1 == selDoc.Document.DeliveryAddress1
                                && InventoryBalance.DeliveryAddress2 == selDoc.Document.DeliveryAddress2

                                && InventoryBalance.GoodId == selRow.row.GoodId
                                && InventoryBalance.UnitId == selRow.row.UnitId);

                        if (ballance is null)
                        {
                            ballance = new InventoryBalancesDeliveriesAnalyticalModel()
                            {
                                Name = $"[good: {selRow.row.Good.Name}]/[unit: {selRow.unit.Name}]",

                                DeliveryMethodId = selDoc.Document.DeliveryMethodId,
                                DeliveryServiceId = selDoc.Document.DeliveryServiceId,
                                DeliveryAddress1 = selDoc.Document.DeliveryAddress1,
                                DeliveryAddress2 = selDoc.Document.DeliveryAddress2,

                                GoodId = selRow.row.GoodId,
                                UnitId = selRow.row.UnitId,
                                Quantity = selRow.row.Quantity * -1
                            };
                            _context.InventoryGoodsBalancesDeliveries.Add(ballance);
                        }
                        else
                        {
                            ballance.Quantity -= selRow.row.Quantity;
                            if (ballance.Quantity == 0)
                            {
                                _context.InventoryGoodsBalancesDeliveries.Remove(ballance);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesDeliveries.Update(ballance);
                            }
                        }

                        #endregion
                        _context.SaveChanges();
                    });

                    _context.MovementTurnoverDeliveryDocuments.Remove(selDoc.Document);

                    await _context.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = true,
                        Info = "Документ удалён",
                        Status = StylesMessageEnum.success.ToString()
                    });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError("Ошибка во время корректировки складских остатков: {0}", ex.Message);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = $"Ошибка во время корректировки складских остатков: {ex.Message}",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
            }
        }
    }
}
