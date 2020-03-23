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

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetDepartments()
        {
            List<DepartmentModel> departments = await _context.Departments.ToListAsync();

            return new ObjectResult(new { departments, roles });
        }
    }
}
