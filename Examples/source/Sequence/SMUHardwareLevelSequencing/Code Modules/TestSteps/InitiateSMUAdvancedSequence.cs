using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CodeSnippets.InstrumentAbstraction
{
    /// <summary>
    /// This class provides example methods demonstrating how to perform Hardware Level Sequencing with SMUs
    /// using DCPower Instrument Abstraction methods from the Semiconductor Test Library.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Initiates a previously configured advanced sequence for the specified SMU pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to initiate the advanced sequence on.</param>
        /// <param name="advanceSequenceName">The name of the advanced sequence to initiate.(advanced sequence name should be same as what was created in <see cref="ConfigureSMUAdvancedSequence"/>).</param>
        public static void InitiateSMUAdvancedSequence(ISemiconductorModuleContext tsmContext, string[] smuPinNames, string advanceSequenceName)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            // Initiate the advanced sequence that was configured earlier
            dcPowerPins.InitiateAdvancedSequence(advanceSequenceName);
        }
    }
}
