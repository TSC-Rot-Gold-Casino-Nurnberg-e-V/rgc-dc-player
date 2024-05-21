using System.Windows;
using TournamentDJ.ViewModel;
using TournamentDJ.Model;

namespace TournamentDJ
{
    partial class AudioDeviceSelectWindow : Window
    {
        internal AudioDeviceSelectWindow(Player player)
        {
            AudioDeviceSelectViewModel viewModel = new AudioDeviceSelectViewModel(player);
            DataContext = viewModel;
            this.Topmost = true;
            InitializeComponent();
            if (viewModel.CloseAction == null)
                viewModel.CloseAction = new Action(this.Close);
        }
    }
}
