namespace TournamentDJ.Model
{
    static class DefaultValues
    {
        public static List<Dance> DefaultDances = new List<Dance>
        {
            new Dance("Uncategorized"),
            new Dance("SlowWaltz", ["[SW]", "[LW]", "(SW)", "(LW)", "[EW]", "(EW)", "Langsamer Walzer", "(Waltz)"]),
            new Dance("Tango", ["[TA]", "(TA)"]),
            new Dance("ViennesseWaltz", ["[WW]", "[VW]", "(WW)", "(VW)", "Wiener Walzer", "Viennese Waltz"]),
            new Dance("Slowfox", ["[SF]", "(SF)", "Slowfoxtrott"]),
            new Dance("Quickstep", ["[QS]", "(QS)", "[Q]", "(Q)", "Quickstep"]),
            new Dance("Samba", ["[SB]", "(SB)"]),
            new Dance("Cha-Cha", ["[CC]", "(CC)", "ChaCha", "[Cha]", "(Cha)"]),
            new Dance("Rumba", ["[RB]", "(RB)"]),
            new Dance("PasoDoble", ["[PD]", "(PD)"]),
            new Dance("PasoDoble Cut"),
            new Dance("Jive", ["[JV]", "(JV)"])
        };
    }
}
