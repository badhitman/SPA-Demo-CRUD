////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
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
