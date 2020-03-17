////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    public class RoleModel : LiteEntityModel
    {
        public RoleModel()
        {
            Users = new List<UserModel>();
        }

        public List<UserModel> Users { get; set; }
    }
}
