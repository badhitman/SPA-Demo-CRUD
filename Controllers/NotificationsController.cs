////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
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
    public class NotificationsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<NotificationsController> _logger;
        private readonly UserObjectModel _user;

        public NotificationsController(AppDataBaseContext context, SessionUser session, ILogger<NotificationsController> logger)
        {
            _context = context;
            _user = session.user;
            _logger = logger;
        }

        // GET: api/Notifications
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetNotification([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<NotificationObjectModel> notifications = _context.Notifications.Where(x => x.RecipientId == _user.Id);

            pagingParameters.Init(await notifications.CountAsync());
            if (pagingParameters.PageNum > 1)
                notifications = notifications.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            notifications = notifications.OrderBy(x => x.DateCreate).Include(x => x.Conversation);

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос уведомлений обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = (await notifications.ToListAsync()).Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Information,
                    x.DateCreate,
                    Conversation = new
                    {
                        x.Conversation.Id,
                        x.Conversation.Name,
                        x.Conversation.Information,
                        Initiator = new
                        {
                            Id = x.Conversation.InitiatorId,
                            Type = x.Conversation.InitiatorType.ToString()
                        },
                        x.Conversation.DateCreate
                    },
                    DeliveryStatus = x.DeliveryStatus.ToString()
                })
            });
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetNotification(int id)
        {
            NotificationObjectModel notification = await _context.Notifications.FindAsync(id);

            if (notification == null)
            {
                return NotFound();
            }

            return notification;
        }

        // PUT: api/Notifications/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotification(int id, NotificationObjectModel ajaxNotification)
        {
            if (id != ajaxNotification.Id)
            {
                return BadRequest();
            }

            _context.Entry(ajaxNotification).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            return NoContent();
        }

        // POST: api/Notifications
        [HttpPost]
        public async Task<ActionResult<object>> PostNotification(NotificationObjectModel ajaxNotification)
        {
            _context.Notifications.Add(ajaxNotification);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNotificationModel", new { id = ajaxNotification.Id }, ajaxNotification);
        }

        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteNotification(int id)
        {
            NotificationObjectModel notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }

            _context.Notifications.Remove(notification);
            await _context.SaveChangesAsync();

            return notification;
        }
    }
}
