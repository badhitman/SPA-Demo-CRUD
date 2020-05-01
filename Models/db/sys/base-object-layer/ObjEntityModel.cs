////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPADemoCRUD.Models
{
    public class ObjEntityModel : BirthdayObjEntityModel
    {
        /// <summary>
        /// Признак "только для чтения". Переименовывать или удалять такой объект может только ROOT
        /// </summary>
        public bool isReadonly { get; set; } = false;

        /// <summary>
        /// Объект помечен как "неактивный"
        /// </summary>
        public bool isDisabled { get; set; } = false;

        /// <summary>
        /// Проверка/Пометка отметки объекта как [Избранный]
        /// </summary>
        [Display(Name = "Избранный (для всех глобально)", Description = "Объект глобально [Избранный]")]
        public bool isGlobalFavorite { get; set; } = false;

        [NotMapped]
        public new string FullInfo => $"{base.FullInfo}{(isReadonly ? "•[readonly!]" : "")}{(isDisabled ? "•[disabled!]" : "")}{(isGlobalFavorite ? "•[global favorite!]" : "")}";
    }
}
