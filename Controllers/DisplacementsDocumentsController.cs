////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    /// <summary>
    /// Документы внутреннего перемещения со склада на склад
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelManager")]
    public class DisplacementsDocumentsController : ControllerBase
    {
        private readonly AppConfig AppOptions;
        private readonly AppDataBaseContext _context;
        private readonly ILogger<DisplacementsDocumentsController> _logger;
        private readonly UserObjectModel _user;

        public DisplacementsDocumentsController(AppDataBaseContext context, ILogger<DisplacementsDocumentsController> logger, SessionUser session, IOptions<AppConfig> options)
        {
            AppOptions = options.Value;
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/DisplacementsDocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDisplacementsWarehouses([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<InternalDisplacementWarehouseDocumentModel> documents = _context.InternalDisplacementWarehouseDocuments.AsQueryable();

            pagingParameters.Init(await documents.CountAsync());
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            documents = documents.OrderByDescending(doc => doc.Id);
            if (pagingParameters.PageNum > 1)
                documents = documents.Skip(pagingParameters.Skip);

            documents = documents
                .Take(pagingParameters.PageSize)
                .Include(doc => doc.Author)
                .Include(doc => doc.WarehouseReceipt);

            var exDoc = documents.Join(_context.Warehouses, doc => doc.WarehouseDebitingId, warehouse => warehouse.Id, (doc, warehouseDebiting) => new
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
                WarehouseDebiting = new
                {
                    warehouseDebiting.Id,
                    warehouseDebiting.Name,
                    warehouseDebiting.Information
                },
                WarehouseReceipt = new
                {
                    doc.WarehouseReceipt.Id,
                    doc.WarehouseReceipt.Name,
                    doc.WarehouseReceipt.Information
                },
                CountRows = _context.GoodMovementDocumentRows.Count(row => row.BodyDocumentId == doc.Id),
                doc.Discriminator
            });

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос документов перемещения обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await exDoc.ToListAsync()
            });
        }

        // GET: api/DisplacementsDocuments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetDisplacementWarehouseDocument(int id)
        {
            var doc = await _context.InternalDisplacementWarehouseDocuments
                .Include(x => x.WarehouseReceipt)
                .Include(x => x.Author)
                .Join(_context.Warehouses, docObj => docObj.WarehouseDebitingId, wareObj => wareObj.Id, (docObj, wareObj) => new
                {
                    docObj.Id,
                    docObj.Name,
                    docObj.Information,
                    WarehouseReceipt = new
                    {
                        docObj.WarehouseReceipt.Id,
                        docObj.WarehouseReceipt.Name,
                        docObj.WarehouseReceipt.Information
                    },
                    WarehouseDebiting = new
                    {
                        wareObj.Id,
                        wareObj.Name,
                        wareObj.Information
                    },
                    Author = new
                    {
                        docObj.Author.Id,
                        docObj.Author.Name,
                        docObj.Author.Information
                    }
                })
                .FirstOrDefaultAsync(x => x.Id == id);

            if (doc == null)
            {
                _logger.LogError("Документ перемещения не найден: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Документ перемещения не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            var rows = await _context.GoodMovementDocumentRows
                    .Where(row => row.BodyDocumentId == doc.Id)
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
                Info = "Запрос документа перемещения обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    doc.Id,
                    doc.Name,
                    doc.WarehouseDebiting,
                    doc.WarehouseReceipt,
                    doc.Information,
                    doc.Author,

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
                    }).ToListAsync(),
                    Warehouses = await _context.Warehouses.Select(x => new { x.Id, x.Name, x.Information }).ToListAsync()
                }
            });
        }

        // POST: api/DisplacementsDocuments
        [HttpPost]
        public async Task<ActionResult<object>> PostDisplacementWarehouseDocument(InternalDisplacementWarehouseDocumentModel ajaxDoc)
        {
            if (ajaxDoc.WarehouseDebitingId == 0
                || ajaxDoc.WarehouseReceiptId == 0
                || ajaxDoc.WarehouseDebitingId == ajaxDoc.WarehouseReceiptId
                || !await _context.Warehouses.AnyAsync(x => x.Id == ajaxDoc.WarehouseDebitingId)
                || !await _context.Warehouses.AnyAsync(x => x.Id == ajaxDoc.WarehouseReceiptId))
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
                .FirstOrDefaultAsync(x => x.Discriminator == nameof(WarehouseDocumentsModel) && x.Name == nameof(InternalDisplacementWarehouseDocumentModel) && x.AuthorId == _user.Id);

            if (parentDoc == null || parentDoc.RowsDocument.Count == 0)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Отсутствует основание для создания документа-перемещения. Укажите в документе склад поступления/списания и добавьте строки в табличную часть",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                try
                {
                    List<RowGoodMovementRegisterModel> rows = parentDoc.RowsDocument;
                    InternalDisplacementWarehouseDocumentModel doc = new InternalDisplacementWarehouseDocumentModel()
                    {
                        Name = "~ перемещение",
                        Information = parentDoc.Information,
                        WarehouseDebitingId = ajaxDoc.WarehouseDebitingId,
                        WarehouseReceiptId = ajaxDoc.WarehouseReceiptId,
                        AuthorId = _user.Id
                    };

                    _context.InternalDisplacementWarehouseDocuments.Add(doc);
                    await _context.SaveChangesAsync();
                    while (rows.Count > 0)
                    {
                        if (rows[0].Quantity == 0)
                        {
                            rows.RemoveAt(0);
                            continue;
                        }
                        
                        int rowBodyDocumentId = doc.Id;
                        int rowGoodId = rows[0].GoodId;
                        double rowQuantity = rows[0].Quantity;
                        int rowUnitId = rows[0].UnitId;

                        InventoryBalancesWarehousesAnalyticalModel iBalance;
                        #region списание количества номенклатуры со склада списания

                        iBalance = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(InventoryBalance =>
                        InventoryBalance.GoodId == rowGoodId
                        && InventoryBalance.UnitId == rowUnitId
                        && InventoryBalance.WarehouseId == doc.WarehouseDebitingId);

                        if (iBalance is null)
                        {
                            iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = doc.WarehouseDebitingId,
                                GoodId = rowGoodId,
                                Quantity = rowQuantity * -1,
                                UnitId = rowUnitId
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                        }
                        else
                        {
                            iBalance.Quantity -= rowQuantity;
                            if (iBalance.Quantity == 0)
                            {
                                _context.InventoryGoodsBalancesWarehouses.Remove(iBalance);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesWarehouses.Update(iBalance);
                            }
                        }
                        await _context.SaveChangesAsync();

                        #endregion
                        #region зачисление количества номенклатуры на склад приёмник

                        iBalance = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(InventoryBalance =>
                        InventoryBalance.GoodId == rowGoodId
                        && InventoryBalance.UnitId == rowUnitId
                        && InventoryBalance.WarehouseId == doc.WarehouseReceiptId);
                        
                        if (iBalance is null)
                        {
                            iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = doc.WarehouseReceiptId,
                                GoodId = rowGoodId,
                                Quantity = rowQuantity,
                                UnitId = rowUnitId
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                        }
                        else
                        {
                            iBalance.Quantity += rowQuantity;
                            if (iBalance.Quantity == 0)
                            {
                                _context.InventoryGoodsBalancesWarehouses.Remove(iBalance);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesWarehouses.Update(iBalance);
                            }
                        }
                        rows[0].BodyDocumentId = doc.Id;
                        _context.GoodMovementDocumentRows.Update(rows[0]);
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

        // PUT: api/DisplacementsDocuments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDisplacementWarehouseDocument(int id, InternalDisplacementWarehouseDocumentModel ajaxDisplacementWarehouseDocument)
        {
            ajaxDisplacementWarehouseDocument.Information = ajaxDisplacementWarehouseDocument.Information.Trim();

            if (!ModelState.IsValid
                || id != ajaxDisplacementWarehouseDocument.Id
                || id < 1
                || !await _context.Warehouses.AnyAsync(x => x.Id == ajaxDisplacementWarehouseDocument.WarehouseReceiptId || x.Id == ajaxDisplacementWarehouseDocument.WarehouseDebitingId))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            InternalDisplacementWarehouseDocumentModel doc = await _context.InternalDisplacementWarehouseDocuments
                .Where(x => x.Id == id)
                .Include(x => x.Author)
                .Include(x => x.WarehouseReceipt)
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

            if (doc.Information == ajaxDisplacementWarehouseDocument.Information
                && doc.WarehouseReceiptId == ajaxDisplacementWarehouseDocument.WarehouseReceiptId
                && doc.WarehouseDebitingId == ajaxDisplacementWarehouseDocument.WarehouseDebitingId)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
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
                    InventoryBalancesWarehousesAnalyticalModel ballance;
                    foreach (var sel in selectorRows)
                    {
                        #region при изменении склада списания
                        if (doc.WarehouseDebitingId != ajaxDisplacementWarehouseDocument.WarehouseDebitingId)
                        {
                            #region возврат количества номенклатуры на предыдущий склад списания

                            ballance = await _context.InventoryGoodsBalancesWarehouses
                                .FirstOrDefaultAsync(InventoryBalance =>
                                InventoryBalance.WarehouseId == doc.WarehouseDebitingId
                                && InventoryBalance.GoodId == sel.row.GoodId
                                && InventoryBalance.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",
                                    WarehouseId = doc.WarehouseDebitingId,
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

                            #region списание количества номенклатуры с нового склада списания

                            ballance = await _context.InventoryGoodsBalancesWarehouses.FirstOrDefaultAsync(x =>
                            x.WarehouseId == ajaxDisplacementWarehouseDocument.WarehouseDebitingId
                            && x.GoodId == sel.row.GoodId
                            && x.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",
                                    WarehouseId = ajaxDisplacementWarehouseDocument.WarehouseDebitingId,
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

                        #region при изменении склада поступления
                        if (doc.WarehouseReceiptId != ajaxDisplacementWarehouseDocument.WarehouseReceiptId)
                        {
                            #region списание с предыдущего склада поступления

                            ballance = await _context.InventoryGoodsBalancesWarehouses
                                .FirstOrDefaultAsync(InventoryBalance =>
                                InventoryBalance.WarehouseId == doc.WarehouseReceiptId
                                && InventoryBalance.GoodId == sel.row.GoodId
                                && InventoryBalance.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",
                                    WarehouseId = doc.WarehouseReceiptId,
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

                            #endregion

                            #region зачисление на новый склад поступления

                            ballance = await _context.InventoryGoodsBalancesWarehouses.FirstOrDefaultAsync(x =>
                            x.WarehouseId == ajaxDisplacementWarehouseDocument.WarehouseReceiptId
                            && x.GoodId == sel.row.GoodId
                            && x.UnitId == sel.row.UnitId);

                            if (ballance is null)
                            {
                                ballance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = $"[good: {sel.row.Good.Name}]/[unit: {sel.unit.Name}]",
                                    WarehouseId = ajaxDisplacementWarehouseDocument.WarehouseReceiptId,
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

                            #endregion
                        }
                        #endregion
                    }

                    doc.WarehouseReceiptId = ajaxDisplacementWarehouseDocument.WarehouseReceiptId;
                    doc.WarehouseDebitingId = ajaxDisplacementWarehouseDocument.WarehouseDebitingId;
                    doc.Information = ajaxDisplacementWarehouseDocument.Information;
                    _context.InternalDisplacementWarehouseDocuments.Update(doc);
                    await _context.SaveChangesAsync();

                    transaction.Commit();
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = true,
                        Info = "Документ сохранён",
                        Status = StylesMessageEnum.success.ToString(),
                        Tag = new { doc.Id, doc.Information, doc.WarehouseReceiptId }
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
        // PATCH: api/DisplacementsDocuments/5
        [HttpPatch("{id}")]
        public async Task<ActionResult<object>> PatchRowDocument([FromBody]RowGoodMovementRegisterModel ajaxDocumentRow, [FromRoute] int id)
        {
            _logger.LogInformation($"Запрос {((ajaxDocumentRow.Id > 0) ? "удаления" : "добавления")} строки. Документ складского перемещения #[{id}]");
            string msg;

            if (id < 1 || id != ajaxDocumentRow.BodyDocumentId

                // проверка наличия документа-владельца
                || !await _context.InternalDisplacementWarehouseDocuments.AnyAsync(x => x.Id == id)
                // если не указан id - значит строка для добавления => указаные [good] и [unit] должны существовать в БД, а количество движения должно быть отличным от нуля
                || (ajaxDocumentRow.Id == 0 && (ajaxDocumentRow.Quantity == 0 || !await _context.Goods.AnyAsync(x => x.Id == ajaxDocumentRow.GoodId) || !await _context.Units.AnyAsync(x => x.Id == ajaxDocumentRow.UnitId)))
                // если id указан - значит строку нужно удалить => проверяем существования нужной строки
                || (ajaxDocumentRow.Id > 0 && !await _context.GoodMovementDocumentRows.AnyAsync(x => x.Id == ajaxDocumentRow.Id && x.BodyDocumentId == ajaxDocumentRow.BodyDocumentId)))
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

            var docDb = await _context.InternalDisplacementWarehouseDocuments
                .Where(x => x.Id == id)
                .Include(x => x.WarehouseReceipt)
                .Include(x => x.RowsDocument)
                .Join(_context.Warehouses, doc => doc.WarehouseDebitingId, warehouse => warehouse.Id, (doc, debWarehouse) => new
                {
                    ReceiptWarehouse = new { doc.WarehouseReceipt.Id, doc.WarehouseReceipt.Name, doc.WarehouseReceipt.Information },
                    DebitingWarehouse = new { debWarehouse.Id, debWarehouse.Name, debWarehouse.Information }
                })
                .FirstOrDefaultAsync();

            UnitGoodObjectModel unit;
            GoodObjectModel good;

            #region удаление строки
            if (ajaxDocumentRow.Id > 0)
            {
                var rowDb = await _context.GoodMovementDocumentRows
                    .Include(row => row.Good)
                    .Join(_context.Units, row => row.UnitId, unit => unit.Id, (row, unit) => new { row, unit })
                    .FirstOrDefaultAsync(sel => sel.row.Id == ajaxDocumentRow.Id);

                using (IDbContextTransaction transaction = _context.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    try
                    {
                        _context.GoodMovementDocumentRows.Remove(rowDb.row);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("документ из БД удалён. требуется отразить движение в количественном балансе...");

                        if (rowDb.row.Quantity != 0)
                        {
                            _logger.LogInformation($"Отражение удаления строки документа поступления номенклатуры в остатках. [склад: ${docDb.DebitingWarehouse.Name}]->[склад: ${docDb.ReceiptWarehouse.Name}] [номенклатура: ${rowDb.row.Good.Name} ${rowDb.row.Quantity} ${rowDb.unit.Name}]");

                            InventoryBalancesWarehousesAnalyticalModel iBalance;
                            #region списание количества у склада поступления

                            iBalance = _context.InventoryGoodsBalancesWarehouses
                                .FirstOrDefault(InventoryBalance =>
                                InventoryBalance.GoodId == rowDb.row.GoodId
                                && InventoryBalance.UnitId == rowDb.row.UnitId
                                && InventoryBalance.WarehouseId == docDb.ReceiptWarehouse.Id);

                            if (iBalance is null)
                            {
                                _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры для склада поступления: {rowDb.row.Quantity * -1}");
                                iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = "*",
                                    WarehouseId = docDb.ReceiptWarehouse.Id,
                                    GoodId = rowDb.row.GoodId,
                                    Quantity = rowDb.row.Quantity * -1,
                                    UnitId = rowDb.row.UnitId
                                };
                                _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                            }
                            else
                            {
                                double result = iBalance.Quantity - rowDb.row.Quantity;
                                _logger.LogInformation($"изменение измерителя: {iBalance.Quantity}-{rowDb.row.Quantity}={result}");
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
                            #region возврат количества складу списания

                            iBalance = _context.InventoryGoodsBalancesWarehouses
                                .FirstOrDefault(balance =>
                                balance.GoodId == rowDb.row.GoodId
                                && balance.UnitId == rowDb.row.UnitId
                                && balance.WarehouseId == docDb.DebitingWarehouse.Id);

                            if (iBalance is null)
                            {
                                _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры: {rowDb.row.Quantity}");
                                iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                                {
                                    Name = "*",
                                    WarehouseId = docDb.DebitingWarehouse.Id,
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
                unit = await _context.Units.FindAsync(ajaxDocumentRow.UnitId);
                good = await _context.Goods.FindAsync(ajaxDocumentRow.GoodId);

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
                            await _context.GoodMovementDocumentRows.AddAsync(ajaxDocumentRow);
                        }
                        else
                        {
                            wRowDocumentDb.Quantity += ajaxDocumentRow.Quantity;
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

                        _logger.LogInformation($"Отражение добавления строки документа поступления номенклатуры в остатках. [склад: ${docDb.DebitingWarehouse.Name}]->[склад: ${docDb.ReceiptWarehouse.Name}] [номенклатура: ${good.Name} ${ajaxDocumentRow.Quantity} ${unit.Name}]");

                        InventoryBalancesWarehousesAnalyticalModel iBalance;
                        #region начисление количества складу поступления

                        iBalance = _context.InventoryGoodsBalancesWarehouses
                            .FirstOrDefault(InventoryBalance =>
                            InventoryBalance.GoodId == good.Id
                            && InventoryBalance.UnitId == unit.Id
                            && InventoryBalance.WarehouseId == docDb.ReceiptWarehouse.Id);

                        if (iBalance is null)
                        {
                            _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры для склада поступления: {ajaxDocumentRow.Quantity}");
                            iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = docDb.ReceiptWarehouse.Id,
                                GoodId = good.Id,
                                Quantity = ajaxDocumentRow.Quantity,
                                UnitId = unit.Id
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                        }
                        else
                        {
                            double result = iBalance.Quantity + ajaxDocumentRow.Quantity;
                            _logger.LogInformation($"изменение измерителя: {iBalance.Quantity}+{ajaxDocumentRow.Quantity}={result}");
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
                        #region списание количества у склада списания

                        iBalance = _context.InventoryGoodsBalancesWarehouses
                            .FirstOrDefault(balance =>
                            balance.GoodId == good.Id
                            && balance.UnitId == unit.Id
                            && balance.WarehouseId == docDb.DebitingWarehouse.Id);

                        if (iBalance is null)
                        {
                            _logger.LogInformation($"Инициализация измерителя складских остатков номенклаутры: {ajaxDocumentRow.Quantity}");
                            iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = docDb.DebitingWarehouse.Id,
                                GoodId = good.Id,
                                Quantity = ajaxDocumentRow.Quantity * -1,
                                UnitId = unit.Id
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                        }
                        else
                        {
                            double result = iBalance.Quantity - ajaxDocumentRow.Quantity;
                            _logger.LogInformation($"изменение измерителя: {iBalance.Quantity}-{ajaxDocumentRow.Quantity}={result}");
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
            #endregion
        }

        // DELETE: api/DisplacementsDocuments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteDisplacementWarehouseDocument(int id)
        {
            string msg;

            var selDoc = await _context.InternalDisplacementWarehouseDocuments
                .Where(receiptDoc => receiptDoc.Id == id)
                .Include(x => x.WarehouseReceipt)
                .Include(x => x.RowsDocument)
                .Join(_context.Warehouses, doc => doc.WarehouseDebitingId, warehouse => warehouse.Id, (Document, DebitingWarehouse) => new
                {
                    Document,
                    DebitingWarehouse = new { DebitingWarehouse.Id, DebitingWarehouse.Name, DebitingWarehouse.Information }
                })
                .FirstOrDefaultAsync();

            if (selDoc == null)
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
                    List<RowGoodMovementRegisterModel> rows = selDoc.Document.RowsDocument;
                    rows.Where(row => row.Quantity != 0).ToList().ForEach(row =>
                    {
                        InventoryBalancesWarehousesAnalyticalModel iBalance;

                        #region спсиание количества номенклатуры со склада поступления

                        iBalance = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(iBalance =>
                        iBalance.GoodId == row.GoodId
                        && iBalance.UnitId == row.UnitId
                        && iBalance.WarehouseId == selDoc.Document.WarehouseReceiptId);

                        if (iBalance is null)
                        {
                            iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = selDoc.Document.WarehouseReceiptId,
                                GoodId = row.GoodId,
                                Quantity = row.Quantity * -1,
                                UnitId = row.UnitId
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                        }
                        else
                        {
                            iBalance.Quantity -= row.Quantity;
                            if (iBalance.Quantity != 0)
                            {
                                _context.InventoryGoodsBalancesWarehouses.Update(iBalance);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesWarehouses.Remove(iBalance);
                            }
                        }
                        _context.SaveChanges();

                        #endregion

                        #region возврат количества номенклатуры складу списания

                        iBalance = _context.InventoryGoodsBalancesWarehouses.FirstOrDefault(iBalance =>
                        iBalance.GoodId == row.GoodId
                        && iBalance.UnitId == row.UnitId
                        && iBalance.WarehouseId == selDoc.DebitingWarehouse.Id);

                        if (iBalance is null)
                        {
                            iBalance = new InventoryBalancesWarehousesAnalyticalModel()
                            {
                                Name = "*",
                                WarehouseId = selDoc.DebitingWarehouse.Id,
                                GoodId = row.GoodId,
                                Quantity = row.Quantity,
                                UnitId = row.UnitId
                            };
                            _context.InventoryGoodsBalancesWarehouses.Add(iBalance);
                        }
                        else
                        {
                            iBalance.Quantity += row.Quantity;
                            if (iBalance.Quantity != 0)
                            {
                                _context.InventoryGoodsBalancesWarehouses.Update(iBalance);
                            }
                            else
                            {
                                _context.InventoryGoodsBalancesWarehouses.Remove(iBalance);
                            }
                        }
                        _context.SaveChanges();

                        #endregion
                    });
                    _context.WarehouseDocuments.Remove(selDoc.Document);
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
