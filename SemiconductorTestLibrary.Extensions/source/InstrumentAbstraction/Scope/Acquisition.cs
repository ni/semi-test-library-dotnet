using System.Linq;

using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
     /// Defines methods for Acquisition on oscilloscope sessions.
     /// </summary>
    public static class Acquisition
    {
        /// <summary>
        /// Configures the acquisition type for the oscilloscope.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="acquisitionType">The type of acquisition to perform.</param>
        public static void ConfigureAcquisitionType(this ScopeSessionsBundle sessionsBundle, ScopeAcquisitionType acquisitionType)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Acquisition.Type = acquisitionType;
            });
        }

        /// <summary>
        /// Gets the acquisition status for the oscilloscope.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>A per-session array of acquisition statuses.</returns>
        public static ScopeAcquisitionStatus[] GetAcquisitionStatus(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Measurement.Status();
            });
        }

        /// <summary>
        /// Gets the configured record length for the acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>A per-session array of record lengths.</returns>
        public static long[] GetRecordLength(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Acquisition.RecordLength;
            });
        }

        /// <summary>
        /// Gets the configured resolution for the acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>A per-session array of resolutions.</returns>
        public static long[] GetResolution(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Acquisition.Resolution;
            });
        }

        /// <summary>
        /// Gets the configured sample mode for the acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>A per-session array of sample modes.</returns>
        public static ScopeSampleMode[] GetSampleMode(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Acquisition.SampleMode;
            });
        }

        /// <summary>
        /// Gets the configured sample rate for the acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>A per-session array of sample rates in samples per second.</returns>
        public static double[] GetSampleRate(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Acquisition.SampleRate;
            });
        }

        /// <summary>
        /// Reads waveform data from all channels in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="numberOfSamples">The number of samples to read.</param>
        /// <returns>PinSiteData of an array of samples, where each element corresponds to one channel in the bundle.</returns>
        public static PinSiteData<double[]> Read(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, long numberOfSamples)
        {
            return sessionsBundle
                .DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
                {
                    var waveform = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString].Measurement
                        .Read(timeout, numberOfSamples, null)
                        .First();
                    return waveform.Samples.Select(sample => sample.Value).ToArray();
                });
        }

        /// <summary>
        /// Reads waveform data from all channels in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="numberOfSamples">The number of samples to read.</param>
        /// <returns>An array of waveforms, where each element corresponds to one channel in the bundle.</returns>
        public static PinSiteData<AnalogWaveform<double>> ReadWaveform(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, long numberOfSamples)
        {
            return sessionsBundle
                .DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
                {
                    return sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString]
                        .Measurement
                        .Read(timeout, numberOfSamples, null)
                        .First();
                });
        }

        /// <summary>
        /// Fetches waveform data from the specified channel without initiating a new acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="pointsToFetch">The number of points to fetch. Use -1 to fetch all available points.</param>
        /// <returns>PinSiteData of an array of samples, where each element corresponds to one channel in the bundle.</returns>
        public static PinSiteData<double[]> Fetch(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, int pointsToFetch)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var waveform = sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString].Measurement.FetchDouble(timeout, pointsToFetch, null).First();
                return waveform.Samples.Select(sample => sample.Value).ToArray();
            });
        }

        /// <summary>
        /// Fetches waveform data from the specified channel without initiating a new acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="pointsToFetch">The number of points to fetch. Use -1 to fetch all available points.</param>
        /// <returns>An array of waveforms, where each element corresponds to one channel in the bundle.</returns>
        public static PinSiteData<AnalogWaveform<double>> FetchWaveform(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, int pointsToFetch)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                return sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString].Measurement.FetchDouble(timeout, pointsToFetch, null).First();
            });
        }
    }
}
