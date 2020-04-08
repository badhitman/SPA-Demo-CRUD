using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models.db
{
    /// <summary>
    /// Объекту добавлена аватарка
    /// </summary>
    public class ObjAvatarModel: ObjEntityModel
    {
        public int? AvatarId { get; set; }
        public FileStorageModel Avatar { get; set; }
    }
}
