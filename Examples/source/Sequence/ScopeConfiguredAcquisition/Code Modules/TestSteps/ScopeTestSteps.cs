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
        /// <param name="range">The range to be applied to the scope.</param>
        /// <param name="offset">The offset to be applied to the scope.</param>
        /// <param name="probeAttenuation">The probe attenuation to be applied to the scope.</param>
        /// <param name="enableTimeInterleavedSampling">A boolean value indicating whether to enable time-interleaved sampling.</param>
        public static void ScopeConfiguredVertical(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            double range,
            double offset,
            double probeAttenuation,
            bool enableTimeInterleavedSampling)
        {
            var verticalSettings = new VerticalSettings
            {
                Range = range,
                Offset = offset,
                ProbeAttenuation = probeAttenuation,
                EnableTimeInterleavedSampling = enableTimeInterleavedSampling,
            };

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
        /// <param name="inputImpedance">The input impedance of the channel, in ohms.</param>
        /// <param name="inputFrequencyMax">The maximum input frequency of the channel, in hertz.</param>
        public static void ScopeConfigureElectricalCharacteristics(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            double inputImpedance,
            double inputFrequencyMax)
        {
            ElectricalCharacteristics electricalCharacteristics = new ElectricalCharacteristics
            {
                InputImpedance = inputImpedance,
                InputFrequencyMax = inputFrequencyMax,
            };
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
        /// <param name="triggerSlope">The slope of the digital trigger.</param>
        /// <param name="holdOffInSeconds">The digital trigger holdoff, in seconds.</param>
        /// <param name="delayInSeconds">The digital trigger delay, in seconds.</param>
        public static void ScopeConfigureTriggerSettings(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            bool immediateTrigger,
            ScopeTriggerSlope triggerSlope,
            double holdOffInSeconds,
            double delayInSeconds)
        {
            DigitalTriggerSettings digitalTriggerSettings = new DigitalTriggerSettings
            {
                TriggerSlope = triggerSlope,
                HoldOff = PrecisionTimeSpan.FromSeconds(holdOffInSeconds),
                Delay = PrecisionTimeSpan.FromSeconds(delayInSeconds),
            };
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
        }

        /// <summary>
        /// This method used for configuring trigger settings.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        public static void ScopeInitiateAcquisition(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
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
