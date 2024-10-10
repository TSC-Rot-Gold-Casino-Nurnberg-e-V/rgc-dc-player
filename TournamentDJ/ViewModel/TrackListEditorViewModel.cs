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

        public ICommand AddToTrackListCommand { get; private set; }
        public ICommand RemoveFromTrackListCommand { get; private set; }

        public ICommand ResetTrackFilterClickCommand { get; private set; }

        public void CreateCommands()
        {
            AddToTrackListCommand = new RelayCommand(ExecuteAddToTrackList);
            RemoveFromTrackListCommand = new RelayCommand(ExecuteRemoveFromTrackList);
            ResetTrackFilterClickCommand = new RelayCommand(ExecuteResetTrackFilterClick);
        }


        public void ExecuteAddToTrackList()
        {
            if (SelectedTrackList != null && SelectedTrackInTracks != null && !SelectedTrackList.Tracks.Contains(SelectedTrackInTracks))
            {
                SelectedTrackList.Tracks.Add(SelectedTrackInTracks);
            }
        }

        public void ExecuteRemoveFromTrackList()
        {
            if (SelectedTrackInList != null)
            {
                SelectedTrackList.Tracks.Remove(SelectedTrackInList);
            }
        }

        public void ExecuteResetTrackFilterClick()
        {
            TrackFilterString = string.Empty;
        }
    }
}

