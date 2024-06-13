using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TournamentDJ.Essentials;
using TournamentDJ.Model;

namespace TournamentDJ.ViewModel
{

    class SelectSpecificTrackViewModel : NotifyObject
    {
        public bool isPlaying = false;
        public bool playOnClick = false;
        public SelectSpecificTrackViewModel() 
        {
            //trackListEditorViewModel = new TrackListEditorViewModel(this);
            //Player = new Player();
            //CreateCommands();
            //FailedUris = new ObservableCollection<Uri>();
            //TracksToAdd = new ObservableCollection<Track>();
            //FilteredTracks = new ObservableCollection<Track>();
            //TrackFilterString = string.Empty;

        }

        

        public void OnWindowClosing(object sender, CancelEventArgs e)
        {
        }
    }
}
