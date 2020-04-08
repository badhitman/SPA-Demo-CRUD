////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models.db
{
    public class BtcTransactionOutModel : LiteEntityModel
    {
        #region Навигационные свойства

        public int BtcTransactionModelId { get; set; }
        /// <summary>
        /// Родительская btc транзакция
        /// </summary>
        public BtcTransactionModel BtcTransactionModel { get; set; }
        public int? UserId { get; set; }
        /// <summary>
        /// Связаный пользователь зачисления
        /// </summary>
        public UserModel User { get; set; }
        #endregion

        #region Реквизиты

        public string Address { get; set; }

        public double Sum { get; set; }

        /// <summary>
        /// Признак: является ли данный tx-out в адрес собственного адреса
        /// </summary>
        public bool IsMine { get; set; }

        #endregion
    }
}
