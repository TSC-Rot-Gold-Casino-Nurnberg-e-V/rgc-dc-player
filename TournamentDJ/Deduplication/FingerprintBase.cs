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

namespace TournamentDJ.Deduplication
{
    public class FingerprintBase
    {
        static InMemoryModelService modelService; // Store fingerprints in memory
        static NAudioService audioService; // Use NAudio for audio processing
        private static FrequencyRange frequencyRange;

        static public void Init()
        {
            modelService = new InMemoryModelService();
            audioService = new NAudioService();
            frequencyRange = new FrequencyRange(318, 4000);
        }

        public static AVHashes CreateFingerprints(Uri uri)
        {
            using (var audioFileReader = new AudioFileReader(uri.LocalPath))
            {
                var samples = audioService.ReadMonoSamplesFromFile(uri.LocalPath, 11024);
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

                return avHashes;
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

        public static void CreateAndAddFingerprint(Uri uri, string id, string title, string artist)
        {
            var fingerprints = CreateFingerprints(uri);
            modelService.Insert(new TrackInfo(id, title, artist), fingerprints);
        }

        public static void AddFingerprint(Track track, AVHashes fingerprints)
        {
            modelService.Insert(new TrackInfo(track.Id.ToString(), track.Title, track.Artist), fingerprints);
        }

        public static int Compare(Uri uri, out AVHashes fingerprint)
        {
            fingerprint = CreateFingerprints(uri);
            var queryResult = QueryFingerprints(fingerprint, modelService, audioService);

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
