////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPADemoCRUD.Models
{
    public class LiteObjEntityModel : EFModel
    {
        [Display(Name = "Наименование", Description = "Короткое название объекта")]
        [Required]
        public string Name { get; set; }

        [NotMapped]
        public string FullInfo => $"[#{Id}]•[name:{Name}]";
    }
}
