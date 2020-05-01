////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Склад номенклатуры (учёт/остатки)
    /// </summary>
    public class WarehouseObjectModel : AvatarObjEntityModel
    {
        public List<InventoryBalancesWarehousesAnalyticalModel> BalancesGoods { get; set; }
    }
}
