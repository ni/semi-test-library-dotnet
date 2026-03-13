using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using System;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.MultiplexedConnection
{
    /// <summary>
    /// This partial class provides setup and cleanup step methods for the multiplexed connection example.
    /// </summary>
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Performs cleanup of NI Generic Multiplexer session data.
        /// Clears session objects for all switch names associated with the provided multiplexer type.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="multiplexerTypeId">The multiplexer type identifier in the pin map.</param>
        public static void CleanupGenericMultiplexerSession(
            ISemiconductorModuleContext tsmContext,
            string multiplexerTypeId)
        {
            try
            {
                var switchNames = tsmContext.GetSwitchNames(multiplexerTypeId);
                foreach (var switchName in switchNames)
                {
                    tsmContext.SetSwitchSession(multiplexerTypeId, switchName, null);
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
