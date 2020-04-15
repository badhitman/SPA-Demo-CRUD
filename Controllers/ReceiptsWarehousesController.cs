////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptsWarehousesController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<ReceiptsWarehousesController> _logger;

        public ReceiptsWarehousesController(AppDataBaseContext context, ILogger<ReceiptsWarehousesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/MovementGoodsWarehouses
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptToWarehouseDocumentModel>>> GetMovementsGoodsWarehouses([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(_context.ReceiptesGoodsToWarehousesRegisters.Count());
            IQueryable<ReceiptToWarehouseDocumentModel> ReceiptToWarehouses = _context.ReceiptesGoodsToWarehousesRegisters.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                ReceiptToWarehouses = ReceiptToWarehouses.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос документов поступления обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = ReceiptToWarehouses.Take(pagingParameters.PageSize).Select(x => new
                {
                    x.Id,
                    Author = new { x.Author.Id, x.Author.Name },
                    WarehouseReceipt = new { x.WarehouseReceipt.Id, x.WarehouseReceipt.Name, x.WarehouseReceipt.Information },
                    x.Information,
                    x.Name
                })
            });
        }

        // GET: api/MovementGoodsWarehouses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptToWarehouseDocumentModel>> GetMovementGoodsWarehousesDocumentModel(int id)
        {
            var movementGoodsWarehousesDocumentModel = await _context.ReceiptesGoodsToWarehousesRegisters.FindAsync(id);

            if (movementGoodsWarehousesDocumentModel == null)
            {
                return NotFound();
            }

            return movementGoodsWarehousesDocumentModel;
        }

        // PUT: api/MovementGoodsWarehouses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovementGoodsWarehousesDocumentModel(int id, ReceiptToWarehouseDocumentModel movementGoodsWarehousesDocumentModel)
        {
            if (id != movementGoodsWarehousesDocumentModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(movementGoodsWarehousesDocumentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovementGoodsWarehousesDocumentModelExists(id))
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

        // POST: api/MovementGoodsWarehouses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<ReceiptToWarehouseDocumentModel>> PostMovementGoodsWarehousesDocumentModel(ReceiptToWarehouseDocumentModel movementGoodsWarehousesDocumentModel)
        {
            _context.ReceiptesGoodsToWarehousesRegisters.Add(movementGoodsWarehousesDocumentModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovementGoodsWarehousesDocumentModel", new { id = movementGoodsWarehousesDocumentModel.Id }, movementGoodsWarehousesDocumentModel);
        }

        // DELETE: api/MovementGoodsWarehouses/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReceiptToWarehouseDocumentModel>> DeleteMovementGoodsWarehousesDocumentModel(int id)
        {
            var movementGoodsWarehousesDocumentModel = await _context.ReceiptesGoodsToWarehousesRegisters.FindAsync(id);
            if (movementGoodsWarehousesDocumentModel == null)
            {
                return NotFound();
            }

            _context.ReceiptesGoodsToWarehousesRegisters.Remove(movementGoodsWarehousesDocumentModel);
            await _context.SaveChangesAsync();

            return movementGoodsWarehousesDocumentModel;
        }

        private bool MovementGoodsWarehousesDocumentModelExists(int id)
        {
            return _context.ReceiptesGoodsToWarehousesRegisters.Any(e => e.Id == id);
        }
    }
}
