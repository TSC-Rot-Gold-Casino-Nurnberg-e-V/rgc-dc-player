using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using TagLib;
using TagLib.Id3v2;
using TournamentDJ.Essentials;
using Windows.Gaming.Input;
using Windows.UI.Notifications;

namespace TournamentDJ.Model
{

    public class Track : NotifyObject
    {
        private static readonly Char fieldDelimiter = '|';
        private static readonly Char dataDelimiter = '~';

        public static Dictionary<int, string> Difficulties = new Dictionary<int, string>
        {
            {-1, "undefined" },
            {4, "too hard"},
            {3, "hard for pros" },
            {2, "for pros" },
            {1, "for intermediates" },
            {0, "for beginners" }
        };

        public static Dictionary<int, string> Characteristics = new Dictionary<int, string>
        {
            {-1, "undefined" },
            {0, "fail"},
            {1, "bad" },
            {2, "ok" },
            {3, "good" },
            {4, "very good" }
        };

        public static Dictionary<int, string> Ratings = new Dictionary<int, string>
        {
            {0, "1"},
            {1, "2" },
            {2, "3" },
            {3, "4" },
            {4, "5" }
        };

        public Track()
        {

        }

        public Track(Uri uri)
        {
            TagLib.File file;
            TagLib.Id3v2.Tag tag;
            string filePath = uri.LocalPath;
            try
            {
                file = TagLib.File.Create(filePath);
                tag = (TagLib.Id3v2.Tag) file.GetTag(TagLib.TagTypes.Id3v2);
            }
            catch (Exception e)
            {
                Logger.LoggerInstance.LogWrite("URI of Track could not be found when trying to create new Track");
                throw;
            }

            Uris = new ObservableCollection<Uri>();

            var dict = GetFieldsFromComment(file.Tag.Comment);

            Rating = 0;
            Difficulty = -1;
            Characteristic = -1;
            Comment = string.Empty;

            //Try getting Spoerl Data, if no other Data exists
            if (dict.Count == 0)
            {
                string cpinf = string.Empty;
                List<TagLib.Id3v2.Frame> frames = tag.GetFrames("COMM").ToList();
                foreach (Frame frame in frames)
                {
                    string text = frame.ToString();
                    if (text != null)
                    {
                        if (text.StartsWith("L:"))
                        {
                            cpinf = text;
                        }
                        else
                        {
                            Comment = text;
                        }
                    }
                }
                int[] ParsedValues = RetrieveSpoerlData(cpinf);

                Rating = ParsedValues[0];
                Characteristic = ParsedValues[1];
                Difficulty = Math.Abs(ParsedValues[2] - 4); //Adapt Values to correct range, CP uses an inverted Range

                if (Difficulty > 4) //Value was undefined or out of Range
                {
                    Difficulty = -1;
                }
            }

            string valString = string.Empty;
            int valInt = 0;
            bool valBool = false;
            bool success = false;

            dict.TryGetValue("Rating", out valString);
            success = int.TryParse(valString, out valInt);
            Rating = success ? valInt : Rating;

            dict.TryGetValue("Difficulty", out valString);
            success = int.TryParse(valString, out valInt);
            Difficulty = success ? valInt : Difficulty;

            dict.TryGetValue("Characteristic", out valString);
            success = int.TryParse(valString, out valInt);
            Characteristic = success ? valInt : Characteristic;

            dict.TryGetValue("PlayCount", out valString);
            success = int.TryParse(valString, out valInt);
            PlayCount = success ? valInt : 0;

            dict.TryGetValue("FlaggedAsFavourite", out valString);
            success = bool.TryParse(valString, out valBool);
            FlaggedAsFavourite = success ? valBool : false;

            dict.TryGetValue("FlaggedForReview", out valString);
            success = bool.TryParse(valString, out valBool);
            FlaggedForReview = success ? valBool : false;

            dict.TryGetValue("Comment", out valString);
            Comment = !string.IsNullOrEmpty(valString) ? valString : Comment;

            dict.TryGetValue("Dance", out valString);
            string danceIdent = !string.IsNullOrEmpty(valString) ? valString : string.Empty;

            Dance = SearchDance(file, danceIdent);
            BeatsPerMinute = file.Tag.BeatsPerMinute;
            Year = (int)file.Tag.Year;
            Duration = file.Properties.Duration;
            Uris.Add(uri);
            Title = (file.Tag.Title != null) ? file.Tag.Title : System.IO.Path.GetFileName(filePath);
            Album = (file.Tag.Album != null) ? file.Tag.Album : string.Empty;
            Genre = (file.Tag.FirstGenre != null) ? file.Tag.FirstGenre : string.Empty;
            ISRC = (file.Tag.ISRC != null) ? file.Tag.ISRC : string.Empty;



        }

