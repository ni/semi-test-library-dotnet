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

            // ScopeAcquisitionType acquisitionType = ScopeAcquisitionType.Normal;
            // scopeBundle.ConfigureAcquisitionType(acquisitionType);
            VerticalSettings verticalSettings = new VerticalSettings
            {
                EnableTimeInterleavedSampling = false,
            };
            scopeBundle.ConfigureVertical(verticalSettings);
            ElectricalCharacteristics electricalCharacteristics = new ElectricalCharacteristics
            {
            };
            scopeBundle.ConfigureElectricalCharacteristics(electricalCharacteristics);
            ClockSettings clockSettings = new ClockSettings
            {
                InputClockSource = ScopeInputClockSource.PxiClock,
            };

            // scopeBundle.ConfigureClock(clockSettings);
            scopeBundle.ConfigureTriggerImmediate();
            scopeBundle.Initiate();

            return scopeBundle.FetchWaveform(PrecisionTimeSpan.FromSeconds(50.0), 100);
        }


        /// <summary>
        /// Gets raw waveform sample values for the specified site and pin, and outputs the sample interval in seconds.
        /// </summary>
        /// <param name="waveforms">Collection of analog waveforms indexed by site and pin.</param>
        /// <param name="siteNumber">Site number used to select the waveform.</param>
        /// <param name="pin">Pin name used to select the waveform.</param>
        /// <param name="dt">When this method returns, contains the waveform sample interval in seconds.</param>
        /// <returns>An array containing the raw sample values of the selected waveform.</returns>
        public static double[] GetWaveformSamples(
            PinSiteData<AnalogWaveform<double>> waveforms, int siteNumber, string pin, out double dt)
        {
            if (waveforms is null)
            {
                dt = 0.0;
                return null;
            }
            else
            {
                AnalogWaveform<double> wfm = waveforms.GetValue(siteNumber, pin);
                dt = wfm.Timing.SampleInterval.TotalSeconds;
                return wfm.GetRawData();
            }
        }
    }
}
