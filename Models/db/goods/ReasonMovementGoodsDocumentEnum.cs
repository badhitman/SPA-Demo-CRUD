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
        /// ручная корректировка пользователем
        /// </summary>
        Manual,

        /// <summary>
        /// Внутреннее перемещение между складами
        /// </summary>
        InternalMovement,

        /// <summary>
        /// Перемещено в обеспечение наличия (выкладка/витрина)
        /// </summary>
        Availability
    }
}
