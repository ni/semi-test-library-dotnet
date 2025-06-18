using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Examples.CustomInstrument.MyCustomInstrument;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.CustomInstrument
{
    /// <summary>
    /// This class contains sample methods to perform Setup and Cleanup operations using Custom Instrument Support provided in STL.
    /// </summary>
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Sample method to perform setup operation. Initializes all custom instruments of specific instrument type.
        /// </summary>
        /// <param name="tsmContext">TSM Context</param>
        public static void SetupMyCustomInstruments(ISemiconductorModuleContext tsmContext)
        {
            var myCustomInstrumentFactory = new MyCustomInstrumentFactory();
            InitializeAndClose.Initialize(tsmContext, myCustomInstrumentFactory);

            OptionallyApplyConfigurations(tsmContext, myCustomInstrumentFactory.InstrumentTypeId);
        }

        /// <summary>
        /// Sample method to perform cleanup operation. Closes all references of custom instruments of specific instrument type.
        /// </summary>
        /// <param name="tsmContext">TSM Context</param>
        public static void CleanupMyCustomInstruments(ISemiconductorModuleContext tsmContext)
        {
            var myCustomInstrumentFactory = new MyCustomInstrumentFactory();
            OptionallyClearConfigurations(tsmContext, myCustomInstrumentFactory.InstrumentTypeId);

            // Close all references of custom instruments.
            InitializeAndClose.Close(tsmContext, myCustomInstrumentFactory.InstrumentTypeId);
        }

        /// <summary>
        /// Applies configurations.
        /// </summary>
        /// <param name="tsmContext">TSM Context.</param>
        /// <param name="InstrumentTypeID">Instrument type Id.</param>
        public static void OptionallyApplyConfigurations(ISemiconductorModuleContext tsmContext, string InstrumentTypeID)
        {
            tsmContext.GetPins(InstrumentTypeID, out var dutPins, out var systemPins);
            var tsmSessionManager = new TSMSessionManager(tsmContext);
            var myCustomInstrumentSessionsBundle = tsmSessionManager.CustomInstrument(InstrumentTypeID, systemPins.Concat(dutPins).ToArray());

            // User can do device configuration using Custom Instrument sessions bundle here.
            double configurationData = 1;
            myCustomInstrumentSessionsBundle.ApplyConfigurations(configurationData);
        }

        /// <summary>
        /// Clears configurations.
        /// </summary>
        /// <param name="tsmContext">TSM Context.</param>
        /// <param name="InstrumentTypeID">Instrument type Id.</param>
        public static void OptionallyClearConfigurations(ISemiconductorModuleContext tsmContext, string InstrumentTypeID)
        {
            tsmContext.GetPins(InstrumentTypeID, out var dutPins, out var systemPins);
            var tsmSessionManager = new TSMSessionManager(tsmContext);
            var myCustomInstrumentSessionsBundle = tsmSessionManager.CustomInstrument(InstrumentTypeID, systemPins.Concat(dutPins).ToArray());

            // Clears configurations.
            myCustomInstrumentSessionsBundle.ClearConfiguration();
        }
    }
    
}
