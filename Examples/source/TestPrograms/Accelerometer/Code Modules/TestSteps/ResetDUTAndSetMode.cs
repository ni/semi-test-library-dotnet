using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Resets the DUT and sets the test mode.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="rstPinName">The DUT's RST pin name, as defined in the pin map file.</param>
        /// <param name="modePinName">The DUT's Test Mode pin name, as defined in the pin map file.</param>
        public static void ResetDUTAndSetMode(ISemiconductorModuleContext semiconductorModuleContext, string rstPinName, string modePinName)
        {
            double vih = semiconductorModuleContext.GetSpecificationsValue("DC.Vih");
            double vil = semiconductorModuleContext.GetSpecificationsValue("DC.Vil");
            double rstPinSettlingTime = semiconductorModuleContext.GetSpecificationsValue("SettlingTimes.SetRST");
            double modePinSettlingTime = semiconductorModuleContext.GetSpecificationsValue("SettlingTimes.SetTestMode");
            double resetTimeInSeconds = semiconductorModuleContext.GetSpecificationsValue("AC.ResetTime");
            double currentLimitRange = 32e-3;

            var sessionManager = new TSMSessionManager(semiconductorModuleContext);
            DigitalSessionsBundle rstPin = sessionManager.Digital(rstPinName);
            DigitalSessionsBundle modePin = sessionManager.Digital(modePinName);

            rstPin.ForceVoltage(vih, currentLimitRange, settlingTime: rstPinSettlingTime);

            // This is separate from and in addtion to the rstPin settling time.
            Utilities.PreciseWait(resetTimeInSeconds);

            // Current limit range already set, no need to set it again.
            rstPin.ForceVoltage(vil, settlingTime: rstPinSettlingTime);
            modePin.ForceVoltage(vih, currentLimitRange, settlingTime: modePinSettlingTime);
        }
    }
}