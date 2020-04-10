////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    public class ObjEntityModel : BirthdayObjEntityModel
    {
        /// <summary>
        /// Признак "только для чтения". Переименовывать или удалять такой объект может только ROOT
        /// </summary>
        public bool Readonly { get; set; } = false;

        /// <summary>
        /// Объект помечен как "неактивный"
        /// </summary>
        public bool isDisabled { get; set; } = false;

        /// <summary>
        /// Проверка/Пометка отметки объекта как [Избранный]
        /// </summary>
        [Display(Name = "Избранный (для всех глобально)", Description = "Объект глобально [Избранный]")]
        public bool IsGlobalFavorite { get; set; } = false;

        [Display(Name = "Информация", Description = "Пользовательское описание")]
        public string Information { get; set; } = "";
    }
}
