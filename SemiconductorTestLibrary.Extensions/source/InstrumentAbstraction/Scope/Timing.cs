using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope
{
    /// <summary>
    /// Defines methods for configuring Scope timing and clock settings.
    /// </summary>
    public static class Timing
    {
        /// <summary>
        /// Configures the timing settings for all sessions in the <see cref="ScopeSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="settings">The <see cref="TimingSettings"/> to configure.</param>
        public static void ConfigureTiming(this ScopeSessionsBundle sessionsBundle, TimingSettings settings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.ConfigureTiming(
                    settings.MinimumSampleRate,
                    settings.MinimumNumberOfPoints,
                    settings.ReferencePosition,
                    settings.NumberOfRecords,
                    settings.EnforceRealtime);
            });
        }

        /// <summary>
        /// Configures the clock settings for all sessions in the <see cref="ScopeSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <param name="settings">The <see cref="ClockSettings"/> to configure.</param>
        public static void ConfigureClock(this ScopeSessionsBundle sessionsBundle, ClockSettings settings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.ConfigureClock(
                    settings.InputClockSource,
                    settings.OutputClockSource,
                    settings.ClockSynchronizationPulseSource,
                    settings.MasterEnabled);
            });
        }

        /// <summary>
        /// Gets the number of records to acquire for each instrument session in the <see cref="ScopeSessionsBundle"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="ScopeSessionsBundle"/> object.</param>
        /// <returns>An array containing the number of records to acquire, one value per instrument session.</returns>
        public static long[] GetNumberOfRecordsToAcquire(this ScopeSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo =>
            {
                return sessionInfo.Session.Timing.NumberOfRecordsToAcquire;
            });
        }
    }
}
