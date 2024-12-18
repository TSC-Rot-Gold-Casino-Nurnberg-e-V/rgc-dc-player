using Microsoft.EntityFrameworkCore;
using System.IO;
using Windows.UI.Text;

namespace TournamentDJ.Model
{
    public class TrackContext : DbContext
    {
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Dance> Dances { get; set; }
        public DbSet<DanceRound> DanceRounds { get; set; }
        public DbSet<TrackList> TrackLists { get; set; }
        public DbSet<OrderElement<Dance>> OrderElementsDance { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            Directory.CreateDirectory(Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TournamentDJ"));

            var dataSource = Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TournamentDJ\\dbMusic.db");
            optionsBuilder
                .UseSqlite($"Data Source={dataSource};");
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
