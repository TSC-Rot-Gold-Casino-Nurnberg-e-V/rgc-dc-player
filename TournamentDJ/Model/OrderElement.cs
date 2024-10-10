using System.ComponentModel.DataAnnotations;

namespace TournamentDJ.Model
{
    public class OrderElement<T>
    {
        public OrderElement() { }
        public OrderElement(T objectToOrder)
        {
            ObjectToOrder = objectToOrder;
        }

        [Key]
        public int Id { get; set; }
        public int OrderRank { get; set; }
        public virtual T ObjectToOrder { get; set; }
    }
}
