////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Файл, сохранённый в БД
    /// </summary>
    public class FileStorageModel : ObjEntityModel
    {
        /// <summary>
        /// Размер данных в байтах
        /// </summary>
        public long Length { get; set; }
    }
}
