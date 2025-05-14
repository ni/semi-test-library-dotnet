using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using SemiconductorTestLibrary.Examples.MultiplexedConnections.Common;

namespace SemiconductorTestLibrary.Examples.MultiplexedConnections
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Call this method from the test executive to setup switch sessions.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object reference.</param>
        public static void SetupSimulatedMultiplexer(ISemiconductorModuleContext tsmContext)
        {
            string[] switchNames = tsmContext.GetSwitchNames();

            foreach (string switchName in switchNames)
            {
                tsmContext.SetSwitchSession(switchName, new SimulatedMultiplexer(switchName));
            }
        }
    }
}