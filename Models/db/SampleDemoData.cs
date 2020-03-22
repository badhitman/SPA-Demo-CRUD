////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////
using System.Linq;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Демо данные (seeds)
    /// </summary>
    public static class SampleDemoData
    {
        public static void Initialize(AppDataBaseContext context)
        {
            if (!context.Departments.Any())
            {
                context.Departments.AddRange(
                    new DepartmentModel { Name = "Бухгалтерия" },
                    new DepartmentModel { Name = "Производство" },
                    new DepartmentModel { Name = "Юристы" });
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new UserModel { Name = "Иванова И", DepartmentId = 1, Role = AccessLevelUserRolesEnum.Admin, Email = "demo1@xxxxxxx.yy", Password = "demo" },
                    new UserModel { Name = "Петрова П", DepartmentId = 1, Role = AccessLevelUserRolesEnum.Manager, Email = "demo2@xxxxxxx.yy", Password = "demo" },
                    new UserModel { Name = "Сидоров С", DepartmentId = 1, Role = AccessLevelUserRolesEnum.Auth, Email = "demo3@xxxxxxx.yy", Password = "demo" },

                    new UserModel { Name = "Ромашкин Р", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo4@xxxxxxx.yy", Password = "demo" },
                    new UserModel { Name = "Лапухин Л", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo5@xxxxxxx.yy", Password = "demo" },

                    new UserModel { Name = "Абрамович А", DepartmentId = 3, Role = AccessLevelUserRolesEnum.ROOT, Email = "demo6@xxxxxxx.yy", Password = "demo" });
                context.SaveChanges();
            }

        }
    }
}
