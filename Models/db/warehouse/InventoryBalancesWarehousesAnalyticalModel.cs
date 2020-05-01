////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Остатки номенклатуры на складах
    /// </summary>
    public class InventoryBalancesWarehousesAnalyticalModel : aInventoryGoodBalancesAnalyticalModel
    {
        public int WarehouseId { get; set; }
        /// <summary>
        /// Склад
        /// </summary>
        public WarehouseObjectModel Warehouse { get; set; }
    }
}
