using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Disables the output of the instruments connected to the DUT.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="settlingTime">The amount of time to wait, in Seconds, for the output to settling after it has been disabled.</param>
        public static void DisableInstrumentOutputs(ISemiconductorModuleContext semiconductorModuleContext, double settlingTime = 0.001)
        {
            var sessionManager = new TSMSessionManager(semiconductorModuleContext);

            InvokeInParallel(
                () =>
                {
                    semiconductorModuleContext.GetPins(InstrumentTypeIdConstants.NIDCPower, out string[] dcPowerDutPinNames, out _);
                    sessionManager.DCPower(dcPowerDutPinNames).PowerDown(settlingTime);
                },
                () =>
                {
                    semiconductorModuleContext.GetPins(InstrumentTypeIdConstants.NIDigitalPattern, out string[] digitalDutPinNames, out _);
                    sessionManager.Digital(digitalDutPinNames).TurnOffOutput(settlingTime);
                });
        }
    }
}