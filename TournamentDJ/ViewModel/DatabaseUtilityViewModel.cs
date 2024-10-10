using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{

    class DatabaseUtilityViewModel : NotifyObject
    {
        public bool isPlaying = false;
        public bool playOnClick = false;
        public DatabaseUtilityViewModel()
        {
            trackListEditorViewModel = new TrackListEditorViewModel(this);
            Player = new Player();
            CreateCommands();
            FailedUris = new ObservableCollection<Uri>();
            TracksToAdd = new ObservableCollection<Track>();
            FilteredTracks = new ObservableCollection<Track>();
            TrackFilterString = string.Empty;

        }

        public ObservableCollection<Track> TracksToAdd
        {
            get { return Get<ObservableCollection<Track>>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        public Player Player { get; private set; }
        public TrackListEditorViewModel trackListEditorViewModel { get; private set; }

        public ObservableCollection<Uri> FailedUris
        {
            get; set;
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

        public ObservableCollection<Track> Tracks
        {
            get { return DatabaseUtility.Tracks; }
            set
            {
                DatabaseUtility.Tracks = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Track> FilteredTracks
        {
            get { return Get<ObservableCollection<Track>>(); }
            set
            {
                Set(value);
            }
        }

        public string TrackFilterString
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                if (value != null || value != string.Empty)
                {
                    FilteredTracks.Clear();
                    foreach (var track in Tracks)
                    {
                        value = value.ToLowerInvariant();
                        if (track.Dance != null && !string.IsNullOrEmpty(track.Dance.Name) && track.Dance.Name.ToLowerInvariant().Contains(value))
                        {
                            FilteredTracks.Add(track);
                            continue;
                        }

                        if (track.Title != null && !string.IsNullOrEmpty(track.Title) && track.Title.ToLowerInvariant().Contains(value))
                        {
                            FilteredTracks.Add(track);
                            continue;
                        }

                        if (track.Genre != null && !string.IsNullOrEmpty(track.Genre) && track.Genre.ToLowerInvariant().Contains(value))
                        {
                            FilteredTracks.Add(track);
                            continue;
                        }
                    }
                }
                else
                {
                    FilteredTracks = Tracks;
                }
            }
        }

        public Track TrackPlaying
        {
            get { return Player.TrackPlaying; }
            set
            {
                if (value == null || value.Equals(Player.TrackPlaying)) return;
                Player.TrackPlaying = value;
                isPlaying = false;
                if (playOnClick)
                {
                    Player.Play();
                    isPlaying = true;
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


        public ICommand ChooseFolderCommand { get; private set; }
        public ICommand SaveDataCommand { get; private set; }

        public ICommand AddToDatabaseCommand { get; private set; }

        public ICommand PlayPauseCommand { get; private set; }
        public ICommand TogglePlayOnClickCommand { get; private set; }
        public ICommand ResetTrackFilterClickCommand { get; private set; }

        public void CreateCommands()
        {
            ChooseFolderCommand = new RelayCommand(ExecuteChooseFolder);
            SaveDataCommand = new RelayCommand(ExecuteSaveData);
            AddToDatabaseCommand = new RelayCommand(ExecuteAddToDatabase);
            PlayPauseCommand = new RelayCommand(ExecutePlayPause);
            TogglePlayOnClickCommand = new RelayCommand(ExecuteTogglePlayOnClick);
            ResetTrackFilterClickCommand = new RelayCommand(ExecuteResetTrackFilterClick);
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

        public void ExecutePlayPause()
        {
            if (isPlaying)
            {
                Player.Stop();
                isPlaying = false;
            }
            else
            {
                Player.Play();
                isPlaying = true;
            }
        }

        public void ExecuteTogglePlayOnClick()
        {
            playOnClick = !playOnClick;
        }

        public void ExecuteSaveData()
        {
            DatabaseUtility.SaveChanges();
        }

        public void ExecuteAddToDatabase()
        {
            DatabaseUtility.AddToDatabase(TracksToAdd);
            TrackFilterString = string.Empty;
        }

        public void ExecuteResetTrackFilterClick()
        {
            TrackFilterString = string.Empty;
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Player.Stop();
        }
    }
}
