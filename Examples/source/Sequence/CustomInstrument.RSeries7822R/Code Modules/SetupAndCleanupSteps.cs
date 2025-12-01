using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.MyCustomInstrument;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument
{
    /// <summary>
    /// This class contains methods to perform setup and cleanup operations for RSeries7822R Instruments using the custom instrument support provided by STL.
    /// </summary>
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes all RSeries7822R Instruments.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void SetupRSeries7822RInstrumentation(ISemiconductorModuleContext tsmContext)
        {
            var myCustomInstrumentFactory = new RSeries7822RFactory();
            InitializeAndClose.Initialize(tsmContext, myCustomInstrumentFactory);
            ApplyLoopBackConfiguration(tsmContext, myCustomInstrumentFactory.InstrumentTypeId);
        }

        /// <summary>
        /// Closes all references of RSeries7822RInstruments.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void CleanupRSeries7822RInstrumentation(ISemiconductorModuleContext tsmContext)
        {
            // Close all references of custom instruments.
            InitializeAndClose.Close(tsmContext, RSeries7822RFactory.CustomInstrumentTypeId);
        }

        /// <summary>
        /// Applies loop back configuration.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="instrumentTypeID">Instrument type Id.</param>
        public static void ApplyLoopBackConfiguration(ISemiconductorModuleContext tsmContext, string instrumentTypeID)
        {
            // Create CustomInstrument Session Bundle.
            tsmContext.GetPins(instrumentTypeID, out var dutPins, out var systemPins);
            var tsmSessionManager = new TSMSessionManager(tsmContext);
            var myCustomInstrumentSessionsBundle = tsmSessionManager.CustomInstrument(instrumentTypeID, systemPins.Concat(dutPins).ToArray());

            // Set LoopBack mode.
            myCustomInstrumentSessionsBundle.EnableLoopBackMode();
        }
    }
}
