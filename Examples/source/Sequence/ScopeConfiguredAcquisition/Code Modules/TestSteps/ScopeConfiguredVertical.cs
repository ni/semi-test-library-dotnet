using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ConfiguredAcquisition
{
    public static partial class ScopeTestSteps
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
    }
}
