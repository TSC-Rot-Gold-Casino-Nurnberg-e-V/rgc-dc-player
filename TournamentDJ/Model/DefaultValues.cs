namespace TournamentDJ.Model
{
    static class DefaultValues
    {
        public static List<Dance> DefaultDances = new List<Dance>
        {
            new Dance("Uncategorized"),
            new Dance("SlowWaltz", ["[SW]", "[LW]", "(SW)", "(LW)", "[EW]", "(EW)", "Langsamer Walzer", "(Waltz)"]),
            new Dance("Tango", ["[TA]", "(TA)"]),
            new Dance("VienneseWaltz", ["[WW]", "[VW]", "(WW)", "(VW)", "Wiener Walzer", "Viennese Waltz"]),
            new Dance("Slowfox", ["[SF]", "(SF)", "Slowfoxtrott"]),
            new Dance("Quickstep", ["[QS]", "(QS)", "[Q]", "(Q)", "Quickstep"]),
            new Dance("Samba", ["[SB]", "(SB)"]),
            new Dance("Cha-Cha", ["[CC]", "(CC)", "ChaCha", "[Cha]", "(Cha)"]),
            new Dance("Rumba", ["[RB]", "(RB)"]),
            new Dance("PasoDoble", ["[PD]", "(PD)"]),
            new Dance("PasoDoble Cut"),
            new Dance("Jive", ["[JV]", "(JV)"])
        };

        public static List<TimeSpan> DefaultTournamentRuntimes = new List<TimeSpan>
        {
            new TimeSpan(0, 0, 5),
            new TimeSpan(0, 0, 15),
            new TimeSpan(0, 0, 60),
            new TimeSpan(0, 0, 75),
            new TimeSpan(0, 0, 90),
            new TimeSpan(0, 0, 95),
            new TimeSpan(0, 0, 100),
            new TimeSpan(0, 0, 105),
            new TimeSpan(0, 0, 110)
        };

        public static List<TimeSpan> DefaultWarmupRuntimes = new List<TimeSpan>
        {
            new TimeSpan(0, 0, 5),
            new TimeSpan(0, 0, 15),
            new TimeSpan(0, 0, 60),
            new TimeSpan(0, 0, 75),
            new TimeSpan(0, 0, 90),
            new TimeSpan(0, 0, 110),
            new TimeSpan(0, 0, 150),
            new TimeSpan(1, 0, 0)
        };
    }
}
