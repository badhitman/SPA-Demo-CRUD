////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using MultiTool;
using System;
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
                string demoPass = glob_tools.GetHashString("demo");
                context.Users.AddRange(
                    new UserModel { Name = "system", DepartmentId = 1, Role = AccessLevelUserRolesEnum.Auth, Email = "*", Password = "*", LastWebVisit = DateTime.MaxValue, Readonly = true, IsGlobalFavorite = true, Information = "Системная учётная запись" },

                    new UserModel { Name = "Иванова И", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "verified", Password = demoPass },
                    new UserModel { Name = "Петрова П", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo1", Password = demoPass },
                    new UserModel { Name = "Сидоров С", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo2", Password = demoPass },
                    new UserModel { Name = "Ромашкин Р", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo3", Password = demoPass },

                    new UserModel { Name = "Кузнецов К", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "privileged", Password = demoPass },
                    new UserModel { Name = "Зайцев З", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo4", Password = demoPass },
                    new UserModel { Name = "Новиков Н", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo5", Password = demoPass },
                    new UserModel { Name = "Морозов М", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo6", Password = demoPass },

                    new UserModel { Name = "Волков В", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Privileged, Email = "manager", Password = demoPass },
                    new UserModel { Name = "Голубев Г", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Manager, Email = "demo7", Password = demoPass },
                    new UserModel { Name = "Богданов Б", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo8", Password = demoPass },
                    new UserModel { Name = "Фёдоров Ф", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Manager, Email = "demo9", Password = demoPass },

                    new UserModel { Name = "Тарасов Т", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "demo10", Password = demoPass },
                    new UserModel { Name = "Егоров Е", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "demo11", Password = demoPass },
                    new UserModel { Name = "Орлов О", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "admin", Password = demoPass },

                    new UserModel { Name = "Абрамович А", DepartmentId = 7, Role = AccessLevelUserRolesEnum.ROOT, Email = "root", Password = demoPass });
                context.SaveChanges();
            }
        }
    }
}
