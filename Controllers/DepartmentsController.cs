////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

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

        public DepartmentsController(AppDataBaseContext context, ILogger<DepartmentsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetDepartments([FromQuery] PaginationParameters pagingParameters)
        {
            pagingParameters.Init(_context.Departments.Count());
            IQueryable<DepartmentModel> departments = _context.Departments.OrderBy(x => x.Id);
            if (pagingParameters.PageNum > 1)
                departments = departments.Skip(pagingParameters.Skip);

            HttpContext.Response.Cookies.Append("rowsCount", pagingParameters.CountAllElements.ToString());
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос дапартаментов обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = await departments.Take(pagingParameters.PageSize).ToListAsync()
            });
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetDepartmentModel(int id)
        {
            var departmentModel = await _context.Departments.FindAsync(id);

            if (departmentModel == null)
            {
                _logger.LogError("Запрашиваемый департамент не найден: id={0}", id);
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Запрашиваемый департамент не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            List<UserModel> usersByDepartment = await _context.Users.Where(x => x.DepartmentId == id).ToListAsync();
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос успешно обработан. Департамент найден.",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { departmentModel.Id, departmentModel.Name, departmentModel.isDisabled, Users = usersByDepartment.Select(x => new { x.Id, x.Name, x.isDisabled }).ToList() }
            });
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartmentModel(int id, DepartmentModel departmentModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Изменение депаратмента невозможно. Ошибка контроля валидации модели");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка валидации модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            if (id != departmentModel.Id)
            {
                _logger.LogError("Изменение депаратмента отклонено. Ошибка в запросе");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка в запросе: id != departmentModel.Id",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            DepartmentModel departmentRow = _context.Departments.FirstOrDefault(x => x.Id == id);
            if (departmentRow.Readonly)
            {
                _logger.LogError("Системный объект запрещено редактировать. Подобные объекты редактируются на уровне sqlcmd");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка доступа к системному объекту (read only). Подобные объекты редактируются на уровне sqlcmd",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Entry(departmentModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DepartmentModelExists(id))
                {
                    _logger.LogError("Департамент не найден: id={0}", id);
                    return new ObjectResult(new ServerActionResult()
                    {
                        Success = false,
                        Info = "Департамент не найден",
                        Status = StylesMessageEnum.danger.ToString()
                    });
                }
                else
                {
                    _logger.LogWarning("Невнятная ошибка с департаментом id:{0}", id);
                    throw;
                }
            }

            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Изменения сохранены",
                Status = StylesMessageEnum.success.ToString(),
                Tag = departmentModel
            });
        }

        // POST: api/Departments
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<DepartmentModel>> PostDepartmentModel(DepartmentModel departmentModel)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Создание депаратмента невозможно. Ошибка контроля валидации модели");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка валидации модели",
                    Status = StylesMessageEnum.danger.ToString(),
                    Tag = ModelState
                });
            }

            _context.Departments.Add(departmentModel);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Департамент создан: id={0}", departmentModel.Id);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Департамент успешно создан: id=" + departmentModel.Id,
                Status = StylesMessageEnum.success.ToString(),
                Tag = departmentModel
            });
        }

        // PATCH: api/Departments/5
        [HttpPatch("{id}")]
        public async Task<ActionResult> PatchDepartmentModel(int id)
        {
            DepartmentModel departmentModel = await _context.Departments.FindAsync(id);
            if (departmentModel is null)
            {
                _logger.LogError("Манипуляция депаратментом невозможна. Объект не найден");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Департамент не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            departmentModel.isDisabled = !departmentModel.isDisabled;
            _context.Departments.Update(departmentModel);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Департамент {0}: id={1}", (departmentModel.isDisabled ? "Выключен" : "Включён"), id);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Объект " + (departmentModel.isDisabled ? "Выключен" : "Включён"),
                Status = departmentModel.isDisabled ? StylesMessageEnum.secondary.ToString() : StylesMessageEnum.success.ToString(),
                Tag = departmentModel.isDisabled
            });
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DepartmentModel>> DeleteDepartmentModel(int id)
        {
            DepartmentModel departmentModel = await _context.Departments.FindAsync(id);
            if (departmentModel is null)
            {
                _logger.LogError("Удаление депаратмента невозможно. Объект не найден");
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Департамент не найден",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }

            _context.Departments.Remove(departmentModel);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Департамент удалён: id={0}", id);
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Департамент удалён",
                Status = StylesMessageEnum.success.ToString()
            });
        }

        private bool DepartmentModelExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
