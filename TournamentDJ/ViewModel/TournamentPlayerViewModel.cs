using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    internal class TournamentPlayerViewModel : PlayerViewModel
    {
        public TournamentPlayerViewModel()
        {
            Runtimes =
            [
                new TimeSpan(0, 0, 5),
                new TimeSpan(0, 0, 15),
                new TimeSpan(0, 0, 30),
                new TimeSpan(0, 0, 90),
                new TimeSpan(0, 0, 105),
            ];

            Runtimes.Add(TimeSpan.Zero);

            NumberOfHeats = new ObservableCollection<int>();

            for(int i = 1; i < 100; i++)
            {
                NumberOfHeats.Add(i);
            }

            SelectedNumberOfHeats = 1;
        }

        public bool IsSingleSelected
        {
            get { return Get<bool>(); }
            set
            {
                Set(value);
                ExecuteCreateDanceRound();
            }
        }

        public int SelectedNumberOfHeats
        {
            get { return Get<int>(); }
            set
            {
                Set(value);
                ExecuteCreateDanceRound();
            }
        }

        public ObservableCollection<int> NumberOfHeats
        {
            get { return Get<ObservableCollection<int>>(); }
            set
            {
                Set(value);
            }
        }

        override public void ExecuteCreateDanceRound()
        {
            TracksToPlay = TrackListBuilder.CreateDanceRound(SelectedDanceRound, SelectedNumberOfHeats, IsSingleSelected, tracklist: SelectedTrackList);
        }

    }
}
