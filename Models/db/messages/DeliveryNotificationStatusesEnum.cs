////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Статусы доставки уведомления
    /// </summary>
    public enum DeliveryNotificationStatusesEnum
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
