using System.Windows;

namespace TournamentDJ
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        public ResourceDictionary LocalizationDictionary
        {
            // You could probably get it via its name with some query logic as well.
            get { return Resources.MergedDictionaries[0]; }
        }

        public void ChangeLocalization(Uri uri)
        {
            LocalizationDictionary.MergedDictionaries.Clear();
            LocalizationDictionary.MergedDictionaries.Add(new ResourceDictionary() { Source = uri });
        }
    }

}
