////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Основание движения номенклатуры. Служит для определения типа данных для контекстного ключа документа
    /// </summary>
    public enum ReasonMovementGoodsDocumentEnum
    {
        /// <summary>
        /// ручная корректировка пользователем => связаный объект: UserModel
        /// </summary>
        Manual,

        /// <summary>
        /// Внутреннее перемещение между складами => связаный документ: MovementGoodsWarehousesDocumentModel
        /// </summary>
        InternalMovement,

        /// <summary>
        /// Отгружено в оборот (продано/отправлено) => связаный объект: MovementTurnoverDeliveryDocumentRowModel (строка, положительная по количеству, документа: MovementTurnoverDeliveryDocumentModel)
        /// </summary>
        Store,

        /// <summary>
        /// Возврат из службы доставки или пункта выдачи на склад => связаный объект: MovementTurnoverDeliveryDocumentRowModel (строка, отрицательная по количеству, документа: MovementTurnoverDeliveryDocumentModel)
        /// </summary>
        Returning
    }
}
