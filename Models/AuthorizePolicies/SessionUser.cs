////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MultiTool;
using System;
using System.Linq;
using System.Security.Claims;

namespace SPADemoCRUD.Models
{
    public class SessionUser
    {
        public readonly UserObjectModel user;

        public SessionUser(AppDataBaseContext dbContext, ILogger<SessionUser> logger, IHttpContextAccessor httpContextccessor)
        {
            if (!httpContextccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                return;
            }
            ClaimsPrincipal currentUser = httpContextccessor.HttpContext.User;
            int userId = int.Parse("0" + httpContextccessor.HttpContext.User.FindFirst(c => c.Type == "id").Value);
            if (userId > 0)
            {
                user = dbContext.Users.Include(x => x.Department).FirstOrDefault(x => x.Id == userId);
            }
            if (!(user is null))
            {
                user.LastWebVisit = DateTime.Now;
                dbContext.Entry(user).Property(x => x.LastWebVisit).IsModified = true;
                //dbContext.Users.Update(user);
                try
                {
                    dbContext.SaveChanges();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    logger.LogError($"{DateTime.Now.ToString(glob_tools.DateTimeFormat)}: Не удалось обновить вермя последней web активности пользователя. DbUpdateConcurrencyException: {ex.Message}\nrequest: {httpContextccessor.HttpContext.Request.Path.Value}");
                }
            }
        }
    }
}
