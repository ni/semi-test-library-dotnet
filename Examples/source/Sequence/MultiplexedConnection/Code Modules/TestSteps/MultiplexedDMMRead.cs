using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.MultiplexedConnection
{
    /// <summary>
    /// This class provides an example method demonstrating how to use STL with a multiplexed connection
    /// where one instrument channel is routed to the same DUT pin across multiple sites.
    /// The method is intended for relay-based routing on the application load board, where
    /// the route names in the pin map map to relay configuration names.
    /// </summary>
    public static class TestSteps
    {
        /// <summary>
        /// Performs user-code setup of NI Generic Multiplexer session data.
        /// Initializes session objects for all switch names associated with the provided multiplexer type.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="multiplexerTypeId">The multiplexer type identifier in the pin map.</param>
        public static void InitializeGenericMultiplexerSession(
            ISemiconductorModuleContext tsmContext,
            string multiplexerTypeId)
        {
            try
            {
                var switchNames = tsmContext.GetSwitchNames(multiplexerTypeId);
                foreach (var switchName in switchNames)
                {
                    tsmContext.SetSwitchSession(multiplexerTypeId, switchName, new object());
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }

        /// <summary>
        /// Demonstrates serial per-site measurement for a multiplexed connection where one instrument channel
        /// is mapped to the same DUT pin across multiple sites.
        /// This method expects that existing TestStandSteps have already initialized the Relay Module and DMM sessions,
        /// and user code has already called <see cref="InitializeGenericMultiplexerSession(ISemiconductorModuleContext, string)"/>.
        /// The method queries site-specific routes from TSM, applies each route using relay configurations,
        /// performs a DMM read for each site context, and publishes the measurement result.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutPinName">The DUT pin name mapped through the multiplexed connection.</param>
        /// <param name="endOfTestingRelayConfigurationName">Relay configuration to apply after all site-specific measurements are complete.</param>
        public static void OneInstrumentChannelToManySitesForOneDutPin(
            ISemiconductorModuleContext tsmContext,
            string dutPinName,
            string endOfTestingRelayConfigurationName = "")
        {
            // Retrieve site-specific route names for the requested DUT pin.
            // Route names are defined in the pin map and map to relay configurations.
            tsmContext.GetSwitchSessions(dutPinName, out ISemiconductorModuleContext[] tsmContexts, out string[] routes);

            // Execute one site at a time because this flow shares a single instrument channel.
            for (int i = 0; i < tsmContexts.Length; i++)
            {
                // Select the relay state for the current site using the main context.
                tsmContext.ApplyRelayConfiguration(routes[i]);
                var sessionManager = new TSMSessionManager(tsmContexts[i]);
                var dmm = sessionManager.DMM(dutPinName);

                // Read measurement and publish it for the active site context.
                var measurement = dmm.Read(maximumTimeInMilliseconds: 1000);
                tsmContexts[i].PublishResults(measurement, $"{dutPinName}_Voltage");
            }

            // Restore the configured end-of-test relay state after site loop completes.
            if (!string.IsNullOrEmpty(endOfTestingRelayConfigurationName))
            {
                tsmContext.ApplyRelayConfiguration(endOfTestingRelayConfigurationName);
            }
        }
    }
}
