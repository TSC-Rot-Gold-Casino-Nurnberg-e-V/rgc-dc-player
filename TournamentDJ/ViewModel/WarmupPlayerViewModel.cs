using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    internal class WarmupPlayerViewModel : PlayerViewModel
    {
        public new WarmupPlayer Player
        {
            get { return Get<WarmupPlayer>(); }
            set { Set(value); }
        }
        public WarmupPlayerViewModel() : base()
        {
            Player = new WarmupPlayer();
            Runtimes =
            [
                new TimeSpan(0, 0, 5),
                new TimeSpan(0, 0, 15),
                new TimeSpan(0, 0, 60),
                new TimeSpan(0, 0, 90),
            ];
            Runtimes.Add(TimeSpan.Zero);
            CreateAdditionalCommands();
            
        }

        public bool OnlyUseUncategorized
        {
            get
            {
                return Player.OnlyUseUncategorized;
            }
            set
            {
                Player.OnlyUseUncategorized = value;
            }
        }


        public ICommand GetAllTracksCommand { get; private set; }

        public void CreateAdditionalCommands()
        {
            GetAllTracksCommand = new RelayCommand(ExecuteGetAllTracks);
        }


        public void ExecuteGetAllTracks()
        {
            TracksToPlay = TrackListBuilder.GetAllTracks();
        }

        public override void ExecuteReselect(Track track)
        {
            Player.Reselect(track, notFavourite: true, overrideParams: true, onlyUseUncategorized: OnlyUseUncategorized);
        }

        public override void ExecuteCreateDanceRound()
        {
            Player.TracksPlayed.Tracks.Clear();
            TracksToPlay = TrackListBuilder.CreateDanceRound(SelectedDanceRound, cantBeFavourite: true, overrideParams: true, onlyUseUncategorized: OnlyUseUncategorized);
        }
    }
}
