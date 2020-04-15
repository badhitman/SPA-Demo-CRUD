////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Документ движения резервов. Резерв - это условно (или безуслвоно) проданое (в т.ч. предоплаченный).
    /// </summary>
    public class MovementTurnoverDeliveryDocumentModel : BodyGoodMovementDocumentModel
    {
        public int? BuyerId { get; set; }
        public UserObjectModel Buyer { get; set; }

        public int DeliveryMethodId { get; set; }
        public DeliveryMethodObjectModel DeliveryMethod { get; set; }

        public int? DeliveryServiceId { get; set; }
        public DeliveryServiceObjectModel DeliveryService { get; set; }

        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }
    }
}
