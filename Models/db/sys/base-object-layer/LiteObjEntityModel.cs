////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    public class LiteObjEntityModel : EFModel
    {
        [Display(Name = "Наименование", Description = "Короткое название объекта")]
        [Required]
        public string Name { get; set; }
    }
}
