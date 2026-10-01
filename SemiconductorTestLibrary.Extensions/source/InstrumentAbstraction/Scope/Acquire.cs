using NationalInstruments.ModularInstruments.NIScope;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for acquisition related operations on the NI-Scope session.
    /// </summary>
    public static class Acquire
    {
        #region Methods on ScopeSessionsBundle

        /// <summary>
        /// Gets the acquisition status of each session in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>The per-session <see cref="ScopeAcquisitionStatus"/> values.</returns>
        public static ScopeAcquisitionStatus[] GetAcquisitionStatus(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Measurement.Status());
        }

        /// <summary>
        /// Gets the record length of each session in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>The per-session record length values.</returns>
        public static long[] GetRecordLength(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Acquisition.RecordLength);
        }

        /// <summary>
        /// Gets the resolution, in bits, of each session in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>The per-session resolution values.</returns>
        public static long[] GetResolution(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Acquisition.Resolution);
        }

        /// <summary>
        /// Gets the sample mode of each session in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>The per-session <see cref="ScopeSampleMode"/> values.</returns>
        public static ScopeSampleMode[] GetSampleMode(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Acquisition.SampleMode);
        }

        /// <summary>
        /// Gets the effective sample rate, in samples per second, of each session in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>The per-session sample rate values.</returns>
        public static double[] GetSampleRate(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Session.Acquisition.SampleRate);
        }
        #endregion
    }
}
