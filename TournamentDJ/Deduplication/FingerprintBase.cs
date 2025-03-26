using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using SoundFingerprinting;
using SoundFingerprinting.Audio.NAudio;
using SoundFingerprinting.Builder;
using SoundFingerprinting.Configuration.Frames;
using SoundFingerprinting.Configuration;
using SoundFingerprinting.Data;
using SoundFingerprinting.InMemory;
using SoundFingerprinting.Query;
using TournamentDJ.Model;
using TournamentDJ.Essentials;
using SoundFingerprinting.DAO;
using System.Diagnostics.Metrics;

namespace TournamentDJ.Deduplication
{
    public class FingerprintBase
    {
        static InMemoryModelService ModelService; // Store fingerprints in memory
        static NAudioService NAudioService;
        private static FrequencyRange FrequencyRange;
        private static int SampleRate;

        static public void Init()
        {
            ModelService = new InMemoryModelService();
            NAudioService = new NAudioService();
            SampleRate = 5512 * 2;
            FrequencyRange = new FrequencyRange(318, 4000);
        }


        /// <summary>
        /// Creates FIngerprints using multiple workers
        /// </summary>
        /// <param name="tracksToFingerprint"></param>
        /// <returns>A list of tracks with computed Fingerprints</returns>
        public static async Task<IEnumerable<Track>> CreateFingerprints(IEnumerable<Track> tracksToFingerprint)
        {
            int threads = Environment.ProcessorCount;
            int i = 0;
            var splits = from track in tracksToFingerprint
                            group track by i++ % threads into part
                            select part.AsEnumerable();

            List<Task<IEnumerable<Track>>> tasks = new List<Task<IEnumerable<Track>>>();
            foreach(IEnumerable<Track> tracks in splits)
            {
                tasks.Add(Task.Run(() => CreateFingerprintsWorker(tracks)));
            }

            await Task.WhenAll(tasks);

            List<Track> computedTracks = new List<Track>();

            foreach(Task<IEnumerable<Track>> task in tasks)
            {
               foreach(var track in task.Result) 
               {
                    computedTracks.Add(track);
               }
            }

            return computedTracks;
        }


        /// <summary>
        /// Creates a new FingerprintWorker that creates Fingerprints for all given tracks.
        /// </summary>
        /// <param name="tracks"></param>
        /// <returns>returns a List with the finished tracks</returns>
        private static IEnumerable<Track> CreateFingerprintsWorker(IEnumerable<Track> tracks)
        {
            var audioService = new NAudioService();

            List<Track> computedTracks = new List<Track>();

            foreach (Track track in tracks)
            {
                try
                {
                    using (var audioFileReader = new AudioFileReader(track.Uris.FirstOrDefault().LocalPath))
                    {
                        double start = 15;
                        double duration = 20;
                        if (track.Duration.TotalSeconds > 40)
                        {
                            start = Math.Floor(track.Duration.TotalSeconds) / 2;
                        }
                        var samples = audioService.ReadMonoSamplesFromFile(track.Uris.FirstOrDefault().LocalPath, SampleRate, duration, start);


                        AVHashes avHashes = FingerprintCommandBuilder.Instance
                            .BuildFingerprintCommand()
                            .From(samples)
                            .WithFingerprintConfig(config =>
                            {
                                // audio configuration
                                config.Audio = new DefaultFingerprintConfiguration();
                                config.Audio.SampleRate = SampleRate;
                                config.Audio.FrequencyRange = FrequencyRange;
                                // video configuration
                                config.Video = new DefaultVideoFingerprintConfiguration();
                                return config;
                            })
                            .UsingServices(audioService)
                            .Hash()
                            .Result;



                        track.Fingerprints = avHashes;
                        computedTracks.Add(track);

                    }
                }
                catch (Exception ex)
                {
                    Logger.LoggerInstance.LogWrite("Creating Fingerprint for Track " + track.Uris.FirstOrDefault() + " failed " + ex.Message);
                }
            }

            return computedTracks;
        }


        /// <summary>
        /// Checks if any tracks can be found, that match avHashes
        /// </summary>
        /// <param name="avHashes">Hashes to compare to</param>
        /// <param name="modelService">Database Service</param>
        /// <param name="audioService"></param>
        /// <returns>AVQueryResult containing all possible matches</returns>
        private static AVQueryResult QueryFingerprints(AVHashes avHashes, IModelService modelService, NAudioService audioService)
        {
            var queryResult = QueryCommandBuilder.Instance
                .BuildQueryCommand()
                .From(avHashes)
                .WithQueryConfig(config =>
                {
                    config.FingerprintConfiguration.Audio.SampleRate = SampleRate;
                    config.Audio.ThresholdVotes = 5;
                    config.Audio.FrequencyRange = FrequencyRange;
                    return config;
                })
                .UsingServices(modelService, audioService)
                .Query()
                .Result;

            return queryResult;
        }

        /// <summary>
        /// Insert Fingerprint to static Model
        /// </summary>
        /// <param name="track"></param>
        public static void AddFingerprintToModel(Track track)
        {
            var fingerprints = track.Fingerprints;
            if(fingerprints != null)
            {
                ModelService.Insert(new TrackInfo(track.Id.ToString(), track.Title, track.Artist), track.Fingerprints);
            }
        }

        /// <summary>
        /// Emptys the static Model that contains the fingerprints
        /// </summary>
        public static void ClearModel()
        {
            ModelService = new InMemoryModelService();
        }

        /// <summary>
        /// Find the ID of the best match with reasonable confidence.
        /// </summary>
        /// <param name="track"></param>
        /// <returns>-1, if no Track was found, otherwise the ID of the best Match</returns>
        public static int FindBestMatchingTrack(Track track)
        {
            var queryResult = QueryFingerprints(track.Fingerprints, ModelService, NAudioService);

            if (queryResult != null && queryResult.ContainsMatches)
            {
                int id = -1;
                if (queryResult.BestMatch.Audio.Confidence > 0.5)
                {
                    int.TryParse(queryResult.BestMatch.TrackId, out id);
                }

                return id;
            }
            else
            {
                return -1;
            }
        }
    }
}
