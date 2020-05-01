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
    [Authorize(Policy = "AccessMinLevelManager")]
    public class TelegramController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<TelegramController> _logger;

        public TelegramController(AppDataBaseContext context, ILogger<TelegramController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Telegram
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetTelegramBotUpdate([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(await _context.TelegramBotUpdates.CountAsync());
            IQueryable<TelegramBotUpdateObjectModel> botUpdates = _context.TelegramBotUpdates.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                botUpdates = botUpdates.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос TelegramBot Updates",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await botUpdates.Include(x => x.User).Take(pagingParameters.PageSize).Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Information,
                    x.DateCreate,
                    x.isBotMessage,
                    User = new
                    {
                        x.User.Id,
                        x.User.Name,
                        Role = x.User.Role.ToString()
                    }
                }).ToListAsync()
            });
        }

        // GET: api/Telegram/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetTelegramBotUpdate(int id)
        {
            TelegramBotUpdateObjectModel telegramBotUpdate = await _context.TelegramBotUpdates
                .Include(x => x.User).ThenInclude(x => x.Avatar)
                .Include(x => x.User).ThenInclude(x => x.Department)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (telegramBotUpdate == null)
            {
                _logger.LogError("Запрошеный TelegramBot Update не найден по ключу: {0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "TelegramBot Update не найден: id=" + id,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос успешно обработан. TelegramBot Update найден. ",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    telegramBotUpdate.Id,
                    telegramBotUpdate.DateCreate,
                    telegramBotUpdate.Name,
                    telegramBotUpdate.Information,
                    telegramBotUpdate.isBotMessage,
                    User = new
                    {
                        telegramBotUpdate.User.Id,
                        telegramBotUpdate.User.Name,
                        telegramBotUpdate.User.Information,
                        telegramBotUpdate.User.isDisabled,
                        telegramBotUpdate.User.LastTelegramVisit,
                        telegramBotUpdate.User.LastWebVisit,
                        telegramBotUpdate.User.isReadonly,
                        Role = telegramBotUpdate.User.Role.ToString(),
                        Avatar = new
                        {
                            telegramBotUpdate.User.Avatar.Id,
                            telegramBotUpdate.User.Avatar.Name,
                            telegramBotUpdate.User.Avatar.Information
                        },
                        Department = new
                        {
                            telegramBotUpdate.User.Department.Id,
                            telegramBotUpdate.User.Department.Name,
                            telegramBotUpdate.User.Department.Information
                        }
                    }
                }
            });
        }

        // DELETE: api/Telegram/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteTelegramBotUpdate(int id)
        {
            TelegramBotUpdateObjectModel telegramBotUpdate = await _context.TelegramBotUpdates.FindAsync(id);
            if (telegramBotUpdate == null)
            {
                _logger.LogError("Удаление TelegramBot Update невозможно. Объект не найден");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "TelegramBot Update не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.TelegramBotUpdates.Remove(telegramBotUpdate);
            await _context.SaveChangesAsync();

            _logger.LogInformation("TelegramBot Update удалён: id={0}", id);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "TelegramBot Update удалён",
                Status = StylesMessageEnum.success.ToString()
            });
        }
    }
}
