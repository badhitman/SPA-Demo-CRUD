////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
{
    public class BtcTransactionObjectModel : BirthdayObjEntityModel
    {
        /// <summary>
        /// Связаные btc выходы
        /// </summary>
        public List<BtcTransactionOutObjectModel> Outputs { get; set; }

        public string TxId { get; set; }

        public double Sum { get; set; }
    }
}
