using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System.Linq;

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
        public static void ExampleTestStep(ISemiconductorModuleContext tsmContext, string pinName)
        {
            _ = tsmContext.GetSwitchSessions(
                pinName,
                "RelayMultiplexer",
                out ISemiconductorModuleContext[] semiconductorModuleContexts,
                out string[] switchRoutes);

            PinSiteData<double> allSiteData = null;
            for ( int i = 0; i < semiconductorModuleContexts.Length; i++ )
            {
                // Each iteration of the For Loop tests one site at a time.
                // This implementation intentionally does NOT using Parallel.For().
                // The measurements must be executed serially in a For Loop because they share an instrument.
                var siteContext = semiconductorModuleContexts[i];
                var route = switchRoutes[i];
                var sessionManager = new TSMSessionManager(siteContext);
                var dmm = sessionManager.DMM(pinName);

                // Set Relay Route(s)
                if (pinName == "A" || pinName == "B")
                {
                    tsmContext.ControlRelay(route, RelayDriverAction.CloseRelay);
                }
                else
                {
                    tsmContext.ApplyRelayConfiguration(route);
                }
                
                // Perform shared instrument operation for current site context.
                PinSiteData<double> measurement = dmm.Read(maximumTimeInMilliseconds: 1000);

                // Add site variance for testing purposes
                measurement = measurement.Select(x => x * (1 + siteContext.SiteNumbers.First()));
                // Combine site data
                if (allSiteData == null)
                {
                    allSiteData = measurement;
                }
                else
                {
                    allSiteData = allSiteData.Combine(measurement);
                }

                // Unset Relay Route(s)
                if (pinName == "A" || pinName == "B")
                {
                    tsmContext.ControlRelay(route, RelayDriverAction.OpenRelay);
                }
            }
            // Publish All Site Results
            tsmContext.PublishResults(allSiteData, "");
        }
    }
}