////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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

        public UnitsGoodsController(AppDataBaseContext context, ILogger<UnitsGoodsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/UnitsGoods
        [HttpGet]
        public ActionResult<IEnumerable<UnitGoodObjectModel>> GetUnits([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(_context.Units.Count());
            IQueryable<UnitGoodObjectModel> unitsGoods = _context.Units.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                unitsGoods = unitsGoods.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос групп номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = unitsGoods.Take(pagingParameters.PageSize).ToList().Select(x => new { x.Id, x.Name, x.Information })
            });
        }

        // GET: api/UnitsGoods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UnitGoodObjectModel>> GetUnitGoodModel([FromQuery] PaginationParametersModel pagingParameters, int id)
        {
            UnitGoodObjectModel unitGoodModel = await _context.Units.FindAsync(id);

            if (unitGoodModel == null)
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
                Tag = new
                {
                    unitGoodModel.Id,
                    unitGoodModel.Name,
                    unitGoodModel.Information,
                    noDelete = _context.GoodMovementDocumentRows.Any(x=>x.UnitId == id),
                    goods = goods.Take(pagingParameters.PageSize).Select(x => new { x.Id, x.GroupId, x.Information, x.isDisabled, x.IsGlobalFavorite, x.Name, x.Price, x.Readonly, x.UnitId })
                }
            });
        }

        // PUT: api/UnitsGoods/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUnitGoodModel(int id, UnitGoodObjectModel unitGoodModel)
        {
            unitGoodModel.Name = unitGoodModel.Name.Trim();
            unitGoodModel.Information = unitGoodModel.Information.Trim();

            if (string.IsNullOrEmpty(unitGoodModel.Name) || string.IsNullOrEmpty(unitGoodModel.Information))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Создаваемый объект должен иметь краткое и полное имена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

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

            if (id != unitGoodModel.Id)
            {
                _logger.LogError("Ошибка в запросе: {0} != unitGoodModel.Id", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_context.Units.Any(x => x.Name == unitGoodModel.Name && x.Id != unitGoodModel.Id))
            {
                _logger.LogError("Дубль имени единицы измерения в бд. Отказ сохранения", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Такое имя уже есть в базе данных. Придумайте уникальное.",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Units.Attach(unitGoodModel);
            _context.Entry(unitGoodModel).Property(x => x.Name).IsModified = true;
            _context.Entry(unitGoodModel).Property(x => x.Information).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                unitGoodModel = _context.Units.Find(id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = "Имя номенклатурной группы сохранено",
                    Status = StylesMessageEnum.success.ToString(),
                    Tag = unitGoodModel
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UnitGoodModelExists(id))
                {
                    _logger.LogError("Единица измерения не найдена {0}", id);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Единица измерения не найдена",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
                else
                {
                    _logger.LogError("Невнятная ошибка во время обновления единицы измерения");
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Невнятная ошибка во время обновлении единицы измерения",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
            }
        }

        // POST: api/UnitsGoods
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<UnitGoodObjectModel>> PostUnitGoodModel(UnitGoodObjectModel unitGoodModel)
        {
            unitGoodModel.Name = unitGoodModel.Name.Trim();
            unitGoodModel.Information = unitGoodModel.Information.Trim();

            if (string.IsNullOrEmpty(unitGoodModel.Name) || string.IsNullOrEmpty(unitGoodModel.Information))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Создаваемый объект должен иметь краткое и полное имена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

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

            if (_context.Units.Any(x => x.Name == unitGoodModel.Name))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Такое имя уже существует",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Units.Add(unitGoodModel);
            await _context.SaveChangesAsync();
            HttpContext.Response.Cookies.Append("rowsCount", _context.Units.Count().ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Номенклатурная группа создана",
                Status = StylesMessageEnum.success.ToString(),
                Tag = unitGoodModel
            });
        }

        // DELETE: api/UnitsGoods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UnitGoodObjectModel>> DeleteUnitGoodModel(int id)
        {
            UnitGoodObjectModel unitGoodModel = await _context.Units.FindAsync(id);
            if (unitGoodModel == null)
            {
                _logger.LogError("Запрашиваемая единица измерения не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая единица измерения не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_context.Goods.Any(x => x.UnitId == id) || _context.GoodMovementDocumentRows.Any(x => x.UnitId == id))
            {
                _logger.LogError("Запрашиваемая единица измерения не может быть удалена (есть ссылки в номенклатуре и/или регистрах): id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая единица измерения не может быть удалена (есть ссылки в номенклатуре и/или регистрах)",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Units.Remove(unitGoodModel);
            await _context.SaveChangesAsync();

            _logger.LogError("Единица измерения удалена: id={0}", id);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Единица измерения удалена",
                Status = StylesMessageEnum.info.ToString()
            });
        }

        private bool UnitGoodModelExists(int id)
        {
            return _context.Units.Any(e => e.Id == id);
        }
    }
}
