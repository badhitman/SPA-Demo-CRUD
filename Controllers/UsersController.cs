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
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.view;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelManager")]
    public class UsersController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(AppDataBaseContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers([FromQuery] PaginationParameters pagingParameters)
        {
            pagingParameters.Init(_context.Users.Count());
            IQueryable<UserModel> users = _context.Users.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                users = users.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос пользователей обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await users.Take(pagingParameters.PageSize).Select(x => new { x.Id, x.Name, Department = x.Department.Name, Role = x.Role.ToString(), x.isDisabled }).ToListAsync()
            });
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUserModel(int id)
        {
            var userModel = await _context.Users.FindAsync(id);

            if (userModel == null)
            {
                _logger.LogError("Запрашиваемый пользователь не найден по ключу: {0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Пользователь не найден: id=" + id,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }
            List<DepartmentModel> departments = await _context.Departments.ToListAsync();
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос успешно обработан. Пользователь найден. ",
                Status = StylesMessageEnum.warning.ToString(),
                Tag = new { userModel.Id, userModel.Name, userModel.TelegramId, userModel.Email, userModel.DepartmentId, userModel.Role, userModel.isDisabled, departments, UsersMetadataController.roles }
            });
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserModel(int id, UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Изменение пользователя невозможно. Ошибка контроля валидации модели");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка валидации модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (id != userModel.Id)
            {
                _logger.LogError("Ошибка контроля связи модели с параметрами запроса: id:{0} != userModel.Id:{1}", id, userModel.Id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе: id != departmentModel.Id",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Entry(userModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка выполнения запроса EF");
                if (!UserModelExists(id))
                {
                    _logger.LogError("Пользователь не найден: id={0}", id);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Пользователь не найден",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
                else
                {
                    _logger.LogWarning("Невнятная ошибка с пользователем id:{0}", id);
                    throw;
                }
            }
            List<DepartmentModel> departments = await _context.Departments.ToListAsync();
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Изменения сохранены",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { userModel.Id, userModel.DepartmentId, userModel.Email, userModel.isDisabled, userModel.Name, userModel.Readonly, userModel.Role, departments, UsersMetadataController.roles }
            });
        }

        // POST: api/Users
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<UserModel>> PostUserModel(UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Создание пользователя невозможно. Ошибка контроля валидации модели");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка валидации модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (_context.Users.Any(x => x.Name == userModel.Name))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Пользователь с таким именем уже существует",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            if (_context.Users.Any(x => x.Email == userModel.Email))
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Пользователь с таким логином уже существует",
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            _context.Users.Add(userModel);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Пользователь создан: id={0}", userModel.Id);
            List<DepartmentModel> departments = await _context.Departments.ToListAsync();
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Пользователь успешно создан: id=" + userModel.Id,
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { userModel.Id, userModel.DepartmentId, userModel.Email, userModel.isDisabled, userModel.Name, userModel.Readonly, userModel.Role, departments, UsersMetadataController.roles }
            });
        }

        // PATCH: api/Users/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchUserModel(int id)
        {
            var userModel = await _context.Users.FindAsync(id);
            if (userModel == null)
            {
                _logger.LogError("Манипуляция пользователем невозможна. Объект не найден");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Пользователь не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            userModel.isDisabled = !userModel.isDisabled;
            _context.Users.Update(userModel);
            await _context.SaveChangesAsync();

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Объект " + (userModel.isDisabled ? "Выключен" : "Включён"),
                Status = userModel.isDisabled ? StylesMessageEnum.secondary.ToString() : StylesMessageEnum.success.ToString(),
                Tag = userModel.isDisabled
            });
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<UserModel>> DeleteUserModel(int id)
        {
            var userModel = await _context.Users.FindAsync(id);
            if (userModel == null)
            {
                _logger.LogError("Удаление пользователя невозможно. Объект не найден");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Пользователь не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Users.Remove(userModel);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Пользователь удалён: id={0}", id);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Пользователь удалён",
                Status = StylesMessageEnum.success.ToString()
            });
        }

        private bool UserModelExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
