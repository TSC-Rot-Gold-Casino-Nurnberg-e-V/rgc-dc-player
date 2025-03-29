using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace TournamentDJ.Model
{
    public class TrackList
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }

        public virtual ObservableCollection<Track> Tracks
        { get; private set; } =
        new ObservableCollection<Track>();

        public bool OverrideTracks(ObservableCollection<Track> newTracks)
        {
            if(newTracks != null)
            {
                Tracks = newTracks;
                return true;
            }
            return false;
        }
    }
}
