////////////////////////////////////////////////
// https://github.com/badhitman 
////////////////////////////////////////////////
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPADemoCRUD.Models
{
    public class LiteEntityModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        /// <summary>
        /// Чтение/Запись коментария для объекта [info]
        /// </summary>
        [Display(Name = "Наименование", Description = "Наименование объекта")]
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; } = "";
    }
}
