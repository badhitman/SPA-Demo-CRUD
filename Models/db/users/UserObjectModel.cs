////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Пользователь/сотрудник
    /// </summary>
    public class UserObjectModel : AvatarObjEntityModel
    {
        public int DepartmentId { get; set; }
        public DepartmentObjectModel Department { get; set; }

        public AccessLevelUserRolesEnum Role { get; set; } = AccessLevelUserRolesEnum.Guest;

        public long TelegramId { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }

        public DateTime LastWebVisit { get; set; }

        public DateTime LastTelegramVisit { get; set; }

        public List<NotificationObjectModel> Notifications { get; set; }

        [NotMapped]
        public new string FullInfo => $"{base.FullInfo} [department id: {DepartmentId}]•[role: {Role}]{(TelegramId != 0 ? "" : $"•[telegram id:{TelegramId}]")}{(Notifications == null || Notifications.Count == 0 ? "" : $"•[notifications: {Notifications.Count}]")}";
    }
}
