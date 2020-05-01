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
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    /// <summary>
    /// Получение метаданных пользователей
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelAdmin")]
    public class UsersMetadataController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public static IEnumerable<object> roles { get; } = Enum.GetValues(typeof(AccessLevelUserRolesEnum)).Cast<AccessLevelUserRolesEnum>().Select(x => new { id = x, name = x.ToString() }).ToArray();

        public UsersMetadataController(AppDataBaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Получить общие метаданные пользователей: перечень департаментов и ролей
        /// </summary>
        /// <returns>два именованных набора данных(поля json объекта): [departments] и [roles]</returns>
        // GET: api/UsersMetadata/
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentObjectModel>>> GetDepartments()
        {
            List<DepartmentObjectModel> departments = await _context.Departments.ToListAsync();

            return new ObjectResult(new { departments, roles });
        }
    }
}
