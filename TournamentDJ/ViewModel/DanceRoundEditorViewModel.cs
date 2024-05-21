using Microsoft.Win32;
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
        public ICommand RemoveFromDanceRoundCommand {  get; private set; }
        public ICommand MoveDanceUpCommand {  get; private set; }
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
            if(SelectedDanceInDances != null)
            {
                SelectedDanceRound.Dances.Add(SelectedDanceInDances);
            }
        }

        public void ExecuteRemoveFromDanceRound()
        {
            if (SelectedDanceInRound != null)
            {
                SelectedDanceRound.Dances.Remove(SelectedDanceInRound);
            }
        }

        public void ExecuteMoveDanceUp()
        {
            if (SelectedDanceInRound != null)
            {
                int index = SelectedDanceRound.Dances.IndexOf(SelectedDanceInRound);

                if(index > 0)
                {
                    SelectedDanceRound.Dances.Move(index, index - 1);
                }
            }
        }

        public void ExecuteMoveDanceDown()
        {
            if (SelectedDanceInRound != null)
            {
                int index = SelectedDanceRound.Dances.IndexOf(SelectedDanceInRound);

                if (index < SelectedDanceRound.Dances.Count - 1)
                {
                    SelectedDanceRound.Dances.Move(index, index + 1);
                }
            }
        }
    }
}
