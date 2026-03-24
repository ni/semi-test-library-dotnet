using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.ModularInstruments.NISwitch;
using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.MultiplexedConnection
{
    /// <summary>
    /// Provides setup and cleanup step methods for the multiplexed connection example.
    /// </summary>
    public static class SetupAndCleanupSteps
    {
        /// <summary>
        /// Performs setup of NI Generic Multiplexer session data.
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
                // Query switch names for the configured multiplexer type.
                var switchNames = tsmContext.GetSwitchNames(multiplexerTypeId);

                foreach (var switchName in switchNames)
                {
                    // Register a placeholder session so TSM can map switch context without direct NI Switch driver calls.
                    tsmContext.SetSwitchSession(multiplexerTypeId, switchName, new object());
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }

        /// <summary>
        /// Performs cleanup of NI Generic Multiplexer session data.
        /// Closes all switch sessions available in the TSM context.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="multiplexerTypeId">The multiplexer type identifier in the pin map.</param>
        public static void CleanupGenericMultiplexerSession(
            ISemiconductorModuleContext tsmContext,
            string multiplexerTypeId)
        {
            try
            {
                var sessions = tsmContext.GetAllSwitchSessions(multiplexerTypeId);

                foreach (var session in sessions)
                {
                    var switchSession = session as NISwitch;
                    switchSession?.Close();
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
