using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{
    internal class AudioDeviceSelectViewModel : NotifyObject
    {
        public Action CloseAction { get; set; }

        public Player Player { get; set; }

        public AudioDeviceSelectViewModel(Player player) 
        {
            CreateCommands();
            Player = player;
        }


        public ICommand ConfirmAudioDeviceCommand { get; private set; }
        public ICommand SkipAudioDeviceCommand { get; private set; }

        public void CreateCommands()
        {
            ConfirmAudioDeviceCommand = new RelayCommand(ExecuteConfirmAudioDevice);
            SkipAudioDeviceCommand = new RelayCommand(ExecuteSkipAudioDevice);
        }

        public void ExecuteConfirmAudioDevice()
        {
            if (Player.SelectedAudioDevice != null && !Player.SelectedAudioDevice.IsDefault)
            {
                CloseAction();
            }
        }

        public void ExecuteSkipAudioDevice()
        {
            CloseAction();
        }
    }
}
