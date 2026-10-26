using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.ConfiguredAcquisition
{
    public static partial class ScopeTestSteps
    {
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
    }
}
