using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentDJ.Model;

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
    }
}
