////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    public class GroupGoodModel : AvatarObjEntityModel
    {
        public List<GoodModel> Goods { get; set; }
    }
}
