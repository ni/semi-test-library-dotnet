using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ConfiguredAcquisition
{
    /// <summary>
    /// This class provides example methods to demonstrate how to use the Scope APIs.
    /// Scope Instrument Extension methods from the Semiconductor Test Library.
    /// These methods can be used to perform waveform acquisition.
    /// </summary>
    public static class ScopeTestSteps
    {
        /// <summary>
        /// This method used for configuring vertical settings.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <param name="verticalSettings">The vertical settings to be applied to the scope.</param>
        public static void ScopeConfiguredVertical(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            VerticalSettings verticalSettings)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            scopeBundle.ConfigureVertical(verticalSettings);
            scopeBundle.Commit();
        }

        /// <summary>
        /// This method used for configuring electrical characteristics.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <param name="electricalCharacteristics">The electrical characteristics to be applied to the scope.</param>
        public static void ScopeConfigureElectricalCharacteristics(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            ElectricalCharacteristics electricalCharacteristics)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            scopeBundle.ConfigureElectricalCharacteristics(electricalCharacteristics);
            scopeBundle.Commit();
        }

        /// <summary>
        /// This method used for configuring trigger settings.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <param name="immediateTrigger">A boolean value indicating whether to configure an immediate trigger or a digital trigger.</param>
        /// <param name="digitalTriggerSettings">The digital trigger settings to be applied to the scope.</param>
        public static void ScopeConfigureTriggerSettings(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            bool immediateTrigger,
            DigitalTriggerSettings digitalTriggerSettings)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            if (immediateTrigger)
            {
                scopeBundle.ConfigureTriggerImmediate();
            }
            else
            {
                scopeBundle.ConfigureTriggerDigital(digitalTriggerSettings);
            }
            scopeBundle.Initiate();
        }

        /// <summary>
        /// This method demonstrates how to configure and acquire data from the Scope using the Semiconductor Module API.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <param name="timeout">The timeout value in seconds for waveform acquisition.</param>
        /// <param name="maxSamples">The maximum number of samples to acquire.</param>
        /// <returns>The acquired waveforms.</returns>
        public static PinSiteData<AnalogWaveform<double>> ScopeFetchWaveform(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            double timeout = 50.0,
            int maxSamples = 1000)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            return scopeBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(timeout), maxSamples);
        }

        /// <summary>
        /// This method demonstrates how to configure and acquire data from the Scope using the Semiconductor Module API.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <returns>The acquired waveforms.</returns>
        public static PinSiteData<AnalogWaveform<double>> ScopeConfiguredAcquisition(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            VerticalSettings verticalSettings = new VerticalSettings
            {
                EnableTimeInterleavedSampling = false,
            };
            scopeBundle.ConfigureVertical(verticalSettings);
            ElectricalCharacteristics electricalCharacteristics = new ElectricalCharacteristics
            {
            };
            scopeBundle.ConfigureElectricalCharacteristics(electricalCharacteristics);
            scopeBundle.ConfigureTriggerImmediate();
            scopeBundle.Initiate();

            return scopeBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(50.0), 1000);
        }

        /// <summary>
        /// Gets raw
        /// </summary>
        /// <param name="waveforms">Collection of analog waveforms indexed by site and pin.</param>
        /// <param name="siteNumber">Site number used to select the waveform.</param>
        /// <param name="pin">Pin name used to select the waveform.</param>
        /// <param name="sampleInterval">When this method returns, contains the waveform sample interval in seconds.</param>
        /// <returns>An array containing the raw sample values of the selected waveform.</returns>
        public static double[] GetWaveformSamples(
            PinSiteData<AnalogWaveform<double>> waveforms, int siteNumber, string pin, out double sampleInterval)
        {
            if (waveforms is null)
            {
                sampleInterval = 0.0;
                return null;
            }
            else
            {
                AnalogWaveform<double> waveform = waveforms.GetValue(siteNumber, pin);
                sampleInterval = waveform.Timing.SampleInterval.TotalSeconds;
                return waveform.GetRawData();
            }
        }
    }
}
