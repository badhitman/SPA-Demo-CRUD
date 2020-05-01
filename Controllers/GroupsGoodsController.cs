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
    public class GroupsGoodsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<GroupsGoodsController> _logger;
        private readonly UserObjectModel _user;

        public GroupsGoodsController(AppDataBaseContext context, ILogger<GroupsGoodsController> logger, SessionUser session)
        {
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/GroupsGoods
        [HttpGet]
        public async Task<ActionResult<object>> GetGroupsGoods([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<GroupGoodsObjectModel> groups = _context.GroupsGoods.AsQueryable();
            if (_user.Role < AccessLevelUserRolesEnum.Admin)
            {
                groups = groups.Where(group => !group.isDisabled);
            }

            pagingParameters.Init(await _context.GroupsGoods.CountAsync());

            groups = groups.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                groups = groups.Skip(pagingParameters.Skip);
            groups = groups.Take(pagingParameters.PageSize);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос групп номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await groups.Include(x => x.Avatar).Select(group => new
                {
                    group.Id,
                    group.Name,
                    group.Information,

                    group.isReadonly,
                    group.isDisabled,
                    group.isGlobalFavorite,
                    Avatar = new
                    {
                        group.Avatar.Id,
                        group.Avatar.Name,
                        group.Avatar.Information
                    },
                    countGoods = _context.Goods.Count(good => good.GroupId == group.Id)
                }).ToListAsync()
            });
        }

        // GET: api/GroupGoods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetGroupGoods([FromQuery] PaginationParametersModel pagingParameters, int id)
        {
            GroupGoodsObjectModel group = await _context.GroupsGoods.Include(group => group.Avatar).FirstOrDefaultAsync(group => group.Id == id);

            if (group is null)
            {
                _logger.LogError("Запрашиваемая группа товаров не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая группа товаров не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            IQueryable<GoodObjectModel> goods = _context.Goods.Where(good => good.GroupId == id);
            pagingParameters.Init(await goods.CountAsync());
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            goods = goods.OrderBy(good => good.Id);
            if (pagingParameters.PageNum > 1)
                goods = goods.Skip(pagingParameters.Skip);
            goods = goods.Take(pagingParameters.PageSize);

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатурнй группы обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    group.Id,
                    group.Name,
                    group.Information,

                    group.isDisabled,
                    group.isGlobalFavorite,
                    group.isReadonly,

                    noDelete = await _context.Goods.AnyAsync(good => good.GroupId == id),

                    Avatar = new
                    {
                        group.Avatar?.Id,
                        group.Avatar?.Name,
                        group.Avatar?.Information
                    },

                    goods = await goods.Select(good => new
                    {
                        good.Id,
                        good.Name,
                        good.Information,
                        good.Price,
                        good.UnitId,

                        good.isReadonly,
                        good.isDisabled,
                        good.isGlobalFavorite
                    }).ToListAsync(),

                    units = await _context.Units.Select(unit => new
                    {
                        unit.Id,
                        unit.Name,
                        unit.Information
                    }).ToListAsync()
                }
            });
        }

        // POST: api/GroupGoods
        [HttpPost]
        public async Task<ActionResult<GroupGoodsObjectModel>> PostGroupGoods(GroupGoodsObjectModel ajaxGroup)
        {
            _logger.LogInformation("Создание номенклатурной группы. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxGroup.Name = ajaxGroup.Name.Trim();
            ajaxGroup.Information = ajaxGroup.Information.Trim();

            if (!ModelState.IsValid || string.IsNullOrEmpty(ajaxGroup.Name))
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

            if (_context.GroupsGoods.Any(x => x.Name.ToLower() == ajaxGroup.Name.ToLower()))
            {
                msg = "Номенклатурная группа с таким именем уже существует. Придумайте уникальное имя.";
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
                ajaxGroup.isDisabled = false;
                ajaxGroup.isGlobalFavorite = false;
                ajaxGroup.isReadonly = false;
            }

            _context.GroupsGoods.Add(ajaxGroup);
            await _context.SaveChangesAsync();

            HttpContext.Response.Cookies.Append("rowsCount", (await _context.GroupsGoods.CountAsync()).ToString());
            msg = "Номенклатурная группа создана";
            _logger.LogInformation(msg);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.success.ToString(),
                Tag = ajaxGroup.Id
            });
        }

        // PUT: api/GroupsGoods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroupsGoods(int id, GroupGoodsObjectModel ajaxGroup)
        {
            _logger.LogInformation($"Редактирование номенклатурной группы [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxGroup.Name = ajaxGroup.Name.Trim();
            ajaxGroup.Information = ajaxGroup.Information.Trim();

            if (!ModelState.IsValid
                || string.IsNullOrEmpty(ajaxGroup.Name)
                || id != ajaxGroup.Id
                || id < 1
                || !await _context.GroupsGoods.AnyAsync(gGroup => gGroup.Id == id))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (await _context.GroupsGoods.AnyAsync(gGroup => gGroup.Name.ToLower() == ajaxGroup.Name.ToLower() && gGroup.Id != id))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Группа с таким именем уже существует в бд. Придумайте уникальное",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            GroupGoodsObjectModel groupDb = await _context.GroupsGoods.FindAsync(id);

            if (groupDb.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Группа находится статусе 'read only'. Ваш уровень доступа [{_user.Role}] не позволяет удалять/редактировать подобные объекты";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            groupDb.Name = ajaxGroup.Name;
            groupDb.Information = ajaxGroup.Information;

            if (_user.Role == AccessLevelUserRolesEnum.ROOT)
            {
                groupDb.isGlobalFavorite = ajaxGroup.isGlobalFavorite;
                groupDb.isDisabled = ajaxGroup.isDisabled;
                groupDb.isReadonly = ajaxGroup.isReadonly;
            }

            try
            {
                await _context.SaveChangesAsync();
                msg = $"Изменения группы [#{groupDb.Id}] сохранены";
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
                msg = $"Во время сохранения изменений в номенклатурной группе [#{id}] произошла ошибка: {ex.Message}{(ex.InnerException is null ? "" : ". InnerException: " + ex.InnerException.Message)}";
                _logger.LogError(ex, msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
        }

        // DELETE: api/GroupGoods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteGroupGoods(int id)
        {
            string msg;
            _logger.LogInformation($"Удаление группы номенклатуры {id}. Инициатор: " + _user.FullInfo);

            GroupGoodsObjectModel group = await _context.GroupsGoods.FindAsync(id);
            if (group == null)
            {
                _logger.LogError("Запрашиваемая группа номенклатуры не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая группа номенклатуры не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (group.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Группа находится статусе 'read only'. Ваш уровень доступа [{_user.Role}] не позволяет удалять/редактировать подобные объекты";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (await _context.Goods.AnyAsync(good => good.GroupId == id))
            {
                msg = $"Запрашиваемая номенклатурная группа не может быть удалена (есть ссылки в номенклатуре): id={id}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.GroupsGoods.Remove(group);
            await _context.SaveChangesAsync();

            msg = $"Группа номенклатуры удалена: id={id}";
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