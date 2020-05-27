////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    /// <summary>
    /// Работа с временными документами. Временные документы обеспечивают сохранения промежуточных состояний формы создания нового документа (поступление,перемещение).
    /// В том числе табличная часть
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelManager")]
    public class WarehouseDocumentsController : ControllerBase
    {
        private readonly AppConfig AppOptions;
        private readonly AppDataBaseContext _context;
        private readonly ILogger<WarehouseDocumentsController> _logger;
        private readonly UserObjectModel _user;

        public WarehouseDocumentsController(AppDataBaseContext context, ILogger<WarehouseDocumentsController> logger, SessionUser session, IOptions<AppConfig> options)
        {
            AppOptions = options.Value;
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        /// <summary>
        /// Получить записи/строки журнала складских документов
        /// </summary>
        /// <param name="pagingParameters">параметры пагинатора</param>
        // GET: api/WarehouseDocuments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetWarehouseDocuments([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<WarehouseDocumentsModel> documents = _context.WarehouseDocuments.Where(x => x.Discriminator.ToLower() != nameof(WarehouseDocumentsModel).ToLower()).AsQueryable();

            pagingParameters.Init(await documents.CountAsync());
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            documents = documents.OrderByDescending(doc => doc.Id);
            if (pagingParameters.PageNum > 1)
                documents = documents.Skip(pagingParameters.Skip);

            documents = documents
                .Take(pagingParameters.PageSize)
                .Include(doc => doc.Author)
                .Include(doc => doc.WarehouseReceipt);

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос складских документов обработан",
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
                    WarehouseReceipt = new
                    {
                        doc.WarehouseReceipt.Id,
                        doc.WarehouseReceipt.Name,
                        doc.WarehouseReceipt.Information
                    },
                    CountRows = _context.GoodMovementDocumentRows.Count(row => row.BodyDocumentId == doc.Id),
                    doc.Discriminator
                }).ToListAsync()
            });
        }

        /// <summary>
        /// Получить временный документ по имени типа
        /// </summary>
        /// <param name="id">имя типа данных документа</param>
        /// <returns></returns>
        // GET: api/WarehouseDocuments/ReceiptToWarehouseDocumentModel
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetWarehouseDocument([FromRoute] string id)
        {
            id = id.Trim();
            if (string.IsNullOrEmpty(id))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе: string.IsNullOrWhiteSpace(id)",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            var Units = await _context.Units
                .OrderBy(unit => unit.Name)
                .Select(unit => new
                {
                    unit.Id,
                    unit.Name,
                    unit.Information
                })
                .ToListAsync();

            var GroupsGoods = await _context.GroupsGoods
                .Include(group => group.Goods).ThenInclude(x => x.Unit)
                .Where(group => group.Goods.Count > 0)
                .OrderBy(group => group.Name)
                .Select(group => new
                {
                    group.Id,
                    group.Name,
                    group.Information,
                    Goods = group.Goods.OrderBy(good => good.Name).Select(good => new
                    {
                        good.Id,
                        good.Name,
                        good.Information,
                        Unit = new
                        {
                            good.Unit.Id,
                            good.Unit.Name,
                            good.Unit.Information
                        }
                    })
                }).ToListAsync();

            WarehouseDocumentsModel doc = await _context.WarehouseDocuments
                .Where(wDocument => wDocument.Discriminator == nameof(WarehouseDocumentsModel) && wDocument.Name == id && wDocument.AuthorId == _user.Id)
                .Include(wDocument => wDocument.WarehouseReceipt)
                .FirstOrDefaultAsync();

            if (doc == null)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = "Запрос временного складского документа обработан [" + id + "]. Документа нет => ответом будет пустышка.",
                    Status = StylesMessageEnum.secondary.ToString(),
                    Tag = new
                    {
                        Units,
                        GroupsGoods,

                        Name = "",
                        warehouseReceipt = new { id = 0, name = "" },
                        Information = "",
                        rows = Array.Empty<object>()
                    }
                });
            }

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос временного складского документа обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    Units,
                    GroupsGoods,

                    doc.Name,
                    warehouseReceipt = new 
                    { 
                        doc.WarehouseReceipt.Id, 
                        doc.WarehouseReceipt.Name, 
                        doc.WarehouseReceipt.Information 
                    },
                    doc.Information,
                    rows = await _context.GoodMovementDocumentRows
                    .Where(row => row.BodyDocumentId == doc.Id)
                    .Include(x => x.Good)
                    .Join(_context.Units, documentRow => documentRow.UnitId, unit => unit.Id, (documentRow, joinedUnit) => new { documentRow.Id, documentRow.Quantity, documentRow.Good, Unit = new { joinedUnit.Id, joinedUnit.Name } })
                    .Select(documentRow => new { documentRow.Id, documentRow.Quantity, Good = new { documentRow.Good.Id, documentRow.Good.Name }, documentRow.Unit }).OrderBy(documentRow => documentRow.Good.Name + documentRow.Unit.Name).ToListAsync()
                }
            });
        }

        /// <summary>
        /// Установка склада для временного документа (по имени типа даных)
        /// </summary>
        /// <param name="ajaxWarehouseDocument">склад</param>
        /// <param name="id">имя типа данных</param>
        /// <returns></returns>
        // PUT: api/WarehouseDocuments/ReceiptToWarehouseDocumentModel
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouseDocument([FromBody]WarehouseDocumentsModel ajaxWarehouseDocument, [FromRoute] string id)
        {
            ajaxWarehouseDocument.Name = ajaxWarehouseDocument.Name.Trim();
            ajaxWarehouseDocument.Information = ajaxWarehouseDocument.Information.Trim();

            id = id.Trim();

            if (string.IsNullOrEmpty(id) || !await _context.Warehouses.AnyAsync(x => x.Id == ajaxWarehouseDocument.WarehouseReceiptId))
            {
                _logger.LogInformation($"Запрос инициализации временного документа [type: '{id}']. Инициатор: " + _user.FullInfo);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            WarehouseDocumentsModel warehouseDocumentDb = await _context.WarehouseDocuments.Include(x => x.WarehouseReceipt).FirstOrDefaultAsync(x => x.Discriminator == nameof(WarehouseDocumentsModel) && x.Name.ToLower() == id.ToLower() && x.AuthorId == _user.Id);
            if (warehouseDocumentDb == null)
            {

                _logger.LogInformation($"Запрос инициализации временного документа [type: '{id}']. Инициатор: " + _user.FullInfo);
                warehouseDocumentDb = new WarehouseDocumentsModel()
                {
                    Name = id,
                    Information = ajaxWarehouseDocument.Information,
                    WarehouseReceiptId = ajaxWarehouseDocument.WarehouseReceiptId,
                    AuthorId = _user.Id
                };
                _context.WarehouseDocuments.Add(warehouseDocumentDb);
                _logger.LogInformation("создание <нового> документа");
            }
            else
            {
                warehouseDocumentDb.Information = ajaxWarehouseDocument.Information;
                warehouseDocumentDb.WarehouseReceiptId = ajaxWarehouseDocument.WarehouseReceiptId;
                _context.WarehouseDocuments.Update(warehouseDocumentDb);
                //_logger.LogInformation($"обновлёние <существующего> документа [#{warehouseDocumentDb.Id}]");
            }
            try
            {
                await _context.SaveChangesAsync();
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = "Временный документ (регистры номенклатуры) сохранён",
                    Status = StylesMessageEnum.secondary.ToString()
                });
            }
            catch (Exception ex)
            {
                string msg = $"Во время инициализации/сохранения временного документа произошла ошибка: {ex.Message}{(ex.InnerException is null ? "" : ". InnerException: " + ex.InnerException.Message)}";
                _logger.LogInformation(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.secondary.ToString()
                });
            }
        }

        /// <summary>
        /// манипуляция строками докумета (удалить/добавить)
        /// </summary>
        /// <param name="warehouseRowDocumentAjax">строка документа. существующая строка удаляется, а новая добалвяется</param>
        /// <param name="id">имя типа данных временного документа</param>
        // PATCH: api/WarehouseDocuments/ReceiptToWarehouseDocumentModel
        [HttpPatch("{id}")]
        public async Task<ActionResult<object>> PatchRowDocument([FromBody]RowGoodMovementRegisterModel warehouseRowDocumentAjax, [FromRoute] string id)
        {
            _logger.LogInformation($"Запрос {((warehouseRowDocumentAjax.Id > 0) ? "удаления" : "добавления")} строки временного документа [type: '{id}']. Инициатор: " + _user.FullInfo);
            string msg;
            id = id.Trim();

            if (string.IsNullOrWhiteSpace(id)

                // проверка наличия подходящего документа-владельца по имени типа
                || !await _context.GoodMovementDocuments.AnyAsync(x => x.Discriminator == nameof(WarehouseDocumentsModel) && x.Name.ToLower() == id.ToLower())
                // если не указан id - значит строка для добавления => указаные [good] и [unit] должны существовать в БД, а количество движения должно быть отличным от нуля
                || (warehouseRowDocumentAjax.Id == 0 && (warehouseRowDocumentAjax.Quantity == 0 || !await _context.Goods.AnyAsync(x => x.Id == warehouseRowDocumentAjax.GoodId) || !await _context.Units.AnyAsync(x => x.Id == warehouseRowDocumentAjax.UnitId)))
                // если id указан - значит строку нужно удалить => проверяем существования подходящей строки документа
                || (warehouseRowDocumentAjax.Id > 0 && !await _context.GoodMovementDocumentRows.Where(x => x.Id == warehouseRowDocumentAjax.Id).Include(x => x.BodyDocument).AnyAsync(x => x.BodyDocument.Discriminator == nameof(WarehouseDocumentsModel) && x.BodyDocument.Name.ToLower() == id.ToLower())))
            {
                msg = $"Ошибка проверки запроса. Задание над строкой временного документа отклонено";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (warehouseRowDocumentAjax.Id == 0 && await _context.GoodMovementDocumentRows.CountAsync(x => x.BodyDocument.Name.ToLower() == id.ToLower()) > AppOptions.MaxNumRowsDocument + 1)
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

            WarehouseDocumentsModel warehouseDocumentDb = await _context.WarehouseDocuments
                .Where(wDocument => wDocument.Discriminator == nameof(WarehouseDocumentsModel) && wDocument.Name == id && wDocument.AuthorId == _user.Id)
                .Include(wDocument => wDocument.WarehouseReceipt)
                .FirstOrDefaultAsync();

            if (warehouseRowDocumentAjax.Id > 0)
            {
                warehouseRowDocumentAjax = _context.GoodMovementDocumentRows.FirstOrDefault(x => x.Id == warehouseRowDocumentAjax.Id);
                _context.GoodMovementDocumentRows.Remove(warehouseRowDocumentAjax);
                await _context.SaveChangesAsync();
                msg = "Строка временного документа удалена";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = msg,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            RowGoodMovementRegisterModel warehouseRowDocumentDb = _context.GoodMovementDocumentRows.Where(x => x.BodyDocumentId == warehouseDocumentDb.Id && x.GoodId == warehouseRowDocumentAjax.GoodId && x.UnitId == warehouseRowDocumentAjax.UnitId).FirstOrDefault();
            if (warehouseRowDocumentDb is null)
            {
                _logger.LogInformation("добавлена <новая> строка");
                warehouseRowDocumentDb = new RowGoodMovementRegisterModel()
                {
                    BodyDocumentId = warehouseDocumentDb.Id,
                    GoodId = warehouseRowDocumentAjax.GoodId,
                    UnitId = warehouseRowDocumentAjax.UnitId,
                    Quantity = warehouseRowDocumentAjax.Quantity
                };
                _context.GoodMovementDocumentRows.Add(warehouseRowDocumentDb);
            }
            else
            {
                _logger.LogInformation("обнаружена <подобная> строка. количество будет добавлено к сущетсвующей строке");
                warehouseRowDocumentDb.Quantity += warehouseRowDocumentAjax.Quantity;
                if (warehouseRowDocumentDb.Quantity == 0)
                {
                    _logger.LogInformation("после операции сложения получился нуль. нулевая строка будет удалена");
                    _context.GoodMovementDocumentRows.Remove(warehouseRowDocumentDb);
                }
                else
                {
                    _context.GoodMovementDocumentRows.Update(warehouseRowDocumentDb);
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                msg = $"Операция над строкой временного документа завершилась ошибкой: {ex.Message}{(ex.InnerException is null ? "" : ". InnerException: " + ex.InnerException.Message)}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.success.ToString()
                });
            }

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Операция над строкой временного документа выполнена",
                Status = StylesMessageEnum.success.ToString()
            });
        }

        /// <summary>
        /// Удаление временного документа (вместе с его табличной частью)
        /// </summary>
        /// <param name="id">имя типа данных, который следует "очистить" от временного документа</param>
        // DELETE: api/WarehouseDocuments/ReceiptToWarehouseDocumentModel
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteWarehouseDocument(string id)
        {
            _logger.LogInformation($"Запрос удаления временного документа [type: '{id}']. Инициатор: " + _user.FullInfo);
            string msg;
            id = id.Trim();
            if (string.IsNullOrEmpty(id))
            {
                msg = $"Ошибка в запросе. Требуется передать имя модели (тип данных)";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            WarehouseDocumentsModel warehouseDocument = await _context.WarehouseDocuments.FirstOrDefaultAsync(x => x.Discriminator == nameof(WarehouseDocumentsModel) && x.Name.ToLower() == id.ToLower() && x.AuthorId == _user.Id);
            if (warehouseDocument == null)
            {
                msg = "Временный документ не обнаружен в БД";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            _context.WarehouseDocuments.Remove(warehouseDocument);
            await _context.SaveChangesAsync();
            msg = $"Документ удалён. В том числе строк/регистров документа: {warehouseDocument.RowsDocument.Count}";
            _logger.LogInformation(msg);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.success.ToString()
            });
        }
    }
}
