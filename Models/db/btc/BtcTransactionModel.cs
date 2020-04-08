////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.Collections.Generic;

namespace SPADemoCRUD.Models
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
