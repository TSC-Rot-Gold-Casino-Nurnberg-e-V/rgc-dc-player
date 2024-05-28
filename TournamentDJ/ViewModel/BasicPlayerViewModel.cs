using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    class BasicPlayerViewModel : PlayerViewModel
    {
        public override TrackList SelectedTrackList {
            get { return Player.SelectedTrackList; }
            set
            {
                Player.SelectedTrackList = value;
                OnPropertyChanged();
            }
        }
    }
}
