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
            Uri = uri;
            Dance = null;
        }

        public virtual Dance? Dance {
            get; set;
        }

        public string? Title
        {
            get; set;
        }

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
