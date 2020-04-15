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
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelAuth")]
    public class NotificationsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly SessionUser _sessionUser;

        public NotificationsController(AppDataBaseContext context, SessionUser sessionUser)
        {
            _context = context;
            _sessionUser = sessionUser;
        }

        // GET: api/Notifications
        [HttpGet]
        public ActionResult<IEnumerable<NotificationObjectModel>> GetNotificationModel([FromQuery] PaginationParametersModel pagingParameters)
        {
            if (_sessionUser.user is null)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса уведомлений. Пользователь текущей сессии не определён",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            var notifications = _context.Notifications.OrderBy(x => x.DateCreate).Include(x => x.Conversation)
                .Where(x => x.RecipientId == _sessionUser.user.Id);
                //.GroupBy(x => x.Conversation, (key, val) => new { key, val, countUnreadMessages = val.Where(x=>x.DeliveryStatus< DeliveryStatusesEnum.Read).Count(), countUndeliveredMessages = val.Where(x => x.DeliveryStatus < DeliveryStatusesEnum.Notified).Count() });

            pagingParameters.Init(notifications.Count());
            if (pagingParameters.PageNum > 1)
                notifications = notifications.Skip(pagingParameters.Skip);
            
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос уведомлений обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = notifications.ToList()
            });
        }

        // GET: api/Notifications/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NotificationObjectModel>> GetNotificationModel(int id)
        {
            var notificationModel = await _context.Notifications.FindAsync(id);

            if (notificationModel == null)
            {
                return NotFound();
            }

            return notificationModel;
        }

        // PUT: api/Notifications/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutNotificationModel(int id, NotificationObjectModel notificationModel)
        {
            if (id != notificationModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(notificationModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NotificationModelExists(id))
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

        // POST: api/Notifications
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<NotificationObjectModel>> PostNotificationModel(NotificationObjectModel notificationModel)
        {
            _context.Notifications.Add(notificationModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNotificationModel", new { id = notificationModel.Id }, notificationModel);
        }

        // DELETE: api/Notifications/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<NotificationObjectModel>> DeleteNotificationModel(int id)
        {
            var notificationModel = await _context.Notifications.FindAsync(id);
            if (notificationModel == null)
            {
                return NotFound();
            }

            //_context.Notifications.Remove(notificationModel);
            //await _context.SaveChangesAsync();

            return notificationModel;
        }

        private bool NotificationModelExists(int id)
        {
            return _context.Notifications.Any(e => e.Id == id);
        }
    }
}
