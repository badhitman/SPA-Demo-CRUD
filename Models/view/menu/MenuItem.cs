////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    public class MenuItem
    {
        public string HtmlDomId { get; set; } = "";

        public string Title { get; set; }

        public string Href { get; set; }

        public string Tooltip { get; set; }

        public bool IsDisabled { get; set; } = false;
    }
}
