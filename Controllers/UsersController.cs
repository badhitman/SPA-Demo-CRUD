////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiTool;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelManager")]
    public class UsersController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<UsersController> _logger;
        private readonly UserObjectModel _user;

        public UsersController(AppDataBaseContext context, ILogger<UsersController> logger, SessionUser session)
        {
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<UserObjectModel> users = _context.Users.AsQueryable();
            if (_user.Role < AccessLevelUserRolesEnum.Admin)
            {
                users = users.Where(x => !x.isDisabled);
            }

            pagingParameters.Init(await users.CountAsync());
            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            users = users.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                users = users.Skip(pagingParameters.Skip);

            users = users.Take(pagingParameters.PageSize)
                .Include(x => x.Department)
                .Include(x => x.Avatar);

            string TypeName = nameof(UserObjectModel);

            var qUsers = from user in users
                         join UserFavoriteLocator in _context.UserFavoriteLocators
                         on new { ObjectId = user.Id, TypeName, UserId = _user.Id }
                         equals new { UserFavoriteLocator.ObjectId, UserFavoriteLocator.TypeName, UserFavoriteLocator.UserId }
                         into joinFavoriteLocator
                         from isFavoriteMark in joinFavoriteLocator.DefaultIfEmpty()
                         select new { user, isFavoriteMark };

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос пользователей обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = (await qUsers.ToListAsync()).Select(selectItem => new
                {
                    selectItem.user.Id,
                    selectItem.user.Name,
                    selectItem.user.Information,
                    selectItem.user.TelegramId,
                    Role = selectItem.user.Role.ToString(),

                    selectItem.user.isReadonly,
                    selectItem.user.isDisabled,
                    selectItem.user.isGlobalFavorite,
                    IsUserFavorite = selectItem.isFavoriteMark != null,
                    selectItem.user.LastTelegramVisit,
                    selectItem.user.LastWebVisit,

                    Department = new
                    {
                        selectItem.user.Department.Id,
                        selectItem.user.Department.Name
                    },
                    Avatar = new
                    {
                        selectItem.user.Avatar?.Id,
                        selectItem.user.Avatar?.Name
                    }
                })
            });
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUser(int id)
        {
            string TypeName = nameof(UserObjectModel);

            IQueryable<UserObjectModel> users = _context.Users.AsQueryable();
            var qUsers = from user in users.Include(x => x.Avatar).Include(x => x.Department)
                         join UserFavoriteLocator in _context.UserFavoriteLocators
                         on new { ObjectId = user.Id, TypeName, UserId = _user.Id }
                         equals new { UserFavoriteLocator.ObjectId, UserFavoriteLocator.TypeName, UserFavoriteLocator.UserId }
                         into joinFavoriteLocator
                         from isFavoriteMark in joinFavoriteLocator.DefaultIfEmpty()
                         select new { user, isFavoriteMark };

            var selectItem = await qUsers.FirstOrDefaultAsync(x => x.user.Id == id);

            if (selectItem == null)
            {
                string msg = $"Запрашиваемый пользователь не найден в БД. Id={id}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос пользователя успешно обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    selectItem.user.Id,
                    selectItem.user.Name,
                    selectItem.user.Information,
                    selectItem.user.isDisabled,
                    selectItem.user.isGlobalFavorite,
                    IsUserFavorite = selectItem.isFavoriteMark != null,
                    selectItem.user.LastTelegramVisit,
                    selectItem.user.LastWebVisit,
                    selectItem.user.isReadonly,
                    selectItem.user.TelegramId,
                    selectItem.user.Email,
                    selectItem.user.Role,

                    noDelete = _context.MovementTurnoverDeliveryDocuments.Any(x => x.BuyerId == selectItem.user.Id)
                    || _context.BtcTransactionOuts.Any(x => x.UserId == selectItem.user.Id)
                    || _context.UserFavoriteLocators.Any(x => x.UserId == selectItem.user.Id)
                    || _context.TelegramBotUpdates.Any(x => x.UserId == selectItem.user.Id)
                    || _context.FileRegisteringObjectRows.Any(x => x.AuthorId == selectItem.user.Id)
                    || _context.Notifications.Any(x => x.RecipientId == selectItem.user.Id)
                    || _context.Messages.Any(x => x.SenderId == selectItem.user.Id)
                    || _context.GoodMovementDocuments.Any(x => x.AuthorId == selectItem.user.Id)
                    || _context.Сonversations.Any(x => x.InitiatorType == ConversationInitiatorsEnum.User && x.InitiatorId == selectItem.user.Id),

                    Department = new
                    {
                        selectItem.user.Department.Id,
                        selectItem.user.Department.Name
                    },
                    Avatar = new
                    {
                        selectItem.user.Avatar?.Id,
                        selectItem.user.Avatar?.Name
                    },

                    departments = await _context.Departments.Include(x => x.Avatar).Select(x => new
                    {
                        x.Id,
                        x.Name,
                        x.Information,
                        x.isDisabled,
                        IsFavorite = x.isGlobalFavorite,
                        x.isReadonly,
                        Avatar = new
                        {
                            x.Avatar.Id,
                            x.Avatar.Name
                        }
                    }).ToListAsync(),
                    UsersMetadataController.roles
                }
            });
        }

        // POST: api/Users
        [HttpPost]
        public async Task<ActionResult<object>> PostUser(UserObjectModel ajaxUser)
        {
            _logger.LogInformation("Создание пользователя. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxUser.Name = ajaxUser.Name.Trim();
            ajaxUser.Information = ajaxUser.Information.Trim();
            ajaxUser.Email = ajaxUser.Email.Trim();

            if (!ModelState.IsValid || string.IsNullOrEmpty(ajaxUser.Name) || !await _context.Departments.AnyAsync(x => x.Id == ajaxUser.DepartmentId))
            {
                msg = "Ошибка запроса. Пользователь не создан";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (await _context.Users.AnyAsync(usr => usr.Name.ToLower() == ajaxUser.Name.ToLower()))
            {
                msg = $"Пользователь с таким именем '{ajaxUser.Name}' уже существует (без учёта регистра). Придумайте уникальное";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            if (await _context.Users.AnyAsync(usr => usr.Email.ToLower() == ajaxUser.Email.ToLower()))
            {
                msg = $"Пользователь с таким e-mail/login '{ajaxUser.Email}' уже существует (без учёта регистра). Придумайте уникальное";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            if (_user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                ajaxUser.isDisabled = false;
                ajaxUser.isGlobalFavorite = false;
                ajaxUser.isReadonly = false;
            }

            if (!string.IsNullOrWhiteSpace(ajaxUser.Password))
            {
                ajaxUser.Password = glob_tools.GetHashString(ajaxUser.Password);
            }

            await _context.Users.AddAsync(ajaxUser);
            await _context.SaveChangesAsync();

            msg = $"Пользователь добавлен в БД: id={ajaxUser.Id}";
            _logger.LogInformation(msg);

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.success.ToString(),
                Tag = ajaxUser.Id
            });
        }

        // PUT: api/Users/5
        [HttpPut("{id}")]
        public async Task<ActionResult<object>> PutUser(int id, UserObjectModel ajaxUser)
        {
            _logger.LogInformation($"Запрос изменения пользователя [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxUser.Name = ajaxUser.Name.Trim();
            ajaxUser.Information = ajaxUser.Information.Trim();
            ajaxUser.Email = ajaxUser.Email.Trim();

            if (!ModelState.IsValid
                || string.IsNullOrEmpty(ajaxUser.Name)
                || id != ajaxUser.Id
                || id <= 0)
            {
                msg = "Изменение пользователя невозможно. Ошибка контроля валидации модели";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (!await _context.Users.AnyAsync(usr => usr.Id == id)
                || await _context.Users.AnyAsync(usr => (usr.Name.ToLower() == ajaxUser.Name.ToLower() || usr.Email.ToLower() == ajaxUser.Email.ToLower()) && usr.Id != id))
            {
                msg = "Ошибка в запросе. При проверке корректности запроса были обнаружены ошибки.";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            UserObjectModel userDb = await _context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (userDb.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Объект только для чтения. Для удаления данного объекта требуется уровень привелегий [{AccessLevelUserRolesEnum.ROOT}]. Ваш уровень привелегий: {_user.Role}"; ;
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            userDb.AvatarId = ajaxUser.AvatarId;
            userDb.DepartmentId = ajaxUser.DepartmentId;
            userDb.Email = ajaxUser.Email;
            userDb.Information = ajaxUser.Information;
            userDb.Name = ajaxUser.Name;

            if (_user.Role == AccessLevelUserRolesEnum.ROOT)
            {
                userDb.Role = ajaxUser.Role;
                userDb.isGlobalFavorite = ajaxUser.isGlobalFavorite;
                userDb.isDisabled = ajaxUser.isDisabled;
                userDb.isReadonly = ajaxUser.isReadonly;
            }

            _context.Users.Update(userDb);

            try
            {
                await _context.SaveChangesAsync();
                msg = $"Изменения пользователя [{userDb.Id}] сохранены";
                _logger.LogInformation(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = true,
                    Info = msg,
                    Status = StylesMessageEnum.success.ToString()
                });
            }
            catch (Exception ex)
            {
                msg = $"Во время изменения 'Пользователя' [{userDb.Id}] произошла ошибка. Exception: {ex.Message}{(ex.InnerException is null ? "" : ". InnerException: " + ex.InnerException.Message)}";
                _logger.LogError(ex, msg);

                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteUser(int id)
        {
            _logger.LogInformation($"Запрос удаления пользователя [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            UserObjectModel user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                msg = $"Удаление пользователя невозможно. Объект не найден";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (user.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Объект только для чтения. Для удаления данного объекта требуется уровень привелегий [{AccessLevelUserRolesEnum.ROOT}]. Ваш уровень привелегий: {_user.Role}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (user.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                msg = $"Объект [#{id}] 'только для чтения'. Для изменения такого объекта требуются привелегии [{AccessLevelUserRolesEnum.ROOT}]. Ваши привилегии: [{_user.Role}]";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (await _context.MovementTurnoverDeliveryDocuments.AnyAsync(x => x.BuyerId == id)
                    || await _context.BtcTransactionOuts.AnyAsync(x => x.UserId == id)
                    || await _context.UserFavoriteLocators.AnyAsync(x => x.UserId == id)
                    || await _context.TelegramBotUpdates.AnyAsync(x => x.UserId == id)
                    || await _context.FileRegisteringObjectRows.AnyAsync(x => x.AuthorId == id)
                    || await _context.Notifications.AnyAsync(x => x.RecipientId == id)
                    || await _context.Messages.AnyAsync(x => x.SenderId == id)
                    || await _context.GoodMovementDocuments.AnyAsync(x => x.AuthorId == id)
                    || await _context.Сonversations.AnyAsync(x => x.InitiatorType == ConversationInitiatorsEnum.User && x.InitiatorId == id))
            {
                msg = "Объект запрещено удалять (на него существуют ссылки)";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            msg = $"Пользователь [#{user.Id}] удалён";
            _logger.LogInformation(msg);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.warning.ToString()
            });
        }
    }
}
