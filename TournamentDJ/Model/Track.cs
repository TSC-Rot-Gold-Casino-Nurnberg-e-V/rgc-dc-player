using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
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
            Genre = (file.Tag.FirstGenre != null)  ? file.Tag.FirstGenre : string.Empty;
            //Duration = (file.Tag.Length != null) ? TimeSpan.TryParse();
            BeatsPerMinute = file.Tag.BeatsPerMinute;
            Year = (int) file.Tag.Year;
            Uri = uri;
            Dance = null;
            FlaggedAsFavourite = false;
            FlaggedForReview = false;
            Difficulty = -1;
            Characteristic = -1;
            Comment = string.Empty;

        }

        public virtual Dance? Dance {
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

    }
}
