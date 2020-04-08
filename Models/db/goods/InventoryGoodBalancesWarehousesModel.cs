using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
{
    /// <summary>
    /// Остатки товара в разрезе складов
    /// </summary>
    public class InventoryGoodBalancesWarehousesModel : LiteEntityModel
    {
        public int WarehouseId { get; set; }
        public WarehouseGoodModel Warehouse { get; set; }

        public int GoodId { get; set; }
        public GoodModel Good { get; set; }

        public double RemnantsOfTheGoods { get; set; }
    }
}
