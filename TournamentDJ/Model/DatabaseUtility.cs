using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using TournamentDJ.Essentials;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using TagLib.Mpeg4;

namespace TournamentDJ.Model
{
    class DatabaseUtility : NotifyObject
    {
        public DatabaseUtility()
        {

            _context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // load the entities into EF Core
            _context.Dances.Load();
            _context.Tracks.Load();
            _context.DanceRounds.Load();
            _context.TrackLists.Load();

            FillDances();
            FillTracks();
            FillDanceRounds();
        }
        
        ~DatabaseUtility() {
            _context.Dispose();
        }

        private static readonly TrackContext _context = new TrackContext();

        public static ObservableCollection<Dance> Dances
        {
            get; set;
        }

        public static ObservableCollection<Track> Tracks
        {
            get; set;
        }

        public static ObservableCollection<DanceRound> DanceRounds
        {
            get; set;
        }

        private static void FillTracks()
        {
            Tracks = _context.Tracks.Local.ToObservableCollection();
        }

        private static void FillDanceRounds()
        {
            DanceRounds = _context.DanceRounds.Local.ToObservableCollection();
        }

        private static void FillDances()
        {
            Dances = _context.Dances.Local.ToObservableCollection();
            if(Dances.Count == 0)
            {
                Dances.Add(new Dance("Uncategorized"));
                Dances.Add(new Dance("SlowWaltz"));
                Dances.Add(new Dance("Tango"));
                Dances.Add(new Dance("Quickstep"));

            }
            SaveChanges();
        }

        public static void SaveChanges()
        {
            // all changes are automatically tracked, including
            // deletes!
            _context.SaveChanges();
        }

        public static void AddToDatabase(ObservableCollection<Track> tracksToAdd)
        {
            foreach (var track in tracksToAdd)
            {
                Track? found = _context.Tracks.Find(track.Uri);
                if (track != null && found == null)
                {
                    Tracks.Add(track);
                }
            }
            tracksToAdd.Clear();
        }

        public static void GetFiles(string selectedPath, ObservableCollection<Track> outTracks, ObservableCollection<Uri> outFailedUris)
        {

            Directory.SetCurrentDirectory(selectedPath);
            string[] filepaths = Directory.GetFiles(selectedPath, "*.mp3", SearchOption.AllDirectories);
            foreach (string filepath in filepaths)
            {
                Uri uri = new Uri(filepath);
                Track track = null;
                if (uri != null)
                {
                    try 
                    { 
                        track = new Track(uri);
                    }
                    catch (Exception ex)
                    {
                        track = null;
                        outFailedUris.Add(uri);
                    }

                    if(track != null)
                    {
                        outTracks.Add(track);
                    }
                }
            }
        }


    }
}
