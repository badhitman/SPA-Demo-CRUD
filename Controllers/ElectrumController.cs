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
    [Authorize(Policy = "AccessMinLevelManager")]
    public class ElectrumController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<ElectrumController> _logger;

        public ElectrumController(AppDataBaseContext context, ILogger<ElectrumController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Electrum
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BtcTransactionModel>>> GetBtcTransactions([FromQuery] PaginationParameters pagingParameters)
        {
            pagingParameters.Init(_context.BtcTransactions.Count());
            IQueryable<BtcTransactionModel> users = _context.BtcTransactions.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                users = users.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос Electrum/BTC транзакций обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await _context.BtcTransactions.ToListAsync()
            });
        }

        // GET: api/Electrum/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BtcTransactionModel>> GetBtcTransactionModel(int id)
        {
            BtcTransactionModel btcTransactionModel = await _context.BtcTransactions.FindAsync(id);

            if (btcTransactionModel == null)
            {
                _logger.LogError("Запрашиваемая btc tx не найдена по ключу: {0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "btc tx не найдена: id=" + id,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос успешно обработан. btc tx найдена. ",
                Status = StylesMessageEnum.success.ToString(),
                Tag = btcTransactionModel
            });
        }
    }
}
