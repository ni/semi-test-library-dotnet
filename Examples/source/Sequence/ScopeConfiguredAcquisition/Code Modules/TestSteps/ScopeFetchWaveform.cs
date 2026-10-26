using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ConfiguredAcquisition
{
    public static partial class ScopeTestSteps
    {
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
    }
}
