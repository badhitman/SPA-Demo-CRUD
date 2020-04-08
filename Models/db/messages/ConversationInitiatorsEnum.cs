////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Типы иницииаторов контекста переписок
    /// </summary>
    public enum ConversationInitiatorsEnum
    {
        /// <summary>
        /// Переписка вне контекста (системная)
        /// </summary>
        System,

        /// <summary>
        /// Переписка между пользователями
        /// </summary>
        User,

        /// <summary>
        /// Переписка в контексте заказов
        /// </summary>
        Order,

        /// <summary>
        /// Переписка в контексте платежей
        /// </summary>
        Payments
    }
}
