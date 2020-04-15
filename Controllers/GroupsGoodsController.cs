////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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

        public GroupsGoodsController(AppDataBaseContext context, ILogger<GroupsGoodsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/GroupsGoods
        [HttpGet]
        public ActionResult<object> GetGroupsGoods([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(_context.GroupsGoods.Count());
            IQueryable<GroupGoodsObjectModel> groupsGoods = _context.GroupsGoods.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                groupsGoods = groupsGoods.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос групп номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = groupsGoods.Include(x => x.Avatar).Include(x => x.Goods).Take(pagingParameters.PageSize).ToList().Select(x => new { x.Id, x.Avatar, x.Name, x.Readonly, x.Goods.Count })
            });
        }

        // GET: api/GroupGoods/5
        [HttpGet("{id}")]
        public ActionResult<object> GetGroupGoods([FromQuery] PaginationParametersModel pagingParameters, int id)
        {
            GroupGoodsObjectModel groupGoodModel = _context.GroupsGoods.Include(x => x.Avatar).FirstOrDefault(x => x.Id == id);

            if (groupGoodModel is null)
            {
                _logger.LogError("Запрашиваемая группа товаров не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая группа товаров не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
            IQueryable<GoodObjectModel> goods = _context.Goods.Where(x => x.GroupId == id).OrderBy(x => x.Id);
            pagingParameters.Init(goods.Count());
            if (pagingParameters.PageNum > 1)
                goods = goods.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            //List<UnitGoodModel> units = _context.Units.ToList();
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатурнй группы обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { groupGoodModel.Id, groupGoodModel.Name, groupGoodModel.Avatar, goods = goods.Take(pagingParameters.PageSize).Select(x => new { x.Id, x.GroupId, x.Information, x.isDisabled, x.IsGlobalFavorite, x.Name, x.Price, x.Readonly, x.UnitId }), units = _context.Units.Select(x => new { x.Id, x.Name, x.Information }) }
            });
        }

        // PUT: api/GroupsGoods/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGroupsGoods(int id, GroupGoodsObjectModel groupGoodModel)
        {
            if (!ModelState.IsValid)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка контроля валидности модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (id != groupGoodModel.Id)
            {
                _logger.LogError("Ошибка в запросе: {0} != groupGoodModel.Id", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
            _context.GroupsGoods.Attach(groupGoodModel);
            _context.Entry(groupGoodModel).Property(x => x.Name).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                groupGoodModel = _context.GroupsGoods.Find(id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = "Имя номенклатурной группы сохранено",
                    Status = StylesMessageEnum.success.ToString(),
                    Tag = groupGoodModel
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GroupsGoodExists(id))
                {
                    _logger.LogError("Группа не найдена {0}", id);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Группа не найдена",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
                else
                {
                    _logger.LogError("Невнятная ошибка во время обновлении номенклатурной группы");
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Невнятная ошибка во время обновлении номенклатурной группы",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
            }
        }

        // POST: api/GroupGoods
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<GroupGoodsObjectModel>> PostGroupGoods(GroupGoodsObjectModel groupGoodModel)
        {
            if (!ModelState.IsValid)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка контроля валидности модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            groupGoodModel.Name = groupGoodModel.Name.Trim();
            if (string.IsNullOrEmpty(groupGoodModel.Name))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Создаваемый объект должен иметь имя",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_context.GroupsGoods.Any(x => x.Name == groupGoodModel.Name))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Такое имя уже существует",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.GroupsGoods.Add(groupGoodModel);
            await _context.SaveChangesAsync();
            HttpContext.Response.Cookies.Append("rowsCount", _context.GroupsGoods.Count().ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Номенклатурная группа создана",
                Status = StylesMessageEnum.success.ToString(),
                Tag = groupGoodModel
            });
        }

        // DELETE: api/GroupGoods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GroupGoodsObjectModel>> DeleteGroupGoods(int id)
        {
            var groupGoodModel = await _context.GroupsGoods.FindAsync(id);
            if (groupGoodModel == null)
            {
                _logger.LogError("Запрашиваемая группа номенклатуры не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая группа номенклатуры не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_context.Goods.Any(x => x.GroupId == id))
            {
                _logger.LogError("Запрашиваемая номенклатурная группа не может быть удалена (есть ссылки в номенклатуре): id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая номенклатурная группа не может быть удалена (есть ссылки в номенклатуре)",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.GroupsGoods.Remove(groupGoodModel);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Группа номенклатуры удалена: id={0}", id);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Группа удалена",
                Status = StylesMessageEnum.info.ToString()
            });
        }

        private bool GroupsGoodExists(int id)
        {
            return _context.GroupsGoods.Any(e => e.Id == id);
        }
    }
}