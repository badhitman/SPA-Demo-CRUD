using System.Linq;

namespace SimpleSPA.Models
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
                    new UserModel { Name = "Иванова И", DepartmentId = 1 },
                    new UserModel { Name = "Петрова П", DepartmentId = 1 },
                    new UserModel { Name = "Сидоров С", DepartmentId = 1 },

                    new UserModel { Name = "Ромашкин Р", DepartmentId = 2 },
                    new UserModel { Name = "Лапухин Л", DepartmentId = 2 },

                    new UserModel { Name = "Абрамович А", DepartmentId = 3 });
                context.SaveChanges();
            }

        }
    }
}
