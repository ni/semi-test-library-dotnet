using NationalInstruments.ModularInstruments.NIScope;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ScopeAcquisition
{
    /// <summary>
    /// This class provides an example test step method to demonstrate how to acquire a signal on a Scope pin
    /// using the Scope Instrument Abstraction extensions from the Semiconductor Test Library.
    /// </summary>
    public static class TestSteps
    {
        /// <summary>
        /// Configures the specified scope pins and initiates a waveform acquisition.
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
        /// Publishs the frequency value using the "Frequency" published data id,
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
        public static void ConfigureInitiateAndFetchScopeAcquisition(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup,
            double range = 1.0,
            double offset = 0.0,
            double probeAttenuation = 1.0,
            bool enableTimeInterleavedSampling = false,
            double inputImpedance = 1e6,
            double inputFrequencyMax = 0)
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
            scopeBundle.ConfigureTriggerImmediate();
            scopeBundle.Initiate();
            var waveforms = scopeBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(50.0), 1000);

            var frequencies = new PinSiteData<double>();
            var amplitudes = new PinSiteData<double>();
            foreach (var pin in waveforms.PinNames)
            {
                frequencies.AddPin(pin);
                amplitudes.AddPin(pin);
                foreach (var site in waveforms.SiteNumbers)
                {
                    var waveform = waveforms.GetValue(site, pin);
                    Utilities.MeasureTone(waveform.GetRawData(), 1.0 / waveform.Timing.SampleInterval.TotalSeconds, out double frequencyHz, out double amplitude);
                    frequencies.AddSite(site);
                    frequencies.SetValue(frequencyHz, pin, site);
                    amplitudes.AddSite(site);
                    amplitudes.SetValue(amplitude, pin, site);
                }
            }
            tsmContext.PublishResults(frequencies, "Frequency");
            tsmContext.PublishResults(amplitudes, "Amplitude");
        }
    }
}
