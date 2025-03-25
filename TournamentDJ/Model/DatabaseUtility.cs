using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using SoundFingerprinting.Data;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Threading;
using TagLib.Ape;
using TournamentDJ.Deduplication;
using TournamentDJ.Essentials;

namespace TournamentDJ.Model
{
    class DatabaseUtility : NotifyObject
    {
        public DatabaseUtility()
        {
            if((_context.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists())
            {
                _context.Database.Migrate();

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
            else
            {
                _context.Database.Migrate();

                // load the entities into EF Core

                _context.Dances.Load();
                FillDances();
                AddDefaultDances();

                _context.DanceRounds.Load();
                FillDanceRounds();
                AddDefaultDanceRounds();

                _context.Tracks.Load();
                FillTracks();

                _context.TrackLists.Load();
                FillTrackLists();
            }

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

        private static void FillDances()
        {
            Dances = _context.Dances.Local.ToObservableCollection();
        }

        private static void AddDefaultDances()
        {
            foreach (var dance in DefaultValues.DefaultDances)
            {
                if (Dances.FirstOrDefault(X => X.Name == dance.Name) == null)
                {
                    Dances.Add(dance);
                }
            }
        }

        private static void FillDanceRounds()
        {
            DanceRounds = _context.DanceRounds.Local.ToObservableCollection();
        }

        private static void AddDefaultDanceRounds()
        {
            var basStd = new DanceRound("BAS Std", 1, 3, 1);
            basStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Slow Waltz")));
            basStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Tango")));
            basStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Viennese Waltz")));
            basStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Slowfox")));
            basStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Quickstep")));
            DanceRounds.Add(basStd);

            var cStd = new DanceRound("C Std", 0, 2, 2);
            cStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Slow Waltz")));
            cStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Tango")));
            cStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Slowfox")));
            cStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Quickstep")));
            DanceRounds.Add(cStd);

            var dStd = new DanceRound("D Std", 0, 1, 3);
            dStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Slow Waltz")));
            dStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Tango")));
            dStd.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Quickstep")));
            DanceRounds.Add(dStd);

            var dStdChild = new DanceRound("D Std Kin", 0, 0, 3);
            dStdChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Slow Waltz")));
            dStdChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Tango")));
            dStdChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Quickstep")));
            DanceRounds.Add(dStdChild);

            var cStdChild = new DanceRound("C Std Kin", 0, 0, 3);
            cStdChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Slow Waltz")));
            cStdChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Tango")));
            cStdChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Slowfox")));
            cStdChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Quickstep")));
            DanceRounds.Add(cStdChild);


            var basLat = new DanceRound("BAS Lat", 1, 3, 1);
            basLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Samba")));
            basLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Cha-Cha")));
            basLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Rumba")));
            basLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Paso Doble Cut")));
            basLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Jive")));
            DanceRounds.Add(basLat);

            var cLat = new DanceRound("C Lat", 0, 2, 2);
            cLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Samba")));
            cLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Cha-Cha")));
            cLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Rumba")));
            cLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Jive")));
            DanceRounds.Add(cLat);

            var dLat = new DanceRound("D Lat", 0, 1, 3);
            dLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Cha-Cha")));
            dLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Rumba")));
            dLat.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Jive")));
            DanceRounds.Add(dLat);

            var dLatChild = new DanceRound("D Lat Kin", 0, 0, 3);
            dLatChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Cha-Cha")));
            dLatChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Rumba")));
            dLatChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Jive")));
            DanceRounds.Add(dLatChild);

            var cLatChild = new DanceRound("C Lat Kin", 0, 0, 3);
            cLatChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Samba")));
            cLatChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Cha-Cha")));
            cLatChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Rumba")));
            cLatChild.OrderElements.Add(new OrderElement<Dance>(Dances.FirstOrDefault(x => x.Name == "Jive")));
            DanceRounds.Add(cLatChild);
        }

        public static void SaveChanges()
        {
            // all changes are automatically tracked, including
            // deletes!
            var saved = _context.SaveChanges();
        }

        public static void LoadFingerprints()
        {
            foreach (Track track in _context.Tracks)
            {
                FingerprintBase.AddFingerprintToModel(track);
            }
        }


        public static async Task<IEnumerable<Track>> CreateFingerprints(IEnumerable<Track> tracks)
        {
            return await FingerprintBase.CreateFingerprints(tracks);
        }

        public static void AddToDatabase(Track trackToAdd)
        {
            Track? found = null;

            found = checkTrackISRC(trackToAdd);

            if (found != null)
            {
                //Track already exist in some way. Just add new Uris
                if (trackToAdd != null && found != null)
                {
                    updateTrackUris(found, trackToAdd);
                }
            }

            //TODO: Check if URI is already in use

            if (found == null && trackToAdd != null)
            {

                int newId = FingerprintBase.FindBestMatchingTrack(trackToAdd);
                if (newId != -1)
                {
                    found = _context.Tracks.FirstOrDefault<Track>(x => x.Id == newId);
                    //Match was found -> Do sanity check
                    if (found != null && CheckMatch(trackToAdd, found))
                    {
                        updateTrackUris(found, trackToAdd);
                        found.UpdateDataInDatabase();
                    }
                }
                //No Match found -> Create new
                else
                {
                    Application.Current.Dispatcher.Invoke(new Action(() => Tracks.Add(trackToAdd)));
                    SaveChanges();
                    FingerprintBase.AddFingerprintToModel(trackToAdd);
                }

            }

            SaveChanges();
        }


        private static bool CheckMatch(Track newTrack, Track existingTrack)
        {
            double difference = newTrack.Duration / existingTrack.Duration;
            //if Track length differs too much, tracks are not equal.
            if (difference > 1.05 || difference < 0.95)
            {
                return false;
            }
            return true;
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


        private static void updateTrackUris(Track found, Track trackToAdd)
        {
            foreach (Uri uri in trackToAdd.Uris)
            {
                //found.Uris.Add(uri);
                Application.Current.Dispatcher.Invoke(new Action(() => found.Uris.Add(uri)));
            }

            //Make sure, every Uri is only saved once
            ObservableCollection<Uri> newUris = new ObservableCollection<Uri>(found.Uris.Distinct<Uri>());
            found.Uris = newUris;
            //Logger.LoggerInstance.LogWrite("Track " + trackToAdd.Uris.First<Uri>().AbsolutePath.ToString() + " already inserted into Database. Save location updated.");
        }

    }
}
