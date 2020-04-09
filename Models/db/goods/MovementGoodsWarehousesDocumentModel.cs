////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Документ внутреннего складского движения номенклатуры
    /// </summary>
    public class MovementGoodsWarehousesDocumentModel : ObjectFileRegisterModel
    {
        /// <summary>
        /// Контекстный ключ. В паре с основанием движения позволяет получить связанный объект базы данных
        /// </summary>
        public int ContextKey { get; set; }
        /// <summary>
        /// Основание движения документа. В паре с контекстным ключём даёт ссылку на объект БД как основание документа
        /// </summary>
        public ReasonMovementGoodsDocumentEnum ReasonMovementDocument { get; set; }

        public int WarehouseId { get; set; }
        /// <summary>
        /// Склад движения
        /// </summary>
        public WarehouseGoodModel Warehouse { get; set; }

        public int GoodId { get; set; }
        /// <summary>
        /// Номенклатура движения
        /// </summary>
        public GoodModel Good { get; set; }

        /// <summary>
        /// Номинал количества движения
        /// </summary>
        public double QuantityGoods { get; set; }
    }
}
