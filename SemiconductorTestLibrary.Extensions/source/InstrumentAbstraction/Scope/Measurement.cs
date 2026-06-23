using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for performing oscilloscope measurements.
    /// </summary>
    public static class Measurement
    {
        /// <summary>
        /// Reads waveform data from all channels in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="recordLength">The number of samples to read.</param>
        /// <returns>An array of waveforms, where each element corresponds to one channel in the bundle.</returns>
        public static PinSiteData<AnalogWaveform<double>> ReadWaveform(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, long recordLength)
        {
            return sessionsBundle
                .DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
                {
                    return sessionInfo.Session.Channels[sitePinInfo.IndividualChannelString]
                        .Measurement
                        .Read(timeout, recordLength, null)
                        .First();
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