////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    public class BtcTransactionOutObjectModel : BirthdayObjEntityModel
    {
        #region Навигационные свойства

        public int BtcTransactionModelId { get; set; }
        /// <summary>
        /// Родительская btc транзакция
        /// </summary>
        public BtcTransactionObjectModel BtcTransactionModel { get; set; }
        public int? UserId { get; set; }
        /// <summary>
        /// Связаный пользователь зачисления
        /// </summary>
        public UserObjectModel User { get; set; }
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
