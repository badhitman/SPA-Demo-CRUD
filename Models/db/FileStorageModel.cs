////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Файл, сохранённый в БД
    /// </summary>
    public class FileStorageModel : LiteEntityModel
    {
        public string Length { get; set; }
    }
}
