////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPADemoCRUD.Models
{
    public class LiteInfoObjEntityModel : LiteObjEntityModel
    {
        [Display(Name = "Информация", Description = "Пользовательское описание")]
        public string Information { get; set; } = "";

        [NotMapped]
        public new string FullInfo => $"{base.FullInfo}•[info:{Information}]";
    }
}
