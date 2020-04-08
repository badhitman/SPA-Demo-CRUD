using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.view;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelRoot")]
    public class ServerController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<ServerController> _logger;
        protected readonly AppConfig conf;

        public ServerController(AppDataBaseContext context, ILogger<ServerController> logger, IOptions<AppConfig> options)
        {
            _context = context;
            _logger = logger;
            conf = options.Value;
        }

        // GET: api/Server
        [HttpGet]
        public ActionResult<object> GetServer()
        {
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос состояния сервера обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = conf
            });
        }
    }
}