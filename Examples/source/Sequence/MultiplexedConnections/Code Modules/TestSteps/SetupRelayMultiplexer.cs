using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace SemiconductorTestLibrary.Examples.MultiplexedConnections
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Call this method from the test executive to setup switch sessions for the RelayMultiplexer.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object reference.</param>
        public static void SetupRelayMultiplexer(ISemiconductorModuleContext tsmContext)
        {
            string[] switchNames = tsmContext.GetSwitchNames("RelayMultiplexer");

            foreach (string switchName in switchNames)
            {
                // Passing the switch data as the session data, since this is a required input but this will not be used.
                tsmContext.SetSwitchSession("RelayMultiplexer", switchName, switchName);
            }
        }
    }
}