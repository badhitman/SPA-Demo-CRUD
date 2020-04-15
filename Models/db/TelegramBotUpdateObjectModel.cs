////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    public class TelegramBotUpdateObjectModel : BirthdayObjEntityModel
    {
        public int UserId { get; set; }
        public UserObjectModel User { get; set; }

        /// <summary>
        /// Признак того что сообщение от бота к пользователю
        /// </summary>
        public bool isBotMessage { get; set; }
    }
}
