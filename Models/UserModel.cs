namespace SimpleSPA.Models
{
    /// <summary>
    /// Пользователь/сотрудник
    /// </summary>
    public class UserModel : LiteEntityModel
    {
        public int DepartmentId { get; set; }
        public DepartmentModel Department { get; set; }
    }
}
