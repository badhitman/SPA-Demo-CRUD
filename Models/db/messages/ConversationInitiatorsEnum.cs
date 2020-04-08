using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
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
