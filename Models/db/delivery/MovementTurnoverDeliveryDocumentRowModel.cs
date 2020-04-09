using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Строка документа
    /// </summary>
    public class MovementTurnoverDeliveryDocumentRowModel : LiteObjEntityModel
    {
        public int MovementTurnoverDeliveryDocumentId { get; set; }
        public MovementTurnoverDeliveryDocumentModel MovementTurnoverDeliveryDocument { get; set; }

        public int WarehouseId { get; set; }
        /// <summary>
        /// Склад списания
        /// </summary>
        public WarehouseGoodModel Warehouse { get; set; }

        public int GoodId { get; set; }
        /// <summary>
        /// Номенклатура движения
        /// </summary>
        public GoodModel Good { get; set; }

        /// <summary>
        /// Номинал количества движения.
        /// Если положительный, то со склада списывается на баланс пункта выдачи или службу доставки.
        /// Если отрицательный, то движение номенклатуры в обратную сторону: на склад из пункта выдачи
        /// </summary>
        public double QuantityGoods { get; set; }
    }
}
