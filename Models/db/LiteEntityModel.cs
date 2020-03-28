////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
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
        [Display(Name = "Наименование (публичное)", Description = "Публичное наименование объекта")]
        [Required(ErrorMessage = "Не указано публичное имя")]
        public string Name { get; set; } = "";

        /// <summary>
        /// Признак "только для чтения". Переименовывать или удалять такой объект может только ROOT
        /// </summary>
        public bool Readonly { get; set; } = false;

        /// <summary>
        /// Объект помечен как "неактивный"
        /// </summary>
        public bool isDisabled { get; set; } = false;
    }
}
