using System;

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
        /// <param name="clockMode">The clock mode to configure.</param>
        public static void ConfigureClockMode(ClockMode clockMode)
        { }

        /// <summary>
        /// Configure Sample Source
        /// </summary>
        /// <param name="source">The sample source to configure.</param>
        public static void ConfigureSampleSource(string source)
        { }

        /// <summary>
        /// Configure Reference Clock
        /// </summary>
        /// <param name="source">The reference clock source.</param>
        /// <param name="frequency">The reference clock frequency.</param>
        public static void ConfigureReferenceClock(string source, double frequency)
        { }
    }
}
