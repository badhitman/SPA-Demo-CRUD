////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Объект с базовой поддержкой записи состояния объекта в видей json файла данных
    /// </summary>
    public class ObjectFileRegisterModel : BirthdayObjEntityModel
    {
        public List<ObjectFileRegisterRowModel> LogRows { get; set; }
    }
}
