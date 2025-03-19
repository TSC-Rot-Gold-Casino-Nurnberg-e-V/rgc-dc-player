using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;
using System.IO;
using Windows.Security.Isolation;

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
        public Logger logger
        {
            get { return Logger.LoggerInstance; }
        }

        public ObservableCollection<Uri> FailedUris
        {
            get; set;
        }

        public int FilesProcessed
        {
            get
            {
                return Get<int>();
            }
            set
            {
                Set(value);
                WorkerProgress = (int)((float)FilesProcessed / (float)FilesToProcess * 100);
            }
        }

        public int FilesToProcess { get; set; }

        public bool IsProcessing {
            get { return Get<bool>(); }
            set { Set(value);
                OnPropertyChanged();
            }
        }

        public int WorkerProgress
        {
            get
            {
                return Get<int>();
            }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        private void WorkerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            WorkerProgress = e.ProgressPercentage;
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
                    Player.Play(true);
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
        public ICommand ApplyTrackFilterClickCommand { get; private set; }
        public ICommand ChooseFileCommand { get; private set; }
        public ICommand ExportFileDataCommand { get; private set; }

        public void CreateCommands()
        {
            ChooseFolderCommand = new RelayCommand(ExecuteChooseFolder);
            SaveDataCommand = new RelayCommand(ExecuteSaveData);
            AddToDatabaseCommand = new RelayCommand(ExecuteAddToDatabase);
            PlayPauseCommand = new RelayCommand(ExecutePlayPause);
            TogglePlayOnClickCommand = new RelayCommand(ExecuteTogglePlayOnClick);
            ResetTrackFilterClickCommand = new RelayCommand(ExecuteResetTrackFilterClick);
            ApplyTrackFilterClickCommand = new RelayCommand(ExecuteApplyTrackFilter);
            ChooseFileCommand = new RelayCommand(ExecuteChooseFile);
            ExportFileDataCommand = new RelayCommand(ExecuteExportFileData);
        }


        public async void ExecuteExportFileData()
        {
            //Dont do shit, if other Task is running.
            if (IsProcessing == true)
            {
                return;
            }

            IsProcessing = true;
            FilesToProcess = Tracks.Count;
            FilesProcessed = 0;

            await Task.Run(() =>
            {
                foreach (Track track in Tracks)
                {
                    track.WriteDataToFile();
                    FilesProcessed++;
                }
            });

            IsProcessing = false;
        }
        


        public void ExecuteChooseFile()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";

            Track track;
            if (openFileDialog.ShowDialog() == true)
            {
                var uri = new Uri(openFileDialog.FileName);
                if (uri != null)
                {
                    try
                    {
                        track = new Track(uri);
                    }
                    catch (Exception)
                    {
                        track = null;
                        FailedUris.Add(uri);
                    }

                    if (track != null)
                    {
                        TracksToAdd.Add(track);
                    }
                }
            }
        }

        public async void ExecuteChooseFolder()
        {
            //Dont do shit, if other Task is running.
            if (IsProcessing == true)
            {
                return;
            }

            IsProcessing = true;

            OpenFolderDialog openFolderDialog = new OpenFolderDialog();
            List<Uri> failedUriList = new List<Uri>();
            List<Track> tracksList = new List<Track>();

            if (openFolderDialog.ShowDialog() == true)
            {
                var path = openFolderDialog.FolderName;

                Directory.SetCurrentDirectory(path);
                string[] filepaths = Directory.GetFiles(path, "*.mp3", SearchOption.AllDirectories);

                FilesToProcess = filepaths.Length;
                FilesProcessed = 0;


                await Task.Run(() =>
                {

                    foreach (string filepath in filepaths)
                    {
                        Uri uri = new Uri(filepath);
                        Track track = null;
                        if (uri != null)
                        {
                            try
                            {
                                track = new Track(uri);
                            }
                            catch (Exception)
                            {
                                track = null;
                                failedUriList.Add(uri);
                            }

                            if (track != null)
                            {
                                tracksList.Add(track);
                            }
                        }
                        FilesProcessed++;
                    }
                });
            }
            foreach(Track track in tracksList)
            {
                TracksToAdd.Add(track);
            }

            foreach(Uri uri in failedUriList)
            {
                FailedUris.Add(uri);
            }

            IsProcessing = false;
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
                Player.Play(true);
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

        private async void ExecuteAddToDatabase()
        {
            //Dont do shit, if other Task is running.
            if(IsProcessing == true)
            {
                return;
            }

            IsProcessing = true;
            FilesToProcess = TracksToAdd.Count;
            FilesProcessed = 0;

            await Task.Run(() =>
            {
                foreach (var trackToAdd in TracksToAdd)
                {
                    DatabaseUtility.AddToDatabase(trackToAdd);
                    FilesProcessed++;
                }
            });

            IsProcessing = false;
            TracksToAdd.Clear();
            ExecuteResetTrackFilterClick();
        }

        public void ExecuteResetTrackFilterClick()
        {
            TrackFilterString = string.Empty;
            SelectedDance = null;
            ExecuteApplyTrackFilter();
        }

        public void ExecuteApplyTrackFilter()
        {
            if (SelectedDance != null)
            {
                FilteredTracks = new ObservableCollection<Track>(Tracks.Where(X => X.Dance == SelectedDance));
            }
            else
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

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Player.Stop();
        }
    }
}
