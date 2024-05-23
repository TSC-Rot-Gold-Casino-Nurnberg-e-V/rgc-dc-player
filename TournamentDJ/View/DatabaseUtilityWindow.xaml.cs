using System.Windows;
using TournamentDJ.ViewModel;

namespace TournamentDJ
{
    /// <summary>
    /// Interaktionslogik für DatabaseUtility.xaml
    /// </summary>
    public partial class DatabaseUtilityWindow : Window
    {
        public DatabaseUtilityWindow()
        {
            DatabaseUtilityViewModel viewModel = new DatabaseUtilityViewModel();
            DataContext = viewModel;
            Closing += viewModel.OnWindowClosing;
            InitializeComponent();
        }
    }
}
