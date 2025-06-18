using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.Examples.CustomInstrument.MyCustomInstrument;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.CustomInstrument
{
    /// <summary>
    /// This class contains sample TestStep methods to test DUTs using DriverOperation extension methods.
    /// </summary>
    /// <remarks>
    /// Considering DUT to be some kind of DAC (Digital to Analog Converter). Creating some common TestSteps to perform Static Tests on the DUT.
    /// Also, considering CustomInstrument to be some kind of DAQ device, which can source digital signal and measure analog signals.
    /// </remarks>
    public static partial class TestSteps
    {
        /// <summary>
        /// Performs functional test by providing specified digital input signal (Pin wise) and measures analog signal (Pin wise).
        /// </summary>
        /// <param name="tsmContext">TSM context.</param>
        /// <param name="digitalInputPins">DAC digital input pins.</param>
        /// <param name="analogOutputPins">DAC analog output pins.</param>
        /// <param name="pinData">Digital input data pinwise.</param>
        /// <param name="publishedDataID">Published data ID.</param>
        public static void FunctionalTest(ISemiconductorModuleContext tsmContext, string[] digitalInputPins, string[] analogOutputPins, double[] pinData, string publishedDataID)
        {
            // Generate pinSiteData.
            int[] sites = tsmContext.SiteNumbers.ToArray();
            var pinSiteData = new PinSiteData<double>(digitalInputPins, sites, pinData);

            // Create TSM session manager.
            var myCustomInstrumentFactory = new MyCustomInstrumentFactory();
            var tsmSessionManager = new TSMSessionManager(tsmContext);

            // Create bundle for DUT digital input pins and apply signal using custom instrument.
            var digitalInputBundle = tsmSessionManager.CustomInstrument(myCustomInstrumentFactory.InstrumentTypeId, digitalInputPins);
            digitalInputBundle.DriverOperationWithPinSiteDataInput(pinSiteData);

            // Create session bundle for analog output pins and measure signal using custom instrument.
            var analogOutputBundle = tsmSessionManager.CustomInstrument(myCustomInstrumentFactory.InstrumentTypeId, analogOutputPins);
            var measurements = analogOutputBundle.DriverOperationThatReturnsPinSiteData();

            // Publish measured data.
            tsmContext.PublishResults(measurements, publishedDataID);
        }
    }
}
