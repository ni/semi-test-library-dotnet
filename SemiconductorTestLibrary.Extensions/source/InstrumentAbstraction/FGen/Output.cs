using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIFgen;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen
{
    /// <summary>
    ///  Defines methods for NI-Fgen output configurations.
    /// </summary>
    public static class Output
    {
        /// <summary>
        /// Configures the signal generator to generate a signal at the output connector.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="outputEnable">Specifies the state of the output enable relay. Set outputEnable to <see langword="true"/> to enable the relay.</param>
        /// <remarks>
        /// Use this method to configure whether the signal that the signal generator produces appears at the output connector.
        /// </remarks>
        public static void ConfigureOutputEnabled(this FgenSessionsBundle sessionsBundle, bool outputEnable)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Output.SetEnabled(sessionInfo.AllChannelsString, outputEnable);
            });
        }

        /// <inheritdoc cref="ConfigureOutputEnabled(FgenSessionsBundle, bool)"/>
        public static void ConfigureOutputEnabled(this FgenSessionsBundle sessionsBundle, SiteData<bool> outputEnable)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Output.SetEnabled(sitePinInfo.IndividualChannelString.Split('/').Last(), outputEnable.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <inheritdoc cref="ConfigureOutputEnabled(FgenSessionsBundle, bool)"/>
        public static void ConfigureOutputEnabled(this FgenSessionsBundle sessionsBundle, PinSiteData<bool> outputEnable)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Output.SetEnabled(sitePinInfo.IndividualChannelString.Split('/').Last(), outputEnable.GetValue(sitePinInfo));
            });
        }

        /// <summary>
        /// Configures the output impedance of the signal generator.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="impedance">Specifies the impedance value that you want the signal generator to use.</param>
        /// <remarks>
        /// This method specifies the output impedance of the NI signal generator at the output connector.
        /// NI signal generators have an output impedance of 50 Ω and an optional 75 Ω on select modules.
        /// <para>
        /// If the load impedance value matches the output impedance, the voltage at the signal output connector is at the necessary level.
        /// The voltage at the signal output connector varies with load output impedance, up to doubling the voltage for a high-impedance load.
        /// </para>
        /// <para>
        /// You cannot change terminal configuration while the device is generating a waveform.
        /// If you want to change the device configuration, call 'Abort' extension method or wait for the generation to complete.
        /// </para>
        /// </remarks>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, double impedance = 50)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Output.SetImpedance(sessionInfo.AllChannelsString, impedance);
            });
        }

        /// <inheritdoc cref="ConfigureOutputImpedance(FgenSessionsBundle, double)"/>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, SiteData<double> impedance)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Output.SetImpedance(sitePinInfo.IndividualChannelString.Split('/').Last(), impedance.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <inheritdoc cref="ConfigureOutputImpedance(FgenSessionsBundle, double)"/>
        public static void ConfigureOutputImpedance(this FgenSessionsBundle sessionsBundle, PinSiteData<double> impedance)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Output.SetImpedance(sitePinInfo.IndividualChannelString.Split('/').Last(), impedance.GetValue(sitePinInfo));
            });
        }

        /// <summary>
        /// Configures the output mode of the signal generator.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="FgenSessionsBundle"/> object.</param>
        /// <param name="outputMode">Specifies the output mode that you want the signal generator to use.</param>
        /// <remarks>
        /// The Configure Output Mode step determines the type of waveforms that will be generated by your device.
        /// Options include Function, Arbitrary, Sequence, FrequencyList, and Script modes.
        /// <para>
        /// As of STL 26.5, only the <see cref="OutputMode.Function"/> output mode is supported.
        /// Attempting to configure any other output mode throws an exception.
        /// </para>
        /// </remarks>
        /// <exception cref="NISemiconductorTestException">Thrown when the output mode is not <see cref="OutputMode.Function"/>.</exception>
        public static void ConfigureOutputMode(this FgenSessionsBundle sessionsBundle, OutputMode outputMode)
        {
            if (outputMode != OutputMode.Function)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.FGen_InvalidOutputModeException, outputMode));
            }
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Output.OutputMode = outputMode;
            });
        }
    }
}
