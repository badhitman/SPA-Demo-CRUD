////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TurnoversController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public TurnoversController(AppDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/MovementTurnoverDelivery
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovementTurnoverDeliveryDocumentModel>>> GetMovementTurnoverDeliveryDocuments()
        {
            return await _context.MovementTurnoverDeliveryDocuments.ToListAsync();
        }

        // GET: api/MovementTurnoverDelivery/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MovementTurnoverDeliveryDocumentModel>> GetMovementTurnoverDeliveryDocumentModel(int id)
        {
            var movementTurnoverDeliveryDocumentModel = await _context.MovementTurnoverDeliveryDocuments.FindAsync(id);

            if (movementTurnoverDeliveryDocumentModel == null)
            {
                return NotFound();
            }

            return movementTurnoverDeliveryDocumentModel;
        }

        // PUT: api/MovementTurnoverDelivery/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMovementTurnoverDeliveryDocumentModel(int id, MovementTurnoverDeliveryDocumentModel movementTurnoverDeliveryDocumentModel)
        {
            if (id != movementTurnoverDeliveryDocumentModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(movementTurnoverDeliveryDocumentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovementTurnoverDeliveryDocumentModelExists(id))
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

        // POST: api/MovementTurnoverDelivery
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<MovementTurnoverDeliveryDocumentModel>> PostMovementTurnoverDeliveryDocumentModel(MovementTurnoverDeliveryDocumentModel movementTurnoverDeliveryDocumentModel)
        {
            _context.MovementTurnoverDeliveryDocuments.Add(movementTurnoverDeliveryDocumentModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMovementTurnoverDeliveryDocumentModel", new { id = movementTurnoverDeliveryDocumentModel.Id }, movementTurnoverDeliveryDocumentModel);
        }

        // DELETE: api/MovementTurnoverDelivery/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<MovementTurnoverDeliveryDocumentModel>> DeleteMovementTurnoverDeliveryDocumentModel(int id)
        {
            var movementTurnoverDeliveryDocumentModel = await _context.MovementTurnoverDeliveryDocuments.FindAsync(id);
            if (movementTurnoverDeliveryDocumentModel == null)
            {
                return NotFound();
            }

            _context.MovementTurnoverDeliveryDocuments.Remove(movementTurnoverDeliveryDocumentModel);
            await _context.SaveChangesAsync();

            return movementTurnoverDeliveryDocumentModel;
        }

        private bool MovementTurnoverDeliveryDocumentModelExists(int id)
        {
            return _context.MovementTurnoverDeliveryDocuments.Any(e => e.Id == id);
        }
    }
}
