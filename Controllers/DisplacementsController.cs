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
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisplacementsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public DisplacementsController(AppDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Displacements
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InternalDisplacementWarehouseDocumentModel>>> GetInternalDisplacementWarehouseRegisters()
        {
            return await _context.InternalDisplacementWarehouseRegisters.ToListAsync();
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
