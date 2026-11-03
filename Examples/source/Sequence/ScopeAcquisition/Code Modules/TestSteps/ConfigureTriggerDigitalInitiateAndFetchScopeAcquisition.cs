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
        /// Configures the specified scope pins for digital triggering and initiates a waveform acquisition.
        /// Then calculates and publishes the measured frequency and amplitude of the acquired waveform.
        /// </summary>
        /// <remarks>
        /// This method performs the following operations:
        /// <list type="number">
        /// <item>Configures the scope's vertical and electrical characteristics</item>
        /// <item>Configures and apply scope trigger settings</item>
        /// <item>Initiates acquisition</item>
        /// <item>Fetches the acquired waveform</item>
        /// <item>Calculates the measured frequency and max amplitude of the signal</item>
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
        /// <param name="holdOff">The hold-off time for the trigger, in seconds.</param>
        /// <param name="delay">The delay time for the trigger, in seconds.</param>
        /// <param name="triggerSlope">The trigger slope for the scope.</param>
        public static void ConfigureTriggerDigitalInitiateAndFetchScopeAcquisition(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            double range = 1.0,
            double offset = 0.0,
            double probeAttenuation = 1.0,
            bool enableTimeInterleavedSampling = false,
            double inputImpedance = 1e6,
            double inputFrequencyMax = 0,
            double holdOff = 0.0,
            double delay = 0.0,
            ScopeTriggerSlope triggerSlope = ScopeTriggerSlope.Positive)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);
            ConfigureScope(
                scopeBundle,
                pinOrPinGroup,
                range,
                offset,
                probeAttenuation,
                enableTimeInterleavedSampling,
                inputImpedance,
                inputFrequencyMax);

            var triggerDigitalSettings = new DigitalTriggerSettings
            {
                HoldOff = PrecisionTimeSpan.FromSeconds(holdOff),
                Delay = PrecisionTimeSpan.FromSeconds(delay),
                TriggerSlope = triggerSlope,
            };
            scopeBundle.ConfigureTriggerDigital(triggerDigitalSettings);

            InitiateFetchAndPublishescopeAcquisition(tsmContext, scopeBundle);
        }
    }
}
