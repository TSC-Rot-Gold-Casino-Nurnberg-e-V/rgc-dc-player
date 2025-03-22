using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    class TrackListEditorViewModel : NotifyObject
    {
        public TrackListEditorViewModel()
        {
            CreateCommands();
            FilteredTracks = new ObservableCollection<Track>();
            TrackFilterString = string.Empty;
        }

        public TrackListEditorViewModel(DatabaseUtilityViewModel databaseUtilityViewModel)
        {
            CreateCommands();
            FilteredTracks = new ObservableCollection<Track>();
            TrackFilterString = string.Empty;
            DatabaseUtilityViewModel = databaseUtilityViewModel;
        }

        public DatabaseUtilityViewModel DatabaseUtilityViewModel { get; set; }

        public ObservableCollection<Track> Tracks
        {
            get { return DatabaseUtility.Tracks; }
            set
            {
                DatabaseUtility.Tracks = value;
                OnPropertyChanged();
            }
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

        public ObservableCollection<TrackList> TrackLists
        {
            get { return DatabaseUtility.TrackLists; }
            set
            {
                DatabaseUtility.TrackLists = value;
                OnPropertyChanged();
            }
        }

        public TrackList SelectedTrackList
        {
            get { return Get<TrackList>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        public Track SelectedTrackInTracks
        {
            get { return Get<Track>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
                DatabaseUtilityViewModel.TrackPlaying = value;
            }
        }

        public Track SelectedTrackInList
        {
            get { return Get<Track>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
                DatabaseUtilityViewModel.TrackPlaying = value;
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

        public Dictionary<int, string> Ratings
        {
            get { return Track.Ratings; }
            private set { Track.Ratings = value; }
        }

        public IList SelectedTracksInTracks { get; set; }
        public IList SelectedTracksInTrackList { get; set; }

        public ObservableCollection<Track> FilteredTracks
        {
            get { return Get<ObservableCollection<Track>>(); }
            set
            {
                Set(value);
            }
        }

        public Dance? SelectedDance
        {
            get { return Get<Dance>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
                ExecuteApplyTrackFilter();
            }
        }

        public string TrackFilterString
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                ExecuteApplyTrackFilter();
            }
        }

        public ICommand AddToTrackListCommand { get; private set; }
        public ICommand RemoveFromTrackListCommand { get; private set; }

        public ICommand ResetTrackFilterClickCommand { get; private set; }
        public ICommand ApplyTrackFilterClickCommand { get; private set; }
        public void CreateCommands()
        {
            AddToTrackListCommand = new RelayCommand(ExecuteAddToTrackList);
            RemoveFromTrackListCommand = new RelayCommand(ExecuteRemoveFromTrackList);
            ResetTrackFilterClickCommand = new RelayCommand(ExecuteResetTrackFilterClick);
            ApplyTrackFilterClickCommand = new RelayCommand(ExecuteApplyTrackFilter);
        }


        public void ExecuteAddToTrackList()
        {
            if (SelectedTrackList != null && SelectedTracksInTracks != null)
            {
                foreach (Track track in SelectedTracksInTracks)
                {
                    if(!SelectedTrackList.Tracks.Contains(track))
                    {
                        SelectedTrackList.Tracks.Add(track);
                    }
                }
            }
        }

        public void ExecuteRemoveFromTrackList()
        {
            if (SelectedTrackList != null && SelectedTracksInTrackList != null)
            {
                //Copy selection over to make sure it doesn't get changed during removal
                List<Track> tracksToRemove = new List<Track>();
                
                foreach (Track track in SelectedTracksInTrackList)
                {
                        tracksToRemove.Add((Track) track);
                }

                foreach (Track track in tracksToRemove)
                {
                    SelectedTrackList.Tracks.Remove(track);
                }
            }
        }

        public void ExecuteResetTrackFilterClick()
        {
            TrackFilterString = string.Empty;
            SelectedDance = null;
            ExecuteApplyTrackFilter();
        }

        public void ExecuteApplyTrackFilter()
        {
            if(SelectedDance != null)
            {
                FilteredTracks = new ObservableCollection<Track>(Tracks.Where(X => X.Dance == SelectedDance));
            } else
            {
                FilteredTracks = Tracks;
            }

            ObservableCollection<Track> newFilterdTracks = new ObservableCollection<Track>();

            if (TrackFilterString != null || TrackFilterString != string.Empty)
            {
                string filterString = TrackFilterString.ToLowerInvariant();
                foreach (var track in FilteredTracks)
                {
                    if (track.Title != null && !string.IsNullOrEmpty(track.Title) && track.Title.ToLowerInvariant().Contains(filterString))
                    {
                        newFilterdTracks.Add(track);
                        continue;
                    }
                }

                FilteredTracks = newFilterdTracks;
            }
        }
    }
}

