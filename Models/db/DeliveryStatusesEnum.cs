using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
{
    /// <summary>
    /// Статусы доставки уведомления
    /// </summary>
    public enum DeliveryStatusesEnum
    {
        /// <summary>
        /// Отправлено
        /// </summary>
        Sent,
        /// <summary>
        /// Получатель уведомлён (видел уведомление в списке уведомлений)
        /// </summary>
        Notified,
        /// <summary>
        /// Прочитано (открывал беседу)
        /// </summary>
        Read
    }
}
