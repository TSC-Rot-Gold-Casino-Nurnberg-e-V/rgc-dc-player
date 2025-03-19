using System.Collections.ObjectModel;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    internal class TournamentPlayerViewModel : PlayerViewModel
    {
        public TournamentPlayerViewModel()
        {
            Runtimes = new ObservableCollection<TimeSpan>();
            foreach(TimeSpan ts in DefaultValues.DefaultTournamentRuntimes)
            {
                Runtimes.Add(ts);
            }

            NumberOfHeats = new ObservableCollection<int>();

            for (int i = 1; i < 20; i++)
            {
                NumberOfHeats.Add(i);
            }

            SelectedNumberOfHeats = 1;
        }

        public bool IsSingleSelected
        {
            get { return Player.IsSingleSelected; }
            set
            {
                Player.IsSingleSelected = value;
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

        public override void ExecutePlay()
        {
            Player.Play(overrideLastPlayed: default, increasePlayCount: true);
        }

    }
}
