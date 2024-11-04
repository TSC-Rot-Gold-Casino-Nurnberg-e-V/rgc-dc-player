namespace TournamentDJ.Model
{
    class WarmupPlayer : Player
    {

        public bool OnlyUseUncategorized { get; set; } = true;
        public override void GetNextTrack()
        {
            //MedPlayer.Pause();

            if (TrackPlaying != null && TracksToPlay.Tracks.Count > 0)
            {
                TracksPlayed.Tracks.Add(TrackPlaying);
            }

            if (TracksToPlay.Tracks.Count > 0)
            {
                TrackPlaying = TracksToPlay.Tracks[0];
                TracksToPlay.Tracks.RemoveAt(0);
            }

            else
            {
                TracksToPlay = TrackListBuilder.CreateDanceRound(SelectedDanceRound, cantBeFavourite: true, overrideParams: true, onlyUseUncategorized: OnlyUseUncategorized);
            }
        }

        public void StopAutoplay()
        {
            MedPlayer.AutoPlay = false;
        }
    }
}
