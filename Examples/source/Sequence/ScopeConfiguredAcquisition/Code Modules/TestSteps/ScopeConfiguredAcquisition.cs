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
        public static void ScopeConfiguredAcquisition(
            ISemiconductorModuleContext tsmContext,
            string pinOrPinGroup)
        {
            TSMSessionManager sessionManager = new TSMSessionManager(tsmContext);
            ScopeSessionsBundle scopeBundle = sessionManager.Scope(pinOrPinGroup);

            // scopeBundle.Acquisition.Type = AcquisitionType;
            scopeBundle.ConfigureVertical(Range, Offset, Coupling, ProbeAttenuation, true);
            scopeBundle.ConfigureElectrical(InputImpedance, InputFrequencyMax);


            scopeBundle.ConfigureClock(SampleRateMin, RecordLengthMin, ReferencePosition, NumberOfRecords, EnforceRealTime);

            // scopeSession.Channels[ChannelName].EnableTimeInterleavedSampling = EnableTimeInterleavedSampling;
            scopeBundle.ConfigureTriggerImmediate();

            scopeBundle.Initiate();

            var waveforms = scopeBundle.FetchWaveform();

            // waveforms = scopeSession.Channels[ChannelName].Measurement.FetchDouble(Timeout, -1, waveforms);
        }
    }
}
