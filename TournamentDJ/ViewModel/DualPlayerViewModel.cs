using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TournamentDJ.Essentials;

namespace TournamentDJ.ViewModel
{
    class DualPlayerViewModel : NotifyObject
    {

        public DualPlayerViewModel() 
        { 
            LeftPlayerViewModel = new BasicPlayerViewModel();
            RightPlayerViewModel = new BasicPlayerViewModel();
            Crossfade = 0.5;
            CreateCommands();
        }

        public BasicPlayerViewModel LeftPlayerViewModel
        {
            get { return Get<BasicPlayerViewModel>(); }
            set { Set(value); }
        }


        public BasicPlayerViewModel RightPlayerViewModel
        {
            get { return Get<BasicPlayerViewModel>(); }
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
                OnPropertyChanged();
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
        public ICommand ResetCrossfade { get; private set; }

        public void CreateCommands()
        {
            ResetCrossfade = new RelayCommand(ExecuteResetCrossfade);
        }


        public void ExecuteResetCrossfade()
        {
            Crossfade = 0.5;
        }
    }
}
