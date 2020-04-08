////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    public class NestedMenu : MenuItem
    {
        public IEnumerable<MenuItem> Childs { get; set; }
    }
}
