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
            return await users.Take(pagingParameters.PageSize).Select(x => new { x.Id, x.Name, Department = x.Department.Name, Role = x.Role.ToString(), x.isDisabled }).ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetUserModel(int id)
        {
            var userModel = await _context.Users.FindAsync(id);

            if (userModel == null)
            {
                _logger.LogError("Запрашиваемый пользователь не найден по ключу: {0}", id);
                return NotFound();
            }
            List<DepartmentModel> departments = await _context.Departments.ToListAsync();
            return new { userModel.Id, userModel.Name, userModel.TelegramId, userModel.Email, userModel.DepartmentId, userModel.Role, userModel.isDisabled, departments, UsersMetadataController.roles };
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserModel(int id, UserModel userModel)
        {
            if (!ModelState.IsValid)
            {
                return new ObjectResult(ModelState);
            }

            if (id != userModel.Id)
            {
                _logger.LogError("Ошибка контроля связи модели с параметрами запроса: id:{0} != userModel.Id:{1}", id, userModel.Id);
                return BadRequest();
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
                    _logger.LogWarning("Такого пользователя нет id:{0}", id);
                    return NotFound();
                }
                else
                {
                    _logger.LogWarning("Невнятная ошибка с пользователем id:{0}", id);
                    throw;
                }
            }

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Изменения сохранены",
                Status = StylesMessageEnum.success.ToString(),
                Tag = userModel
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
                return new ObjectResult(ModelState);
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

            return CreatedAtAction(nameof(GetUserModel), new { id = userModel.Id }, userModel);
        }

        // PATCH: api/Users/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchUserModel(int id)
        {
            var userModel = await _context.Users.FindAsync(id);
            if (userModel == null)
            {
                return NotFound();
            }

            userModel.isDisabled = !userModel.isDisabled;
            _context.Users.Update(userModel);
            await _context.SaveChangesAsync();

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Объекту установлено новое состояние",
                Status = StylesMessageEnum.success.ToString(),
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
                return NotFound();
            }

            _context.Users.Remove(userModel);
            await _context.SaveChangesAsync();

            return userModel;
        }

        private bool UserModelExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
