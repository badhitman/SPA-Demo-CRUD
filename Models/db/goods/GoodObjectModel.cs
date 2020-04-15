////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Номенклатура
    /// </summary>
    public class GoodObjectModel : AvatarObjEntityModel
    {
        public int GroupId { get; set; }
        /// <summary>
        /// Группа номенклатуры
        /// </summary>
        public GroupGoodsObjectModel Group { get; set; }

        public int UnitId { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        public UnitGoodObjectModel Unit { get; set; }

        /// <summary>
        /// Сводка/срез остатков номенклатуры в разрезе складов
        /// </summary>
        public List<InventoryGoodBalancesWarehousesAnalyticalModel> BalancesWarehouses { get; set; }

        /// <summary>
        /// Документы движения номенклатуры в контексте складов
        /// </summary>
        public List<ReceiptToWarehouseDocumentModel> ReceiptesGoodsToWarehousesDocuments { get; set; }

        public double Price { get; set; }
    }
}
