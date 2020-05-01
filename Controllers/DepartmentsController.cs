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
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelAdmin")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly ILogger<DepartmentsController> _logger;
        private readonly UserObjectModel _user;

        public DepartmentsController(AppDataBaseContext context, ILogger<DepartmentsController> logger, SessionUser session)
        {
            _context = context;
            _logger = logger;
            _user = session.user;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetDepartments([FromQuery] PaginationParametersModel pagingParameters)
        {
            IQueryable<DepartmentObjectModel> departments = _context.Departments.AsQueryable();
            if (_user.Role < AccessLevelUserRolesEnum.Admin)
            {
                departments = departments.Where(x => !x.isDisabled);
            }

            pagingParameters.Init(await departments.CountAsync());

            departments = departments.OrderBy(x => x.Id);

            if (pagingParameters.PageNum > 1)
                departments = departments.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());

            departments = departments.Take(pagingParameters.PageSize);

            string TypeName = nameof(DepartmentObjectModel);
            var qDepartments = from department in departments
                               join UserFavoriteLocator in _context.UserFavoriteLocators
                               on new { ObjectId = department.Id, TypeName, UserId = _user.Id }
                               equals new { UserFavoriteLocator.ObjectId, UserFavoriteLocator.TypeName, UserFavoriteLocator.UserId }
                               into joinFavoriteLocator
                               from isFavoriteMark in joinFavoriteLocator.DefaultIfEmpty()
                               select new { department, isFavoriteMark };

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос дапартаментов обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = (await qDepartments.ToListAsync()).Select(selectItem => new
                {
                    selectItem.department.Id,
                    selectItem.department.Name,
                    selectItem.department.Information,

                    selectItem.department.isDisabled,
                    selectItem.department.isGlobalFavorite,
                    IsUserFavorite = selectItem.isFavoriteMark != null,
                    selectItem.department.isReadonly,

                    Avatar = new
                    {
                        selectItem.department.Avatar?.Id,
                        selectItem.department.Avatar?.Name
                    }
                })
            });
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetDepartment(int id)
        {
            IQueryable<DepartmentObjectModel> departments = _context.Departments.Where(x => x.Id == id);
            string TypeName = nameof(DepartmentObjectModel);

            var exDepartments = from department in departments
                                join UserFavoriteLocator in _context.UserFavoriteLocators
                                on new { ObjectId = department.Id, TypeName, UserId = _user.Id }
                                equals new { UserFavoriteLocator.ObjectId, UserFavoriteLocator.TypeName, UserFavoriteLocator.UserId }
                                into joinFavoriteLocator
                                from isFavoriteMark in joinFavoriteLocator.DefaultIfEmpty()
                                select new { department, isFavoriteMark };

            var selDepartment = await exDepartments.FirstOrDefaultAsync();

            if (selDepartment == null)
            {
                string msg = $"Запрашиваемый департамент не найден: id={id}";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            IQueryable<UserObjectModel> usersByDepartment = _context.Users.Where(x => x.DepartmentId == id).Include(x => x.Avatar);

            TypeName = nameof(UserObjectModel);
            var exUsersByDepartment = from user in usersByDepartment
                                      join UserFavoriteLocator in _context.UserFavoriteLocators
                                      on new { ObjectId = user.Id, TypeName, UserId = _user.Id }
                                      equals new { UserFavoriteLocator.ObjectId, UserFavoriteLocator.TypeName, UserFavoriteLocator.UserId }
                                      into joinFavoriteLocator
                                      from isFavoriteMark in joinFavoriteLocator.DefaultIfEmpty()
                                      select new { user, isFavoriteMark };

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос департамента успешно обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    selDepartment.department.Id,
                    selDepartment.department.Name,
                    selDepartment.department.Information,

                    selDepartment.department.isDisabled,
                    selDepartment.department.isGlobalFavorite,
                    IsUserFavorite = selDepartment.isFavoriteMark != null,
                    selDepartment.department.isReadonly,

                    noDelete = await _context.Users.AnyAsync(x => x.DepartmentId == id),

                    Avatar = new
                    {
                        selDepartment.department.Avatar?.Id,
                        selDepartment.department.Avatar?.Name
                    },
                    Users = (await exUsersByDepartment.ToListAsync()).Select(selectUserByDepartment => new
                    {
                        selectUserByDepartment.user.Id,
                        selectUserByDepartment.user.Name,
                        selectUserByDepartment.user.Information,
                        role = selectUserByDepartment.user.Role.ToString(),
                        selectUserByDepartment.user.TelegramId,

                        selectUserByDepartment.user.LastTelegramVisit,
                        selectUserByDepartment.user.LastWebVisit,

                        selectUserByDepartment.user.isReadonly,
                        selectUserByDepartment.user.isDisabled,
                        selectUserByDepartment.user.isGlobalFavorite,
                        IsUserFavorite = selectUserByDepartment.isFavoriteMark != null,

                        Avatar = new
                        {
                            selectUserByDepartment.user.Avatar?.Id,
                            selectUserByDepartment.user.Avatar?.Name
                        }
                    })
                }
            });
        }

        // POST: api/Departments
        [HttpPost]
        public async Task<ActionResult<object>> PostDepartment(DepartmentObjectModel ajaxDepartment)
        {
            _logger.LogInformation("Добавление департамента. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxDepartment.Name = ajaxDepartment.Name.Trim();
            ajaxDepartment.Information = ajaxDepartment.Information.Trim();

            if (!ModelState.IsValid || string.IsNullOrEmpty(ajaxDepartment.Name))
            {
                msg = "Ошибка запроса. Департамент не создан";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (await _context.Departments.AnyAsync(dprt => dprt.Name.ToLower() == ajaxDepartment.Name.ToLower()))
            {
                msg = "Имя не уникальное. Придумайте другое";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (_user.Role != AccessLevelUserRolesEnum.ROOT)
            {
                ajaxDepartment.isDisabled = false;
                ajaxDepartment.isGlobalFavorite = false;
                ajaxDepartment.isReadonly = false;
            }

            _context.Departments.Add(ajaxDepartment);
            await _context.SaveChangesAsync();
            msg = $"Департамент создан: id={ajaxDepartment.Id}";
            _logger.LogInformation(msg);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.success.ToString(),
                Tag = ajaxDepartment.Id
            });
        }

        // PUT: api/Departments/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartment(int id, DepartmentObjectModel ajaxDepartment)
        {
            _logger.LogInformation($"Изменение департамента [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            ajaxDepartment.Name = ajaxDepartment.Name.Trim();
            ajaxDepartment.Information = ajaxDepartment.Information.Trim();

            if (!ModelState.IsValid
                || string.IsNullOrEmpty(ajaxDepartment.Name)
                || id != ajaxDepartment.Id
                || id < 1
                || !await _context.Departments.AnyAsync(dprt => dprt.Id == id)
                || await _context.Departments.AnyAsync(dprt => dprt.Name.ToLower() == ajaxDepartment.Name.ToLower() && dprt.Id != id))
            {
                msg = "Изменение депаратмента невозможно. Ошибка запроса";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            DepartmentObjectModel departmentDb = await _context.Departments.FirstOrDefaultAsync(dprt => dprt.Id == id);
            if (departmentDb.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
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

            departmentDb.Name = ajaxDepartment.Name;
            departmentDb.Information = ajaxDepartment.Information;
            departmentDb.AvatarId = ajaxDepartment.AvatarId;

            if (_user.Role == AccessLevelUserRolesEnum.ROOT)
            {
                departmentDb.isDisabled = ajaxDepartment.isDisabled;
                departmentDb.isGlobalFavorite = ajaxDepartment.isGlobalFavorite;
                departmentDb.isReadonly = ajaxDepartment.isReadonly;
            }

            _context.Departments.Update(departmentDb);

            try
            {
                await _context.SaveChangesAsync();
                msg = $"Изменения департамента [#{departmentDb.Id}] сохранены";
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
                msg = $"Во время сохранения изменений в департаменте [#{departmentDb.Id}] произошла непредвиденая ошибка: {ex.Message}{(ex.InnerException is null ? "" : ". InnerException: " + ex.InnerException.Message)}";
                _logger.LogWarning(ex, msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteDepartment(int id)
        {
            _logger.LogInformation($"Удаление департамента [#{id}]. Инициатор: " + _user.FullInfo);
            string msg;

            DepartmentObjectModel department = await _context.Departments.FindAsync(id);
            if (department is null)
            {
                msg = $"Удаление депаратмента невозможно. Объект [#{id}] не найден в бд";
                _logger.LogError(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            if (department.isReadonly && _user.Role != AccessLevelUserRolesEnum.ROOT)
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

            if (await _context.Users.AnyAsync(x => x.DepartmentId == id))
            {
                msg = $"Удаление депаратмента невозможно. В нём числятся сотрудники. Перед удалением перенесесите или удалите сотрудников";
                _logger.LogWarning(msg);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = msg,
                    Status = StylesMessageEnum.warning.ToString()
                });
            }

            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();

            msg = $"Департамент [#{id}] удалён";
            _logger.LogInformation(msg);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = msg,
                Status = StylesMessageEnum.success.ToString()
            });
        }
    }
}
