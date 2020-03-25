////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPADemoCRUD.Models;
using SPADemoCRUD.Models.view;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelAdmin")]
    public class DepartmentsController : ControllerBase
    {
        private readonly AppDataBaseContext _context;

        public DepartmentsController(AppDataBaseContext context)
        {
            _context = context;
        }

        // GET: api/Departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentModel>>> GetDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        // GET: api/Departments/5
        [HttpGet("{id}")]
        public async Task<ActionResult<object>> GetDepartmentModel(int id)
        {
            var departmentModel = await _context.Departments.FindAsync(id);

            if (departmentModel == null)
            {
                return NotFound();
            }

            List<UserModel> usersByDepartment = await _context.Users.Where(x => x.DepartmentId == id).ToListAsync();

            return new { departmentModel.Id, departmentModel.Name, Users = usersByDepartment.Select(x => new { x.Id, x.Name, x.isDisabled }).ToList() };
        }

        // PUT: api/Departments/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDepartmentModel(int id, DepartmentModel departmentModel)
        {
            if (!ModelState.IsValid)
            {
                return new ObjectResult(ModelState);
            }

            if (id != departmentModel.Id)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
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
                return new ObjectResult(ModelState);
            }

            _context.Departments.Add(departmentModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetDepartmentModel), new { id = departmentModel.Id }, departmentModel); ;
        }

        // DELETE: api/Departments/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<DepartmentModel>> DeleteDepartmentModel(int id)
        {
            var departmentModel = await _context.Departments.FindAsync(id);
            if (departmentModel == null)
            {
                return NotFound();
            }

            _context.Departments.Remove(departmentModel);
            await _context.SaveChangesAsync();

            return departmentModel;
        }

        private bool DepartmentModelExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
