////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Пользователь/сотрудник
    /// </summary>
    public class UserModel : LiteEntityModel
    {
        public int DepartmentId { get; set; }
        public DepartmentModel Department { get; set; }

        public AccessLevelUserRolesEnum Role { get; set; } = AccessLevelUserRolesEnum.Guest;

        public long TelegramId { get; set; }

        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
