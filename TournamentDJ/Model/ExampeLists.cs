using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentDJ.Model
{
    class ExampeLists
    {
        public List<Uri> SlowWaltz;
        public List<Uri> Tango;
        public List<Uri> VienesseWaltz;
        public List<Uri> Slowfox;
        public List<Uri> Quickstep;

        public ExampeLists() 
        { 
            SlowWaltz = new List<Uri>();
            Tango = new List<Uri>();
            VienesseWaltz = new List<Uri>();
            Slowfox = new List<Uri>();
            Quickstep = new List<Uri>();

            SlowWaltz.Add(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\01 Concerning Hobbits [SW].mp3"));
            SlowWaltz.Add(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\02 Promise [SW].mp3"));
            SlowWaltz.Add(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\03 Dark Waltz [SW].mp3"));
            SlowWaltz.Add(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\04 Bridal Veil Falls [SW].mp3"));
            SlowWaltz.Add(new Uri(@"C:\Media\Vivi\Very Ballroom 2 [Disc 1]\10 Stonefire [SW].mp3"));
    
        }
    }
}
