using System.Windows;
using TournamentDJ.ViewModel;

namespace TournamentDJ
{
    /// <summary>
    /// Interaktionslogik für DatabaseUtility.xaml
    /// </summary>
    public partial class SelectSpecificTrackWindow : Window
    {
        public SelectSpecificTrackWindow()
        {
            SelectSpecificTrackViewModel viewModel = new SelectSpecificTrackViewModel();
            DataContext = viewModel;
            Closing += viewModel.OnWindowClosing;
            InitializeComponent();
        }
    }
}
