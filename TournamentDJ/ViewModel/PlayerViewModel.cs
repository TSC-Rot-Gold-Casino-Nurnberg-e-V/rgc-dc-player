﻿using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using TagLib.Mpeg;
using TournamentDJ.Essentials;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    internal class PlayerViewModel : NotifyObject
    {
        public PlayerViewModel() 
        {
            Player = new Player();
            TracksToPlay = new TrackList();
            TracksToPlay.AddTrack(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\01 Concerning Hobbits [SW].mp3"));
            TracksToPlay.AddTrack(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\02 Promise [SW].mp3"));
            TracksToPlay.AddTrack(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\03 Dark Waltz [SW].mp3"));
            TracksToPlay.AddTrack(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\01 Concerning Hobbits [SW].mp3"));
            TracksToPlay.AddTrack(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\02 Promise [SW].mp3"));
            TracksToPlay.AddTrack(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\03 Dark Waltz [SW].mp3"));
            TrackPlaying = new Track(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\03 Dark Waltz [SW].mp3"));
            CreateCommands();
        }

        public Player Player
        {
            get { return Get<Player>(); }
            set { Set(value); }
        }

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
                OnPropertyChanged(); ;
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

        public DanceRound SelectedDanceRound
        {
            get { return Get<DanceRound>(); }
            set { Set(value); }
        }

        public ICommand PlayClickCommand { get; private set; }
        public ICommand FadeoutClickCommand { get; private set; }
        public ICommand StopClickCommand { get; private set; }
        public ICommand OpenFileCommand { get; private set; }
        public ICommand ReselectClickCommand { get; private set; }

        public ICommand StartAutplayClickCommand { get; private set; }
        public ICommand CreateDanceRoundClickCommand { get; private set; }

        public void CreateCommands()
        {
            PlayClickCommand = new RelayCommand(ExecutePlay);
            FadeoutClickCommand = new RelayCommand(ExecuteFadeout);
            StopClickCommand = new RelayCommand(ExecuteStop);
            OpenFileCommand = new RelayCommand(ExecuteOpenFile);
            ReselectClickCommand = new RelayCommand<Track>(ExecuteReselect);  
            StartAutplayClickCommand = new RelayCommand(ExecuteStartAutoplay);
            CreateDanceRoundClickCommand = new RelayCommand(ExecuteCreateDanceRound);
        }


        public void ExecutePlay()
        {
            Player.Play();
        }

        public void ExecuteFadeout()
        {
            Player.Fadeout();
        }

        public void ExecuteStop()
        {
            Player.Stop();
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

        public void ExecuteReselect(Track track)
        {
            Player.Reselect(track);
        }

        public void ExecuteStartAutoplay()
        {
            Player.StartAutoPlay();
        }

        public virtual void ExecuteCreateDanceRound()
        {
            TracksToPlay = TrackListBuilder.CreateDanceRound(SelectedDanceRound);
        }


    }
}

