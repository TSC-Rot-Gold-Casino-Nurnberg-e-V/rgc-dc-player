using System.Collections.ObjectModel;
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

        public string[]? DanceIdentifiers { get; set; }


        public virtual ICollection<Track> Tracks
        { get; private set; } =
            new ObservableCollection<Track>();


        public Dance(string name)
        {
            Name = name;
        }

        public Dance() { }

        public Dance(string name, string[] danceIdentifiers)
        {
            Name = name;
            DanceIdentifiers = danceIdentifiers;
        }
    }
}
