////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SPADemoCRUD.Models;

namespace SPADemoCRUD.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AccessMinLevelAuth")]
    public class ProfileController : ControllerBase
    {
        private readonly AppDataBaseContext _context;
        private readonly UserObjectModel _user;

        public ProfileController(AppDataBaseContext context, SessionUser session)
        {
            _context = context;
            _user = session.user;
        }

        // GET: api/Profilies
        [HttpGet]
        public ActionResult<object> GetProfile()
        {
            return new ObjectResult(new ServerActionResult()
            {
                Success = true,
                Info = "Запрос профиля обработан",
                Status = StylesMessageEnum.success.ToString(),
                Tag = new
                {
                    _user.DateCreate,
                    Department = _user.Department.Name,
                    _user.Email,
                    _user.Id,
                    _user.isDisabled,
                    _user.Name,
                    _user.isReadonly,
                    Role = _user.Role.ToString()
                }
            });
        }

        // GET: api/Profile/5
        [HttpGet("{id}")]
        public async Task<ActionResult<СonversationDocumentModel>> GetProfile(int id)
        {
            var сonversation = await _context.Сonversations.FindAsync(id);

            if (сonversation == null)
            {
                return null; // NotFound();
            }

            return null; // сonversationModel;
        }

        // PUT: api/Profile/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(int id, СonversationDocumentModel ajaxConversation)
        {
            if (id != ajaxConversation.Id)
            {
                return null; // BadRequest();
            }

            _context.Entry(ajaxConversation).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw;
            }

            return null; // NoContent();
        }

        // POST: api/Profile
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<object>> PostProfile(СonversationDocumentModel ajaxConversation)
        {
            _context.Сonversations.Add(ajaxConversation);
            await _context.SaveChangesAsync();

            return null; // CreatedAtAction("GetСonversationModel", new { id = сonversationModel.Id }, сonversationModel);
        }

        // DELETE: api/Profile/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<object>> DeleteProfile(int id)
        {
            СonversationDocumentModel сonversation = await _context.Сonversations.FindAsync(id);
            if (сonversation == null)
            {
                return null; // NotFound();
            }

            _context.Сonversations.Remove(сonversation);
            await _context.SaveChangesAsync();

            return null; // сonversationModel;
        }
    }
}
