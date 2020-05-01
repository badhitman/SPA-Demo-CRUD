////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    public class UserFavoriteMarkModel : EFModel
    {
        public int UserId { get; set; }
        public UserObjectModel User { get; set; }

        public string TypeName { get; set; }
        public int ObjectId { get; set; }
    }
}