////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Пользователь/сотрудник
    /// </summary>
    public class UserModel : ObjAvatarModel
    {
        public int DepartmentId { get; set; }
        public DepartmentModel Department { get; set; }

        public AccessLevelUserRolesEnum Role { get; set; } = AccessLevelUserRolesEnum.Guest;

        public long TelegramId { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public DateTime LastWebVisit { get; set; }

        public DateTime LastTelegramVisit { get; set; }

        public List<NotificationModel> Notifications { get; set; }
    }
}
