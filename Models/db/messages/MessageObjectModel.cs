////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    public class MessageObjectModel : ObjEntityModel
    {
        /// <summary>
        /// Диалог, в рамках которого пришло уведомление
        /// </summary>
        public int ConversationId { get; set; }
        public СonversationDocumentModel Conversation { get; set; }

        public int SenderId { get; set; }
    }
}
