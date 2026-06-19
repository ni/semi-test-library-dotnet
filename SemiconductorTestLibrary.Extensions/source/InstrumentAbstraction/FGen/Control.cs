using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Control class for controlling waveform generation operations.
    /// </summary>
    public static class Control
    {
        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(this FgenSessionsBundle sessionsBundle, OutputMode outputMode)
        { }

        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(this FgenSessionsBundle sessionsBundle, SiteData<OutputMode> outputMode)
        { }

        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(this FgenSessionsBundle sessionsBundle, PinSiteData<OutputMode> outputMode)
        { }

        /// <summary>
        /// Commit.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void Commit(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// Initiate.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void Initiate(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// IsDone.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void IsDone(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// WaitUntilDone.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void WaitUntilDone(this FgenSessionsBundle sessionsBundle)
        { }

        /// <summary>
        /// Abort.
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        public static void Abort(this FgenSessionsBundle sessionsBundle)
        { }
    }
}
