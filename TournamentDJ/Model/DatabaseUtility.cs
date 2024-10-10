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

            //_context.Database.EnsureDeleted();
            _context.Database.EnsureCreated();

            // load the entities into EF Core

            _context.Dances.Load();
            FillDances();

            //_context.OrderElementsDance.Load();
            //FillOrderElements();

            _context.DanceRounds.Load();
            FillDanceRounds();

            _context.Tracks.Load();
            FillTracks();

            _context.TrackLists.Load();
            FillTrackLists();
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

        public static ObservableCollection<TrackList> TrackLists
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

        private static void FillTrackLists()
        {
            TrackLists = _context.TrackLists.Local.ToObservableCollection();
        }

        private static void FillDanceRounds()
        {
            DanceRounds = _context.DanceRounds.Local.ToObservableCollection();
        }

        private static void FillDances()
        {
            Dances = _context.Dances.Local.ToObservableCollection();
            foreach (var dance in DefaultValues.DefaultDances)
            {
                if (Dances.FirstOrDefault(X => X.Name == dance.Name) == null)
                {
                    Dances.Add(dance);
                }
            }
        }

        //private static void FillorderElements()
        //{

        //}

        public static void SaveChanges()
        {
            // all changes are automatically tracked, including
            // deletes!
            var saved = _context.SaveChanges();
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
