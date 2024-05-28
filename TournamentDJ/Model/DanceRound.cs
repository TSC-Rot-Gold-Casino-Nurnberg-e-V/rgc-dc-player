using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        public int MinDifficulty { get; set; }
        public int MaxDifficulty { get; set; }
        public int MinCharacteristics { get; set; }

        public virtual List<Dance> Dances
        { get; private set; }
            = new List<Dance>();

        public DanceRound(string name)
        {
            Name = name;
            MinDifficulty = 0;
            MaxDifficulty = 3;
            MinCharacteristics = 1;
        }

        public DanceRound()
        {
            Name = "New DanceRound";
            MinDifficulty = 0;
            MaxDifficulty = 3;
            MinCharacteristics = 1;
        }
    }
}
