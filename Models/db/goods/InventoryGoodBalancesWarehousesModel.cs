﻿////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Остатки номенклатуры на складах
    /// </summary>
    public class InventoryGoodBalancesWarehousesModel : ObjectFileRegisterModel
    {
        public int WarehouseId { get; set; }
        /// <summary>
        /// Склад
        /// </summary>
        public WarehouseGoodModel Warehouse { get; set; }

        public int GoodId { get; set; }
        /// <summary>
        /// Номенклатура
        /// </summary>
        public GoodModel Good { get; set; }

        /// <summary>
        /// Остаток (количество)
        /// </summary>
        public double RemnantsOfTheGoods { get; set; }
    }
}
