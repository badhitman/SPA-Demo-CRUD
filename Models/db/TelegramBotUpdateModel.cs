using SPADemoCRUD.Models.db.sys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
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
