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
using Microsoft.Extensions.Logging;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelAuth")]
    public class UnitsGoodsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<UnitsGoodsController> _logger;
        private readonly UserObjectModel _user;

        public UnitsGoodsController(AppDataBaseContext context, ILogger<UnitsGoodsController> logger, SessionUser session)
        {
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/UnitsGoods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUnits([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<UnitGoodObjectModel> units = _context.Units.AsQueryable();

            pagingParameters.Init(await units.CountAsync());

            units = _context.Units.OrderBy(x => x.Id);

            if (pagingParameters.PageNum > 1)
                units = units.Skip(pagingParameters.Skip);

            string TypeName = nameof(UnitGoodObjectModel);
            var qUnits = from unit in units
                         join UserFavoriteLocator in _context.UserFavoriteLocators
                         on new { ObjectId = unit.Id, TypeName, UserId = _user.Id }
                         equals new { UserFavoriteLocator.ObjectId, UserFavoriteLocator.TypeName, UserFavoriteLocator.UserId }
                         into joinFavoriteLocator
                         from isFavoriteMark in joinFavoriteLocator.DefaultIfEmpty()
                         select new { unit, isFavoriteMark };

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос групп номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await qUnits.Take(pagingParameters.PageSize).Select(selectItem => new
                {
                    selectItem.unit.Id,
                    selectItem.unit.Name,
                    selectItem.unit.Information,
                    isUserFavorite = selectItem.isFavoriteMark != null
                }).ToListAsync()
            });
        }

        // GET: api/UnitsGoods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUnitGood([FromQuery] PaginationParametersModel pagingParameters, int id)
        {
            UnitGoodObjectModel unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                _logger.LogError("Запрашиваемая единица измерения не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая единица измерения не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            IQueryable<GoodObjectModel> goods = _context.Goods.Where(x => x.UnitId == id).OrderBy(x => x.Id);
            if (_user.Role < AccessLevelUserRolesEnum.Admin)
            {
                goods = goods.Where(x => !x.isDisabled);
            }

            pagingParameters.Init(await goods.CountAsync());
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            if (pagingParameters.PageNum > 1)
                goods = goods.Skip(pagingParameters.Skip);

            string TypeName = nameof(UnitGoodObjectModel);
            var qGoods = from good in goods.Include(x => x.Unit).Include(x => x.Group)
                         join UserFavoriteLocator in _context.UserFavoriteLocators
                         on new { ObjectId = good.Id, TypeName, UserId = _user.Id }
                         equals new { UserFavoriteLocator.ObjectId, UserFavoriteLocator.TypeName, UserFavoriteLocator.UserId }
                         into joinFavoriteLocator
                         from isFavoriteMark in joinFavoriteLocator.DefaultIfEmpty()
                         select new { good, isFavoriteMark };

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатурнй группы обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    unit.Id,
                    unit.Name,
                    unit.Information,
                    noDelete = await _context.GoodMovementDocumentRows.AnyAsync(x => x.UnitId == id) || pagingParameters.CountAllElements > 0,
                    goods = await qGoods.Take(pagingParameters.PageSize).Select(selectItem => new
                    {
                        selectItem.good.Id,
                        selectItem.good.Name,
                        selectItem.good.Price,
                        selectItem.good.Information,

                        selectItem.good.isReadonly,
                        selectItem.good.isDisabled,
                        selectItem.good.isGlobalFavorite,

                        isUserFavorite = selectItem.isFavoriteMark != null,

                        Group = new
                        {
                            selectItem.good.Group.Id,
                            selectItem.good.Group.Name,
                            selectItem.good.Group.Information
                        },
                        Unit = new
                        {
                            selectItem.good.Unit.Id,
                            selectItem.good.Unit.Name
                        }
                    }).ToListAsync()
                }
            });
        }

        // POST: api/UnitsGoods
        [HttpPost]
        public async Task<ActionResult<object>> PostUnitGood(UnitGoodObjectModel ajaxUnit)
        {
            _logger.LogInformation("Создание единицы измерения. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxUnit.Name = ajaxUnit.Name.Trim();
            ajaxUnit.Information = ajaxUnit.Information.Trim();

            if (!ModelState.IsValid || string.IsNullOrEmpty(ajaxUnit.Name))
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

            if (await _context.Units.AnyAsync(x => x.Name.ToLower() == ajaxUnit.Name.ToLower()))
            {
                msg = "Такое имя уже существует. Придумайте уникальное имя";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            await _context.Units.AddAsync(ajaxUnit);
            await _context.SaveChangesAsync();
            HttpContext.Response.Cookies.Append("rowsCount", (await _context.Units.CountAsync()).ToString());
            msg = $"Номенклатурная группа создана [#{ajaxUnit.Id}]";
            _logger.LogInformation(msg);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.success.ToString(),
                Tag = ajaxUnit.Id
            });
        }

        // PUT: api/UnitsGoods/5
        [HttpPut("{id}")]
        [Authorize(Policy = "AccessMinLevelAdmin")]
        public async Task<IActionResult> PutUnitGood(int id, UnitGoodObjectModel ajaxUnit)
        {
            _logger.LogInformation($"Редактирование единицы измерения [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxUnit.Name = ajaxUnit.Name.Trim();
            ajaxUnit.Information = ajaxUnit.Information.Trim();

            if (!ModelState.IsValid || string.IsNullOrEmpty(ajaxUnit.Name))
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

            if (id != ajaxUnit.Id || !await _context.Units.AnyAsync(x => x.Id == id))
            {
                msg = $"Ошибка в запросе. Проверка: {id} != unitGoodAjax.Id || await _context.Units.AnyAsync(x => x.Id == {id})";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (await _context.Units.AnyAsync(x => x.Name.ToLower() == ajaxUnit.Name.ToLower() && x.Id != ajaxUnit.Id))
            {
                msg = "Единица измерения с таким именем уже существует. Придумайте уникальное";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            UnitGoodObjectModel unitGoodDb = await _context.Units.FindAsync(ajaxUnit.Id);
            unitGoodDb.Name = ajaxUnit.Name;
            unitGoodDb.Information = ajaxUnit.Information;
            _context.Units.Update(unitGoodDb);

            try
            {
                await _context.SaveChangesAsync();
                msg = $"Изменения в [еденице измерения] [#{unitGoodDb.Id}] сохранены";
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
                msg = $"Во время сохранения изменений 'Единицы измерения' [#{unitGoodDb.Id}] произошла ошибка: {ex.Message}{(ex.InnerException == null ? "" : ". InnerException: " + ex.InnerException.Message)}";
                _logger.LogError(ex, msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
        }

        // DELETE: api/UnitsGoods/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "AccessMinLevelAdmin")]
        public async Task<ActionResult<object>> DeleteUnitGood(int id)
        {
            _logger.LogInformation($"Удаление единицы измерения {id}. Инициатор: " + _user.FullInfo);
            string msg;

            UnitGoodObjectModel unit = await _context.Units.FindAsync(id);
            if (unit == null)
            {
                msg = $"Запрашиваемая единица измерения не найдена: id={id}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (await _context.Goods.AnyAsync(x => x.UnitId == id) || await _context.GoodMovementDocumentRows.AnyAsync(x => x.UnitId == id))
            {
                msg = $"Запрашиваемая единица измерения не может быть удалена (есть ссылки в номенклатуре и/или регистрах): id={id}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();

            msg = $"Единица измерения удалена: id={id}";
            _logger.LogError(msg);

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.info.ToString()
            });
        }
    }
}
