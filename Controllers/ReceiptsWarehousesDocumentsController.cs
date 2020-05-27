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
using Microsoft.Extensions.Options;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelManager")]
    public class ReceiptsWarehousesDocumentsController : ControllerBase
    {
        private readonly AppConfig AppOptions;
        private readonly AppDataBaseContext _context;
        private readonly ILogger<ReceiptsWarehousesDocumentsController> _logger;
        private readonly UserObjectModel _user;

        public ReceiptsWarehousesDocumentsController(AppDataBaseContext context, ILogger<ReceiptsWarehousesDocumentsController> logger, SessionUser session, IOptions<AppConfig> options)
        {
            AppOptions = options.Value;
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/ReceiptsWarehousesDocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetReceiptsWarehouses([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<ReceiptToWarehouseDocumentModel> documents = _context.ReceiptesGoodsToWarehousesDocuments.AsQueryable();

            pagingParameters.Init(await documents.CountAsync());
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            documents = documents.OrderByDescending(x => x.Id);
            if (pagingParameters.PageNum > 1)
                documents = documents.Skip(pagingParameters.Skip);

            documents = documents
                .Take(pagingParameters.PageSize)
                .Include(x => x.Author)
                .Include(x => x.WarehouseReceipt);

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос документов поступления обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = documents.Select(doc => new
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
                    WarehouseReceipt = new
                    {
                        doc.WarehouseReceipt.Id,
                        doc.WarehouseReceipt.Name,
                        doc.WarehouseReceipt.Information
                    },
                    CountRows = _context.GoodMovementDocumentRows.Count(row => row.BodyDocumentId == doc.Id),
                    doc.Discriminator
                })
            });
        }

        // GET: api/ReceiptsWarehousesDocuments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetReceiptWarehouseDocument(int id)
        {
            ReceiptToWarehouseDocumentModel document = await _context.ReceiptesGoodsToWarehousesDocuments
                .Include(x => x.Author)
                .Include(x => x.WarehouseReceipt)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (document == null)
            {
                _logger.LogError("Документ поступления не найден: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Документ поступления не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            var rows = await _context.GoodMovementDocumentRows
                    .Where(row => row.BodyDocumentId == document.Id)
                    .Include(row => row.Good)
                    .OrderBy(sel => sel.Good.Name)
                    .Join(_context.Units, row => row.UnitId, unit => unit.Id, (row, unit) => new
                    {
                        row.Id,
                        row.Quantity,
                        Good = new
                        {
                            row.Good.Id,
                            row.Good.Name,
                            row.Good.Information
                        },
                        unit
                    })
                    .Select(selItem => new
                    {
                        selItem.Id,
                        selItem.Quantity,
                        Good = new
                        {
                            selItem.Good.Id,
                            selItem.Good.Name,
                            selItem.Good.Information
                        },
                        Unit = new
                        {
                            selItem.unit.Id,
                            selItem.unit.Name,
                            selItem.unit.Information
                        }
                    }).ToListAsync();

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос документа поступления обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    document.Id,
                    document.Name,
                    WarehouseReceipt = new
                    {
                        document.WarehouseReceipt.Id,
                        document.WarehouseReceipt.Name,
                        document.WarehouseReceipt.Information
                    },
                    document.Information,

                    rows,

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

        // POST: api/ReceiptsWarehousesDocuments
        [HttpPost]
        public async Task<ActionResult<object>> PostReceiptWarehouseDocument(ReceiptToWarehouseDocumentModel ajaxDoc)
        {
            if (ajaxDoc.WarehouseReceiptId == 0 || !await _context.Warehouses.AnyAsync(x => x.Id == ajaxDoc.WarehouseReceiptId))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            WarehouseDocumentsModel parentDoc = await _context.WarehouseDocuments
                .Include(x => x.WarehouseReceipt)
                .Include(x => x.RowsDocument)
                .FirstOrDefaultAsync(x => x.Discriminator == nameof(WarehouseDocumentsModel) && x.Name == nameof(ReceiptToWarehouseDocumentModel) && x.AuthorId == _user.Id);

            if (parentDoc == null || parentDoc.RowsDocument.Count == 0)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Отсутствует основание для создания документа-поступления. Укажите в документе склад поступления и добавьте строки в табличную часть",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    List<RowGoodMovementRegisterModel> rows = parentDoc.RowsDocument;
                    ReceiptToWarehouseDocumentModel receiptToWarehouseDocument = new ReceiptToWarehouseDocumentModel()
                    {
                        Name = "~ поступление",
                        Information = parentDoc.Information,
                        WarehouseReceiptId = parentDoc.WarehouseReceiptId,
                        AuthorId = _user.Id
                    };

                    _context.ReceiptesGoodsToWarehousesDocuments.Add(receiptToWarehouseDocument);
                    await _context.SaveChangesAsync();
                    while (rows.Count > 0)
                    {
                        if (rows[0].Quantity == 0)
                        {
                            rows.RemoveAt(0);
                            continue;
                        }
                        rows[0].BodyDocumentId = receiptToWarehouseDocument.Id;

                        #region зачисление на склад-приёмник

                        InventoryBalancesWarehousesAnalyticalModel InventoryGoodBalanceWarehouse = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(InventoryBalance => InventoryBalance.GoodId == rows[0].GoodId && InventoryBalance.UnitId == rows[0].UnitId && InventoryBalance.WarehouseId == parentDoc.WarehouseReceiptId);
                        if (InventoryGoodBalanceWarehouse is null)
                        {
                            InventoryGoodBalanceWarehouse = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = parentDoc.WarehouseReceiptId,
                                GoodId = rows[0].GoodId,
                                Quantity = rows[0].Quantity,
                                UnitId = rows[0].UnitId
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(InventoryGoodBalanceWarehouse);
                        }
                        else
                        {
                            InventoryGoodBalanceWarehouse.Quantity += rows[0].Quantity;
                            if (InventoryGoodBalanceWarehouse.Quantity == 0)
                            {
                                _context.InventoryGoodsBalancesWarehouses.Remove(InventoryGoodBalanceWarehouse);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesWarehouses.Update(InventoryGoodBalanceWarehouse);
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
                        Tag = new { receiptToWarehouseDocument.Id }
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

        // PUT: api/ReceiptsWarehousesDocuments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceiptWarehouseDocument(int id, ReceiptToWarehouseDocumentModel ajaxReceiptWarehouseDocument)
        {
            ajaxReceiptWarehouseDocument.Information = ajaxReceiptWarehouseDocument.Information.Trim();
            ajaxReceiptWarehouseDocument.Name = ajaxReceiptWarehouseDocument.Name.Trim();

            if (!ModelState.IsValid
                || id != ajaxReceiptWarehouseDocument.Id
                || id < 1
                || !await _context.Warehouses.AnyAsync(x => x.Id == ajaxReceiptWarehouseDocument.WarehouseReceiptId))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            ReceiptToWarehouseDocumentModel ReceiptWarehouseDocumentDb = await _context.ReceiptesGoodsToWarehousesDocuments
                .Where(x => x.Id == id)
                .Include(x => x.Author)
                .Include(x => x.WarehouseReceipt)
                .FirstOrDefaultAsync();

            if (ReceiptWarehouseDocumentDb is null)
            {
                _logger.LogError("Документ поступления не найден в БД. id=", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (ReceiptWarehouseDocumentDb.Information == ajaxReceiptWarehouseDocument.Information && ReceiptWarehouseDocumentDb.WarehouseReceiptId == ajaxReceiptWarehouseDocument.WarehouseReceiptId)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "В документе нет изменений. Сохранению подлежат [склад поступления] и [комментарий/примечание]",
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
                    if (ReceiptWarehouseDocumentDb.WarehouseReceiptId != ajaxReceiptWarehouseDocument.WarehouseReceiptId)
                    {
                        foreach (var sel in selectorRows)
                        {
                            InventoryBalancesWarehousesAnalyticalModel ballance = await _context.InventoryGoodsBalancesWarehouses
                                .FirstOrDefaultAsync(InventoryBalance =>
                                InventoryBalance.WarehouseId == ReceiptWarehouseDocumentDb.WarehouseReceiptId
                                && InventoryBalance.GoodId == sel.row.GoodId
                                && InventoryBalance.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",
                                    WarehouseId = ReceiptWarehouseDocumentDb.WarehouseReceiptId,
                                    GoodId = sel.row.GoodId,
                                    UnitId = sel.row.UnitId,
                                    Quantity = sel.row.Quantity * -1
                                };
                                _context.InventoryGoodsBalancesWarehouses.Add(ballance);
                            }
                            else
                            {
                                ballance.Quantity -= sel.row.Quantity;
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

                            ballance = await _context.InventoryGoodsBalancesWarehouses.FirstOrDefaultAsync(x => x.WarehouseId == ajaxReceiptWarehouseDocument.WarehouseReceiptId && x.GoodId == sel.row.GoodId && x.UnitId == sel.row.UnitId);
                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",
                                    WarehouseId = ajaxReceiptWarehouseDocument.WarehouseReceiptId,
                                    GoodId = sel.row.GoodId,
                                    UnitId = sel.row.UnitId,
                                    Quantity = sel.row.Quantity
                                };
                                _context.InventoryGoodsBalancesWarehouses.Add(ballance);
                            }
                            else
                            {
                                ballance.Quantity += sel.row.Quantity;
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
                        }
                    }

                    ReceiptWarehouseDocumentDb.WarehouseReceiptId = ajaxReceiptWarehouseDocument.WarehouseReceiptId;
                    ReceiptWarehouseDocumentDb.Information = ajaxReceiptWarehouseDocument.Information;
                    _context.ReceiptesGoodsToWarehousesDocuments.Update(ReceiptWarehouseDocumentDb);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = true,
                        Info = "Документ сохранён",
                        Status = StylesMessageEnum.success.ToString(),
                        Tag = new { ReceiptWarehouseDocumentDb.Id, ReceiptWarehouseDocumentDb.Information, ReceiptWarehouseDocumentDb.WarehouseReceiptId }
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
        /// <param name="ajaxDocumentRow">строка документа. существующая строка удаляется, а новая добалвяется</param>
        /// <param name="id">идентификатор документа</param>
        // PATCH: api/ReceiptsWarehousesDocuments/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<object>> PatchRowDocument([FromBody]RowGoodMovementRegisterModel ajaxDocumentRow, [FromRoute] int id)
        {
            _logger.LogInformation($"Запрос {((ajaxDocumentRow.Id > 0) ? "удаления" : "добавления")} строки. Документ поступления #[{id}]");
            string msg;

            if (id < 1 || id != ajaxDocumentRow.BodyDocumentId

                // проверка наличия документа-владельца
                || !await _context.ReceiptesGoodsToWarehousesDocuments.AnyAsync(x => x.Id == id)
                // если не указан id - значит строка для добавления => указаные [good] и [unit] должны существовать в БД, а количество движения должно быть отличным от нуля
                || (ajaxDocumentRow.Id == 0 && (ajaxDocumentRow.Quantity == 0 || !await _context.Goods.AnyAsync(x => x.Id == ajaxDocumentRow.GoodId) || !await _context.Units.AnyAsync(x => x.Id == ajaxDocumentRow.UnitId)))
                // если id указан - значит строку нужно удалить => проверяем существования нужной строки
                || (ajaxDocumentRow.Id > 0 && !await _context.GoodMovementDocumentRows.AnyAsync(x => x.Id == ajaxDocumentRow.Id && x.BodyDocumentId == id)))
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

            if (ajaxDocumentRow.Id == 0 && await _context.GoodMovementDocumentRows.CountAsync(x => x.BodyDocumentId == id) > AppOptions.MaxNumRowsDocument + 1)
            {
                msg = $"Добавление новой строки к документу невозможно. Достигнут предельный лимит количества строк: " + AppOptions.MaxNumRowsDocument;
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            ReceiptToWarehouseDocumentModel warehouseDocumentDb = await _context.ReceiptesGoodsToWarehousesDocuments
            .Where(x => x.Id == id)
            .Include(x => x.WarehouseReceipt)
            .Include(x => x.RowsDocument)
            .FirstOrDefaultAsync();

            UnitGoodObjectModel unit;
            GoodObjectModel good;

            if (ajaxDocumentRow.Id > 0)
            {
                var sel = _context.GoodMovementDocumentRows
                    .Where(x => x.Id == ajaxDocumentRow.Id)
                    .Include(x => x.Good)
                    .Join(_context.Units, row => row.UnitId, unit => unit.Id, (row, unit) => new { row, unit })
                    .FirstOrDefault();

                using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        _context.GoodMovementDocumentRows.Remove(sel.row);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("документ из БД удалён. требуется отразить движение в количественном балансе...");

                        if (sel.row.Quantity != 0)
                        {
                            _logger.LogInformation($"Отражение удаления строки документа поступления номенклатуры в остатках. [склад: ${warehouseDocumentDb.WarehouseReceipt.Name}][номенклатура: ${sel.row.Good.Name} ${sel.row.Quantity} ${sel.unit.Name}]");
                            InventoryBalancesWarehousesAnalyticalModel InventoryGoodBalanceWarehouse = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(InventoryBalance => InventoryBalance.GoodId == sel.row.GoodId && InventoryBalance.UnitId == sel.row.UnitId && InventoryBalance.WarehouseId == warehouseDocumentDb.WarehouseReceiptId);
                            if (InventoryGoodBalanceWarehouse is null)
                            {
                                _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры: {sel.row.Quantity * -1}");
                                InventoryGoodBalanceWarehouse = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = "*",
                                    WarehouseId = warehouseDocumentDb.WarehouseReceiptId,
                                    GoodId = sel.row.GoodId,
                                    Quantity = sel.row.Quantity * -1,
                                    UnitId = sel.row.UnitId
                                };
                                _context.InventoryGoodsBalancesWarehouses.Add(InventoryGoodBalanceWarehouse);
                            }
                            else
                            {
                                double result = InventoryGoodBalanceWarehouse.Quantity - sel.row.Quantity;
                                _logger.LogInformation($"изменение измерителя: {InventoryGoodBalanceWarehouse.Quantity}-{sel.row.Quantity}={result}");
                                InventoryGoodBalanceWarehouse.Quantity = result;
                                if (result == 0)
                                {
                                    _logger.LogInformation("Накопитель с нулевым балансом подлежит удалению...");
                                    _context.InventoryGoodsBalancesWarehouses.Remove(InventoryGoodBalanceWarehouse);
                                }
                                else
                                {
                                    _context.InventoryGoodsBalancesWarehouses.Update(InventoryGoodBalanceWarehouse);
                                }
                            }

                            _context.SaveChanges();
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
            else
            {
                unit = await _context.Units.FindAsync(ajaxDocumentRow.UnitId);
                good = await _context.Goods.FindAsync(ajaxDocumentRow.GoodId);

                _logger.LogInformation("Открытие транзакционной сессии для добавления строки регистра для документа поступления");
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        RowGoodMovementRegisterModel warehouseRowDocumentDb = _context.GoodMovementDocumentRows
                            .Where(x => x.BodyDocumentId == id && x.GoodId == good.Id && x.UnitId == unit.Id)
                            .FirstOrDefault();

                        if (warehouseRowDocumentDb is null)
                        {
                            await _context.GoodMovementDocumentRows.AddAsync(ajaxDocumentRow);
                        }
                        else
                        {
                            warehouseRowDocumentDb.Quantity += ajaxDocumentRow.Quantity;
                            if (warehouseRowDocumentDb.Quantity != 0)
                            {
                                _context.GoodMovementDocumentRows.Update(warehouseRowDocumentDb);
                            }
                            else
                            {
                                _context.GoodMovementDocumentRows.Remove(warehouseRowDocumentDb);
                            }
                        }
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Отражение добавления строки документа поступления номенклатуры в остатках. [склад: ${warehouseDocumentDb.WarehouseReceipt.Name}][номенклатура: ${good.Name} ${ajaxDocumentRow.Quantity} ${unit.Name}]");
                        InventoryBalancesWarehousesAnalyticalModel InventoryGoodBalanceWarehouse = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(InventoryBalance => InventoryBalance.GoodId == ajaxDocumentRow.GoodId && InventoryBalance.UnitId == ajaxDocumentRow.UnitId && InventoryBalance.WarehouseId == warehouseDocumentDb.WarehouseReceiptId);
                        if (InventoryGoodBalanceWarehouse is null)
                        {
                            _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры: {ajaxDocumentRow.Quantity}");
                            InventoryGoodBalanceWarehouse = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = warehouseDocumentDb.WarehouseReceiptId,
                                GoodId = ajaxDocumentRow.GoodId,
                                Quantity = ajaxDocumentRow.Quantity,
                                UnitId = ajaxDocumentRow.UnitId
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(InventoryGoodBalanceWarehouse);
                        }
                        else
                        {
                            double result = InventoryGoodBalanceWarehouse.Quantity + ajaxDocumentRow.Quantity;
                            _logger.LogInformation($"изменение измерителя: {InventoryGoodBalanceWarehouse.Quantity}+{ajaxDocumentRow.Quantity}={result}");
                            InventoryGoodBalanceWarehouse.Quantity = result;
                            _context.InventoryGoodsBalancesWarehouses.Update(InventoryGoodBalanceWarehouse);
                        }
                        await _context.SaveChangesAsync();

                        await transaction.CommitAsync();
                        _logger.LogInformation("Транзакционная сессия для добавления строки регистра для документа поступления - успешно завершена => строка документа добавлена");
                        return new ObjectResult(new ServerActionResult()
                        {
                            Success = true,
                            Info = "Строка документа добавлена",
                            Status = StylesMessageEnum.success.ToString(),
                            Tag = new { ajaxDocumentRow.Id }
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

        // DELETE: api/ReceiptsWarehousesDocuments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteReceiptWarehouseDocument(int id)
        {
            string msg;
            ReceiptToWarehouseDocumentModel doc = await _context.ReceiptesGoodsToWarehousesDocuments
                .Where(receiptDoc => receiptDoc.Id == id)
                .Include(receiptDoc => receiptDoc.RowsDocument)
                .FirstOrDefaultAsync();

            if (doc == null)
            {
                msg = $"Документ поступления не найден в БД. id={id}";
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
                    List<RowGoodMovementRegisterModel> rows = doc.RowsDocument;
                    rows.Where(row => row.Quantity != 0).ToList().ForEach(row =>
                      {
                          InventoryBalancesWarehousesAnalyticalModel InventoryGoodBalanceWarehouse = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(iBalance =>
                          iBalance.GoodId == row.GoodId
                          && iBalance.UnitId == row.UnitId
                          && iBalance.WarehouseId == doc.WarehouseReceiptId);

                          if (InventoryGoodBalanceWarehouse is null)
                          {
                              InventoryGoodBalanceWarehouse = new InventoryBalancesWarehousesAnalyticalModel()
                              {
                                  Name = "*",
                                  WarehouseId = doc.WarehouseReceiptId,
                                  GoodId = row.GoodId,
                                  Quantity = row.Quantity * -1,
                                  UnitId = row.UnitId
                              };
                              _context.InventoryGoodsBalancesWarehouses.Add(InventoryGoodBalanceWarehouse);
                          }
                          else
                          {
                              InventoryGoodBalanceWarehouse.Quantity -= row.Quantity;
                              if (InventoryGoodBalanceWarehouse.Quantity != 0)
                              {
                                  _context.InventoryGoodsBalancesWarehouses.Update(InventoryGoodBalanceWarehouse);
                              }
                              else
                              {
                                  _context.InventoryGoodsBalancesWarehouses.Remove(InventoryGoodBalanceWarehouse);
                              }

                          }
                          _context.SaveChanges();
                      });
                    _context.WarehouseDocuments.Remove(doc);
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
