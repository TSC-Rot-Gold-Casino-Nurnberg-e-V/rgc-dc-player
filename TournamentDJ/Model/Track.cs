using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore;
using SoundFingerprinting.Data;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using TagLib;
using TagLib.Id3v2;
using TagLib.NonContainer;
using TournamentDJ.Audio;
using TournamentDJ.Essentials;
using Windows.Gaming.Input;
using Windows.UI.Notifications;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Xml;
using Windows.Devices.Sms;

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

        public Track(Uri uri, AVHashes fingerprints) : this(uri)
        {
            Fingerprints = fingerprints;
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
                foreach (TagLib.Id3v2.Frame frame in frames)
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

            //Override SpoerlData, if possible
            UpdateData(dict, file);

            Uris.Add(uri);
            Title = (file.Tag.Title != null) ? file.Tag.Title : System.IO.Path.GetFileName(filePath);
            Artist = (file.Tag.FirstPerformer != null) ? file.Tag.FirstPerformer : "Unknown Artist";

        }

        #region Properties
        [Key]
        public int Id { get; set; }

        public virtual Dance? Dance
        {
            get; set;
        }

        public string? _avHashes { get; set;}

        [NotMapped]
        public AVHashes Fingerprints
        {
            get
            {
                try
                {
                    MemoryStream stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(_avHashes));
                    var ser = new DataContractSerializer(typeof(AVHashes));
                    var x = ser.ReadObject(stream);
                    return (AVHashes)x;
                }
                catch (Exception e)
                {

                }
                return null;
            }
            set
            {
                MemoryStream stream = new MemoryStream();
                var ser = new DataContractSerializer(typeof(AVHashes));
                ser.WriteObject(stream, value);
                _avHashes = Encoding.UTF8.GetString(stream.ToArray());
            }
        }
        public string? Title
        {
            get
            {
                return Get<string>();
            }
            set
            {
                Set(value);
                LastDataUpdateTimestamp = DateTime.Now;
            }
        }

        public int PlayCount { get; set; }

        public string? Comment
        {
            get
            {
                return Get<string>();
            }
            set
            {
                Set(value);
                LastDataUpdateTimestamp = DateTime.Now;
            }
        }

        public DateTime LastPlayedTime { get; set; } = DateTime.MinValue;
        public DateTime LastDataUpdateTimestamp{ get; private set; } = DateTime.MinValue;

        public bool FlaggedAsFavourite {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
                LastDataUpdateTimestamp = DateTime.Now;
            }
        }

        public bool FlaggedForReview
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
                LastDataUpdateTimestamp = DateTime.Now;
            }
        }

        public int Year { get; set; }

        public int Difficulty {
            get
            {
                return Get<int>();
            }
            set
            {
                if (value != Rating)
                {
                    Set(value);
                    LastDataUpdateTimestamp = DateTime.Now;
                }
            }
        }
        public int Characteristic {
            get
            {
                return Get<int>();
            }
            set
            {
                if (value != Rating)
                {
                    Set(value);
                    LastDataUpdateTimestamp = DateTime.Now;
                }
            }
        }
        public int Rating {
            get
            {
                return Get<int>();
            }
            set
            {
                if (value != Rating)
                {
                    Set(value);
                    LastDataUpdateTimestamp = DateTime.Now;
                }
            }
        }

        public string? Genre
        {
            get; set;
        }
        public string? Artist
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

        #endregion

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

        /// <summary>
        /// Creates identifiers from a Dance object
        /// </summary>
        /// <param name="dance"></param>
        /// <returns>List of strings containing the identifier of the current dance</returns>
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

        /// <summary>
        /// Tries finding a matching dance to the current file, using a best effort approach
        /// </summary>
        /// <param name="file"></param>
        /// <param name="danceIdent"></param>
        /// <returns>the dance identifier of the first dance found, otherwise null</returns>
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

        /// <summary>
        /// Creates a comment from a key/value dictionary using proper delimiters that can be stored
        /// </summary>
        /// <param name="dict"></param>
        /// <returns>a string that can be written to an id3v2 Tag in the correct Format</returns>
        private string CreateNewComment(Dictionary<string, string> dict)
        {
            string newComment = string.Empty;
            foreach (var entry in dict)
            {
                newComment += entry.Key + dataDelimiter + entry.Value + fieldDelimiter;
            }
            return newComment;
        }

        /// <summary>
        /// Creates key/value pairs from a string
        /// </summary>
        /// <param name="comment"></param>
        /// <returns>a key/value dictionary including the correct properties for creating a comment</returns>
        private static Dictionary<string, string> GetFieldsFromComment(string comment)
        {
            if (string.IsNullOrEmpty(comment)) return new Dictionary<string, string>();
            return comment.Split(fieldDelimiter, StringSplitOptions.RemoveEmptyEntries)
                .Select (part => part.Split(dataDelimiter, StringSplitOptions.RemoveEmptyEntries))
                .Where (part => part.Length == 2)
                .ToDictionary (sp => sp[0], sp  => sp[1]);
        }

        /// <summary>
        /// Creates key/value pairs from this object
        /// </summary>
        /// <returns>a key/value dictionary including the correct properties for creating a comment</returns>
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
                fields.Add("Dance", Dance.Name.ToString());
            if (!string.IsNullOrEmpty(Comment))
                fields.Add("Comment", Comment);

            return fields;
        }
        
        /// <summary>
        /// Writes all relevant data from this object to a files id3v2 Tag
        /// </summary>
        /// <returns>true, if data could was written succesfully</returns>
        public bool WriteDataToFile()
        {
            string comment = CreateNewComment(GetFields());

            TagLib.File file;
            TagLib.Id3v2.Tag newTag = new TagLib.Id3v2.Tag();
            TagLib.Id3v2.Tag oldTag = new TagLib.Id3v2.Tag();
            foreach (Uri uri in Uris)
            {
                try
                {
                    file = TagLib.File.Create(uri.LocalPath);
                    oldTag = (TagLib.Id3v2.Tag)file.GetTag(TagLib.TagTypes.Id3v2);
                    if(oldTag != null  && !oldTag.IsEmpty)
                    {
                        oldTag.CopyTo(newTag, true);
                    }
                    break; //Leave Loop as soon as we find one valid Tag 
                }
                catch (Exception e)
                {
                    Logger.LoggerInstance.LogWrite("URI of Track could not be found, when trying to read Tag (WriteDataToFile)  " + uri +"  " + e.Message);
                    return false;
                }
            }

            newTag.Comment = comment;
            newTag.Title = Title;

            if(Dance != null && newTag.FirstGenre != Dance.Name)
            {
                newTag.Genres = newTag.Genres.Prepend(Dance.Name).ToArray();
            }

            //return, if nothing changed
            if (newTag.Comment == oldTag.Comment && newTag.Title == oldTag.Title)
            {
                return false;
            }

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
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Compares all comments from all files corresponding to this track. Using the newest tag, data in the database get's updated
        /// </summary>
        /// <returns>true if data was updated</returns>
        public bool UpdateDataInDatabase()
        {
            TagLib.Id3v2.Tag newestTag = new TagLib.Id3v2.Tag();
            newestTag.DateTagged = DateTime.MinValue;

            TagLib.File file = null;
            TagLib.File newestTagFile = null;

            //get newest Tag from all URIs
            foreach (Uri uri in Uris)
            {
                try
                {
                    file = TagLib.File.Create(uri.LocalPath);
                    if(file != null && file.Tag.DateTagged > newestTag.DateTagged)
                    {
                        file.Tag.CopyTo(newestTag, true);
                        newestTagFile = file;
                    }
                }
                catch (Exception e)
                {
                    Logger.LoggerInstance.LogWrite("Getting tag from file failed   " + e.Message);
                    return false;
                }
            }

            if (newestTagFile != null && newestTag.DateTagged > LastDataUpdateTimestamp)
            {
                var dict = GetFieldsFromComment(newestTag.Comment);
                UpdateData(dict, newestTagFile);
                Logger.LoggerInstance.LogWrite("Updated Track " + Title);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates this objects data using a key/value dictionary using best-effort apporach.
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="file"></param>
        private void UpdateData(Dictionary<string, string> dict, TagLib.File file)
        {
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
            Album = (file.Tag.Album != null) ? file.Tag.Album : string.Empty;
            Genre = (file.Tag.FirstGenre != null) ? file.Tag.FirstGenre : string.Empty;
            ISRC = (file.Tag.ISRC != null) ? file.Tag.ISRC : string.Empty;

            LastDataUpdateTimestamp = DateTime.Now;
        }


        public void CalculateBPM()
        {
            try
            {
                BPMDetector bpmDetector = new BPMDetector(Uris.FirstOrDefault().AbsoluteUri);
                if (bpmDetector.Groups.Length > 0)
                {
                    this.BeatsPerMinute = (uint?) bpmDetector.Groups[0].Tempo;

                    if (bpmDetector.Groups.Length > 1)
                    {
                        Console.WriteLine("Other options are:");
                        for (int i = 1; i < bpmDetector.Groups.Length; ++i)
                        {
                            Console.WriteLine(String.Format("{0} BPM ({1} samples)", bpmDetector.Groups[i].Tempo, bpmDetector.Groups[i].Count));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LoggerInstance.LogWrite("Calculating BPM failed:    " + ex.Message);
            }

        }
    }
}
