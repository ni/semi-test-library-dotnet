using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries7822R;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument
{
    /// <summary>
    /// This class contains sample method to perform digital read write test using RSeries card via the custom instrument support provided by STL.
    /// </summary>
    public static class TestStep
    {
        /// <summary>
        /// Demonstrates the use of extension methods to perform a digital read write test, write digital data to the DUT's digital input pins, read the DUT's output pin(s), and publish the measured data.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="digitalInputPins">The DUT's digital input pin(s).</param>
        /// <param name="digitalOutputPins">The DUT's digital output pin(s).</param>
        /// <param name="pinData">The per-pin digital data to be sent to the DUT.</param>
        /// <param name="publishedDataID">The data id to use for publishing the measurement result.</param>
        public static void DigitalReadWriteTest(ISemiconductorModuleContext tsmContext, string[] digitalInputPins, string[] digitalOutputPins, double[] pinData, string publishedDataID)
        {
            // The amount of time in seconds to wait for the DUT's output to settle.
            double settlingTime = 0.2;

            // Get active sites and create PinSiteData object for digital input data.
            int[] sites = tsmContext.SiteNumbers.ToArray();
            var pinSiteData = new PinSiteData<double>(digitalInputPins, sites, pinData);

            // Create TSM session manager.
            var tsmSessionManager = new TSMSessionManager(tsmContext);

            // Create sessions bundle for DUT digital input pins and sessions bundle for digital output pins.
            var digitalInputBundle = tsmSessionManager.CustomInstrument(RSeries7822RFactory.CustomInstrumentTypeId, digitalInputPins);
            var digitalOutputBundle = tsmSessionManager.CustomInstrument(RSeries7822RFactory.CustomInstrumentTypeId, digitalOutputPins);

            // Write data to digital input pins, wait for setting time and measure data on digital output pins.
            digitalInputBundle.WriteData(pinSiteData);
            Utilities.PreciseWait(settlingTime);
            var measurements = digitalOutputBundle.MeasureData();

            // Publish measured data.
            tsmContext.PublishResults(measurements, publishedDataID);
        }
    }
}
