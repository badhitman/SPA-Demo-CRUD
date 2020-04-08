////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GoodsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public GoodsController(AppDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Goods
        [HttpGet]
        public ActionResult<IEnumerable<GoodModel>> GetGoods([FromQuery] PaginationParameters pagingParameters)
        {
            pagingParameters.Init(_context.Goods.Count());
            IQueryable<GoodModel> goods = _context.Goods.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                goods = goods.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = goods.Include(x => x.Group).Include(x => x.Unut).Take(pagingParameters.PageSize).ToList().Select(x => new { x.AvatarId, GroupName = x.Group.Name, x.Price, x.Readonly, UnitName = x.Unut.Information })
            });
        }

        // GET: api/Goods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoodModel>> GetGoodModel(int id)
        {
            var goodModel = await _context.Goods.FindAsync(id);

            if (goodModel == null)
            {
                return NotFound();
            }

            return goodModel;
        }

        // PUT: api/Goods/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoodModel(int id, GoodModel goodModel)
        {
            if (id != goodModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(goodModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoodModelExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Goods
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<GoodModel>> PostGoodModel(GoodModel goodModel)
        {
            //_context.Goods.Add(goodModel);
            //await _context.SaveChangesAsync();

            //return CreatedAtAction("GetGoodModel", new { id = goodModel.Id }, goodModel);
            return new ObjectResult(new ServerActionResult()
            {
                Success = false,
                Info = "Запрос номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { }
            });
        }

        // DELETE: api/Goods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GoodModel>> DeleteGoodModel(int id)
        {
            var goodModel = await _context.Goods.FindAsync(id);
            if (goodModel == null)
            {
                return NotFound();
            }

            _context.Goods.Remove(goodModel);
            await _context.SaveChangesAsync();

            return goodModel;
        }

        private bool GoodModelExists(int id)
        {
            return _context.Goods.Any(e => e.Id == id);
        }
    }
}
