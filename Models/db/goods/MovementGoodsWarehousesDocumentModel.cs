using SPADemoCRUD.Models.db.sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
{
    /// <summary>
    /// Документ движения номенклатуры по складу
    /// </summary>
    public class MovementGoodsWarehousesDocumentModel : BirthdayEntityModel
    {
        public int UserId { get; set; }
        /// <summary>
        /// Автор документа
        /// </summary>
        public UserModel User { get; set; }

        public int WarehouseId { get; set; }
        public WarehouseGoodModel Warehouse { get; set; }

        public int GoodId { get; set; }
        public GoodModel Good { get; set; }

        /// <summary>
        /// Номинал количества движения
        /// </summary>
        public double QuantityGoods { get; set; }
    }
}
