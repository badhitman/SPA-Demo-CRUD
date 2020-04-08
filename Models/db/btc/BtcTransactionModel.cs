////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using SPADemoCRUD.Models.db.sys;
using System.Collections.Generic;

namespace SPADemoCRUD.Models.db
{
    public class BtcTransactionModel : BirthdayEntityModel
    {
        /// <summary>
        /// Связаные btc выходы
        /// </summary>
        public List<BtcTransactionOutModel> Outputs { get; set; }

        public string TxId { get; set; }

        public double Sum { get; set; }
    }
}
