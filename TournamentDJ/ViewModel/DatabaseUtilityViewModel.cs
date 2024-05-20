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

    class DatabaseUtilityViewModel : NotifyObject
    {

        public DatabaseUtilityViewModel() 
        {
            CreateCommands();
            FailedUris = new ObservableCollection<Uri>();
            TracksToAdd = new ObservableCollection<Track>();
        }

        public ObservableCollection<Track> TracksToAdd
        {
            get { return Get<ObservableCollection<Track>>(); }
            set {
                Set(value);
                  OnPropertyChanged(); }
        }

        public ObservableCollection<Uri> FailedUris
        {
            get; set;
        }

        public ObservableCollection<Dance> Dances
        {
            get { return DatabaseUtility.Dances; }
            set { DatabaseUtility.Dances = value;
                OnPropertyChanged(); 
            }
        }

        public ObservableCollection<Track> Tracks
        {
            get { return DatabaseUtility.Tracks; }
            set {
                DatabaseUtility.Tracks = value;
                    OnPropertyChanged();}
        }


        public ICommand ChooseFolderCommand { get; private set; }
        public ICommand SaveDataCommand { get; private set; }

        public ICommand AddToDatabaseCommand { get; private set; }

        public void CreateCommands()
        {
            ChooseFolderCommand = new RelayCommand(ExecuteChooseFolder);
            SaveDataCommand = new RelayCommand(ExecuteSaveData);
            AddToDatabaseCommand = new RelayCommand(ExecuteAddToDatabase);
        }


        public void ExecuteChooseFolder()
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            if (openFolderDialog.ShowDialog() == true)
            {
                var path = openFolderDialog.FolderName;
                DatabaseUtility.GetFiles(path, TracksToAdd, FailedUris);
            }
        }

        public void ExecuteSaveData()
        {
            DatabaseUtility.SaveChanges();
        }

        public void ExecuteAddToDatabase()
        {
            DatabaseUtility.AddToDatabase(TracksToAdd);
        }
    }
}
