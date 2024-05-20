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
            DataContext = new DatabaseUtilityViewModel();
            InitializeComponent();
        }
    }
}
