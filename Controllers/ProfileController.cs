////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly SessionUser _sessionUser;

        public ProfileController(AppDataBaseContext context, SessionUser sessionUser)
        {
            _context = context;
            _sessionUser = sessionUser;
        }

        // GET: api/Profile
        [HttpGet]
        public ActionResult<object> GetProfile()
        {
            if (_sessionUser.user is null)
            {
                return new ObjectResult(new ServerActionResult()
                {
                    Success = false,
                    Info = "Ошибка обработки запроса профиля. Пользователь текущей сессии не определён",
                    Status = StylesMessageEnum.danger.ToString()
                });
            }
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос профиля обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new { _sessionUser.user.DateCreate, Department = _sessionUser.user.Department.Name, _sessionUser.user.Email, _sessionUser.user.Id, _sessionUser.user.isDisabled, _sessionUser.user.Name, _sessionUser.user.Readonly, Role = _sessionUser.user.Role.ToString() }
            });
        }

        // GET: api/Profile/5
        [HttpGet("{id}")]
        public async Task<ActionResult<СonversationModel>> GetProfileModel(int id)
        {
            var сonversationModel = await _context.Сonversations.FindAsync(id);

            if (сonversationModel == null)
            {
                return null; // NotFound();
            }

            return null; // сonversationModel;
        }

        // PUT: api/Profile/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfileModel(int id, СonversationModel сonversationModel)
        {
            if (id != сonversationModel.Id)
            {
                return null; // BadRequest();
            }

            _context.Entry(сonversationModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!СonversationModelExists(id))
                {
                    return null; // NotFound();
                }
                else
                {
                    throw;
                }
            }

            return null; // NoContent();
        }

        // POST: api/Profile
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<СonversationModel>> PostProfileModel(СonversationModel сonversationModel)
        {
            _context.Сonversations.Add(сonversationModel);
            await _context.SaveChangesAsync();

            return null; // CreatedAtAction("GetСonversationModel", new { id = сonversationModel.Id }, сonversationModel);
        }

        // DELETE: api/Profile/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<СonversationModel>> DeleteProfileModel(int id)
        {
            var сonversationModel = await _context.Сonversations.FindAsync(id);
            if (сonversationModel == null)
            {
                return null; // NotFound();
            }

            _context.Сonversations.Remove(сonversationModel);
            await _context.SaveChangesAsync();

            return null; // сonversationModel;
        }

        private bool СonversationModelExists(int id)
        {
            return _context.Сonversations.Any(e => e.Id == id);
        }
    }
}
