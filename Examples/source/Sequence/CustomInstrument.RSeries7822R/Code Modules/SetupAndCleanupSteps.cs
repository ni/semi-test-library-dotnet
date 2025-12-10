using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries7822R;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries7822R
{
    /// <summary>
    /// This class contains methods to perform setup and cleanup operations for RSeries7822R instruments using the Custom Instrument support provided by STL.
    /// </summary>
    public static class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes all RSeries7822R instrument sessions associated with the pin map and applies the loop back configuration based depending on value of the enableLoopBackConfiguration input parameter (default: true/enabled).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="enableLoopBackConfiguration">Optional parameter to enable/disable Loopback configuration. Default value is 'true'</param>
        public static void SetupRSeries7822RInstrumentation(ISemiconductorModuleContext tsmContext, bool enableLoopBackConfiguration = true)
        {
            var myCustomInstrumentFactory = new RSeries7822RFactory();
            InitializeAndClose.Initialize(tsmContext, myCustomInstrumentFactory);
            if (enableLoopBackConfiguration)
            {
                ApplyLoopBackConfiguration(tsmContext, myCustomInstrumentFactory.InstrumentTypeId);
            }
        }

        /// <summary>
        /// Closes all RSeries7822R instrument sessions.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void CleanupRSeries7822RInstrumentation(ISemiconductorModuleContext tsmContext)
        {
            InitializeAndClose.Close(tsmContext, RSeries7822RFactory.CustomInstrumentTypeId);
        }

        /// <summary>
        /// Applies loop back configuration.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="instrumentTypeId">Instrument type Id.</param>
        public static void ApplyLoopBackConfiguration(ISemiconductorModuleContext tsmContext, string instrumentTypeId)
        {
            // Create CustomInstrument Sessions Bundle.
            tsmContext.GetPins(instrumentTypeId, out var dutPins, out var systemPins);
            var sessionManager = new TSMSessionManager(tsmContext);
            var rSeriesSessionsBundle = sessionManager.CustomInstrument(instrumentTypeId, systemPins.Concat(dutPins).ToArray());

            // Enable LoopBack mode
            rSeriesSessionsBundle.EnableLoopBack();
        }
    }
}
