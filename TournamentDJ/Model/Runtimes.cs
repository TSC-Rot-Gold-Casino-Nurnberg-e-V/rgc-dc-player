using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
