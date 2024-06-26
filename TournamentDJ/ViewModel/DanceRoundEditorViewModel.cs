﻿using System.Collections.ObjectModel;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    public class DanceRoundEditorViewModel : NotifyObject
    {

        public DanceRoundEditorViewModel()
        {
            CreateCommands();
        }

        public ObservableCollection<Dance> Dances
        {
            get { return DatabaseUtility.Dances; }
            set
            {
                DatabaseUtility.Dances = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<DanceRound> DanceRounds
        {
            get { return DatabaseUtility.DanceRounds; }
            set
            {
                DatabaseUtility.DanceRounds = value;
                OnPropertyChanged();
            }
        }

        public DanceRound SelectedDanceRound
        {
            get { return Get<DanceRound>(); }
            set
            {
                Set(value);
                OrderedDancesInSelectedRound = OrderedDancesInSelectedRound;
                OnPropertyChanged();
            }
        }

        public Dance SelectedDanceInDances
        {
            get { return Get<Dance>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        public Dance SelectedDanceInRound
        {
            get { return Get<Dance>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        public int SelectedDanceInRoundIndex
        {
            get { return Get<int>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Dance> OrderedDancesInSelectedRound
        {
            get
            {
                if (SelectedDanceRound != null)
                {
                    return SelectedDanceRound.GetDancesInOrder();
                }
                return null;
            }
            set
            {
                if (SelectedDanceRound != null)
                {
                    SelectedDanceRound.SetDancesInOrder(value);
                }
                OnPropertyChanged();
            }
        }

        public Dictionary<int, string> Difficulties
        {
            get { return Track.Difficulties; }
            private set { Track.Difficulties = value; }
        }

        public Dictionary<int, string> Characteristics
        {
            get { return Track.Characteristics; }
            private set { Track.Characteristics = value; }
        }

        public ICommand AddToDanceRoundCommand { get; private set; }
        public ICommand RemoveFromDanceRoundCommand { get; private set; }
        public ICommand MoveDanceUpCommand { get; private set; }
        public ICommand MoveDanceDownCommand { get; private set; }

        public void CreateCommands()
        {
            AddToDanceRoundCommand = new RelayCommand(ExecuteAddToDanceRound);
            RemoveFromDanceRoundCommand = new RelayCommand(ExecuteRemoveFromDanceRound);
            MoveDanceDownCommand = new RelayCommand(ExecuteMoveDanceDown);
            MoveDanceUpCommand = new RelayCommand(ExecuteMoveDanceUp);
        }


        public void ExecuteAddToDanceRound()
        {
            if (SelectedDanceInDances != null)
            {
                ObservableCollection<Dance> dances = OrderedDancesInSelectedRound;
                if (dances == null)
                {
                    dances = new ObservableCollection<Dance>();
                }
                dances.Add(SelectedDanceInDances);
                OrderedDancesInSelectedRound = dances;
            }
        }

        public void ExecuteRemoveFromDanceRound()
        {
            if (SelectedDanceInRound != null && OrderedDancesInSelectedRound != null)
            {
                ObservableCollection<Dance> dances = OrderedDancesInSelectedRound;
                if (dances == null)
                {
                    return;
                }
                dances.Remove(SelectedDanceInRound);
                OrderedDancesInSelectedRound = dances;
            }
        }

        public void ExecuteMoveDanceUp()
        {
            if (SelectedDanceInRound != null && OrderedDancesInSelectedRound != null)
            {
                ObservableCollection<Dance> dances = OrderedDancesInSelectedRound;
                if (dances == null)
                {
                    return;
                }

                int index = SelectedDanceInRoundIndex;

                if (index > 0)
                {
                    dances.Move(index, index - 1);
                    OrderedDancesInSelectedRound = dances;
                    SelectedDanceInRoundIndex = index - 1;
                    SelectedDanceInRound = OrderedDancesInSelectedRound[index - 1];
                }
            }
        }

        public void ExecuteMoveDanceDown()
        {
            if (SelectedDanceInRound != null && OrderedDancesInSelectedRound != null)
            {
                ObservableCollection<Dance> dances = OrderedDancesInSelectedRound;
                if (dances == null)
                {
                    return;
                }

                int index = SelectedDanceInRoundIndex;

                if (index < dances.Count - 1)
                {
                    dances.Move(index, index + 1);
                    OrderedDancesInSelectedRound = dances;
                    SelectedDanceInRoundIndex = index + 1;
                    SelectedDanceInRound = OrderedDancesInSelectedRound[index + 1];
                }
            }
        }
    }
}
