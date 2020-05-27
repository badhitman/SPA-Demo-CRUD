////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

namespace SPADemoCRUD.Models
{
    public abstract class aInventoryGoodBalancesAnalyticalModel : ObjectFileRegisterModel
    {
        public int GoodId { get; set; }
        /// <summary>
        /// Номенклатура
        /// </summary>
        public GoodObjectModel Good { get; set; }

        /// <summary>
        /// Единица измерения
        /// </summary>
        public int UnitId { get; set; }
        //public UnitGoodObjectModel Unit { get; set; }

        /// <summary>
        /// Остаток (количество)
        /// </summary>
        public double Quantity { get; set; }
    }
}
