using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ScopeAcquisition
{
    public static partial class TestSteps
    {
        /// <summary>
        /// Configures the specified scope pins to use a digital trigger, initiates acquisition and fetches the acquired waveform.
        /// Then calculates and publishes the measured frequency and amplitude of the acquired waveform.
        /// </summary>
        /// <remarks>
        /// This method performs the following operations:
        /// <list type="number">
        /// <item>Configures the scope's vertical and electrical characteristics</item>
        /// <item>Configures and applies scope trigger settings</item>
        /// <item>Initiates acquisition</item>
        /// <item>Fetches the acquired waveform</item>
        /// <item>Calculates the measured frequency and peak-to-peak amplitude of the signal</item>
        /// <item>
        /// Publishes the frequency value using the "Frequency" published data id,
        /// and the amplitude value using the "Amplitude" published data id.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinOrPinGroup">The name of the scope pin or pin group to be used for waveform acquisition.</param>
        /// <param name="range">The vertical range to be applied to the scope.</param>
        /// <param name="offset">The vertical offset to be applied to the scope.</param>
        /// <param name="probeAttenuation">The probe attenuation to be applied to the scope.</param>
        /// <param name="enableTimeInterleavedSampling">A boolean value indicating whether to enable time-interleaved sampling.</param>
        /// <param name="inputImpedance">The input impedance of the scope, in Ohms.</param>
        /// <param name="inputFrequencyMax">The maximum input frequency of the scope, in Hertz.</param>
        /// <param name="triggerSource">The digital trigger source to start the acquisition</param>
        /// <param name="triggerSlope">The trigger slope for the scope.</param>
        /// <param name="holdOff">The hold-off time for the trigger, in seconds.</param>
        /// <param name="delay">The delay time for the trigger, in seconds.</param>
        public static void ConfigureInitiateAndFetchScopeAcquisitionUsingDigitalTrigger(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            double range = 1.0,
            double offset = 0.0,
            double probeAttenuation = 1.0,
            bool enableTimeInterleavedSampling = false,
            double inputImpedance = 1e6,
            double inputFrequencyMax = 0.0,
            string triggerSource = "",
            ScopeTriggerSlope triggerSlope = ScopeTriggerSlope.Positive,
            double holdOff = 0.0,
            double delay = 0.0)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);

            // 1. Configure the scope's vertical and electrical characteristics
            // Note that the settings are grouped into specific class objects before being passed to the STL method.
            // This ensures the settings are applied as efficiently as possible by the instrument driver.
            VerticalSettings verticalSettings = new VerticalSettings
            {
                Range = range,
                Offset = offset,
                ProbeAttenuation = probeAttenuation,
                EnableTimeInterleavedSampling = enableTimeInterleavedSampling,
            };
            scopeBundle.ConfigureVertical(verticalSettings);
            ElectricalCharacteristics electricalCharacteristics = new ElectricalCharacteristics
            {
                InputImpedance = inputImpedance,
                InputFrequencyMax = inputFrequencyMax,
            };
            scopeBundle.ConfigureElectricalCharacteristics(electricalCharacteristics);
            scopeBundle.Commit();

            // 2. Configure and apply scope trigger settings
            DigitalTriggerSettings digitalTriggerSettings = new DigitalTriggerSettings ()
            {
                TriggerSource = triggerSource,
                TriggerSlope = triggerSlope,
                HoldOff = PrecisionTimeSpan.FromSeconds(holdOff),
                Delay = PrecisionTimeSpan.FromSeconds(delay),
            };
            scopeBundle.ConfigureTriggerDigital(digitalTriggerSettings);

            // 3. Initiate acquisition
            scopeBundle.Initiate();

            // 4. Fetch the acquired waveform
            PinSiteData<AnalogWaveform<double>> waveforms = scopeBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(50.0), 1000);

            // 5. Calculate the measured frequency and peak-to-peak amplitude of the signal
            Utilities.CalculateWaveformFrequencyAndPeakToPeakAmplitude(waveforms, out PinSiteData<double> frequencies, out PinSiteData<double> amplitudes);

            // 6. Publish the calculated frequency and amplitude values
            tsmContext.PublishResults(frequencies, "Frequency");
            tsmContext.PublishResults(amplitudes, "Amplitude");
        }
    }
}
