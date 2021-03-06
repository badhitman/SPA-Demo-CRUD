////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using Microsoft.AspNetCore.Authorization;

namespace SPADemoCRUD.Models
{
    public class AccessMinLevelRequirement : IAuthorizationRequirement
    {
        protected internal AccessLevelUserRolesEnum Role { get; set; }

        public AccessMinLevelRequirement(AccessLevelUserRolesEnum role)
        {
            Role = role;
        }
    }
}
