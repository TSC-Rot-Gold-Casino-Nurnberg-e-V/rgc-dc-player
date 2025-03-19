using Microsoft.Win32;
using System.Collections.ObjectModel;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    internal class PlayerViewModel : NotifyObject
    {
        public PlayerViewModel()
        {
            Player = new Player();
            CreateCommands();
            //UseTracklist = false;
        }

        public Player Player
        {
            get { return Get<Player>(); }
            set { Set(value); }
        }

        SelectSpecificTrackWindow SelectSpecificTrackWindow { get; set; }

        public ObservableCollection<TimeSpan> Runtimes
        {
            get { return Get<ObservableCollection<TimeSpan>>(); }
            set { Set(value); }
        }

        public TrackList TracksToPlay
        {
            get { return Player.TracksToPlay; }
            set
            {
                Player.TracksToPlay = value;
                OnPropertyChanged();
            }
        }

        public Track TrackPlaying
        {
            get { return Player.TrackPlaying; }
            set
            {
                if (value.Equals(Player.TrackPlaying)) return;
                Player.TrackPlaying = value;
                OnPropertyChanged();
            }
        }

        public Track SelectedSpecificTrack
        {
            get { return Player.SelectedSpecificTrack; }
            set { Player.SelectedSpecificTrack = value; }
        }

        public ObservableCollection<Track> TracksToSelectFrom
        {
            get { return Player.TracksToSelectFrom; }
            set { Player.TracksToSelectFrom = value; }
        }

        public string TrackFilterString
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                //ApplySelectSpecificTrackFilter();
            }
        }


        public TimeSpan SelectedTimeSpan
        {
            get { return Player.SelectedTimeSpan; }
            set
            {
                if (value.Equals(Player.SelectedTimeSpan)) return;
                Player.SelectedTimeSpan = value;
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

        public ObservableCollection<TrackList> TrackLists
        {
            get { return DatabaseUtility.TrackLists; }
            set
            {
                DatabaseUtility.TrackLists = value;
                OnPropertyChanged();
            }
        }

        public virtual TrackList SelectedTrackList
        {
            get { return Player.SelectedTrackList; }
            set
            {
                Player.SelectedTrackList = value;
                ExecuteCreateDanceRound();
            }
        }

        public bool UseTracklist
        {
            get { return Get<bool>(); }
            set
            {
                Set(value);
                if (value == false)
                {
                    SelectedTrackList = null;
                };
            }
        }

        public DanceRound SelectedDanceRound
        {
            get { return Player.SelectedDanceRound; }
            set
            {
                Player.SelectedDanceRound = value;
                ExecuteCreateDanceRound();
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

        public ICommand PlayClickCommand { get; private set; }
        public ICommand FadeoutClickCommand { get; private set; }
        public ICommand StopClickCommand { get; private set; }
        public ICommand NextClickCommand { get; private set; }
        public ICommand PreviousClickCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand ReselectClickCommand { get; private set; }
        public ICommand SetTrackPlaying { get; private set; }

        public ICommand StartAutplayClickCommand { get; private set; }
        public ICommand CreateDanceRoundClickCommand { get; private set; }

        public ICommand SelectSpecificTrackCommand { get; private set; }
        public ICommand OpenSelectSpecificTrackWindowCommand { get; private set; }

        public void CreateCommands()
        {
            PlayClickCommand = new RelayCommand(ExecutePlay);
            FadeoutClickCommand = new RelayCommand(ExecuteFadeout);
            StopClickCommand = new RelayCommand(ExecuteStop);
            OpenFileCommand = new RelayCommand(ExecuteOpenFile);
            ReselectClickCommand = new RelayCommand<Track>(ExecuteReselect);
            StartAutplayClickCommand = new RelayCommand(ExecuteStartAutoplay);
            CreateDanceRoundClickCommand = new RelayCommand(ExecuteCreateDanceRound);
            NextClickCommand = new RelayCommand(ExecuteNext);
            PreviousClickCommand = new RelayCommand(ExecutePrevious);
            SetTrackPlaying = new RelayCommand<Track>(ExecuteSetTrackPlaying);
            SelectSpecificTrackCommand = new RelayCommand(ExecuteSelectSpecificTrack);
            OpenSelectSpecificTrackWindowCommand = new RelayCommand<Track>(ExecuteOpenSelectSpecificTrackWindow);
        }


        public virtual void ExecutePlay()
        {
            Player.Play();
        }

        public virtual void ExecuteFadeout()
        {
            Player.Fadeout();
        }

        public void ExecuteStop()
        {
            Player.Stop();
        }

        public void ExecuteNext()
        {
            Player.GetNextTrack();
        }

        public void ExecutePrevious()
        {
            Player.GetPreviousTrack();
        }

        public void ExecuteOpenFile()
        {
            if (Player.MedPlayer.PlaybackSession.PlaybackState == Windows.Media.Playback.MediaPlaybackState.Playing)
            {
                return;
            }
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "MP3 files (*.mp3)|*.mp3|All files (*.*)|*.*";
            if (openFileDialog.ShowDialog() == true)
            {
                var newUri = new Uri(openFileDialog.FileName);
                TrackPlaying = new Track(newUri);
            }
        }

        public virtual void ExecuteReselect(Track track)
        {
            Player.Reselect(track, SelectedDanceRound);
        }

        public void ExecuteStartAutoplay()
        {
            Player.StartAutoPlay();
        }

        public void ExecuteSetTrackPlaying(Track track)
        {
            TrackPlaying = track;
        }

        public virtual void ExecuteCreateDanceRound()
        {
            Player.TracksPlayed.Tracks.Clear();
            TracksToPlay = TrackListBuilder.CreateDanceRound(SelectedDanceRound, tracklist: SelectedTrackList);
        }





        public void ExecuteOpenSelectSpecificTrackWindow(Track track)
        {

            Player.CreateTracksToSelectFrom(track);
            SelectSpecificTrackWindow = new SelectSpecificTrackWindow();
            SelectSpecificTrackWindow.DataContext = this;
            SelectSpecificTrackWindow.ShowDialog();
        }

        public void ExecuteSelectSpecificTrack()
        {
            Player.SelectSpecificTrack();
            SelectSpecificTrackWindow.Close();
        }
    }
}

