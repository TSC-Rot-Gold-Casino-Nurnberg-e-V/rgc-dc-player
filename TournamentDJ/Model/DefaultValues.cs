namespace TournamentDJ.Model
{
    static class DefaultValues
    {
        public static List<Dance> DefaultDances = new List<Dance>
        {
            new Dance("Uncategorized"),
            new Dance("Slow Waltz", ["[SW]", "[LW]", "(SW)", "(LW)", "[EW]", "(EW)", "Langsamer Walzer", "(Waltz)", "SlowWaltz"]),
            new Dance("Tango", ["[TA]", "(TA)"]),
            new Dance("Viennese Waltz", ["[WW]", "[VW]", "(WW)", "(VW)", "Wiener Walzer", "VienneseWaltz"]),
            new Dance("Slowfox", ["[SF]", "(SF)", "Slowfoxtrott"]),
            new Dance("Quickstep", ["[QS]", "(QS)", "[Q]", "(Q)", "Quickstep"]),
            new Dance("Samba", ["[SB]", "(SB)"]),
            new Dance("Cha-Cha", ["[CC]", "(CC)", "ChaCha", "[Cha]", "(Cha)"]),
            new Dance("Rumba", ["[RB]", "(RB)"]),
            new Dance("Paso Doble", ["[PD]", "(PD)", "PasoDoble"]),
            new Dance("Paso Doble Cut"),
            new Dance("Jive", ["[JV]", "(JV)"]),
            new Dance("Filler"),
            new Dance("Hymn"),
            new Dance("Prize Presentation"),
            new Dance("Entry"),
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
