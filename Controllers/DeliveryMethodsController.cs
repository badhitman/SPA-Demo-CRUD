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
    public class DeliveryMethodsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<DeliveryMethodsController> _logger;

        public DeliveryMethodsController(AppDataBaseContext context, ILogger<DeliveryMethodsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Delivery
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDeliveryMethods([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(await _context.DeliveryServices.CountAsync());
            IQueryable<DeliveryServiceObjectModel> delServices = _context.DeliveryServices.OrderBy(x => x.Id);
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
        public async Task<ActionResult<object>> GetDeliveryMethod(int id)
        {
            DeliveryMethodObjectModel deliveryMethod = await _context.DeliveryMethods.FindAsync(id);

            if (deliveryMethod == null)
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
                Tag = new { deliveryMethod.Id, deliveryMethod.Name }
            });
        }

        // POST: api/Delivery
        [HttpPost]
        public async Task<ActionResult<object>> PostDeliveryMethod(DeliveryMethodObjectModel deliveryMethodAjax)
        {
            deliveryMethodAjax.Name = deliveryMethodAjax.Name.Trim();
            _context.DeliveryMethods.Add(deliveryMethodAjax);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDeliveryMethodModel", new { id = deliveryMethodAjax.Id }, deliveryMethodAjax);
        }

        // PUT: api/Delivery/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeliveryMethod(int id, DeliveryMethodObjectModel deliveryMethodAjax)
        {
            deliveryMethodAjax.Name = deliveryMethodAjax.Name.Trim();
            if (id != deliveryMethodAjax.Id || string.IsNullOrEmpty(deliveryMethodAjax.Name))
            {
                _logger.LogError("Запрашиваемая доставка не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая доставка не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Entry(deliveryMethodAjax).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // DELETE: api/Delivery/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteDeliveryMethod(int id)
        {
            DeliveryMethodObjectModel deliveryMethod = await _context.DeliveryMethods.FindAsync(id);
            if (deliveryMethod == null)
            {
                return NotFound();
            }

            //_context.DeliveryMethods.Remove(deliveryMethodModel);
            //await _context.SaveChangesAsync();

            return deliveryMethod;
        }
    }
}
