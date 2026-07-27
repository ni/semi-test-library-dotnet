using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Defines utility methods for NI-FGen session.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Resets the instrument to a known state. This VI aborts the generation, clears all routes, and resets session properties to the default values.
        /// This VI does not, however, commit the session properties or configure the device hardware to its default state.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
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
