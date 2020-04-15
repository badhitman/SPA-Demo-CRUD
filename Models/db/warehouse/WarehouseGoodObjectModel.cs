////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Склад номенклатуры (учёт/остатки)
    /// </summary>
    public class WarehouseGoodObjectModel : AvatarObjEntityModel
    {
        public List<InventoryGoodBalancesWarehousesAnalyticalModel> BalancesGoods { get; set; }
    }
}
