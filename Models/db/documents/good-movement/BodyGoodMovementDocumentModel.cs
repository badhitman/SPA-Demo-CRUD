////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models
{
    public abstract class BodyGoodMovementDocumentModel : ObjectFileRegisterModel
    {
        public int AuthorId { get; set; }
        /// <summary>
        /// Автор документа
        /// </summary>
        public UserObjectModel Author { get; set; }

        public string Discriminator { get; set; }

        /// <summary>
        /// Строки документа
        /// </summary>
        public List<RowGoodMovementRegisterModel> RowsDocument { get; set; }
    }
}
