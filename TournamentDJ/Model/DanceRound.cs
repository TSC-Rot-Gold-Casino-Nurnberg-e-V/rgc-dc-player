using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentDJ.Model
{
    public class DanceRound
    {
        [Key]
        public int DanceRoundId { get; set; }
        public string? Name { get; set; }

        public virtual ObservableCollection<Dance> Dances
        { get; private set; } =
            new ObservableCollection<Dance>();

        public DanceRound(string name)
        {
            Name = name;
        }

        public DanceRound()
        {
        }
    }
}
