////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    public class GroupGoodsObjectModel : AvatarObjEntityModel
    {
        public List<GoodObjectModel> Goods { get; set; }
    }
}
