using System.Collections.ObjectModel;

namespace TournamentDJ.Model
{
    class Runtimes : ObservableCollection<int>
    {
        public Runtimes()
        {
            Add(60);
            Add(75);
            Add(90);
            Add(105);
        }

    }
}
