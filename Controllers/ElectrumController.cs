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
        public async Task<ActionResult<IEnumerable<object>>> GetBtcTransactions([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(await _context.BtcTransactions.CountAsync());
            IQueryable<BtcTransactionObjectModel> users = _context.BtcTransactions.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                users = users.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос Electrum/BTC транзакций обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = (await _context.BtcTransactions.ToListAsync()).Select(tx => new
                {
                    tx.Id,
                    tx.Name,
                    tx.Information,
                    tx.Sum,
                    tx.TxId,
                    CountOutputs = _context.BtcTransactionOuts.Count(txOut => txOut.BtcTransactionId == tx.Id)
                })
            });
        }

        // GET: api/Electrum/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetBtcTransaction(int id)
        {
            BtcTransactionObjectModel tx = await _context.BtcTransactions.Include(x => x.Outputs).ThenInclude(x => x.User).FirstOrDefaultAsync(x => x.Id == id);

            if (tx == null)
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
                Tag = new
                {
                    tx.Id,
                    tx.Name,
                    tx.Information,
                    tx.Sum,
                    tx.TxId,
                    Outputs = tx.Outputs.Select(txOut => new
                    {
                        txOut.Id,
                        txOut.Information,
                        txOut.Name,
                        txOut.Address,
                        txOut.DateCreate,
                        txOut.IsMine,
                        txOut.Sum,
                        User = new
                        {
                            txOut.User.Id,
                            txOut.User.Name,
                            txOut.User.Information,
                            txOut.User.isDisabled,
                            txOut.User.LastWebVisit,
                            Role = txOut.User.Role.ToString(),
                            Telegram = new
                            {
                                id = txOut.User.TelegramId,
                                LastVisit = txOut.User.LastTelegramVisit
                            }
                        }
                    })
                }
            });
        }
    }
}
