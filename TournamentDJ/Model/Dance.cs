using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace TournamentDJ.Model
{
    public class Dance
    {
        [Key]
        public int DanceTypeId { get; set; }
        public string? Name { get; set; }

        public int MinBPM { get; set; }
        public int MaxBPM { get; set; }

        public virtual ICollection<Track> Tracks
        { get; private set; } =
            new ObservableCollection<Track>();

        public virtual ICollection<DanceRound> DanceRounds
        { get; private set; } =
            new ObservableCollection<DanceRound>();


        public Dance(string name)
        {
            Name = name;
        }

        public Dance() { }
    }
}
