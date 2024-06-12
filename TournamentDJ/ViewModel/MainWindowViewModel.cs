using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows;
using TournamentDJ.Essentials;
using TournamentDJ.Model;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;


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

        public Logger logger { get { return Logger.LoggerInstance; } }

        public DatabaseUtility dbUtil = new DatabaseUtility();


        public MainWindowViewModel()
        {
            TournamentPlayerViewModel = new TournamentPlayerViewModel();
            WarmupPlayerViewModel = new WarmupPlayerViewModel();
            DualPlayerViewModel = new DualPlayerViewModel();
            CreateCommands();
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