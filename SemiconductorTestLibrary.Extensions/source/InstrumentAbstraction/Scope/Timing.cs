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
        public static void ConfigureTiming(this ScopeSessionsBundle sessionsBundle, ScopeTimingSettings timingSettings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.ConfigureTiming(timingSettings.SampleRateMin.Value, timingSettings.RecordLengthMin.Value, timingSettings.ReferencePosition.Value, timingSettings.NumberOfRecords.Value, timingSettings.EnforceRealTime.Value);
            });
        }
    }
}
