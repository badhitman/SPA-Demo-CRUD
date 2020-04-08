////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    public class BirthdayEntityModel : LiteEntityModel
    {
        /// <summary>
        /// Дата создания объекта
        /// </summary>
        [Display(Name = "Дата создания", Description = "Дата/Время создания объекта")]
        public virtual DateTime DateCreate { get; set; }
    }
}
