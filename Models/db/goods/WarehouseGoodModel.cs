////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Склад номенклатуры (учёт)
    /// </summary>
    public class WarehouseGoodModel : AvatarObjEntityModel
    {
        public List<InventoryGoodBalancesWarehousesModel> BalancesGoods { get; set; }
    }
}
