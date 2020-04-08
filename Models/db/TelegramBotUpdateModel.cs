////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    public class TelegramBotUpdateModel : BirthdayEntityModel
    {
        public int UserId { get; set; }
        public UserModel User { get; set; }

        /// <summary>
        /// Признак того что сообщение от бота к пользователю
        /// </summary>
        public bool isBotMessage { get; set; }
    }
}
