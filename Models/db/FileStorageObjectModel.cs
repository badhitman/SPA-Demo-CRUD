////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Файл, сохранённый в БД
    /// </summary>
    public class FileStorageObjectModel : ObjEntityModel
    {
        /// <summary>
        /// Размер данных в байтах
        /// </summary>
        public long Length { get; set; }
        /// <summary>
        /// Если файл подчинён записи снимка данных объекта. Владелец файла - запись регистра доступа к объекту.
        /// Требуется хранить целостность таких файлов в ассоциации с записью лога
        /// </summary>
        public ObjectFileRegisterRowModel LogAccessorRow { get; set; }
    }
}
