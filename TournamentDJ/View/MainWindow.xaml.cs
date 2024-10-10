using System.Windows;
using TournamentDJ.ViewModel;

namespace TournamentDJ
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            MainWindowViewModel viewModel = new MainWindowViewModel();
            DataContext = viewModel;
            Loaded += viewModel.WindowOpened;
            Closing += viewModel.OnWindowClosing;
            SetDefaultLanguage();
            InitializeComponent();

        }

        private void SetDefaultLanguage()
        {
            ResourceDictionary dict = new ResourceDictionary();
            dict.Source = new Uri("..\\Localization\\StringResources.xaml",
                                      UriKind.Relative);
            this.Resources.MergedDictionaries.Add(dict);
        }

        public void ExecuteSetLanguageDictionary(string language)
        {
            ResourceDictionary dict = new ResourceDictionary();
            switch (language)
            {
                case "en-US":
                    dict.Source = new Uri("..\\Localization\\StringResources.xaml",
                                  UriKind.Relative);
                    break;
                case "de-DE":
                    dict.Source = new Uri("..\\Localization\\StringResources.de-DE.xaml",
                                       UriKind.Relative);
                    break;
                default:
                    dict.Source = new Uri("..\\Localization\\StringResources.xaml",
                                      UriKind.Relative);
                    break;
            }
            this.Resources.MergedDictionaries.Add(dict);
        }
    }
}