using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace Accelerometer
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Toggles the power relay based on the powerOn input parameter (true = closes relay or false = opens relay).
        /// After applying the appropriate relay action, the method will wait for the settling
        /// based on the value defined by the SettlingTimes.Relay symbol within the loaded specifications file.
        /// Throws an exception if the SettlingTimes.Relay symbol cannot be found within the loaded specifications file.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="powerOn">Whether to close (true) or open (false) the power relay.</param>
        public static void PowerToggle(ISemiconductorModuleContext semiconductorModuleContext, bool powerOn)
        {
            const string relaySettlingTimeSpecSymbol = "SettlingTimes.Relay";
            double settlingTimeInSeconds = semiconductorModuleContext.GetSpecificationsValue(relaySettlingTimeSpecSymbol);
            RelayDriverAction relayAction = powerOn ? RelayDriverAction.CloseRelay : RelayDriverAction.OpenRelay;
            semiconductorModuleContext.ControlRelay("POWER_RELAY", relayAction, settlingTimeInSeconds);
        }
    }
}
