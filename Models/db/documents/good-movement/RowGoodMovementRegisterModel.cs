////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models
{
    public class RowGoodMovementRegisterModel : EFModel
    {
        public int BodyDocumentId { get; set; }
        public BodyGoodMovementDocumentModel BodyDocument { get; set; }

        //public MovementGoodsWarehousesDocumentModel MovementGoodsWarehousesDocument { get; set; }

        public int GoodId { get; set; }
        /// <summary>
        /// Номенклатура движения
        /// </summary>
        public GoodObjectModel Good { get; set; }

        /// <summary>
        /// Единица измерения движения
        /// </summary>
        public int UnitId { get; set; }

        /// <summary>
        /// Номинал количества движения
        /// </summary>
        public double Quantity { get; set; }

        /// <summary>
        /// Цена движения
        /// </summary>
        public double Price { get; set; }
    }
}
