using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ConfigureInitiateAndFetchScopeAcquisition
{
    public static class TestSteps
    {
        /// <summary>
        /// Configures the vertical settings, such as input range and offset, for the target scope pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">Name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <param name="range">The range to be applied to the scope.</param>
        /// <param name="offset">The offset to be applied to the scope.</param>
        /// <param name="probeAttenuation">The probe attenuation to be applied to the scope.</param>
        /// <param name="enableTimeInterleavedSampling">A boolean value indicating whether to enable time-interleaved sampling.</param>
        /// <param name="inputImpedance">The input impedance of the channel, in ohms.</param>
        /// <param name="inputFrequencyMax">The maximum input frequency of the channel, in hertz.</param>
        /// <param name="immediateTrigger">A boolean value indicating whether to configure an immediate trigger or a digital trigger.</param>
        /// <param name="siteNumber">Site number used to select the waveform.</param>
        /// <param name="sampleInterval">When this method returns, contains the waveform sample interval in seconds.</param>
        public static void ScopeConfiguredVertical(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            double range,
            double offset,
            double probeAttenuation,
            bool enableTimeInterleavedSampling,
            double inputImpedance,
            double inputFrequencyMax,
            bool immediateTrigger,
            int siteNumber,
            out double sampleInterval)
        {
            var verticalSettings = new VerticalSettings
            {
                Range = range,
                Offset = offset,
                ProbeAttenuation = probeAttenuation,
                EnableTimeInterleavedSampling = enableTimeInterleavedSampling,
            };
            ElectricalCharacteristics electricalCharacteristics = new ElectricalCharacteristics
            {
                InputImpedance = inputImpedance,
                InputFrequencyMax = inputFrequencyMax,
            };
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            scopeBundle.ConfigureVertical(verticalSettings);
            scopeBundle.ConfigureElectricalCharacteristics(electricalCharacteristics);
            scopeBundle.Commit();
            if (immediateTrigger)
            {
                scopeBundle.ConfigureTriggerImmediate();
            }
            else
            {
                DigitalTriggerSettings digitalTriggerSettings = new DigitalTriggerSettings
                {
                    TriggerSlope = ScopeTriggerSlope.Positive,
                    HoldOff = PrecisionTimeSpan.FromSeconds(0.0),
                    Delay = PrecisionTimeSpan.FromSeconds(0.0),
                };
                scopeBundle.ConfigureTriggerDigital(digitalTriggerSettings);
            }
            scopeBundle.Initiate();
            var waveforms = scopeBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(50.0), 1000);

            AnalogWaveform<double> waveform = waveforms.GetValue(siteNumber, pinOrPinGroup);
            sampleInterval = waveform.Timing.SampleInterval.TotalSeconds;
            var data = waveform.GetRawData();
            // The data variable contains the acquired waveform data for the specified
            // site number and pin or pin group.
            // Need to process the data for frequency measurement.
        }
    }
}
