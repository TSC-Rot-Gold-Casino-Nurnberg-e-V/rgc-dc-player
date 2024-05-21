using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserDataTasks.DataProvider;
using Windows.Globalization;

namespace TournamentDJ.Model
{
    class TrackListBuilder
    {
       public TrackListBuilder()
        { 

        }


        //TODO: Remove this method, its only for testing purposes and WILL LEAD TO PROBLEMS when used with large databases
        public static TrackList GetAllTracks()
        {
            TrackList trackList = new TrackList(); 
            foreach (Track track in DatabaseUtility.Tracks)
            {
                trackList.Tracks.Add(track);
            }

            return trackList;
        }


        public static TrackList CreateDanceRound(DanceRound roundToCreate, int heats = 1, bool single = false, TrackList tracklist = null, bool hasToBeFavorite = false)
        {
            if (roundToCreate == null) { return null; }

            TrackList tracks = new TrackList();

            if (single || heats == 1)
            {
                foreach (var dance in roundToCreate.Dances)
                {
                    Track trackToAdd = GetRandomTrack(dance, tracklist, roundToCreate.MinDifficulty, roundToCreate.MaxDifficulty, roundToCreate.MinCharacteristics, hasToBeFavorite);
                    for (int i = 0; i < heats; i++)
                    {
                        tracks.Tracks.Add(trackToAdd);
                    }
                }
                
            } 
            else
            {
                foreach (var dance in roundToCreate.Dances)
                {
                    var list = GetRandomTracks(dance, heats, tracklist, roundToCreate.MinDifficulty, roundToCreate.MaxDifficulty, roundToCreate.MinCharacteristics, hasToBeFavorite);
                    foreach (var track in list)
                    {
                        tracks.Tracks.Add(track);
                    }
                }
            }




            return tracks;
        }

        public static Track GetRandomTrack(Dance dance, TrackList trackListToUse = null, int minDiff = 4, int maxDiff = 0, int minChar = 0, bool hasToBeFavorite = false)
        {
            Track[] TracksWithDance;

            if(trackListToUse != null)
            {
                TracksWithDance = trackListToUse.Tracks.Where(X => X.Dance == dance).ToArray();
            }
            else
            {
                if(hasToBeFavorite)
                {
                    TracksWithDance = DatabaseUtility.Tracks.Where(X => X.Dance == dance && X.Difficulty <= minDiff && X.Difficulty >= maxDiff && X.Characteristic >= minChar && X.FlaggedAsFavourite == true).ToArray();

                }
                else
                {
                    TracksWithDance = DatabaseUtility.Tracks.Where(X => X.Dance == dance && X.Difficulty <= minDiff && X.Difficulty >= maxDiff && X.Characteristic >= minChar).ToArray();
                }
            }

            
            Random random = new Random(Guid.NewGuid().GetHashCode());
            if ( TracksWithDance.Length > 0 )
            {
                int randInt = random.Next(0, TracksWithDance.Length - 1);
                return TracksWithDance[randInt];
            }
            else
            {
                return null;
            }
        }

        public static List<Track> GetRandomTracks(Dance dance, int count, TrackList trackListToUse = null, int minDiff = 4, int maxDiff = 1, int minChar = 1, bool hasToBeFavorite = false)
        {
            List<Track> TracksWithDance;

            if (trackListToUse != null)
            {
                TracksWithDance = trackListToUse.Tracks.Where(X => X.Dance == dance).ToList();
            }
            else
            {
                if (hasToBeFavorite)
                {
                    TracksWithDance = DatabaseUtility.Tracks.Where(X => X.Dance == dance && X.Difficulty <= minDiff && X.Difficulty >= maxDiff && X.Characteristic >= minChar && X.FlaggedAsFavourite == true).ToList();

                }
                else
                {
                    TracksWithDance = DatabaseUtility.Tracks.Where(X => X.Dance == dance && X.Difficulty <= minDiff && X.Difficulty >= maxDiff && X.Characteristic >= minChar).ToList();
                }
            }

            Random random = new Random(Guid.NewGuid().GetHashCode());
            List<Track> list = new List<Track>();

            if(TracksWithDance.Count == 0)
            {
                return null;
            }
           
            if(TracksWithDance.Count < count)
            {

                //If list is still too short, just double the list
                while(TracksWithDance.Count < count)
                {
                    TracksWithDance = TracksWithDance.Concat(TracksWithDance).ToList();
                }
            }

            if (TracksWithDance.Count > 0)
            {
                for(int i = 0; i < count; i++)
                {
                    int randInt = random.Next(0, TracksWithDance.Count - 1);
                    Track trackToAdd = TracksWithDance[randInt];
                    list.Add(trackToAdd);
                    TracksWithDance.Remove(trackToAdd);
                }
            }
            return list;
        }

    }
}
