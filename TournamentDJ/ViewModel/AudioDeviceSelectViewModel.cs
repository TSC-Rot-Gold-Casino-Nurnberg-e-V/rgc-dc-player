using System.Collections.ObjectModel;
using System.Windows;
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
            Languages = new ObservableCollection<Uri>();
            Languages.Add(new Uri("pack://application:,,,/TournamentDJ;component/Localization/StringResources.xaml"));
            Languages.Add(new Uri("pack://application:,,,/TournamentDJ;component/Localization/StringResources.de-DE.xaml"));

            SelectedLanguage = Languages.First();
        }

        public ObservableCollection<Uri> Languages { get; set; }

        public Uri SelectedLanguage {
            get 
            { 
                return Get<Uri>(); 
            }
            set 
            { 
                Set(value);
                var app = (App) Application.Current;
                app.ChangeLocalization(value);
            }
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
