using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    class BasicPlayerViewModel : PlayerViewModel
    {
        public override TrackList SelectedTrackList
        {
            get { return Player.SelectedTrackList; }
            set
            {
                Player.SelectedTrackList = value;
                OnPropertyChanged();
            }
        }
    }
}
