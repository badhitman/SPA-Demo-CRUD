////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    public class NestedMenuModel : MenuItemModel
    {
        public IEnumerable<MenuItemModel> Childs { get; set; }
    }
}
