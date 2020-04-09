using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SPADemoCRUD.Models
{
    /// <summary>
    /// Документ движения резервов. Резерв - это условно (или безуслвоно) проданое (в т.ч. предоплаченный).
    /// </summary>
    public class MovementTurnoverDeliveryDocumentModel : ObjectFileRegisterModel
    {
        public int? BuyerId { get; set; }
        public UserModel Buyer { get; set; }

        public int? DeliveryMethodId { get; set; }
        public DeliveryMethodModel DeliveryMethod { get; set; }
        
        public int? DeliveryServiceId { get; set; }
        public DeliveryServiceModel DeliveryService { get; set; }

        public string DeliveryAddress1 { get; set; }
        public string DeliveryAddress2 { get; set; }

        public List<MovementTurnoverDeliveryDocumentRowModel> DocumentRows { get; set; }
    }
}
