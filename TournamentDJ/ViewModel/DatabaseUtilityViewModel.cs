using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;
using System.IO;
using Windows.Security.Isolation;
using System.Diagnostics;
using TournamentDJ.Deduplication;
using System.Windows.Threading;

namespace TournamentDJ.ViewModel
{

    class DatabaseUtilityViewModel : NotifyObject
    {
        public bool isPlaying = false;
        public bool playOnClick = false;

        #region Constructor
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
        #endregion

        #region Properties
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

        public Dictionary<int, string> Ratings
        {
            get { return Track.Ratings; }
            private set { Track.Ratings = value; }
        }

        #endregion

        #region Methods
        private async Task LoadFingerprints()
        {
            Logger.LoggerInstance.LogWrite("Loading Fingerprints");
            FilesToProcess = Tracks.Count;
            FilesProcessed = 0;
            await Task.Run(() =>
            {
                foreach (Track track in Tracks)
                {
                    FingerprintBase.AddFingerprintToModel(track);
                    FilesProcessed++;
                }
            });
        }

        private async Task AddTracks(IEnumerable<Track> computedTracks)
        {
            Logger.LoggerInstance.LogWrite("Adding Tracks to existing Database");

            FilesToProcess = TracksToAdd.Count;
            FilesProcessed = 0;
            await Task.Run(() =>
            {
                foreach (var trackToAdd in computedTracks)
                {
                    DatabaseUtility.AddToDatabase(trackToAdd);
                    FilesProcessed++;
                }
            });
            return;
        }
        #endregion

        #region Commands
        public ICommand ChooseFolderCommand { get; private set; }
        public ICommand SaveDataCommand { get; private set; }

        public ICommand AddToDatabaseCommand { get; private set; }

        public ICommand PlayPauseCommand { get; private set; }
        public ICommand TogglePlayOnClickCommand { get; private set; }
        public ICommand ResetTrackFilterClickCommand { get; private set; }
        public ICommand ApplyTrackFilterClickCommand { get; private set; }
        public ICommand ChooseFileCommand { get; private set; }
        public ICommand ExportFileDataCommand { get; private set; }
        public ICommand UpdateFileDataCommand { get; private set; }

        public ICommand AddToDatabaseMultithread { get; private set; }

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
            UpdateFileDataCommand = new RelayCommand(ExecuteUpdateFileData);
        }


        /// <summary>
        /// Adds all tracks  in TracksToAdd to Database
        /// </summary>
        private async void ExecuteAddToDatabase()
        {
            //Dont do shit, if other Task is running.
            if (IsProcessing == true)
            {
                return;
            }

            IsProcessing = true;

            FingerprintBase.ClearModel(); //Make sure Model is reset before computing new Tracks

            await LoadFingerprints();

            Logger.LoggerInstance.LogWrite("Computing fingerprints. This can take some time");
            var computedTracks = await FingerprintBase.CreateFingerprints(TracksToAdd);

            await AddTracks(computedTracks);
           
            TracksToAdd.Clear();
            FingerprintBase.ClearModel(); // Clear Model to free some space in Memory
            ExecuteResetTrackFilterClick();
            IsProcessing = false;
        }

        /// <summary>
        /// Takes each track und writes the data that is currently in the database in each file that corresponds to that track
        /// </summary>
        private async void ExecuteExportFileData()
        {
            //Dont do shit, if other Task is running.
            if (IsProcessing == true)
            {
                return;
            }

            IsProcessing = true;
            FilesToProcess = Tracks.Count;
            FilesProcessed = 0;
            int filesUpdated = 0;

            await Task.Run(() =>
            {
                foreach (Track track in Tracks)
                {
                    if (track.WriteDataToFile())
                    {
                        filesUpdated++;
                    };
                    FilesProcessed++;
                }
            });

            Logger.LoggerInstance.LogWrite("Processed " + FilesProcessed + " Tracks and updated " + filesUpdated);
            IsProcessing = false;
        }


        /// <summary>
        /// Reads data from every track and corresponding file and updates the values stored in the Database accordingly
        /// </summary>
        private async void ExecuteUpdateFileData()
        {
            //Dont do shit, if other Task is running.
            if (IsProcessing == true)
            {
                return;
            }

            IsProcessing = true;
            FilesToProcess = Tracks.Count;
            FilesProcessed = 0;
            int filesUpdated = 0;

            await Task.Run(() =>
            {
                foreach (Track track in Tracks)
                {
                    if (track.UpdateDataInDatabase())
                    {
                        filesUpdated++;
                    };
                    FilesProcessed++;
                }
            });

            Logger.LoggerInstance.LogWrite("Processed " + FilesProcessed + " Tracks and updated " + filesUpdated);
            IsProcessing = false;
        }


        /// <summary>
        /// Opens a file dialog to add a single file to TracksToAdd
        /// </summary>
        private void ExecuteChooseFile()
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

        /// <summary>
        /// Opens a file Dialog to get all files from a folder-structure
        /// </summary>
        private async void ExecuteChooseFolder()
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

        /// <summary>
        /// Toggle play/pause
        /// </summary>
        private void ExecutePlayPause()
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

        /// <summary>
        /// Activates play on click
        /// </summary>
        private void ExecuteTogglePlayOnClick()
        {
            playOnClick = !playOnClick;
        }

        /// <summary>
        /// Save all data to database
        /// </summary>
        private void ExecuteSaveData()
        {
            DatabaseUtility.SaveChanges();
        }

        /// <summary>
        /// Resets the current track filter back to defaults (no filter)
        /// </summary>
        private void ExecuteResetTrackFilterClick()
        {
            TrackFilterString = string.Empty;
            SelectedDance = null;
            ExecuteApplyTrackFilter();
        }

        /// <summary>
        /// Applies the selected track filter to the track selection and updates FilteredTracks
        /// </summary>
        private void ExecuteApplyTrackFilter()
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
        #endregion

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            Player.Stop();
        }
    }
}
