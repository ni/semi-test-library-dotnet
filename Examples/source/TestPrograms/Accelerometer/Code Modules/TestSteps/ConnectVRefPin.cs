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
        /// Applies the appropriate relay configuration to connect the VRef pin to the DUT.
        /// After applying the relay configuration, the method will wait for the settling
        /// based on the value defined by the SettlingTimes.Relay symbol within the loaded specifications file.
        /// Throws an exception if the SettlingTimes.Relay symbol cannot be found within the loaded specifications file.
        /// </summary>
        /// <param name="semiconductorModuleContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="relayConfiguration">The appropriate relay configuration to connect the VRef pin to the DUT,</param>
        public static void ConnectVRefPin(ISemiconductorModuleContext semiconductorModuleContext, string relayConfiguration)
        {
            const string relaySettlingTimeSpecSymbol = "SettlingTimes.Relay";
            double settlingTimeInSeconds = semiconductorModuleContext.GetSpecificationsValue(relaySettlingTimeSpecSymbol);
            semiconductorModuleContext.ApplyRelayConfiguration(relayConfiguration, settlingTimeInSeconds);
        }
    }
}