        [Key]
        public int Id { get; set; }

        public virtual Dance? Dance
        {
            get; set;
        }

        public string? Title
        {
            get; set;
        }

        public int PlayCount { get; set; }

        public string? Comment
        {
            get; set;
        }

        public DateTime LastPlayedTime { get; set; } = DateTime.MinValue;

        public bool FlaggedAsFavourite { get; set; }

        public bool FlaggedForReview { get; set; }

        public int Year { get; set; }

        public int Difficulty { get; set; }
        public int Characteristic { get; set; }
        public int Rating { get; set; }

        public string? Genre
        {
            get; set;
        }

        public string? ISRC
        {
            get; set;
        }

        public string? Album
        {
            get { return Get<string>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        public TimeSpan Duration
        {
            get { return Get<TimeSpan>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        public uint? BeatsPerMinute
        {
            get { return Get<uint>(); }
            set
            {
                Set(value);
                OnPropertyChanged();
            }
        }

        //Collection of Uris, where the Track might be found.
        public ObservableCollection<Uri> Uris
        {
            get; set;
        }

        public virtual ObservableCollection<Tag> Tags
        {
            get; set;
        } = new ObservableCollection<Tag>();

        public virtual ObservableCollection<TrackList> TrackLists
        { get; private set; } =
        new ObservableCollection<TrackList>();


        //returns an int array with lenght 4, containing L, C, B and N, as used in Competition Player by Sebastian Spörl
        //Likeness = rating
        //Characteristic = proper dance and phrasing
        //Difficulty = how hard it is to hear the rhythm
        //Nasty = if there are long parts without Music/rhythm or similar stuff
        private int[] RetrieveSpoerlData(string comment)
        {
            int[] properties = new int[4];
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = -1;
            }

            //Comment should look like "CPINFL:2/C:2/B:2/N:0T", we only care about the 4 integers
            string pattern = @"-?\d\/-?\d\/-?\d\/-?\d";

            if (comment == null)
            {
                return properties;
            }
            comment = comment.Replace("L:", "");
            comment = comment.Replace("N:", "");
            comment = comment.Replace("B:", "");
            comment = comment.Replace("C:", "");
            Match match = Regex.Match(comment, pattern);
            if (!match.Success)
            {
                return properties;
            }
            comment = match.Value;
            MatchCollection matches = Regex.Matches(comment, @"(-?\d)");
            if (matches.Count != 4)
            {
                return properties;
            }

            for (int i = 0; i < 4; i++)
            {
                if (!int.TryParse(matches[i].Value, out properties[i]))
                {
                    return properties;
                }
            }

            return properties;
        }

        private List<string> GetDanceIdentifiers(Dance dance)
        {
            List<string> danceIdentifiers = new List<string>();

            if (dance.DanceIdentifiers != null)
            {
                danceIdentifiers = dance.DanceIdentifiers.ToList();
            }

            if (dance.Name != null)
            {
                danceIdentifiers.Add(dance.Name);
            }

            //Normalize all identifier strings
            for (int i = 0; i < danceIdentifiers.Count; i++)
            {
                danceIdentifiers[i] = danceIdentifiers[i].Replace(" ", "").ToLowerInvariant();
            }

            return danceIdentifiers;
        }
        private Dance SearchDance(TagLib.File file, string danceIdent = null)
        {
            //Compare with danceIdent first
            if (!string.IsNullOrEmpty(danceIdent))
            {
                foreach (var dance in DatabaseUtility.Dances)
                {
                    var danceIdentifiers = GetDanceIdentifiers(dance);
                    foreach (var ident in danceIdentifiers)
                    {
                        if (danceIdent.Equals(ident))
                        {
                            return dance;
                        }
                    }
                }
            }

            //Compare with genre next
            foreach (var dance in DatabaseUtility.Dances)
            {
                var danceIdentifiers = GetDanceIdentifiers(dance);

                string genre = string.Empty;
                if (file.Tag.FirstGenre != null)
                {
                    genre = file.Tag.FirstGenre.Replace(" ", "").ToLowerInvariant();
                }

                foreach (var ident in danceIdentifiers)
                {
                    if (genre.Equals(ident))
                    {
                        return dance;
                    }
                }
            }

            //search in Name last, as this is a lot slower
            foreach (var dance in DatabaseUtility.Dances)
            {
                var danceIdentifiers = GetDanceIdentifiers(dance);

                string name = string.Empty;
                if (file.Tag.Title != null)
                {
                    name = file.Tag.Title.Replace(" ", "").ToLowerInvariant();
                }

                foreach (var ident in danceIdentifiers)
                {
                    if (name.Contains(ident))
                    {
                        return dance;
                    }
                }
            }

            //no dance found
            return null;
        }

        private string CreateNewComment(Dictionary<string, string> dict)
        {
            string newComment = string.Empty;
            foreach (var entry in dict)
            {
                newComment += entry.Key + dataDelimiter + entry.Value + fieldDelimiter;
            }
            return newComment;
        }

        private static Dictionary<string, string> GetFieldsFromComment(TagLib.File file)
        {
            return GetFieldsFromComment(file.Tag.Comment);
        }

        private static Dictionary<string, string> GetFieldsFromComment(string comment)
        {
            if (string.IsNullOrEmpty(comment)) return new Dictionary<string, string>();
            return comment.Split(fieldDelimiter, StringSplitOptions.RemoveEmptyEntries)
                .Select (part => part.Split(dataDelimiter, StringSplitOptions.RemoveEmptyEntries))
                .Where (part => part.Length == 2)
                .ToDictionary (sp => sp[0], sp  => sp[1]);
        }

        private Dictionary<string, string> GetFields()
        {
            Dictionary<string, string> fields = new Dictionary<string, string>();
            fields.Add("Rating", Rating.ToString());
            fields.Add("Difficulty", Difficulty.ToString());
            fields.Add("Characteristic", Characteristic.ToString());
            fields.Add("PlayCount", PlayCount.ToString());
            fields.Add("FlaggedForReview", FlaggedForReview.ToString());
            fields.Add("FlaggedAsFavourite", FlaggedAsFavourite.ToString());
            if (Dance != null && Dance.DanceIdentifiers != null && Dance.DanceIdentifiers.FirstOrDefault() != null)
                fields.Add("Dance", Dance.DanceIdentifiers.FirstOrDefault().ToString());
            if (!string.IsNullOrEmpty(Comment))
                fields.Add("Comment", Comment);

            return fields;
        }

        public void WriteDataToFile()
        {
            string comment = CreateNewComment(GetFields());

            TagLib.File file;
            TagLib.Id3v2.Tag newTag = new TagLib.Id3v2.Tag();
            foreach(Uri uri in Uris)
            {
                try
                {
                    file = TagLib.File.Create(uri.LocalPath);
                    newTag = (TagLib.Id3v2.Tag)file.GetTag(TagLib.TagTypes.Id3v2);
                    break; //Leave Loop as soon as we find one valid Tag 
                }
                catch (Exception e)
                {
                    Logger.LoggerInstance.LogWrite("URI of Track could not be found, when trying to read Tag (WriteDataToFile)  " + uri);
                    throw;
                }
            }

            newTag.Title = Title;
            newTag.Comment = comment;
            newTag.DateTagged = DateTime.Now;

            foreach (Uri uri in Uris)
            {
                try
                {
                    file = TagLib.File.Create(uri.LocalPath);
                    newTag.CopyTo(file.Tag, true);
                    file.Save();
                }
                catch (Exception e)
                {
                    Logger.LoggerInstance.LogWrite("Saving new Tag failed   " + e.Message);
                }
            }
        }


        ///TODO Finish up
        /// <summary>
        /// Compares two tags and writes the newer one to both files.
        /// if useConservative is true, the more conservative Values for Rating, Difficulty and Characteristic will be used.
        /// THIS DOES NOT CHECK IF THE TRACKS ARE EQUAL AT ALL
        /// </summary>
        /// <param name="track1"></param>
        /// <param name="track2"></param>
        public static void CompareAndSetTag(Track track1, Track track2, bool useConservative = false)
        {
            try
            {
                var file1 = TagLib.File.Create(track1.Uris.FirstOrDefault().LocalPath);
                var file2 = TagLib.File.Create(track2.Uris.FirstOrDefault().LocalPath);

                var tag1 = file1.Tag;
                var tag2 = file2.Tag;

                if (useConservative)
                {
                    track1.Rating = track1.Rating < track2.Rating ? track1.Rating : track2.Rating;
                    track1.Difficulty = track1.Difficulty > track2.Difficulty ? track1.Difficulty : track2.Difficulty;
                    track1.Characteristic = track1.Characteristic < track2.Characteristic ? track1.Characteristic : track2.Characteristic;
                }

                TagLib.Tag newerTag = null;
                if (tag1 == null || tag2 == null) { return; }
                if (tag1.DateTagged.GetValueOrDefault() > tag2.DateTagged.GetValueOrDefault())
                {
                    tag1.CopyTo(file2.Tag, true);
                }
                else
                {
                    tag2.CopyTo(file1.Tag, true);
                }
            }
            catch(Exception e)
            {
                Logger.LoggerInstance.LogWrite("Could not compare Tracks " + e.Message);
            }
        }
    }
}
