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
    [Route("api/[controller]")]
    [ApiController]
    public class DisplacementsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<DisplacementsController> _logger;

        public DisplacementsController(AppDataBaseContext context, ILogger<DisplacementsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Displacements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InternalDisplacementWarehouseDocumentModel>>> GetInternalDisplacementWarehouseRegisters([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(_context.InternalDisplacementWarehouseRegisters.Count());
            IQueryable<InternalDisplacementWarehouseDocumentModel> Displacements = _context.InternalDisplacementWarehouseRegisters.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                Displacements = Displacements.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос документов внутренних перемещений обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = Displacements.Include(x => x.Author).Include(x => x.WarehouseReceipt).Take(pagingParameters.PageSize).Select(x => new
                {
                    x.Id,
                    Author = new { x.Author.Id, x.Author.Name },
                    WarehouseReceipt = new { x.WarehouseReceipt.Id, x.WarehouseReceipt.Name, x.WarehouseReceipt.Information },
                    WarehouseDebiting = GetWarehouse(x.WarehouseDebitingId, _context),
                    x.Information,
                    x.Name
                })
            }); ; ;
        }

        private static object GetWarehouse(int warehouseDebitingId, AppDataBaseContext context)
        {
            WarehouseGoodObjectModel warehouse = context.WarehousesGoods.Find(warehouseDebitingId);

            return new { warehouse?.Id, warehouse?.Name, warehouse?.Information, avatar = new { warehouse?.Avatar?.Id, warehouse?.Avatar?.Name } };
        }



        // GET: api/Displacements/5
        [HttpGet("{id}")]
        public async Task<ActionResult<InternalDisplacementWarehouseDocumentModel>> GetInternalDisplacementWarehouseDocumentModel(int id)
        {
            var internalDisplacementWarehouseDocumentModel = await _context.InternalDisplacementWarehouseRegisters.FindAsync(id);

            if (internalDisplacementWarehouseDocumentModel == null)
            {
                return NotFound();
            }

            return internalDisplacementWarehouseDocumentModel;
        }

        // PUT: api/Displacements/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInternalDisplacementWarehouseDocumentModel(int id, InternalDisplacementWarehouseDocumentModel internalDisplacementWarehouseDocumentModel)
        {
            if (id != internalDisplacementWarehouseDocumentModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(internalDisplacementWarehouseDocumentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InternalDisplacementWarehouseDocumentModelExists(id))
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

        // POST: api/Displacements
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<InternalDisplacementWarehouseDocumentModel>> PostInternalDisplacementWarehouseDocumentModel(InternalDisplacementWarehouseDocumentModel internalDisplacementWarehouseDocumentModel)
        {
            _context.InternalDisplacementWarehouseRegisters.Add(internalDisplacementWarehouseDocumentModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInternalDisplacementWarehouseDocumentModel", new { id = internalDisplacementWarehouseDocumentModel.Id }, internalDisplacementWarehouseDocumentModel);
        }

        // DELETE: api/Displacements/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<InternalDisplacementWarehouseDocumentModel>> DeleteInternalDisplacementWarehouseDocumentModel(int id)
        {
            var internalDisplacementWarehouseDocumentModel = await _context.InternalDisplacementWarehouseRegisters.FindAsync(id);
            if (internalDisplacementWarehouseDocumentModel == null)
            {
                return NotFound();
            }

            _context.InternalDisplacementWarehouseRegisters.Remove(internalDisplacementWarehouseDocumentModel);
            await _context.SaveChangesAsync();

            return internalDisplacementWarehouseDocumentModel;
        }

        private bool InternalDisplacementWarehouseDocumentModelExists(int id)
        {
            return _context.InternalDisplacementWarehouseRegisters.Any(e => e.Id == id);
        }
    }
}
