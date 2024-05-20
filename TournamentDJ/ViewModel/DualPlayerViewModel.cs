using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TournamentDJ.Essentials;

namespace TournamentDJ.ViewModel
{
    class DualPlayerViewModel : NotifyObject
    {

        public DualPlayerViewModel() 
        { 
            LeftPlayerViewModel = new PlayerViewModel();
            RightPlayerViewModel = new PlayerViewModel();
            Crossfade = 0.5;
        }

        public PlayerViewModel LeftPlayerViewModel
        {
            get { return Get<PlayerViewModel>(); }
            set { Set(value); }
        }


        public PlayerViewModel RightPlayerViewModel
        {
            get { return Get<PlayerViewModel>(); }
            set { Set(value); }
        }

        public double Crossfade
        {
            get { return Get<double>(); } 
            set 
            { 
                Set(value);
                double leftActualVolume = (LeftPlayerVolume * (1.0 - value)) * 2;
                double rightActualVolume = (RightPlayerVolume * value) * 2;
                if (leftActualVolume >= 1.0)
                {
                    leftActualVolume = 1.0;
                }
                if (rightActualVolume >= 1.0)
                {
                    rightActualVolume = 1.0;
                }
                LeftPlayerViewModel.Player.MedPlayer.Volume = leftActualVolume;
                RightPlayerViewModel.Player.MedPlayer.Volume = rightActualVolume;
            }
        }

        public double LeftPlayerVolume
        {
            get { return Get<double>(); }
            set
            {
                Set((double)value);
                //Make sure values are updated
                Crossfade = Crossfade;
            }
        }

        public double RightPlayerVolume
        {
            get { return Get<double>(); }
            set
            {
                Set((double)value);
                //Make sure values are updated
                Crossfade = Crossfade;
            }
        }
    }
}
