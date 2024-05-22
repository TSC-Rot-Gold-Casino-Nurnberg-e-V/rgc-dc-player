using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TournamentDJ.Model
{
    static class DefaultValues
    {
        public static List<Dance> DefaultDances = new List<Dance>
        {
            new Dance("Uncategorized"),
            new Dance("SlowWaltz", ["[SW]", "[LW]", "(SW)", "(LW)", "[EW]", "(EW)", "Langsamer Walzer"]),
            new Dance("Tango", ["[TA]", "(TA)"]),
            new Dance("ViennesseWaltz", ["[WW]", "[VW]", "(WW)", "(VW)", "Wiener Walzer"]),
            new Dance("Slowfox", ["[SF]", "(SF)", "Slowfoxtrott"]),
            new Dance("Quickstep", ["[QS]", "(QS)", "[Q]", "(Q)", "Quickstep"]),
            new Dance("Samba", ["[SB]", "(SB)"]),
            new Dance("Cha-Cha", ["[CC]", "(CC)", "ChaCha", "[Cha]", "(Cha)"]),
            new Dance("Rumba", ["[RB]", "(RB)"]),
            new Dance("PasoDoble", ["[PD]", "(PD)"]),
            new Dance("Jive", ["[JV]", "(JV)"])
        };
    }
}
