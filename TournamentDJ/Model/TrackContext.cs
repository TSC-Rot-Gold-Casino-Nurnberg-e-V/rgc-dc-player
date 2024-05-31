using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

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
            optionsBuilder.UseSqlite(
                "Data Source=dbMusic.db");
            optionsBuilder.UseLazyLoadingProxies();
        }
    }
}
