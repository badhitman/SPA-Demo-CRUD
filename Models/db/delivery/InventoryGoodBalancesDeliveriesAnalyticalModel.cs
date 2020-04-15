////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Номенклатура в обороте/доставке (учёт/остатки)
    /// </summary>
    public class InventoryGoodBalancesDeliveriesAnalyticalModel : aInventoryGoodBalancesAnalyticalModel
    {
        public int DeliveryMethodId { get; set; }
        /// <summary>
        /// Метод доставки
        /// </summary>
        public DeliveryMethodObjectModel DeliveryMethod { get; set; }

        /// <summary>
        /// Служба доставки
        /// </summary>
        public DeliveryServiceObjectModel DeliveryService { get; set; }

        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
    }
}
