using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
{
    /// <summary>
    /// Склад номенклатуры (учёт)
    /// </summary>
    public class WarehouseGoodModel : ObjAvatarModel
    {
        public List<InventoryGoodBalancesWarehousesModel> BalancesGoods { get; set; }
    }
}
