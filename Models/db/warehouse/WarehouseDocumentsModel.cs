////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// складские документы
    /// </summary>
    public class WarehouseDocumentsModel : BodyGoodMovementDocumentModel
    {
        public int WarehouseId { get; set; }
        /// <summary>
        /// Склад движения
        /// </summary>
        public WarehouseObjectModel Warehouse { get; set; }
    }
}
