using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentDJ.Model;

namespace TournamentDJ.Model
{
    class TrackList : ObservableCollection<Track>
    {
        public void AddTrack(Track track)
        {
            this.Add(track);
        }

        public void AddTrack(Uri uri)
        {
            this.Add(new Track(uri));
        }

        public void ChangeTrack(int index, Track track)
        {
            this[index] = track;
        }

        public void ChangeTrack(int index, Uri uri)
        {
            this[index] = new Track(uri);
        }


    }
}
