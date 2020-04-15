////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Остатки номенклатуры на складах
    /// </summary>
    public class InventoryGoodBalancesWarehousesAnalyticalModel : aInventoryGoodBalancesAnalyticalModel
    {
        public int WarehouseId { get; set; }
        /// <summary>
        /// Склад
        /// </summary>
        public WarehouseGoodObjectModel Warehouse { get; set; }
    }
}
