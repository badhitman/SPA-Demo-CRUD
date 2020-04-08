using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
{
    public class GroupGoodModel : ObjAvatarModel
    {
        public List<GoodModel> Goods { get; set; }
    }
}
