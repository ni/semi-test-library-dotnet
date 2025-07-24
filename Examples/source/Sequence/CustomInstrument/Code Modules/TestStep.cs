using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.MyCustomInstrument;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument
{
    /// <summary>
    /// This class contains sample methods to perform high-level testing of the DUT using the custom instrument support provided by STL.
    /// </summary>
    /// <remarks>
    /// This hypothetical example considers the Device Under Test (DUT) to be some kind of Digital to Analog Converter (DAC) and the Custom Instrument to be some kind of multifunctional Data Acquisition (DAQ) device capable of sourcing digital signals and acquiring analog signals. The DUT is connected to the Custom Instrument so that it can receive digital signals from the Custom Instrument and output an analog signal back to the Custom Instrument to be acquired.
    /// </remarks>
    public static class TestStep
    {
        /// <summary>
        /// Demonstrates the use of the HighLevelDriverOperations extension methods to perform a hypothetical functional test of the DUT, where the Custom Instrument sends a digital signal, waits for the DUT to settle, and then measures the analog signal returned from the DUT.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="digitalInputPins">The DUT's digital input pins.</param>
        /// <param name="analogOutputPins">The DUT's analog output pin(s).</param>
        /// <param name="pinData">The per-pin digital data to be sent to the DUT.</param>
        /// <param name="publishedDataID">The data id to use for publishing the measurement result.</param>
        public static void FunctionalTest(ISemiconductorModuleContext tsmContext, string[] digitalInputPins, string[] analogOutputPins, double[] pinData, string publishedDataID)
        {
            // The amount of time in seconds to wait for the DUT's output to settle.
            double settlingTime = 0.2;

            // Generate pinSiteData.
            int[] sites = tsmContext.SiteNumbers.ToArray();
            var pinSiteData = new PinSiteData<double>(digitalInputPins, sites, pinData);

            // Create TSM session manager.
            var tsmSessionManager = new TSMSessionManager(tsmContext);

            // Create bundle for DUT digital input pins and apply signal using custom instrument.
            var digitalInputBundle = tsmSessionManager.CustomInstrument(MyCustomInstrumentFactory.CustomInstrumentTypeId, digitalInputPins);
            digitalInputBundle.WriteData(pinSiteData);

            // Need to wait for the DUT's output to settle before measuring.
            Utilities.PreciseWait(settlingTime);

            // Create session bundle for analog output pins and measure signal using custom instrument.
            var analogOutputBundle = tsmSessionManager.CustomInstrument(MyCustomInstrumentFactory.CustomInstrumentTypeId, analogOutputPins);
            var measurements = analogOutputBundle.MeasureData();

            // Publish measured data.
            tsmContext.PublishResults(measurements, publishedDataID);
        }
    }
}
