using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentDJ.Model
{
    class TouchPlayer : Player
    {

        public TrackList SelectedSecondaryTrackList
        {
            get { return Get<TrackList>(); }
            set { Set(value); }
        }

        public override async void Fadeout(double duration = 5.0)
        {
            isFading = true;

            int tickrate = 10; //Ticks per second
            int targetTicks = (int)(duration * tickrate);
            double startingVolume = this.MedPlayer.Volume;
            double decrement = startingVolume / (duration * tickrate);
            for (int i = 0; i < targetTicks; i++)
            {
                double newVolume = MedPlayer.Volume - decrement;
                MedPlayer.Volume = newVolume;
                await Task.Delay(1000 / tickrate);
            }

            MedPlayer.Pause();
            MedPlayer.PlaybackSession.Position = TimeSpan.Zero;
            MedPlayer.Volume = startingVolume;

            isFading = false;

            
        }

        public virtual async void Fadein(double duration = 3.0)
        {
            isFading = true;

            int tickrate = 10; //Ticks per second
            int targetTicks = (int)(duration * tickrate);
            double endVolume = MedPlayer.Volume;
            double increment = endVolume / (duration * tickrate);
            MedPlayer.Volume = 0.0;

            Play();

            for (int i = 0; i < targetTicks; i++)
            {
                double newVolume = MedPlayer.Volume + increment;
                MedPlayer.Volume = newVolume;
                await Task.Delay(1000 / tickrate);
            }
            isFading = false;
        }


        public override void GetNextTrack()
        {
            return;
        }

        public void StopAutoplay()
        {
            MedPlayer.AutoPlay = false;
        }

    }
}
