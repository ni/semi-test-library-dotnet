using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    /// Defines methods for NI-Fgen clock configurations.
    /// </summary>
    public static class Clock
    {
        /// <summary>
        /// Configures the sample clock mode of the underlying signal generators.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="clockMode">Specifies the <see cref="ClockMode"/>.</param>
        /// <remarks>
        /// <para>
        /// For the underlying signal generators that support it, this method allows switching the sample clock to <see cref="ClockMode.HighResolution"/> mode.
        /// When in <see cref="ClockMode.DivideDown"/> mode, the sample rate can only be set to certain frequencies, based on dividing down the sample clock.
        /// However, in <see cref="ClockMode.HighResolution"/> mode, the sample rate may be set to any value.
        /// </para>
        /// <para>
        /// Supported clock modes vary by signal generator model. Some signal generators support <see cref="ClockMode.Automatic"/> and <see cref="ClockMode.HighResolution"/> modes,
        /// while others support <see cref="ClockMode.Automatic"/> and <see cref="ClockMode.DivideDown"/> modes. Refer to the documentation of the underlying signal generator for its supported clock modes.
        /// If the specified clock mode is not supported by the underlying signal generator, an exception will be thrown.
        /// </para>
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// The operation cannot be completed because the device is not configurable while it is generating a signal. If you want to change the clock mode, call the Abort method or wait for the generation to complete.
        /// </exception>
        /// <exception cref="NISemiconductorTestException">
        /// Unsupported clock mode is specified for the underlying signal generators.
        /// </exception>
        public static void ConfigureClockMode(this FgenSessionsBundle sessionsBundle, ClockMode clockMode)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.SampleClock.ClockMode = clockMode;
            });
        }

        /// <summary>
        /// Configures the sample clock source of the underlying signal generators.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="source">Specifies the sample clock source.</param>
        /// <remarks>
        /// <para>
        /// Supported sample clock sources vary by signal generator model. Most signal generators support "OnboardClock" and "PXI_Trig0" to "PXI_Trig7",
        /// while some signal generators with a Clock In terminal support "ClkIn" and may not have an onboard clock.
        /// Refer to the documentation of the underlying signal generator for its supported sample clock sources.
        /// </para>
        /// <para>
        /// The default value is "OnboardClock".
        /// </para>
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// The operation cannot be completed because the device is not configurable while it is generating a signal.
        /// If you want to change the sample clock source, call the Abort method or wait for the generation to complete.
        /// </exception>
        public static void ConfigureSampleSource(this FgenSessionsBundle sessionsBundle, string source)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.SampleClock.Source = source;
            });
        }

        /// <summary>
        /// Configures the reference clock source and frequency of the underlying signal generators, which is used to phase-lock the sample clock timebase.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="source">Specifies the reference clock source. Set to "None" to unset the reference clock.</param>
        /// <param name="frequency">Specifies the reference clock frequency in hertz (Hz).</param>
        /// <remarks>
        /// <para>
        /// The signal generator uses the reference clock to tune the sample clock timebase of the signal generator so that the frequency, stability, and accuracy of the sample clock timebase match that of the reference clock.
        /// </para>
        /// <para>
        /// Supported reference clock frequencies vary by signal generator model. Some signal generators support a reference clock frequency range of 10 MHz to 400 MHz,
        /// while others support only a fixed reference clock frequency of 100 MHz. Refer to the documentation of the underlying signal generator for its supported reference clock frequencies.
        /// If the specified frequency is not supported by the underlying signal generator, an exception will be thrown.
        /// </para>
        /// <para>
        /// Supported reference clock sources also vary by signal generator model:
        /// <list type="bullet">
        /// <item><description>Some signal generators support "None", "OnboardRefClk", "OnboardReferenceClk", "OnboardReferenceClock", "PXI_Clk", and "PXIe_Clk100".</description></item>
        /// <item><description>Other signal generators support only "None", "PXI_Clk", and "PXI_Clk10". These signal generators do not have an onboard reference clock and do not support "PXIe_Clk100".</description></item>
        /// </list>
        /// Refer to the documentation of the underlying signal generator for its supported reference clock sources.
        /// If the specified source is not supported by the underlying signal generator, an exception will be thrown.
        /// </para>
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">
        /// The operation cannot be completed because the device is not configurable while it is generating a signal. If you want to change the reference clock, call the Abort method or wait for the generation to complete.
        /// Or, the value set for source is invalid.
        /// Or, the value set for frequency is invalid.
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
