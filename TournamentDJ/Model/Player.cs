﻿using System.Collections.ObjectModel;
using System.Windows.Threading;
using TournamentDJ.Essentials;
using Windows.Devices.Enumeration;
using Windows.Media.Core;
using Windows.Media.Playback;
using System.IO;

namespace TournamentDJ.Model
{
    class Player : NotifyObject
    {
        Random rand;
        public bool isFading = false;
        public Player()
        {
            AudioDevices = new ObservableCollection<DeviceInformation>();
            FindAudioDevices();
            MedPlayer = new MediaPlayer();
            MedPlayer.CurrentStateChanged += MedPlayerHasChanged;
            Timer = new DispatcherTimer();
            TracksToPlay = new TrackList();
            TracksPlayed = new TrackList();
            SelectedTrackList = null;
            Timer.Interval = TimeSpan.FromSeconds(1);
            Timer.Tick += timer_Tick;
            Timer.Start();
            rand = new Random(Guid.NewGuid().GetHashCode());
            MedPlayer.MediaEnded += MediaEnded;
            MedPlayer.MediaFailed += MedPlayer_MediaFailed;
            MedPlayer.AutoPlay = false;
        }

        public virtual void MediaEnded(MediaPlayer sender, object args)
        {
            if(MedPlayer.AutoPlay == false)
            {
                App.Current.Dispatcher.Invoke(delegate
                {
                    Stop();
                });
            }
            else
            {
                App.Current.Dispatcher.Invoke(delegate
                {
                    GetNextTrack();
                });
            }

        }

        public virtual void MedPlayerHasChanged(MediaPlayer sender, object args)
        {
            IsPlaying = MedPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Playing;
        }

        public async Task FindAudioDevices()
        {
            AudioDevices.Clear();
            var deviceFinder = DeviceInformation.FindAllAsync(DeviceClass.AudioRender);
            while (deviceFinder.Status != Windows.Foundation.AsyncStatus.Completed)
            {
                await Task.Delay(100);
            }
            DeviceInformationCollection devices = deviceFinder.GetResults();
            foreach (var device in devices)
            {
                AudioDevices.Add(device);
                if (SelectedAudioDevice == null && device.IsDefault)
                {
                    SelectedAudioDevice = device;
                }
            }
        }

        private void MedPlayer_MediaFailed(MediaPlayer sender, object args)
        {
            Logger.LoggerInstance.LogWrite("Media Failed");
            //TODO Add Handling if first URI fails -> It is unkown, when exactly MediaFailed can occur
            MedPlayer.Source = null;
            //throw new NotImplementedException();
        }

        public String timeData
        {
            get { return Get<String>(); }
            set
            {
                Set(value);
            }
        }

        public ObservableCollection<DeviceInformation> AudioDevices
        {
            get { return Get<ObservableCollection<DeviceInformation>>(); }
            set
            {
                Set(value);
            }
        }


        public TrackList TracksToPlay
        {
            get { return Get<TrackList>(); }
            set
            {
                Set(value);
                if (TracksToPlay != null && TracksToPlay.Tracks.Count > 0)
                {
                    GetNextTrack();
                }
                OnPropertyChanged();
            }
        }

        public TrackList TracksPlayed
        {
            get { return Get<TrackList>(); }
            set { Set(value); }
        }
        public Track SelectedSpecificTrack
        {
            get { return Get<Track>(); }
            set { Set(value); }
        }

        public ObservableCollection<Track> TracksToSelectFrom
        {
            get { return Get<ObservableCollection<Track>>(); }
            set { Set(value); }
        }

        public DanceRound SelectedDanceRound
        {
            get { return Get<DanceRound>(); }
            set { Set(value); }
        }

        public TrackList SelectedTrackList
        {
            get { return Get<TrackList>(); }
            set { Set(value); }
        }


        public bool IsPlaying
        {
            get { return Get<bool>(); }
            private set { Set(value); }
        }

        public DeviceInformation SelectedAudioDevice
        {
            get { return Get<DeviceInformation>(); }
            set
            {
                Set(value);
                try
                {
                    MedPlayer.AudioDevice = value;
                }
                //If device could not be loaded, reload available audio devices
                catch (Exception)
                {
                    foreach (DeviceInformation device in AudioDevices)
                    {
                        if (device.IsDefault)
                        {
                            MedPlayer.AudioDevice = device;
                            break;
                        }
                    }
                    FindAudioDevices();
                }
            }
        }

