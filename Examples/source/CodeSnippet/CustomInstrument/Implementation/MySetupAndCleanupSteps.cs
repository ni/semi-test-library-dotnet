using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.CustomInstrument.Examples
{
    /// <summary>
    /// This class contains sample methods to perform Setup and Cleanup operations using Custom Instrument Support provided in STL.
    /// </summary>
    public static partial class MySetupAndCleanupSteps
    {
        /// <summary>
        /// Sample method to perform setup operation. Initializes all custom instruments of specific instrument type ID.
        /// </summary>
        /// <param name="tsmContext">TSM Context</param>
        public static void SetupMyCustomInstruments(ISemiconductorModuleContext tsmContext)
        {
            var myCustomInstrumentFactory = new MyCustomInstrumentFactory();
            InitializeAndClose.Initialize(tsmContext, myCustomInstrumentFactory);

            /// Optionally create CustomInstrument session Bundle to do device configuration at the time of setup.
            tsmContext.GetPins(myCustomInstrumentFactory.InstrumentTypeId, out var dutPins, out var systemPins);
            var tsmSessionManager = new TSMSessionManager(tsmContext);
            CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle = tsmSessionManager.CustomInstrument(myCustomInstrumentFactory.InstrumentTypeId, systemPins.Concat(dutPins).ToArray());

            // User can do device configuration using custom instrument sessions bundle here.
            double parameter1 = 1;
            double parameter2 = 2; 
            // configure devices.
            myCustomInstrumentSessionsBundle.ConfigureMyCustomInstrument(parameter1, parameter2);
        }

        /// <summary>
        /// Sample method to perform Cleanup operation. Closes all references of custom instruments of specific instrument type ID.
        /// </summary>
        /// <param name="tsmContext">TSM Context</param>
        public static void CleaupMyCustomInstruments(ISemiconductorModuleContext tsmContext)
        {
            var myCustomInstrumentFactory = new MyCustomInstrumentFactory();

            // Optionally user can clear device configurations before closing sessions.
            // Create CustomInstrument session Bundle call clear device configurations.
            tsmContext.GetPins(myCustomInstrumentFactory.InstrumentTypeId, out var dutPins, out var systemPins);
            var tsmSessionManager = new TSMSessionManager(tsmContext);
            CustomInstrumentSessionsBundle myCustomInstrumentSessionsBundle = tsmSessionManager.CustomInstrument(myCustomInstrumentFactory.InstrumentTypeId, systemPins.Concat(dutPins).ToArray());

            // Clear configurations
            myCustomInstrumentSessionsBundle.ClearConfiguration();

            // Close all references of customInstruments
            InitializeAndClose.Close(tsmContext, myCustomInstrumentFactory);
        }
    }
}
