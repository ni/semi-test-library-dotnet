using NationalInstruments.SemiconductorTestLibrary.Common;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using SemiconductorTestLibrary.Examples.MultiplexedConnections.Common;
using System.Drawing;
using System.Reflection;

namespace SemiconductorTestLibrary.Examples.MultiplexedConnections
{
    /// <summary>
    /// Partial class containing all test steps for the project.
    /// This is declared as a partial class so that test code modules can be managed as unique methods in separate files.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Call this method from the test executive to execute the test code.
        /// 1. Get the switch session for the pin.
        /// 2. Open the connection for the switch route.
        /// 3. Get the sessions bundle for the pin.
        /// 4. Acquire & Publish measurement data.
        /// 5. Close the connection for the switch route.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object reference.</param>
        /// <param name="dmmPin">The DMM pin that is multiplexed to multiple sites.</param>
        public static void ExampleTestStep(ISemiconductorModuleContext tsmContext, string dmmPin)
        {
            object[] switchSessions = tsmContext.GetSwitchSessions(
                dmmPin,
                SimulatedMultiplexer.MultiplexerTypeId,
                out ISemiconductorModuleContext[] semiconductorModuleContexts,
                out string[] switchRoutes);

            for ( int i = 0; i < semiconductorModuleContexts.Length; i++ )
            {
                // Each iteration of the For Loop tests one site at a time.
                // This implementation intentionally does NOT using Parallel.For().
                // The measurements must be executed serially in a For Loop because they share an instrument.
                var route = switchRoutes[i];
                var session = switchSessions[i] as SimulatedMultiplexer;
                var sessionManager = new TSMSessionManager(semiconductorModuleContexts[i]);
                var dmm = sessionManager.DMM(dmmPin);
                session.ConnectRoute(route);
                dmm.ReadAndPublish(maximumTimeInMilliseconds: 1000);
                session.DisconnectRoute(route);
            }
        }
    }
}