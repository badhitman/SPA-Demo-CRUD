////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    public class СonversationDocumentModel : ObjEntityModel
    {
        /// <summary>
        /// Тип инициатора (контекст) переписки
        /// </summary>
        public ConversationInitiatorsEnum InitiatorType { get; set; }
        public int InitiatorId { get; set; }

        /// <summary>
        /// Разосланные уведомления в контексте данной переписки
        /// </summary>
        public List<NotificationObjectModel> Notifications { get; set; }

        /// <summary>
        /// Сообщения в контексте беседы
        /// </summary>
        public List<MessageObjectModel> Messages { get; set; }
    }
}
