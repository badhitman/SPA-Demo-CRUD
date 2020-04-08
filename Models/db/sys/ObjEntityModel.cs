////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SPADemoCRUD.Models.db;
using SPADemoCRUD.Models.db.sys;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPADemoCRUD.Models
{
    public class ObjEntityModel : BirthdayEntityModel
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

        [Display(Name = "Наименование", Description = "Короткое название объекта")]
        public string Name { get; set; }
    }
}
