using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    /// Defines utility methods for NI-FGen.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Resets the signal generator to a known state.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <remarks>
        /// This method aborts signal generation, resets all attributes to default values, and stops the export of all external signals and events.
        /// </remarks>
        public static void Reset(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Utility.Reset();
            });
        }

        /// <summary>
        /// Performs a hard reset on the device. Generation is stopped, all routes are released, external bidirectional terminals are tri-stated,
        /// FPGAs are reset, hardware is configured to its default state, and all session properties are reset to their default states.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        public static void ResetDevice(this FgenSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Utility.ResetDevice();
            });
        }
    }
}
