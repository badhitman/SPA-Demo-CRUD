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
        public int WarehouseReceiptId { get; set; }
        /// <summary>
        /// Склад поступления
        /// </summary>
        public WarehouseObjectModel WarehouseReceipt { get; set; }
    }
}
