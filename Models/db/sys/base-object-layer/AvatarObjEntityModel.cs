////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Объекту добавлена аватарка
    /// </summary>
    public class AvatarObjEntityModel : ObjEntityModel
    {
        public int? AvatarId { get; set; }
        public FileStorageObjectModel Avatar { get; set; }
    }
}
