using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;
using Windows.Media.Playback;

namespace TournamentDJ.ViewModel
{
    class TouchPlayerViewModel : PlayerViewModel
    {

        public TouchPlayer Player
        {
            get { return Get<TouchPlayer>(); }
            set { Set(value); }
        }
        public TouchPlayerViewModel() : base()
        {
            Player = new TouchPlayer();
            CreateAdditionalCommands();
        }

        public override TrackList SelectedTrackList
        {
            get { return Player.SelectedTrackList; }
            set
            {
                Player.SelectedTrackList = value;
                OnPropertyChanged();
            }
        }

        public TrackList SelectedSecondaryTrackList
        {
            get { return Player.SelectedSecondaryTrackList; }
            set
            {
                Player.SelectedSecondaryTrackList = value;
                OnPropertyChanged();
            }
        }




        public ICommand PlayOnTrackSelectedCommand { get; private set; }

        public void CreateAdditionalCommands()
        {
            PlayOnTrackSelectedCommand = new RelayCommand<Track>(ExecutePlayOnTrackSelect);
        }


        public async void ExecutePlayOnTrackSelect(Track track)
        {
            if(Player.MedPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                Player.Fadeout(3.0);
            }
            while (Player.MedPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing)
            {
                await Task.Delay(100);
            }
            Player.TrackPlaying = track;
            Player.Fadein(1.0);
        }

    }


}
