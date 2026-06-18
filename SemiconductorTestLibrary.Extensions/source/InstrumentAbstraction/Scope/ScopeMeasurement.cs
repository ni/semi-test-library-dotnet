using NationalInstruments.SemiconductorTestLibrary.Common;

using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for performing oscilloscope measurements.
    /// </summary>
    public static class ScopeMeasurement
    {
        /// <summary>
        /// Reads waveform data from the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="channelName">The name of the channel to read from.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="recordLength">The number of samples to read.</param>
        /// <param name="waveforms">The waveform array to populate with the data.</param>
        /// <returns>An array of waveforms containing the measurement data.</returns>
        public static AnalogWaveformCollection<double>[] ReadWaveform(this ScopeSessionsBundle sessionsBundle, string channelName, PrecisionTimeSpan timeout, long recordLength, AnalogWaveformCollection<double>[] waveforms)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Channels[channelName].Measurement.Read(timeout, recordLength, waveforms[0]);
            });
        }

        /// <summary>
        /// Fetches waveform data from the specified channel without initiating a new acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="channelName">The name of the channel to fetch from.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="pointsToFetch">The number of points to fetch. Use -1 to fetch all available points.</param>
        /// <param name="waveforms">The waveform array to populate with the data.</param>
        /// <returns>An array of waveforms containing the fetched data.</returns>
        public static AnalogWaveformCollection<double>[] FetchWaveform(this ScopeSessionsBundle sessionsBundle, string channelName, PrecisionTimeSpan timeout, int pointsToFetch, AnalogWaveformCollection<double>[] waveforms)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Channels[channelName].Measurement.FetchDouble(timeout, pointsToFetch, waveforms[0]);
            });
        }

        /// <summary>
        /// Fetches waveform data with detailed waveform information from the specified channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="channelName">The name of the channel to fetch from.</param>
        /// <param name="timeout">The maximum time to wait for the data.</param>
        /// <param name="pointsToFetchMax">The maximum number of points to fetch.</param>
        /// <param name="waveforms">The waveform array to populate with the data.</param>
        /// <param name="waveformInfo">Output parameter containing detailed waveform information.</param>
        /// <returns>An array of waveforms containing the fetched data.</returns>
        // public static AnalogWaveformCollection<double>[][] FetchWaveformWithInfo(this ScopeSessionsBundle sessionsBundle, string channelName, PrecisionTimeSpan timeout, int pointsToFetchMax, AnalogWaveformCollection <double>[] waveforms, out WaveformInfo[][] waveformInfo)
        // {
        //    var waveformInfoList = new List<WaveformInfo[]>();
        //    var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
        //    {
        //        WaveformInfo[] info;
        //        var wfm = sessionInfo.Session.Channels[channelName].Measurement.FetchDouble(timeout, pointsToFetchMax, waveforms, out info);
        //        waveformInfoList.Add(info);
        //        return wfm;
        //    });
        //    waveformInfo = waveformInfoList.ToArray();
        //    return results;
        // }
    }
}