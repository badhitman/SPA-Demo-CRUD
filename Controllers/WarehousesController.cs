////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    /// <summary>
    /// Склады количественного учёта номенклатуры
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<WarehousesController> _logger;
        private readonly UserObjectModel _user;

        public WarehousesController(AppDataBaseContext context, ILogger<WarehousesController> logger, SessionUser session)
        {
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/Warehouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetWarehouses([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<WarehouseObjectModel> Warehouses = _context.Warehouses.AsQueryable();

            pagingParameters.Init(await _context.Warehouses.CountAsync());

            Warehouses = Warehouses.OrderBy(x => x.Id);

            if (pagingParameters.PageNum > 1)
                Warehouses = Warehouses.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос складов хранения обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = Warehouses.Take(pagingParameters.PageSize).Select(warehouse => new
                {
                    warehouse.Id,
                    warehouse.Name,
                    warehouse.Information,
                    warehouse.isReadonly,
                    warehouse.isDisabled,
                    warehouse.isGlobalFavorite
                })
            });
        }

        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetWarehouse([FromQuery] PaginationParametersModel pagingParameters, [FromRoute] int id)
        {
            WarehouseObjectModel warehouse = await _context.Warehouses.Include(x => x.Avatar).FirstOrDefaultAsync(x => x.Id == id);

            if (warehouse == null)
            {
                _logger.LogError("Склад не найден: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Склад не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            IQueryable<WarehouseDocumentsModel> wDocuments = _context.WarehouseDocuments
                .Where(wDoc => wDoc.WarehouseId == id || (wDoc.Discriminator == nameof(InternalDisplacementWarehouseDocumentModel) && ((InternalDisplacementWarehouseDocumentModel)wDoc).WarehouseDebitingId == id)).OrderBy(x => x.Id);

            pagingParameters.Init(await wDocuments.CountAsync());
            if (pagingParameters.PageNum > 1)
                wDocuments = wDocuments.Skip(pagingParameters.Skip);

            wDocuments = wDocuments.Take(pagingParameters.PageSize)
                .Include(wDoc => wDoc.Author)
                .Include(wDoc => wDoc.Warehouse);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    warehouse.Id,
                    warehouse.Name,
                    warehouse.Information,
                    warehouse.isDisabled,
                    warehouse.isGlobalFavorite,
                    warehouse.isReadonly,
                    Documents = (await wDocuments.ToListAsync()).Select(wDoc => BodyGoodMovementDocumentModel.getDocument(wDoc, _context)),

                    Avatar = new
                    {
                        warehouse.Avatar?.Id,
                        warehouse.Avatar?.Name
                    },

                    noDelete = _context.WarehouseDocuments.Any(wDoc => wDoc.WarehouseId == id)
                    || _context.InternalDisplacementWarehouseDocuments.Any(dwDoc => dwDoc.WarehouseDebitingId == id)
                    || _context.InventoryGoodsBalancesWarehouses.Any(x => x.WarehouseId == id)
                }
            });
        }

        // POST: api/Warehouses
        [HttpPost]
        public async Task<ActionResult<object>> PostWarehouse([FromBody] WarehouseObjectModel ajaxWarehouse)
        {
            _logger.LogInformation("Создание склада. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxWarehouse.Name = ajaxWarehouse.Name.Trim();
            ajaxWarehouse.Information = ajaxWarehouse.Information.Trim();

            if (!ModelState.IsValid || string.IsNullOrEmpty(ajaxWarehouse.Name))
            {
                msg = "Ошибка контроля валидности модели";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (_context.Warehouses.Any(x => x.Name.ToLower() == ajaxWarehouse.Name.ToLower()))
            {
                msg = "Склад с таким именем уже существует в бд. Придумайте уникальное";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                ajaxWarehouse.isDisabled = false;
                ajaxWarehouse.isGlobalFavorite = false;
                ajaxWarehouse.isReadonly = false;
            }

            _context.Warehouses.Add(ajaxWarehouse);
            await _context.SaveChangesAsync();

            HttpContext.Response.Cookies.Append("rowsCount", (await _context.Warehouses.CountAsync()).ToString());

            msg = $"Склад создан [#{ajaxWarehouse.Id}]";
            _logger.LogInformation(msg);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.success.ToString(),
                Tag = ajaxWarehouse.Id
            });
        }

        // PUT: api/Warehouses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouse([FromRoute] int id, [FromBody] WarehouseObjectModel ajaxWarehouse)
        {
            _logger.LogInformation($"Запрос изменения склада [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxWarehouse.Name = ajaxWarehouse.Name.Trim();
            ajaxWarehouse.Information = ajaxWarehouse.Information.Trim();

            if (!ModelState.IsValid
                || id != ajaxWarehouse.Id
                || id < 0
                || string.IsNullOrEmpty(ajaxWarehouse.Name)
                || !_context.Warehouses.Any(x => x.Id != id))
            {
                msg = "Ошибка контроля валидности модели";
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (_context.Warehouses.Any(x => x.Name.ToLower() == ajaxWarehouse.Name.ToLower() && x.Id != id))
            {
                _logger.LogWarning("Дубль склада (по имени)");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Склад с тиким именем уже существует. Придумайте уникальное",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            WarehouseObjectModel warehouseDb = _context.Warehouses.FirstOrDefault(x => x.Id == id);
            if (warehouseDb.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Объект только для чтения. Для удаления данного объекта требуется уровень привелегий [{AccessLevelUserRolesEnum.ROOT}]. Ваш уровень привелегий: {_user.Role}"; ;
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            warehouseDb.Name = ajaxWarehouse.Name;
            warehouseDb.Information = ajaxWarehouse.Information;

            if (_user.Role == AccessLevelUserRolesEnum.ROOT)
            {
                warehouseDb.isGlobalFavorite = ajaxWarehouse.isGlobalFavorite;
                warehouseDb.isDisabled = ajaxWarehouse.isDisabled;
                warehouseDb.isReadonly = ajaxWarehouse.isReadonly;
            }

            _context.Update(warehouseDb);

            try
            {
                msg = $"Склад [{warehouseDb.Id}] сохранён";
                await _context.SaveChangesAsync();
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = msg,
                    Status = StylesMessageEnum.success.ToString()
                });
            }
            catch (Exception ex)
            {
                msg = $"Во время изменения 'Склада' [{warehouseDb.Id}] произошла ошибка. Exception: {ex.Message}{(ex.InnerException is null ? "" : ". InnerException: " + ex.InnerException.Message)}";
                _logger.LogError(ex, msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
        }

        // DELETE: api/Warehouses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteWarehouse(int id)
        {
            _logger.LogInformation($"Запрос удаления склада [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            WarehouseObjectModel warehouse = await _context.Warehouses.FindAsync(id);
            if (warehouse == null)
            {
                msg = $"Запрашиваемый склад не найден: id={id}";
                _logger.LogError(msg, id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (warehouse.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Объект [#{id}] 'только для чтения'. Для изменения такого объекта требуются привелегии [{AccessLevelUserRolesEnum.ROOT}]. Ваши привилегии: [{_user.Role}]";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (await _context.WarehouseDocuments.AnyAsync(x => x.WarehouseId == id)
                || await _context.InternalDisplacementWarehouseDocuments.AnyAsync(x => x.WarehouseDebitingId == id)
                || await _context.InventoryGoodsBalancesWarehouses.AnyAsync(x => x.WarehouseId == id))
            {
                msg = $"Запрашиваемйй склад не может быть удалён. Существуют ссылки в складских документах или существуют количественные остатки номенклатуры: id={0}";
                _logger.LogWarning(msg, id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Warehouses.Remove(warehouse);
            await _context.SaveChangesAsync();

            msg = $"Склад удалён: id={id}";
            _logger.LogWarning(msg);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.info.ToString()
            });
        }
    }
}
