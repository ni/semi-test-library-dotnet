﻿using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.MyCustomInstrument;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument
{
    /// <summary>
    /// This class contains sample method to perform digital read write test using RSeries card via CustomInstrument feature of STL.
    /// </summary>
    public static class TestStep
    {
        /// <summary>
        /// Demonstrates the use of extension methods to perform a digital read write test, write digital data to the DUT's digital input pins, read the DUT's output pin(s), and publish the measured data.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="digitalInputPins">The DUT's digital input pins.</param>
        /// <param name="digitalOutputPins">The DUT's analog output pin(s).</param>
        /// <param name="pinData">The per-pin digital data to be sent to the DUT.</param>
        /// <param name="publishedDataID">The data id to use for publishing the measurement result.</param>
        public static void DigitalReadWriteTest(ISemiconductorModuleContext tsmContext, string[] digitalInputPins, string[] digitalOutputPins, double[] pinData, string publishedDataID)
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
            var digitalOutputBundle = tsmSessionManager.CustomInstrument(MyCustomInstrumentFactory.CustomInstrumentTypeId, digitalOutputPins);
            var measurements = digitalOutputBundle.MeasureData();

            // Publish measured data.
            tsmContext.PublishResults(measurements, publishedDataID);
        }
    }
}
