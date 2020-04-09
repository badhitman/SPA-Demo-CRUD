////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Типы записей/строк регистра
    /// </summary>
    public enum ObjectFileRegisterRowTypesEnum
    {
        /// <summary>
        /// Снимок состояния объекта (json)
        /// </summary>
        snapshot,

        /// <summary>
        /// Дополнительные прикреплённые файлы
        /// </summary>
        Attachment,

        /// <summary>
        /// другое (иные записи)
        /// </summary>
        Other
    }
}
