using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

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
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(OutputMode outputMode)
        { }

        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(SiteData<OutputMode> outputMode)
        { }

        /// <summary>
        /// Configure output mode.
        /// </summary>
        /// <param name="outputMode">The output mode to configure.</param>
        public static void ConfigureOutputMode(PinSiteData<OutputMode> outputMode)
        { }

        /// <summary>
        /// Commit.
        /// </summary>
        public static void Commit()
        { }

        /// <summary>
        /// Initiate.
        /// </summary>
        public static void Initiate()
        { }

        /// <summary>
        /// IsDone.
        /// </summary>
        public static void IsDone()
        { }

        /// <summary>
        /// WaitUntilDone.
        /// </summary>
        public static void WaitUntilDone()
        { }

        /// <summary>
        /// Abort.
        /// </summary>
        public static void Abort()
        { }

        /// <summary>
        /// Configure channel
        /// </summary>
        public static void ConfigureChannel()
        { }
    }
}
