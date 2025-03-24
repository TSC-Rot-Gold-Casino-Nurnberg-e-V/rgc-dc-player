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
using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using TournamentDJ.Essentials;

namespace TournamentDJ.Deduplication
{
    public class FingerprintBase
    {
        static InMemoryModelService modelService; // Store fingerprints in memory
        static NAudioService nAudioService;
        private static FrequencyRange frequencyRange;

        static public void Init()
        {
            modelService = new InMemoryModelService();
            nAudioService = new NAudioService();
            frequencyRange = new FrequencyRange(318, 4000);
        }



        public static async Task CreateFingerprints(IEnumerable<Track> tracksToFingerprint, int threads = 6)
        {
            int i = 0;
            var splits = from track in tracksToFingerprint
                            group track by i++ % threads into part
                            select part.AsEnumerable();

            List<Task> tasks = new List<Task>();
            int worker = 1;
            foreach(IEnumerable<Track> tracks in splits)
            {
                tasks.Add(Task.Run(() => CreateFingerprintsWorker(tracks, worker)));
                worker++;
            }

            await Task.WhenAll(tasks);

            return;
        }



        public static void CreateFingerprintsWorker(IEnumerable<Track> tracks, int worker)
        {
            Logger.LoggerInstance.LogWrite("Created Worker: " + worker+  " with " + tracks.Count() + " Tracks to do");
            var audioService = new NAudioService();

            foreach (Track track in tracks)
            {

                using (var audioFileReader = new AudioFileReader(track.Uris.FirstOrDefault().LocalPath))
                {
                    var samples = audioService.ReadMonoSamplesFromFile(track.Uris.FirstOrDefault().LocalPath, 11024);
                    var avHashes = FingerprintCommandBuilder.Instance
                        .BuildFingerprintCommand()
                        .From(samples)
                        .WithFingerprintConfig(config =>
                        {
                            // audio configuration
                            config.Audio = new DefaultFingerprintConfiguration();
                            config.Audio.SampleRate = 11024;
                            config.Audio.FrequencyRange = frequencyRange;
                            // video configuration
                            config.Video = new DefaultVideoFingerprintConfiguration();
                            return config;
                        })
                        .UsingServices(audioService)
                        .Hash()
                        .Result;

                    track.Fingerprints = avHashes;
                    Logger.LoggerInstance.LogWrite("Worker: " + worker + " finished a Track");

                }
            }
        }



        public static AVQueryResult QueryFingerprints(AVHashes avHashes, IModelService modelService, NAudioService audioService)
        {
            var queryResult = QueryCommandBuilder.Instance
                .BuildQueryCommand()
                .From(avHashes)
                .WithQueryConfig(config =>
                {
                    config.FingerprintConfiguration.Audio.SampleRate = 11024;
                    config.Audio.ThresholdVotes = 8;
                    config.Audio.FrequencyRange = frequencyRange;
                    return config;
                })
                .UsingServices(modelService, audioService)
                .Query()
                .Result;

            return queryResult;
        }

        public static void AddFingerprintToModel(Track track)
        {
            var fingerprints = track.Fingerprints;
            if(fingerprints != null)
            {
                modelService.Insert(new TrackInfo(track.Id.ToString(), track.Title, track.Artist), track.Fingerprints);
            }
        }

        public static int Compare(Track track)
        {
            var queryResult = QueryFingerprints(track.Fingerprints, modelService, nAudioService);

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
