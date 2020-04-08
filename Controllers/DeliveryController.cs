using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.db.delivery;
using SPADemoCRUD.Models.view;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DeliveryController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<DeliveryController> _logger;

        public DeliveryController(AppDataBaseContext context, ILogger<DeliveryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Delivery
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeliveryServiceModel>>> GetDeliveryMethods([FromQuery] PaginationParameters pagingParameters)
        {
            pagingParameters.Init(_context.DeliveryServices.Count());
            IQueryable<DeliveryServiceModel> delServices = _context.DeliveryServices.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                delServices = delServices.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос дапартаментов обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await delServices.Take(pagingParameters.PageSize).ToListAsync()
            });
        }

        // GET: api/Delivery/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeliveryMethodModel>> GetDeliveryMethodModel(int id)
        {
            var deliveryMethodModel = await _context.DeliveryMethods.FindAsync(id);

            if (deliveryMethodModel == null)
            {
                _logger.LogError("Запрашиваемая доставка не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая доставка не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос успешно обработан. Департамент найден.",
                Status = StylesMessageEnum.success.ToString(),
                Tag = deliveryMethodModel
            });
        }

        // PUT: api/Delivery/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeliveryMethodModel(int id, DeliveryMethodModel deliveryMethodModel)
        {
            if (id != deliveryMethodModel.Id)
            {
                _logger.LogError("Запрашиваемая доставка не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая доставка не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Entry(deliveryMethodModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeliveryMethodModelExists(id))
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

        // POST: api/Delivery
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<DeliveryMethodModel>> PostDeliveryMethodModel(DeliveryMethodModel deliveryMethodModel)
        {
            _context.DeliveryMethods.Add(deliveryMethodModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeliveryMethodModel", new { id = deliveryMethodModel.Id }, deliveryMethodModel);
        }

        // DELETE: api/Delivery/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DeliveryMethodModel>> DeleteDeliveryMethodModel(int id)
        {
            var deliveryMethodModel = await _context.DeliveryMethods.FindAsync(id);
            if (deliveryMethodModel == null)
            {
                return NotFound();
            }

            _context.DeliveryMethods.Remove(deliveryMethodModel);
            await _context.SaveChangesAsync();

            return deliveryMethodModel;
        }

        private bool DeliveryMethodModelExists(int id)
        {
            return _context.DeliveryMethods.Any(e => e.Id == id);
        }
    }
}
