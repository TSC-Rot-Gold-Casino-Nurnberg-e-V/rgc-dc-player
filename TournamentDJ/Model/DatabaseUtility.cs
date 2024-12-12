using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.IO;
using TournamentDJ.Essentials;

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

            _context.DanceRounds.Load();
            FillDanceRounds();

            _context.Tracks.Load();
            FillTracks();

            _context.TrackLists.Load();
            FillTrackLists();
        }

        ~DatabaseUtility()
        {
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

        public static void SaveChanges()
        {
            // all changes are automatically tracked, including
            // deletes!
            var saved = _context.SaveChanges();
        }

        public static void AddToDatabase(Track trackToAdd)
        {
            Track? found = null;

            found = checkTrackISRC(trackToAdd);

            //Check for Track with same length and title
            if (found == null)
            {
                found = _context.Tracks.FirstOrDefault<Track>(x => x.Title == trackToAdd.Title && x.Duration == trackToAdd.Duration);
            }

            //There are some really weird and rare cases when duplicate URIS are found, that have to be handled here.
            //1. A Track gets added, with the same URI, but different lengths and Title, e.g. a file gets replaced
            //   with a newer version, but the same file name. Solution -> Old Track gets removed, and new Version is added as new Track
            //2. A new Track gets added with same URI and length, but different Title and ISRC.
            // Solution -> Old Track is updated to reflect the changes. !!THIS CAN LEAD TO WRONG CATEGORIZATION!!
            //Because of this, the whole Database has to be searched for duplicate URIs with every insertion.

            if (found == null)
            {
                foreach (Uri uri in trackToAdd.Uris)
                {
                    found = checkForDuplicateUri(uri);

                    if (found != null)
                    {
                        //1
                        if (found.Duration != trackToAdd.Duration && found.Title != trackToAdd.Title)
                        {
                            found = handleDuplicateUriWithDifferentTrack(uri);
                        }

                        //2
                        if (found.Duration == trackToAdd.Duration)
                        {
                            found = handleDuplicateUriWithSimilarTrack(uri, trackToAdd);
                        }
                    }
                }
            }

            //Track already exist in some way. Just add new Uris
            if (trackToAdd != null && found != null)
            {
                updateTrackUris(found, trackToAdd);
            }

            //Track is completly new
            if (trackToAdd != null && found == null)
            {
                Tracks.Add(trackToAdd);
            }
        }

        private static Track checkTrackISRC(Track trackToAdd)
        {
            Track? found = null;
            //ISRC exists and is not empty
            if (trackToAdd.ISRC != null && trackToAdd.ISRC != string.Empty)
            {
                found = _context.Tracks.FirstOrDefault<Track>(x => x.ISRC == trackToAdd.ISRC);
            }

            //If file with same ISRC is used in different lengths, it should be different Tracks
            if (found != null)
            {
                if (found.Duration != trackToAdd.Duration)
                {
                    trackToAdd.ISRC = string.Empty;
                    found = null;
                    //Logger.LoggerInstance.LogWrite("Track " + trackToAdd.Uris.First<Uri>().AbsolutePath.ToString() + " with same ISRC, but different lengths was added as new Track");
                }
            }
            return found;
        }


        private static Track checkForDuplicateUri(Uri uri)
        {
            Track? found = null;
            foreach (Track track in _context.Tracks)
            {
                if (track.Uris.Contains(uri))
                {
                    found = track;
                    break;
                }
            }
            return found;
        }

        private static Track handleDuplicateUriWithDifferentTrack(Uri uri)
        {
            Track? found = null;

            found.Uris.Remove(uri);
            if (found.Uris.Count == 0)
            {
                _context.Tracks.Remove(found);
            }
            found = null;

            return found;
        }

        private static Track handleDuplicateUriWithSimilarTrack(Uri uri, Track trackToAdd)
        {
            Track? found = null;

            found.Title = trackToAdd.Title;
            found.ISRC = (found.ISRC == null || found.ISRC == string.Empty) ? trackToAdd.ISRC : string.Empty;
            //Logger.LoggerInstance.LogWrite("Track " + found.Uris.First<Uri>().AbsolutePath.ToString() + " was updated");

            return found;
        }

        private static void updateTrackUris(Track found, Track trackToAdd)
        {
            foreach (Uri uri in trackToAdd.Uris)
            {
                found.Uris.Add(uri);
            }

            //Make sure, every Uri is only saved once
            found.Uris = new ObservableCollection<Uri>(found.Uris.Distinct<Uri>());
            //Logger.LoggerInstance.LogWrite("Track " + trackToAdd.Uris.First<Uri>().AbsolutePath.ToString() + " already inserted into Database. Save location updated.");
        }

    }
}
