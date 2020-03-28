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
                    new DepartmentModel { Name = "user", Readonly = true },
                    new DepartmentModel { Name = "Хозяйственный блок" },
                    new DepartmentModel { Name = "Снабженцы" },
                    new DepartmentModel { Name = "Секретариат" },
                    new DepartmentModel { Name = "Производство" },
                    new DepartmentModel { Name = "Бухгалтерия" },
                    new DepartmentModel { Name = "ИТ" });
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new UserModel { Name = "Лапухин Л", DepartmentId = 1, Role = AccessLevelUserRolesEnum.Auth, Email = "auth", Password = "demo" },

                    new UserModel { Name = "Иванова И", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "verified", Password = "demo" },
                    new UserModel { Name = "Петрова П", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo1", Password = "demo" },
                    new UserModel { Name = "Сидоров С", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo2", Password = "demo" },
                    new UserModel { Name = "Ромашкин Р", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo3", Password = "demo" },

                    new UserModel { Name = "Кузнецов К", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "privileged", Password = "demo" },
                    new UserModel { Name = "Зайцев З", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo4", Password = "demo" },
                    new UserModel { Name = "Новиков Н", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo5", Password = "demo" },
                    new UserModel { Name = "Морозов М", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo6", Password = "demo" },

                    new UserModel { Name = "Волков В", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Privileged, Email = "manager", Password = "demo" },
                    new UserModel { Name = "Голубев Г", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Manager, Email = "demo7", Password = "demo" },
                    new UserModel { Name = "Богданов Б", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo8", Password = "demo" },
                    new UserModel { Name = "Фёдоров Ф", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Manager, Email = "demo9", Password = "demo" },

                    new UserModel { Name = "Тарасов Т", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "demo10", Password = "demo" },
                    new UserModel { Name = "Егоров Е", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "demo11", Password = "demo" },
                    new UserModel { Name = "Орлов О", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "admin", Password = "demo" },

                    new UserModel { Name = "Абрамович А", DepartmentId = 7, Role = AccessLevelUserRolesEnum.ROOT, Email = "root", Password = "demo" });
                context.SaveChanges();
            }
        }
    }
}
