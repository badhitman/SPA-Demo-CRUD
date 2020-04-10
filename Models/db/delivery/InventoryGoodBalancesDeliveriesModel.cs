////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Номенклатура в обороте/доставке (учёт/остатки)
    /// </summary>
    public class InventoryGoodBalancesDeliveriesModel : ObjectFileRegisterModel
    {
        public int DeliveryMethodId { get; set; }
        /// <summary>
        /// Метод доставки
        /// </summary>
        public DeliveryMethodModel DeliveryMethod { get; set; }

        public int DeliveryServiceId { get; set; }
        /// <summary>
        /// Служба доставки
        /// </summary>
        public DeliveryServiceModel DeliveryService { get; set; }
        
        public int GoodId { get; set; }
        /// <summary>
        /// Номенклатура
        /// </summary>
        public GoodModel Good { get; set; }

        /// <summary>
        /// Остаток (количество)
        /// </summary>
        public double RemnantsOfTheGoods { get; set; }
    }
}
