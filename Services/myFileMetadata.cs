////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SPADemoCRUD.Models;
using System.IO;

namespace SPADemoCRUD.Services
{
    public class myFileMetadata
    {
        public FileInfo FileInfo { get; set; }
        public FileInfo ThumbFileInfo { get; set; }
        public FileStorageModel Object { get; set; }
    }
}
