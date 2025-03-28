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

        /// <summary>
        /// Array of strings that identify this specific dance
        /// </summary>
        public string[]? DanceIdentifiers { get; set; }


        public virtual ICollection<Track> Tracks
        { get; private set; } =
            new ObservableCollection<Track>();

        /// <summary>
        /// A type of Dance, for which tracks can be categorized
        /// </summary>
        /// <param name="name"></param>
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