        public Track TrackPlaying
        {
            get { return Get<Track>(); }
            set
            {
                Set(value);
                //Make sure, the Player opens the newly set Track
                if (value != null)
                {
                    //Try for at least one correct Path.
                    foreach(Uri uri in value.Uris)
                    {
                        if(File.Exists(uri.LocalPath))
                        {
                            OpenFile(uri);
                        }
                    }
                }
            }
        }


        public TimeSpan SelectedTimeSpan
        {
            get { return Get<TimeSpan>(); }
            set { Set(value); }
        }

        public bool IsSingleSelected
        {
            get { return Get<bool>(); }
            set
            {
                Set(value);
            }
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if (MedPlayer.Source != null)
            {
                if (MedPlayer.PlaybackSession.Position >= SelectedTimeSpan && SelectedTimeSpan != TimeSpan.Zero && !isFading)
                {
                    Fadeout();
                }

                if (MedPlayer.PlaybackSession.NaturalDuration.TotalMilliseconds > 1.0)
                {
                    timeData = String.Format("{0} / {1}", MedPlayer.PlaybackSession.Position.ToString(@"mm\:ss"), MedPlayer.NaturalDuration.ToString(@"mm\:ss"));
                }
            }
            else
            {
                timeData = "No file selected...";
            }

        }

        public MediaPlayer MedPlayer
        {
            get { return Get<MediaPlayer>(); }
            set { Set(value); }
        }

        public DispatcherTimer Timer
        {
            get { return Get<DispatcherTimer>(); }
            set { Set(value); }
        }


        public void OpenFile(Uri newUri)
        {

            MedPlayer.Source = MediaSource.CreateFromUri(newUri);
        }

        public async void Play(bool overrideLastPlayed = false, bool increasePlayCount = false)
        {
            while (MedPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Buffering || MedPlayer.PlaybackSession.PlaybackState == MediaPlaybackState.Opening)
            {
                await Task.Delay(100);
            }
            MedPlayer.Play();

            if (!overrideLastPlayed && TrackPlaying != null)
            {
                TrackPlaying.LastPlayedTime = DateTime.Now;
            }

            if (increasePlayCount  && TrackPlaying != null) 
            {
                TrackPlaying.PlayCount++;
            }
        }

        public void Stop()
        {
            MedPlayer.AutoPlay = false;
            double volume = MedPlayer.Volume;
            MedPlayer.Pause();
            MedPlayer.PlaybackSession.Position = TimeSpan.Zero;
            GetNextTrack();
            MedPlayer.Volume = volume;
        }


        public virtual async void Fadeout(double duration = 5.0)
        {
            isFading = true;

            int tickrate = 10; //Ticks per second
            double startingVolume = MedPlayer.Volume;
            double decrement = startingVolume / (duration * tickrate);
            while (MedPlayer.Volume > 0.0)
            {
                double newVolume = MedPlayer.Volume - decrement;
                if (newVolume < 0.0)
                {
                    newVolume = 0.0;
                }
                MedPlayer.Volume = newVolume;
                await Task.Delay(1000 / tickrate);
            }

            MedPlayer.Pause();
            GetNextTrack();
            MedPlayer.PlaybackSession.Position = TimeSpan.Zero;
            MedPlayer.Volume = startingVolume;

            isFading = false;
        }

