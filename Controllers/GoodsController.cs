////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelAuth")]
    public class GoodsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<GoodsController> _logger;
        private readonly UserObjectModel _user;

        public GoodsController(AppDataBaseContext context, ILogger<GoodsController> logger, SessionUser session)
        {
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/Goods
        [HttpGet]
        public async Task<ActionResult<object>> GetGoods([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<GoodObjectModel> goods = _context.Goods.AsQueryable();
            if (_user.Role < AccessLevelUserRolesEnum.Admin)
            {
                goods = goods.Where(x => !x.isDisabled);
            }

            pagingParameters.Init(await goods.CountAsync());

            goods = goods.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                goods = goods.Skip(pagingParameters.Skip);

            goods = goods
                .Take(pagingParameters.PageSize)
                .Include(x => x.Avatar)
                .Include(x => x.Unit)
                .Include(x => x.Group);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = goods.Select(good => new
                {
                    good.Id,
                    good.Name,
                    good.Information,
                    good.Price,

                    good.isReadonly,
                    good.isDisabled,
                    good.isGlobalFavorite,

                    Avatar = new
                    {
                        good.Avatar.Id,
                        good.Avatar.Name,
                        good.Avatar.Information
                    },

                    Group = new
                    {
                        good.Group.Id,
                        good.Group.Name,
                        good.Group.Information
                    },
                    Unit = new
                    {
                        good.Unit.Id,
                        good.Unit.Name,
                        good.Unit.Information
                    }
                })
            });
        }

        // GET: api/Goods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetGood([FromQuery] PaginationParametersModel pagingParameters, int id)
        {
            GoodObjectModel good = await _context.Goods
                .Where(x => x.Id == id)
                .Include(x => x.Avatar)
                .Include(x => x.Unit)
                .Include(x => x.Group)
                .FirstOrDefaultAsync();

            if (good == null)
            {
                _logger.LogError("Номенклатура не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Номенклатура не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            IQueryable<RowGoodMovementRegisterModel> goodRegisters = _context.GoodMovementDocumentRows.Where(x => x.GoodId == id);
            pagingParameters.Init(await goodRegisters.CountAsync());

            goodRegisters = goodRegisters.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                goodRegisters = goodRegisters.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            goodRegisters = goodRegisters
                .Take(pagingParameters.PageSize)
                .Include(x => x.BodyDocument).ThenInclude(x => x.Author);

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    good.Id,
                    good.Name,
                    good.Information,
                    good.Price,
                    good.GroupId,
                    good.UnitId,

                    good.isReadonly,
                    good.isDisabled,
                    good.isGlobalFavorite,

                    noDelete = await _context.GoodMovementDocumentRows.AnyAsync(x => x.GoodId == id) || await _context.InventoryGoodsBalancesDeliveries.AnyAsync(x => x.GoodId == id) || await _context.InventoryGoodsBalancesWarehouses.AnyAsync(x => x.GoodId == id),

                    Avatar = new
                    {
                        id = good.AvatarId,
                        good.Avatar?.Name
                    },

                    Units = await _context.Units.Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Information
                    }).ToListAsync(),
                    Groups = await _context.GroupsGoods.Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Information
                    }).ToListAsync(),

                    Registers = await goodRegisters
                    .Select(x => new
                    {
                        x.Id,
                        Document = BodyGoodMovementDocumentModel.getDocument(x.BodyDocument, _context),
                        x.Quantity,
                        x.UnitId
                    }).ToListAsync()
                }
            });
        }

        // POST: api/Goods/9
        [HttpPost("{id}")]
        public async Task<ActionResult<object>> PostGood([FromRoute] int id, [FromBody] GoodObjectModel ajaxGood)
        {
            _logger.LogInformation("Создание номенклатуры. Инициатор: " + _user.FullInfo);

            ajaxGood.Name = ajaxGood.Name.Trim();
            ajaxGood.Information = ajaxGood.Information.Trim();

            if (!ModelState.IsValid
                || string.IsNullOrEmpty(ajaxGood.Name)
                || ajaxGood.GroupId <= 0
                || ajaxGood.GroupId != id
                || ajaxGood.UnitId <= 0
                || !await _context.GroupsGoods.AnyAsync(group => group.Id == ajaxGood.GroupId)
                || !await _context.Units.AnyAsync(unit => unit.Id == ajaxGood.UnitId))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка контроля валидности модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (await _context.Goods.AnyAsync(good => good.Name.ToLower() == ajaxGood.Name.ToLower()))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Номенклатура с таким именем уже существует в бд. Придумайте уникальное",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                ajaxGood.isDisabled = false;
                ajaxGood.isGlobalFavorite = false;
                ajaxGood.isReadonly = false;
            }

            await _context.Goods.AddAsync(ajaxGood);
            await _context.SaveChangesAsync();

            HttpContext.Response.Cookies.Append("rowsCount", (await _context.Goods.CountAsync()).ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Номенклатура создана",
                Status = StylesMessageEnum.success.ToString(),
                Tag = ajaxGood.Id
            });
        }

        // PUT: api/Goods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoodModel([FromRoute]int id, [FromBody] GoodObjectModel ajaxGood)
        {
            _logger.LogInformation($"Редактирование номенклатуры [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxGood.Name = ajaxGood.Name.Trim();
            ajaxGood.Information = ajaxGood.Information.Trim();

            if (!ModelState.IsValid
                || ajaxGood.GroupId < 1
                || string.IsNullOrEmpty(ajaxGood.Name)
                || id != ajaxGood.Id
                || !await _context.Goods.AnyAsync(x => x.Id == id)
                || !await _context.GroupsGoods.AnyAsync(x=>x.Id == ajaxGood.GroupId))
            {
                msg = "Ошибка в запросе";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (await _context.Goods.AnyAsync(x => x.Name.ToLower() == ajaxGood.Name.ToLower() && x.Id != id))
            {
                msg = "Номенклатура с таким именем уже существует в бд. Придумайте уникальное";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            GoodObjectModel goodDb = await _context.Goods.FindAsync(ajaxGood.Id);

            if (goodDb.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Номенклатура находится статусе 'read only'. Ваш уровень доступа [{_user.Role}] не позволяет редактировать/удалять подобные объекты";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            goodDb.Name = ajaxGood.Name;
            goodDb.Information = ajaxGood.Information;
            goodDb.Price = ajaxGood.Price;
            goodDb.UnitId = ajaxGood.UnitId;
            goodDb.GroupId = ajaxGood.GroupId;

            if (_user.Role == AccessLevelUserRolesEnum.ROOT)
            {
                goodDb.isGlobalFavorite = ajaxGood.isGlobalFavorite;
                goodDb.isDisabled = ajaxGood.isDisabled;
                goodDb.isReadonly = ajaxGood.isReadonly;
            }

            _context.Goods.Update(goodDb);

            try
            {
                await _context.SaveChangesAsync();
                msg = $"Номенклатура [#{id}] сохранена";
                _logger.LogInformation(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = msg,
                    Status = StylesMessageEnum.success.ToString()
                });
            }
            catch (Exception ex)
            {
                msg = $"Ошибка SQL доступа во время обновления номенклатуры [#{id}]: {ex.Message}{(ex.InnerException is null ? "" : ". InnerException: " + ex.InnerException.Message)}";
                _logger.LogError(ex, msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
        }

        // DELETE: api/Goods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteGood(int id)
        {
            string msg;
            _logger.LogInformation($"Удаление номенклатуры {id}. Инициатор: " + _user.FullInfo);

            GoodObjectModel good = await _context.Goods.FindAsync(id);
            if (good == null)
            {
                msg = $"Запрашиваемая номенклатура не найдена: id={0}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (good.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Номенклатура находится статусе 'read only'. Ваш уровень доступа [{_user.Role}] не позволяет удалять/редактировать подобные объекты";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (await _context.GoodMovementDocumentRows.AnyAsync(x => x.GoodId == id)
                || await _context.InventoryGoodsBalancesDeliveries.AnyAsync(x => x.GoodId == id)
                || await _context.InventoryGoodsBalancesWarehouses.AnyAsync(x => x.GoodId == id))
            {
                msg = $"Запрашиваемая номенклатура не может быть удалена (есть ссылки в регистрах сущетсвуют остатки): id={id}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Goods.Remove(good);
            await _context.SaveChangesAsync();

            msg = $"Номенклатура удалена: id={id}";
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
