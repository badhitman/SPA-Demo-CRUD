////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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
    [Route("api/[controller]")]
    [ApiController]
    public class WarehousesController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<WarehousesController> _logger;

        public WarehousesController(AppDataBaseContext context, ILogger<WarehousesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Warehouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WarehouseGoodObjectModel>>> GetWarehousesGoods([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(_context.WarehousesGoods.Count());
            IQueryable<WarehouseGoodObjectModel> Warehouses = _context.WarehousesGoods.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                Warehouses = Warehouses.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос складов хранения обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = Warehouses.Include(x => x.Avatar).Take(pagingParameters.PageSize).Select(x => new
                {
                    avatar = new { x.Avatar.Id, x.Avatar.Name },
                    x.Id,
                    x.Information,
                    x.Name
                })
            }); ; ;
        }

        // GET: api/Warehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<WarehouseGoodObjectModel>> GetWarehouseGoodObjectModel(int id)
        {
            var warehouseGoodObjectModel = await _context.WarehousesGoods.FindAsync(id);

            if (warehouseGoodObjectModel == null)
            {
                return NotFound();
            }

            return warehouseGoodObjectModel;
        }

        // PUT: api/Warehouses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutWarehouseGoodObjectModel(int id, WarehouseGoodObjectModel warehouseGoodObjectModel)
        {
            if (id != warehouseGoodObjectModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(warehouseGoodObjectModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WarehouseGoodObjectModelExists(id))
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

        // POST: api/Warehouses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<WarehouseGoodObjectModel>> PostWarehouseGoodObjectModel(WarehouseGoodObjectModel warehouseGoodObjectModel)
        {
            _context.WarehousesGoods.Add(warehouseGoodObjectModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetWarehouseGoodObjectModel", new { id = warehouseGoodObjectModel.Id }, warehouseGoodObjectModel);
        }

        // DELETE: api/Warehouses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<WarehouseGoodObjectModel>> DeleteWarehouseGoodObjectModel(int id)
        {
            var warehouseGoodObjectModel = await _context.WarehousesGoods.FindAsync(id);
            if (warehouseGoodObjectModel == null)
            {
                return NotFound();
            }

            _context.WarehousesGoods.Remove(warehouseGoodObjectModel);
            await _context.SaveChangesAsync();

            return warehouseGoodObjectModel;
        }

        private bool WarehouseGoodObjectModelExists(int id)
        {
            return _context.WarehousesGoods.Any(e => e.Id == id);
        }
    }
}
