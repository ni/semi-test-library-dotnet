using NationalInstruments.ModularInstruments.NIScope;
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
    }
}
