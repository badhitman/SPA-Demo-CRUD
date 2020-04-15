////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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
    public class GoodsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<GoodsController> _logger;

        public GoodsController(AppDataBaseContext context, ILogger<GoodsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Goods
        [HttpGet]
        public ActionResult<object> GetGoods([FromQuery] PaginationParametersModel pagingParameters)
        {
            pagingParameters.Init(_context.GroupsGoods.Count());
            IQueryable<GoodObjectModel> groupsGoods = _context.Goods.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                groupsGoods = groupsGoods.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = groupsGoods.Include(x => x.Avatar).Include(x => x.Unit).Include(x => x.Group).Take(pagingParameters.PageSize).Select(x => new { x.Id, x.Avatar, x.Name, x.Readonly, x.Group, x.Information, x.Price, x.Unit })
            });
        }

        // GET: api/Goods/5
        [HttpGet("{id}")]
        public async Task<ActionResult<GoodObjectModel>> GetGoodModel([FromQuery] PaginationParametersModel pagingParameters, int id)
        {
            GoodObjectModel goodModel = await _context.Goods.Include(x => x.Avatar).Include(x => x.Unit).Include(x => x.Group).FirstOrDefaultAsync(x => x.Id == id);

            if (goodModel == null)
            {
                _logger.LogError("Номенклатура не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Номенклатура не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            IQueryable<RowGoodMovementRegisterModel> goodRegisters = _context.GoodMovementDocumentRows.Where(x => x.GoodId == id).OrderBy(x => x.Id);
            pagingParameters.Init(goodRegisters.Count());
            if (pagingParameters.PageNum > 1)
                goodRegisters = goodRegisters.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос номенклатуры обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    avatar = new { id = goodModel.AvatarId, goodModel.Avatar?.Name },
                    goodModel.Id,
                    goodModel.Name,
                    goodModel.Information,
                    goodModel.Price,
                    goodModel.isDisabled,
                    goodModel.IsGlobalFavorite,
                    goodModel.Readonly,
                    goodModel.GroupId,
                    goodModel.UnitId,
                    units = _context.Units.Select(x => new { x.Id, x.Name, x.Information }),
                    groups = _context.GroupsGoods.Select(x => new { x.Id, x.Name, x.Information }),
                    registers = goodRegisters.Include(x => x.BodyDocument).Take(pagingParameters.PageSize).Select(x => getRegister(x, _context))
                }
            });
        }

        /// <summary>
        /// Формирование анонимного/динамического объекта: запись регистра оборотов номенклатуры
        /// </summary>
        /// <param name="x">объект строки записи регистра</param>
        /// <param name="context">контекст базы данных</param>
        /// <returns>возвращается запись регистра в виде объекта анонимного типа</returns>
        private static object getRegister(RowGoodMovementRegisterModel x, AppDataBaseContext context)
        {
            return new
            {
                x.Id,
                document = getDocument(x.BodyDocument, context),
                x.Quantity,
                x.Price,
                x.UnitId
            };
        }

        /// <summary>
        /// Формирование анономного/динамического объекта: документ-владелец записи регистра.
        /// </summary>
        /// <param name="document">объект-документ, строки которого представляют записи регистров</param>
        /// <param name="context">контекст базы данных</param>
        /// <returns>возвращается объект-документ-владелец записи регистра в виде объекта анонимного типа</returns>
        private static object getDocument(BodyGoodMovementDocumentModel document, AppDataBaseContext context)
        {
            switch (document.Discriminator.ToLower())
            {
                case "receipttowarehousedocumentmodel":
                    ReceiptToWarehouseDocumentModel MovementGoodsWarehousesDocument = context.ReceiptesGoodsToWarehousesRegisters.Include(x => x.Author).Include(x => x.WarehouseReceipt).FirstOrDefault(x => x.Id == document.Id);
                    MovementGoodsWarehousesDocument.WarehouseReceipt = context.WarehousesGoods.FirstOrDefault(x => x.Id == MovementGoodsWarehousesDocument.WarehouseReceiptId);
                    return new
                    {
                        document.DateCreate,
                        author = new { id = document.AuthorId, document.Author.Name },
                        document.Id,
                        document.Name,
                        document.Information,
                        apiName = "movementgoodswarehouses",
                        about = "Поступление на склад",
                        Warehouse = new { MovementGoodsWarehousesDocument.WarehouseReceipt.Id, MovementGoodsWarehousesDocument.WarehouseReceipt.Name, MovementGoodsWarehousesDocument.WarehouseReceipt.Information }
                    };
                case "movementturnoverdeliverydocumentmodel":
                    MovementTurnoverDeliveryDocumentModel MovementTurnoverDeliveryDocument = context.MovementTurnoverDeliveryDocuments.Include(x => x.DeliveryService).Include(x => x.DeliveryMethod).Include(x => x.Buyer).FirstOrDefault(x => x.Id == document.Id);
                    return new
                    {
                        document.DateCreate,
                        author = new { id = document.AuthorId, document.Author.Name },
                        document.Id,
                        document.Name,
                        document.Information,
                        apiName = "movementturnoverdelivery",
                        about = "Отгрузка/Доставка",
                        buyer = new
                        {
                            MovementTurnoverDeliveryDocument.Buyer?.Id,
                            MovementTurnoverDeliveryDocument.Buyer?.Name
                        },
                        deliveryMethod = new
                        {
                            MovementTurnoverDeliveryDocument.DeliveryMethod?.Id,
                            MovementTurnoverDeliveryDocument.DeliveryMethod?.Name
                        },
                        deliveryService = new
                        {
                            MovementTurnoverDeliveryDocument.DeliveryService?.Id,
                            MovementTurnoverDeliveryDocument.DeliveryService?.Name
                        },
                        MovementTurnoverDeliveryDocument.DeliveryAddress1,
                        MovementTurnoverDeliveryDocument.DeliveryAddress2
                    };
                default:
                    return new { document.Id, document.Name, document.Information };
            }
        }

        // PUT: api/Goods/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutGoodModel(int id, GoodObjectModel goodModel)
        {
            if (!ModelState.IsValid)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка контроля валидности модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (id != goodModel.Id)
            {
                _logger.LogError("Ошибка в запросе: {0} != goodModel.Id", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Goods.Attach(goodModel);
            _context.Entry(goodModel).Property(x => x.Name).IsModified = true;
            _context.Entry(goodModel).Property(x => x.Price).IsModified = true;
            _context.Entry(goodModel).Property(x => x.UnitId).IsModified = true;
            _context.Entry(goodModel).Property(x => x.GroupId).IsModified = true;

            try
            {
                await _context.SaveChangesAsync();
                goodModel = _context.Goods.Find(id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = "Номенклатура сохранена",
                    Status = StylesMessageEnum.success.ToString(),
                    Tag = goodModel
                });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!GoodModelExists(id))
                {
                    _logger.LogError("Номенклатура не найдена {0}", id);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Номенклатура не найдена",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
                else
                {
                    _logger.LogError("Невнятная ошибка во время обновлении номенклатуры");
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Невнятная ошибка во время обновлении номенклатуры",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
            }
        }

        // POST: api/Goods/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost("{id}")]
        public async Task<ActionResult<GroupGoodsObjectModel>> PostGood(int id, GoodObjectModel good)
        {
            if (!ModelState.IsValid)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка контроля валидности модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (good.GroupId != id)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе: good.GroupId != " + id,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            good.Name = good.Name.Trim();
            if (string.IsNullOrEmpty(good.Name))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Создаваемый объект должен иметь имя",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (good.GroupId <= 0)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Требуется указать группу",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
            if (!_context.GroupsGoods.Any(x => x.Id == good.GroupId))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Группа номенклатуры не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (good.UnitId <= 0)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Требуется указать единицу измерения",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
            if (!_context.Units.Any(x => x.Id == good.UnitId))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Единица измерения номенклатуры не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_context.Goods.Any(x => x.Name == good.Name))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Номенклатура с таким именем уже существует в бд. Придумайте уникальное",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
            _context.Goods.Add(good);
            await _context.SaveChangesAsync();
            HttpContext.Response.Cookies.Append("rowsCount", _context.Goods.Count().ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Номенклатура создана",
                Status = StylesMessageEnum.success.ToString(),
                Tag = good
            });
        }

        // DELETE: api/Goods/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<GoodObjectModel>> DeleteGoodModel(int id)
        {
            var goodModel = await _context.Goods.FindAsync(id);
            if (goodModel == null)
            {
                _logger.LogError("Запрашиваемая номенклатура не найдена: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая номенклатура не найдена",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_context.GoodMovementDocumentRows.Any(x => x.GoodId == id))
            {
                _logger.LogError("Запрашиваемая номенклатура не может быть удалена (есть ссылки в регистрах): id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемая номенклатура не может быть удалена (есть ссылки в регистрах)",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Goods.Remove(goodModel);
            await _context.SaveChangesAsync();

            _logger.LogWarning("Номенклатура удалена: id={0}", id);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Номенклатура удалена",
                Status = StylesMessageEnum.info.ToString()
            });
        }

        private bool GoodModelExists(int id)
        {
            return _context.Goods.Any(e => e.Id == id);
        }
    }
}
