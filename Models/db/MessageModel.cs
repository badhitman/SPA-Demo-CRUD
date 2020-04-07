using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
{
    public class MessageModel : ObjEntityModel
    {
        /// <summary>
        /// Диалог, в рамках которого пришло уведомление
        /// </summary>
        public int ConversationId { get; set; }
        public СonversationModel Conversation { get; set; }

        public int SenderId { get; set; }
    }
}
