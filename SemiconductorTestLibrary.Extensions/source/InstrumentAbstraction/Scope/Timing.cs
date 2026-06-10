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
        /// <param name="sampleRateMin">The minimum sample rate in samples per second.</param>
        /// <param name="recordLengthMin">The minimum record length in samples.</param>
        /// <param name="referencePosition">The reference position as a percentage of the record length.</param>
        /// <param name="numberOfRecords">The number of records to acquire.</param>
        /// <param name="enforceRealTime">Whether to enforce real-time acquisition.</param>
        public static void ConfigureTiming(this ScopeSessionsBundle sessionsBundle, double sampleRateMin, int recordLengthMin, double referencePosition, int numberOfRecords, bool enforceRealTime)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.ConfigureTiming(sampleRateMin, recordLengthMin, referencePosition, numberOfRecords, enforceRealTime);
            });
        }
    }
}
