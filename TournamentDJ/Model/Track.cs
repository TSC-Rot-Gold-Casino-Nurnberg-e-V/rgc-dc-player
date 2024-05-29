using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TournamentDJ.Essentials;

namespace TournamentDJ.Model
{
    public class Track : NotifyObject
    {
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
        public Track(Uri uri)
        {
            TagLib.File file;
            string filePath = uri.LocalPath;
            try
            {
                file = TagLib.File.Create(filePath);
            }
            catch (Exception ex)
            {
                throw;
            }

            Title = (file.Tag.Title != null) ? file.Tag.Title : file.Name;
            Album = (file.Tag.Album != null) ? file.Tag.Album : string.Empty;
            Genre = (file.Tag.FirstGenre != null) ? file.Tag.FirstGenre : string.Empty;
            //Duration = (file.Tag.Length != null) ? TimeSpan.TryParse();
            BeatsPerMinute = file.Tag.BeatsPerMinute;
            Year = (int)file.Tag.Year;
            Uri = uri;
            Dance = SearchDance(file);
            FlaggedAsFavourite = false;
            FlaggedForReview = false;
            Comment = string.Empty;


            int[] ParsedValues = RetrieveSpoerlData(file);
            Characteristic = ParsedValues[1];
            Difficulty = Math.Abs(ParsedValues[2] - 4); //Adapt Values to correct range, CP uses an inverted Range

            if (Difficulty > 4) //Value was undefined or out of Range
            {
                Difficulty = -1;
            }
        }

        public virtual Dance? Dance
        {
            get; set;
        }

        public string? Title
        {
            get; set;
        }

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

        public string? Genre
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

        [Key]
        public Uri Uri
        {
            get; set;
        }


        //returns an int array with lenght 4, containing L, C, B and N, as used in Competition Player by Sebastian Spörl
        private int[] RetrieveSpoerlData(TagLib.File file)
        {
            int[] properties = new int[4];
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = -1;
            }


            //Comment should look like "CPINFL:2/C:2/B:2/N:0T", we only care about the 4 integers
            string pattern = @"-?\d\/-?\d\/-?\d\/-?\d";
            string comment = file.Tag.Comment;
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


        private Dance SearchDance(TagLib.File file)
        {
            foreach (var dance in DatabaseUtility.Dances)
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

                string genre = string.Empty;
                //Compare with genre first
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

                //search in Name second, as this is a lot slower
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
            return null;
        }

    }
}
