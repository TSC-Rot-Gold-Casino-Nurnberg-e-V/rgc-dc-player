using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Windows;
using System.Windows.Input;
using TournamentDJ.Audio;
using TournamentDJ.Essentials;
using TournamentDJ.Model;


namespace TournamentDJ.ViewModel
{
    internal class MainWindowViewModel : NotifyObject
    {
        public TournamentPlayerViewModel TournamentPlayerViewModel
        {
            get { return Get<TournamentPlayerViewModel>(); }
            set { Set(value); }
        }

        public PlayerViewModel WarmupPlayerViewModel
        {
            get { return Get<PlayerViewModel>(); }
            set { Set(value); }
        }

        public TouchPlayerViewModel TouchPlayerViewModel
        {
            get { return Get<TouchPlayerViewModel>(); }
            set { Set(value); }
        }

        public DualPlayerViewModel DualPlayerViewModel
        {
            get { return Get<DualPlayerViewModel>(); }
            set { Set(value); }
        }

        public ObservableCollection<string> AvailableLanguages
        {
            get { return Get<ObservableCollection<string>>(); }
            set { Set(value); }
        }

        public bool AdvancedModeActive {
            get
            {
                return Get<bool>();
            } 
            set 
            {
                Set(value);
                AdvancedModeNotActive = !value;
                OnPropertyChanged(); 
            }
        }

        public bool AdvancedModeNotActive
        {
            get
            {
                return Get<bool>();
            }
            private set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        public Logger logger { get { return Logger.LoggerInstance; } }

        public DatabaseUtility dbUtil = new DatabaseUtility();


        public MainWindowViewModel()
        {
            FingerprintBase.Init();
            TournamentPlayerViewModel = new TournamentPlayerViewModel();
            WarmupPlayerViewModel = new WarmupPlayerViewModel();
            DualPlayerViewModel = new DualPlayerViewModel();
            TouchPlayerViewModel = new TouchPlayerViewModel();
            CreateCommands();
            GetModeConfig();
        }

        /// <summary>
        /// Reads the mode config from the config file and applies it to the Main-Window
        /// </summary>
        private void GetModeConfig()
        {
            string? mode = ConfigurationManager.AppSettings["StartupMode"];
            if (mode != null)
            {
                switch (mode)
                {
                    case "simple":
                        AdvancedModeActive = false;
                    break;
                    case "advanced":
                        AdvancedModeNotActive = true;
                        break;
                    default:
                        AdvancedModeActive = false;
                    break;
                }
            }
        }

        /// <summary>
        /// Example for how to update a Setting in the config file
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void UpdateSettings(string key, string value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        public ICommand OpenDatabaseUtilityCommand { get; private set; }

        public void CreateCommands()
        {
            OpenDatabaseUtilityCommand = new RelayCommand(ExecuteOpenDatabaseUtility);
        }

        public void ExecuteOpenDatabaseUtility()
        {
            var databaseUtilityWindow = new DatabaseUtilityWindow();
            databaseUtilityWindow.ShowDialog();
        }

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
            DatabaseUtility.SaveChanges();
        }

        public void WindowOpened(object sender, RoutedEventArgs e)
        {
            var ChooseAudioDeviceWindow = new AudioDeviceSelectWindow(TournamentPlayerViewModel.Player);
            ChooseAudioDeviceWindow.ShowDialog();
        }
    }

}