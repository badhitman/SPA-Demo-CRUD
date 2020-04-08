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
                    new DepartmentModel { Information = "user", Readonly = true },
                    new DepartmentModel { Information = "Хозяйственный блок" },
                    new DepartmentModel { Information = "Снабженцы" },
                    new DepartmentModel { Information = "Секретариат" },
                    new DepartmentModel { Information = "Производство" },
                    new DepartmentModel { Information = "Бухгалтерия" },
                    new DepartmentModel { Information = "ИТ" });
                context.SaveChanges();
            }

            if (!context.Users.Any())
            {
                string demoPass = glob_tools.GetHashString("demo");
                context.Users.AddRange(
                    new UserModel { Information = "system", DepartmentId = 1, Role = AccessLevelUserRolesEnum.Auth, Email = "*", Password = "*", LastWebVisit = DateTime.MaxValue, Readonly = true, IsGlobalFavorite = true, Name = "Системная учётная запись" },

                    new UserModel { Information = "Иванова И", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "verified", Password = demoPass },
                    new UserModel { Information = "Петрова П", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo1", Password = demoPass },
                    new UserModel { Information = "Сидоров С", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo2", Password = demoPass },
                    new UserModel { Information = "Ромашкин Р", DepartmentId = 2, Role = AccessLevelUserRolesEnum.Verified, Email = "demo3", Password = demoPass },

                    new UserModel { Information = "Кузнецов К", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "privileged", Password = demoPass },
                    new UserModel { Information = "Зайцев З", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo4", Password = demoPass },
                    new UserModel { Information = "Новиков Н", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo5", Password = demoPass },
                    new UserModel { Information = "Морозов М", DepartmentId = 4, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo6", Password = demoPass },

                    new UserModel { Information = "Волков В", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Privileged, Email = "manager", Password = demoPass },
                    new UserModel { Information = "Голубев Г", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Manager, Email = "demo7", Password = demoPass },
                    new UserModel { Information = "Богданов Б", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Privileged, Email = "demo8", Password = demoPass },
                    new UserModel { Information = "Фёдоров Ф", DepartmentId = 5, Role = AccessLevelUserRolesEnum.Manager, Email = "demo9", Password = demoPass },

                    new UserModel { Information = "Тарасов Т", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "demo10", Password = demoPass },
                    new UserModel { Information = "Егоров Е", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "demo11", Password = demoPass },
                    new UserModel { Information = "Орлов О", DepartmentId = 6, Role = AccessLevelUserRolesEnum.Admin, Email = "admin", Password = demoPass },

                    new UserModel { Information = "Абрамович А", DepartmentId = 7, Role = AccessLevelUserRolesEnum.ROOT, Email = "root", Password = demoPass });
                context.SaveChanges();
            }
        }
    }
}
