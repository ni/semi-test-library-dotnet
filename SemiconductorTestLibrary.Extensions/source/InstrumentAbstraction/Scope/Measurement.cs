using NationalInstruments.SemiconductorTestLibrary.Common;

using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for performing oscilloscope measurements.
    /// </summary>
    public static class Measurement
    {
        /// <summary>
        /// Reads waveform data from the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="recordLength">The number of samples to read.</param>
        /// <returns>An array of waveforms containing the measurement data.</returns>
        public static AnalogWaveformCollection<double>[] ReadWaveform(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, long recordLength)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Channels[sessionInfo.AllChannelsString].Measurement.Read(timeout, recordLength, null);
            });
        }

        /// <summary>
        /// Fetches waveform data from the specified channel without initiating a new acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="pointsToFetch">The number of points to fetch. Use -1 to fetch all available points.</param>
        /// <returns>An array of waveforms containing the fetched data.</returns>
        public static AnalogWaveformCollection<double>[] FetchWaveform(this ScopeSessionsBundle sessionsBundle, PrecisionTimeSpan timeout, int pointsToFetch)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Channels[sessionInfo.AllChannelsString].Measurement.FetchDouble(timeout, pointsToFetch, null);
            });
        }
    }
}