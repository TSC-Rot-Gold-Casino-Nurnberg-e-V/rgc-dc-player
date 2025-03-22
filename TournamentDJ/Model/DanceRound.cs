using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace TournamentDJ.Model
{
    public class DanceRound
    {
        [Key]
        public int DanceRoundId { get; set; }
        public string? Name { get; set; }

        public int MinDifficulty { get; set; }
        public int MaxDifficulty { get; set; }
        public int MinCharacteristics { get; set; }

        public virtual ObservableCollection<OrderElement<Dance>> OrderElements
        { get; private set; }
            = new ObservableCollection<OrderElement<Dance>>();

        public DanceRound(string name = "New DanceRound", int  minDifficulty = 0, int maxDifficulty = 3, int minCharacteristics = 1)
        {
            Name = name;
            MinDifficulty = minDifficulty;
            MaxDifficulty = maxDifficulty;
            MinCharacteristics = minCharacteristics;
        }

        public DanceRound()
        {
            Name = "New DanceRound";
            MinDifficulty = 0;
            MaxDifficulty = 3;
            MinCharacteristics = 1;
        }

        public ObservableCollection<Dance> GetDancesInOrder()
        {
            ObservableCollection<Dance> dancesInOrder = new ObservableCollection<Dance>();
            var orderedElements = OrderElements.OrderBy(X => X.OrderRank);
            foreach (var orderElement in orderedElements)
            {
                dancesInOrder.Add(orderElement.ObjectToOrder);
            }
            return dancesInOrder;
        }

        public void SetDancesInOrder(ObservableCollection<Dance> dances)
        {
            OrderElements.Clear();
            for (int i = 0; i < dances.Count; i++)
            {
                OrderElement<Dance> newElement = new OrderElement<Dance>(dances[i]);
                newElement.OrderRank = i;
                OrderElements.Add(newElement);
            }
        }
    }
}
