using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db.sys
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
