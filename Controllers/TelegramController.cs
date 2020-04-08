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
using SPADemoCRUD.Models.db;
using SPADemoCRUD.Models.view;

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
        public async Task<ActionResult<IEnumerable<TelegramBotUpdateModel>>> GetTelegramBotUpdate([FromQuery] PaginationParameters pagingParameters)
        {
            pagingParameters.Init(_context.TelegramBotUpdates.Count());
            IQueryable<TelegramBotUpdateModel> botUpdates = _context.TelegramBotUpdates.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                botUpdates = botUpdates.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос TelegramBot Updates",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await botUpdates.Take(pagingParameters.PageSize).Select(x => new { x.Id, x.Information }).ToListAsync()
            });
        }

        // GET: api/Telegram/5
        [HttpGet("{id}")]
        public async Task<ActionResult<TelegramBotUpdateModel>> GetTelegramBotUpdateModel(int id)
        {
            TelegramBotUpdateModel telegramBotUpdateModel = await _context.TelegramBotUpdates.FindAsync(id);

            if (telegramBotUpdateModel == null)
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
                Tag = telegramBotUpdateModel
            });
        }

        // DELETE: api/Telegram/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<TelegramBotUpdateModel>> DeleteTelegramBotUpdateModel(int id)
        {
            TelegramBotUpdateModel telegramBotUpdateModel = await _context.TelegramBotUpdates.FindAsync(id);
            if (telegramBotUpdateModel == null)
            {
                _logger.LogError("Удаление TelegramBot Update невозможно. Объект не найден");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "TelegramBot Update не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.TelegramBotUpdates.Remove(telegramBotUpdateModel);
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
