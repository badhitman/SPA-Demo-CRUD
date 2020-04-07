using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;

namespace SPADemoCRUD.Models.AuthorizePolicies
{
    public class SessionUser
    {
        public readonly UserModel user;

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
                user = dbContext.Users.FirstOrDefault(x => x.Id == userId);
            }
            if (!(user is null))
            {
                user.LastWebVisit = DateTime.Now;
                dbContext.Users.Update(user);
                dbContext.SaveChanges();
            }
        }
    }
}
