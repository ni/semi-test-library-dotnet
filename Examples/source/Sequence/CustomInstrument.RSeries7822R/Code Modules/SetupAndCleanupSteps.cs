using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.MyCustomInstrument;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument
{
    /// <summary>
    /// This class contains sample methods to perform instrument setup and cleanup operations using the custom instrument support provided by STL.
    /// </summary>
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Sample method to perform setup operation. Initializes all custom instruments of specific instrument type.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void SetupMyCustomInstruments(ISemiconductorModuleContext tsmContext)
        {
            var myCustomInstrumentFactory = new RSeries7822RFactory();
            InitializeAndClose.Initialize(tsmContext, myCustomInstrumentFactory);
            OptionallyApplyConfigurations(tsmContext, myCustomInstrumentFactory.InstrumentTypeId);
        }

        /// <summary>
        /// Sample method to perform cleanup operation. Closes all references of custom instruments of specific instrument type.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        public static void CleanupMyCustomInstruments(ISemiconductorModuleContext tsmContext)
        {
            OptionallyClearConfigurations(tsmContext, RSeries7822RFactory.CustomInstrumentTypeId);

            // Close all references of custom instruments.
            InitializeAndClose.Close(tsmContext, RSeries7822RFactory.CustomInstrumentTypeId);
        }

        /// <summary>
        /// Sample method to apply device configuration.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="instrumentTypeID">Instrument type Id.</param>
        public static void OptionallyApplyConfigurations(ISemiconductorModuleContext tsmContext, string instrumentTypeID)
        {
            var myCustomInstrumentSessionsBundle = CreateCustomInstrumentSessionsBundleForAllInstrumentPins(tsmContext, instrumentTypeID);

            // User can do device configuration using Custom Instrument sessions bundle here.
            string configurationPreset = "LoopBack";
            myCustomInstrumentSessionsBundle.ApplyConfiguration(configurationPreset);
        }

        /// <summary>
        /// Sample method to clear previously applied device configuration.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="instrumentTypeID">Instrument type Id.</param>
        public static void OptionallyClearConfigurations(ISemiconductorModuleContext tsmContext, string instrumentTypeID)
        {
            var myCustomInstrumentSessionsBundle = CreateCustomInstrumentSessionsBundleForAllInstrumentPins(tsmContext, instrumentTypeID);
            myCustomInstrumentSessionsBundle.ClearConfiguration();
        }

        private static CustomInstrumentSessionsBundle CreateCustomInstrumentSessionsBundleForAllInstrumentPins(ISemiconductorModuleContext tsmContext, string instrumentTypeID)
        {
            tsmContext.GetPins(instrumentTypeID, out var dutPins, out var systemPins);
            var tsmSessionManager = new TSMSessionManager(tsmContext);
            return tsmSessionManager.CustomInstrument(instrumentTypeID, systemPins.Concat(dutPins).ToArray());
        }
    }
}
