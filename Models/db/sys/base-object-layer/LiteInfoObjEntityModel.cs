////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    public class LiteInfoObjEntityModel : LiteObjEntityModel
    {
        [Display(Name = "Информация", Description = "Пользовательское описание")]
        public string Information { get; set; } = "";
    }
}
