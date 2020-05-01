////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Документ внутреннего складского перемещения номенклатуры
    /// </summary>
    public class InternalDisplacementWarehouseDocumentModel : WarehouseDocumentsModel
    {
        /// <summary>
        /// Корреспондирующий Склад (списания/обеспечения)
        /// </summary>
        public int WarehouseDebitingId { get; set; }
    }
}