        public void Reselect(Track track, DanceRound round = null, bool notFavourite = false, bool overrideParams = false, bool onlyUseUncategorized = false)
        {
            if (track == null) return;

            if (TrackPlaying == track && MedPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Playing && !IsSingleSelected)
            {

                if (round == null)
                {
                    TrackPlaying = TrackListBuilder.GetRandomTrack(TrackPlaying.Dance, SelectedTrackList, cantBeFavourite: notFavourite, overrideParams: overrideParams, onlyUseUncategorized: onlyUseUncategorized);
                }
                else
                {
                    TrackPlaying = TrackListBuilder.GetRandomTrack(TrackPlaying.Dance, SelectedTrackList, round.MinDifficulty, round.MaxDifficulty, round.MinCharacteristics, cantBeFavourite: notFavourite, overrideParams: overrideParams, onlyUseUncategorized: onlyUseUncategorized);
                }
                return;
            }


            if (IsSingleSelected)
            {
                Track newTrack;
                if (round == null)
                {
                    newTrack = TrackListBuilder.GetRandomTrack(track.Dance, SelectedTrackList, cantBeFavourite: notFavourite, overrideParams: overrideParams, onlyUseUncategorized: onlyUseUncategorized);
                }
                else
                {
                    newTrack = TrackListBuilder.GetRandomTrack(track.Dance, SelectedTrackList, round.MinDifficulty, round.MaxDifficulty, round.MinCharacteristics, cantBeFavourite: notFavourite, overrideParams: overrideParams, onlyUseUncategorized: onlyUseUncategorized);
                }

                for (int i = 0; i < TracksToPlay.Tracks.Count; i++)
                {
                    if (TracksToPlay.Tracks[i] == track)
                    {
                        TracksToPlay.Tracks[i] = newTrack;
                    }
                }
                if (TrackPlaying == track && MedPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
                {
                    TrackPlaying = newTrack;
                }
                return;
            }

            int indexToReselect = TracksToPlay.Tracks.IndexOf(track);

            //No file selected (improper Binding?)
            if (indexToReselect < 0)
            {
                return;
            }

            else if (round == null)
            {
                TracksToPlay.Tracks[indexToReselect] = TrackListBuilder.GetRandomTrack(TracksToPlay.Tracks[indexToReselect].Dance, SelectedTrackList, cantBeFavourite: notFavourite);
            }

            else
            {
                TracksToPlay.Tracks[indexToReselect] = TrackListBuilder.GetRandomTrack(TracksToPlay.Tracks[indexToReselect].Dance, SelectedTrackList, round.MinDifficulty, round.MaxDifficulty, round.MinCharacteristics, cantBeFavourite: notFavourite);
            }
        }


        public virtual void GetNextTrack()
        {
            MedPlayer.Pause();

            //Get Next Song
            if (TrackPlaying != null && TracksToPlay != null && TracksToPlay.Tracks != null && TracksToPlay.Tracks.Count > 0)
            {
                TracksPlayed.Tracks.Add(TrackPlaying);
            }

            //Get First Track when loading new list
            if (TracksToPlay != null && TracksToPlay.Tracks != null && TracksToPlay.Tracks.Count > 0)
            {
                TrackPlaying = TracksToPlay.Tracks[0];
                TracksToPlay.Tracks.RemoveAt(0);
            }
        }

        public void GetPreviousTrack()
        {
            MedPlayer.Pause();

            if (TrackPlaying != null && TracksPlayed.Tracks.Count > 0)
            {
                TracksToPlay.Tracks.Insert(0, TrackPlaying);
            }

            if (TracksToPlay != null && TracksPlayed.Tracks.Count > 0)
            {
                TrackPlaying = TracksPlayed.Tracks[TracksPlayed.Tracks.Count - 1];
                TracksPlayed.Tracks.RemoveAt(TracksPlayed.Tracks.Count - 1);
            }
        }

        public void StartAutoPlay()
        {
            MedPlayer.AutoPlay = true;
            Play();
        }

        private Track TrackToSelectSpecific { get; set; }
        public void CreateTracksToSelectFrom(Track track)
        {
            TrackToSelectSpecific = track;
            if(track != null)
            {
                TracksToSelectFrom = new ObservableCollection<Track>(DatabaseUtility.Tracks.Where(X => X.Dance == track.Dance && !X.FlaggedForReview));
            }
        }

        public void SelectSpecificTrack()
        {
            var track = TrackToSelectSpecific;
            if (SelectedSpecificTrack != null && TrackPlaying == track && MedPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Playing)
            {
                TrackPlaying = SelectedSpecificTrack;
            }

            if (SelectedSpecificTrack != null && TracksToPlay != null && track != null)
            {
                int indexToReselect = TracksToPlay.Tracks.IndexOf(track);
                TracksToPlay.Tracks[indexToReselect] = SelectedSpecificTrack;
            }
        }
    }
}
