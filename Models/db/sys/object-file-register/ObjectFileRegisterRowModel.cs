////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SPADemoCRUD.Controllers;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Ведение файлового регистра для объекта. Базовый функционал на основе хранимых файлов.
    /// При изменении или удалении объекта, можно сохранить json данные объекта. Таким образом будет храниться вся история состояний объекта в виде json снимков в файлах.
    /// </summary>
    public class ObjectFileRegisterRowModel : BirthdayObjEntityModel
    {
        /// <summary>
        /// Тип записи регистра
        /// </summary>
        public ObjectFileRegisterRowTypesEnum RowType { get; set; } = ObjectFileRegisterRowTypesEnum.snapshot;

        public int UserId { get; set; }
        /// <summary>
        /// Пользователь, инициирующий запись в регистр
        /// </summary>
        public UserModel User { get; set; }

        public int FileStorageId { get; set; }
        /// <summary>
        /// Связаный файл куда записаны данные регистра
        /// </summary>
        public FileStorageModel FileStorage { get; set; }
    }
}
