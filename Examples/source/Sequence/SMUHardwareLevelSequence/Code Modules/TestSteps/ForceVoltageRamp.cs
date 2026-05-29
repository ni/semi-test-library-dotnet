using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SMUHardwareLevelSequence
{
    /// <summary>
    /// This class provides example methods demonstrating how to perform Hardware Level Sequencing with SMUs
    /// using DCPower Instrument Abstraction methods from the Semiconductor Test Library.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Forces a hardware-timed voltage ramp sequence on the specified SMU pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force the voltage ramp on.</param>
        /// <param name="startVoltage">The starting value of a voltage ramp.</param>
        /// <param name="stopVoltage">The ending value of a voltage ramp.</param>
        /// <param name="numberOfSteps">The number of steps in the voltage ramp sequence.</param>
        public static void ForceVoltageRamp(ISemiconductorModuleContext tsmContext, string[] smuPinNames, double startVoltage = 0, double stopVoltage = 3, int numberOfSteps = 10)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            DCPowerSessionsBundle dcPowerPins = sessionManager.DCPower(smuPinNames);

            double[] voltageSequence = HelperMethods.CreateRampSequence(outputStart: startVoltage, outputStop: stopVoltage, numberOfPoints: numberOfSteps);
            dcPowerPins.ForceVoltageSequence(voltageSequence);
        }
    }
}
