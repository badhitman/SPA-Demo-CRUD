using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
{
    /// <summary>
    /// Номенклатура
    /// </summary>
    public class GoodModel : ObjAvatarModel
    {
        public int GroupId { get; set; }
        /// <summary>
        /// Группа номенклатуры
        /// </summary>
        public GroupGoodModel Group { get; set; }

        public int UnutId { get; set; }
        /// <summary>
        /// Единица измерения
        /// </summary>
        public UnitGoodModel Unut { get; set; }

        /// <summary>
        /// Сводка/срез остатков номенклатуры в разрезе складов
        /// </summary>
        public List<InventoryGoodBalancesWarehousesModel> BalancesWarehouses { get; set; }

        /// <summary>
        /// Документы движения номенклатуры в контексте складов
        /// </summary>
        public List<MovementGoodsWarehousesDocumentModel> MovementsGoodsWarehousesDocuments { get; set; }

        public double Price { get; set; }
    }
}
