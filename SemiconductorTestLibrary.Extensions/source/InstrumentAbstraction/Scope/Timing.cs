using NationalInstruments.ModularInstruments.NIScope;

using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for configuring oscilloscope trigger behavior.
    /// </summary>
    public static class Timing
    {
        /// <summary>
        /// Configures the timing parameters for the acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="timingSettings">The timing settings for the acquisition.</param>
        public static void ConfigureTiming(this ScopeSessionsBundle sessionsBundle, TimingSettings timingSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.ConfigureTiming(timingSettings.SampleRateMin, timingSettings.RecordLengthMin, timingSettings.ReferencePosition, timingSettings.NumberOfRecords, timingSettings.EnforceRealTime);
            });
        }

        /// <summary>
        /// Configures the clock settings for the oscilloscope.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="clockSettings">The clock settings for the oscilloscope.</param>
        public static void ConfigureClock(this ScopeSessionsBundle sessionsBundle, ClockSettings clockSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.ConfigureClock(clockSettings.InputClockSource, clockSettings.OutputClockSource, clockSettings.ClockSynchronizationPulseSource, clockSettings.MasterEnabled);
            });
        }

        /// <summary>
        /// Gets the number of records to acquire for each instrument and channel in the sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>A per-session array of the number of records to acquire.</returns>
        public static long[] GetNumberOfRecordsToAcquire(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Timing.NumberOfRecordsToAcquire;
            });
        }
    }
}
