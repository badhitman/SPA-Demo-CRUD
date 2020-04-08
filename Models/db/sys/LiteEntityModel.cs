////////////////////////////////////////////////
// © https://github.com/badhitman - @fakegov 
////////////////////////////////////////////////

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SPADemoCRUD.Models
{
    public class LiteEntityModel
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Информация", Description = "Пользовательское описание")]
        public string Information { get; set; } = "";



        public override bool Equals(object other)
        {
            if (Id == 0 || other is null || other.GetType() != this.GetType())
                return false;
            ObjEntityModel norm_other = (ObjEntityModel)other;
            if (norm_other.Id == 0)
                return false;

            return this.Id.Equals(norm_other.Id);
        }

        public override int GetHashCode()
        {
            if (Id == 0)
                return 0;
            return (this.GetType().Name + this.Id.ToString()).GetHashCode();
        }

        public static bool operator ==(LiteEntityModel a1, LiteEntityModel a2)
        {
            if (a1 is null && a2 is null)
                return true;
            else if (a1 is null || a2 is null)
                return false;
            return a1.Equals(a2);
        }

        public static bool operator !=(LiteEntityModel a1, LiteEntityModel a2)
        {
            if (a1 is null && a2 is null)
                return true;
            else if (a1 is null && !(a2 is null) || a2 is null && !(a1 is null))
                return true;
            return !a1.Equals(a2);
        }
    }
}
