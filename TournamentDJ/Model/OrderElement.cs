using System.ComponentModel.DataAnnotations;

namespace TournamentDJ.Model
{
    /// <summary>
    /// Class that is used to Order other objects in a specific way, using Entity Framework
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
