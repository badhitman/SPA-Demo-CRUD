////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models.db
{
    public class BtcTransactionModel : LiteEntityModel
    {
        /// <summary>
        /// Связаные btc выходы
        /// </summary>
        public List<BtcTransactionOutModel> Outputs { get; set; }

        public string TxId { get; set; }

        public double Sum { get; set; }
    }
}
