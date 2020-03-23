////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models.view.menu
{
    public class NestedMenu : MenuItem
    {
        public IEnumerable<MenuItem> Childs { get; set; }
    }
}
