using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Уровень доступа/привелегий пользователя
    /// </summary>
    public enum AccessLevelUserRolesEnum
    {
        /// <summary>
        /// 0.Гость
        /// </summary>
        [Display(Name = "Гость (аноним)", Description = "Анонимный пользователь")]
        Guest,

        /// <summary>
        /// 1.Авторизован
        /// </summary>
        [Display(Name = "Зарегистрированый", Description = "Рядовой зарегистрированный пользователь")]
        Auth,

        /// <summary>
        /// 2.Проверенный
        /// </summary>
        [Display(Name = "Проверенный", Description = "Рядовой пользователь, но чем то выделяется кроме формальной регистрации")]
        Verified,

        /// <summary>
        /// 3.Привилегированный
        /// </summary>
        [Display(Name = "Привилегированный", Description = "Особые разрешения для рядовых пользователей")]
        Privileged,

        /// <summary>
        /// 4.Менеджер (управляющий/модератор)
        /// </summary>
        [Display(Name = "Менеджер/Модератор", Description = "Младший администратор")]
        Manager,

        /// <summary>
        /// 5.Администратор
        /// </summary>
        [Display(Name = "Администратор", Description = "Старший администратор")]
        Admin,

        /// <summary>
        /// 6.Владелец (суперпользователь)
        /// </summary>
        [Display(Name = "ROOT/Суперпользователь")]
        ROOT
    }
}
