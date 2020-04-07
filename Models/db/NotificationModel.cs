﻿using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models.db
{
    /// <summary>
    /// Уведомление пользователя
    /// </summary>
    public class NotificationModel : LiteEntityModel
    {
        /// <summary>
        /// Статус доставки уведомления (в контексте получателя)
        /// </summary>
        public DeliveryStatusesEnum DeliveryStatus { get; set; }

        /// <summary>
        /// Диалог, в рамках которого пришло уведомление
        /// </summary>
        public int ConversationId { get; set; }
        public СonversationModel Conversation { get; set; }

        public int RecipientId { get; set; }
        public UserModel Recipient { get; set; }
    }
}
