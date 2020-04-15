////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Документ складского поступления номенклатуры
    /// </summary>
    public class ReceiptToWarehouseDocumentModel : BodyGoodMovementDocumentModel
    {
        public int WarehouseReceiptId { get; set; }
        /// <summary>
        /// Склад поступления
        /// </summary>
        public WarehouseGoodObjectModel WarehouseReceipt { get; set; }
    }
}
