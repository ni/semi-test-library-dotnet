using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    ///  Defines methods for NI-Fgen clock configurations.
    /// </summary>
    public static class Clock
    {
        /// <summary>
        /// Configures the sample clock mode of the uderlying signal generators.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="clockMode">Specifies the clcok mode."/></param>
        /// <Remarks>
        /// <para>
        /// For the underlying signal generators that support it, this property allows switching the sample clock to a HighResolution clocking mode.
        /// When in DivideDown sampling mode,the sample rate can only be set to certain frequencies, based on dividing down the sample clock.However, in HighResolution mode, the samplerate may be set to any value.
        /// </para>
        /// </Remarks>
        /// <exception cref="NISemiconductorTestException">
        /// The operation cannot be completed because the property is not configurable while it is generating a signal. If you want to change the clock mode, call AbortGeneration methodor wait for the generation to complete.
        /// </exception>
        public static void ConfigureClockMode(this FgenSessionsBundle sessionsBundle, ClockMode clockMode)
            {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.SampleClock.ClockMode = clockMode;
            });
        }

        /// <summary>
        /// Configures the sample clock source of the uderlying signal generators.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="source">Specifies the clcok source."/></param>
        /// <Remarks>
        /// The default value is "OnboardClock".
        /// <para>
        /// Valid values for clockSource are "OnboardClock", "PXI_Trig0" to "PXI_Trig7".
        /// </para>
        /// </Remarks>
        /// <exception cref="NISemiconductorTestException">
        /// The operation cannot be completed because the property is not configurable while it is generating a signal.
        /// If you want to change the clock mode, call AbortGeneration methodor wait for the generation to complete.
        /// </exception>
        public static void ConfigureSampleSource(this FgenSessionsBundle sessionsBundle, string source)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.SampleClock.Source = source;
            });
        }

        /// <summary>
        /// Configures the sample clock source of the uderlying signal generators.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="source">Specifies the reference clcok source."/></param>
        /// <param name="frequency">Specifies the reference clock frequency in hertz (Hz)."/></param>
        /// <Remarks>
        /// <para>
        /// The signal generator uses the reference clock to tune the sample clock timebase of the signalgenerator so that the frequency, stability, and accuracy of the sample clock timebase matches thatof the reference clock.
        /// </para>
        /// </Remarks>
        /// <exception cref="NISemiconductorTestException">
        /// The operation cannot be completed because the property is not configurable while it is generating a signal. If you want to change the clock mode, call AbortGeneration methodor wait for the generation to complete.
        /// Or The value set for source is an invalid value.
        /// Or The value set for frequency is an invalid value.
        /// </exception>
        public static void ConfigureReferenceClock(this FgenSessionsBundle sessionsBundle, string source, double frequency)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.ReferenceClock.Configure(source, frequency);
            });
        }
    }
}
