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
        public WarmupPlayerViewModel() : base()
        {
            base.Player = new WarmupPlayer();
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


        public ICommand GetAllTracksCommand { get; private set; }

        public void CreateAdditionalCommands()
        {
            GetAllTracksCommand = new RelayCommand(ExecuteGetAllTracks);
        }


        public void ExecuteGetAllTracks()
        {
            TracksToPlay = TrackListBuilder.GetAllTracks();
        }
    }
}
