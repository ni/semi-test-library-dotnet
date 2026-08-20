using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen
{
    /// <summary>
    /// Container for Clock extension methods
    /// </summary>
    public static class Clock
    {
        /// <summary>
        /// Configure Clock Mode
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="clockMode">The clock mode to configure.</param>
        public static void ConfigureClockMode(this FgenSessionsBundle sessionsBundle, string clockMode)
        { }

        /// <summary>
        /// Configure Sample Source
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="source">The sample source to configure.</param>
        public static void ConfigureSampleSource(this FgenSessionsBundle sessionsBundle, string source)
        { }

        /// <summary>
        /// Configure Reference Clock
        /// </summary>
        /// <param name="sessionsBundle">The FGen sessionsBundle.</param>
        /// <param name="source">The reference clock source.</param>
        /// <param name="frequency">The reference clock frequency.</param>
        public static void ConfigureReferenceClock(this FgenSessionsBundle sessionsBundle, string source, double frequency)
        { }
    }
}
