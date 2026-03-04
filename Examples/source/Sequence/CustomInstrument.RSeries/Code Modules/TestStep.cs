using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.Common.Utilities;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries
{
    /// <summary>
    /// This class contains sample method to perform digital read write test using RSeries card via the Custom Instrument support provided by STL.
    /// </summary>
    public static class TestStep
    {
        /// <summary>
        /// Demonstrates the use of extension methods to perform a digital read/write test by writing data to the DUT digital input ports, reading from the digital output ports, and publishing the resulting output data from the DUT.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutDigitalInputPorts">The pin group names corresponding to the DUT digital input ports.</param>
        /// <param name="dutDigitalOutputPorts">The pin group names corresponding to the DUT digital output ports.</param>
        /// <param name="portData">The byte data to be written to the DUT's digital input ports.</param>
        /// <param name="publishedDataID">The data id to use for publishing the values read-back from the DUT's digital output ports.</param>
        public static void DigitalReadWriteTest(ISemiconductorModuleContext tsmContext, string[] dutDigitalInputPorts, string[] dutDigitalOutputPorts, byte[] portData, string publishedDataID)
        {
            // The amount of time in seconds to wait for the DUT's output to settle.
            double settlingTime = 0.2;

            // Get active sites and create PinSiteData object for digital input data.
            int[] sites = tsmContext.SiteNumbers.ToArray();

            // Need to expand the pin groups representing the DUT's digital input ports into individual pins as well as their associated data,
            // which is necessary to construct the appropriate PinSiteData object.
            ExpandDataForPinGroups(tsmContext, dutDigitalInputPorts, portData, out List<bool> expandedPinData, out List<string> expandedInputPins);
            var pinSitePinData = new PinSiteData<bool>(expandedInputPins.ToArray(), sites, expandedPinData.ToArray());

            // Create TSM session manager.
            var sessionManager = new TSMSessionManager(tsmContext);

            // Create sessions bundle for DUT digital input pins and sessions bundle for digital output pins.
            var digitalInputBundle = sessionManager.CustomInstrument(RSeries7822RFactory.CustomInstrumentTypeId, dutDigitalInputPorts);
            var digitalOutputBundle = sessionManager.CustomInstrument(RSeries7822RFactory.CustomInstrumentTypeId, dutDigitalOutputPorts);

            // Write data to DUT's digital input pins, wait for settling time and read data on digital output pins.
            digitalInputBundle.WriteData(pinSitePinData);
            Utilities.PreciseWait(settlingTime);

            // Read data from DUT's digital output pins
            PinSiteData<bool> perPinData = digitalOutputBundle.ReadData();

            // Packs per-pin output values into a single byte based on pin groups passed in via the digitalOutputPins input parameter,
            PinSiteData<byte> results = ConvertGroupedChannelDataToByte(tsmContext, dutDigitalOutputPorts, perPinData);

            // Publish port-based data.
            foreach (var portName in dutDigitalOutputPorts)
            {
                tsmContext.PublishResults(results.ExtractPin(portName), publishedDataID);
            }
        }

        /// <summary>
        /// This method expands the pin groups representing the DUT's digital input ports into individual pins as well as their associated data.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutDigitalInputPorts">The pin group names corresponding to the DUT digital input ports.</param>
        /// <param name="portData">The byte data to be written to the DUT's digital input ports.</param>
        /// <param name="expandedPinData">The expanded data values for each pin of the DUT's digital input ports.</param>
        /// <param name="expandedInputPins">The expanded pin names of each pin group corresponding to the DUT's digital input ports.</param>
        private static void ExpandDataForPinGroups(
            ISemiconductorModuleContext tsmContext,
            string[] dutDigitalInputPorts,
            byte[] portData,
           out List<bool> expandedPinData,
           out List<string> expandedInputPins)
        {
            expandedPinData = new List<bool>();
            expandedInputPins = new List<string>();
            for (int pinIndex = 0; pinIndex < dutDigitalInputPorts.Length; pinIndex++)
            {
                int pinValue = portData[pinIndex];
                string pinOrPinGroup = dutDigitalInputPorts[pinIndex];
                if (pinValue > 1)
                {
                    var expandedPins = tsmContext.FilterPinsByInstrumentType(new[] { pinOrPinGroup }, RSeries7822RFactory.CustomInstrumentTypeId);
                    if (expandedPins.Length > 1)
                    {
                        for (int expandedPinIndex = 0; expandedPinIndex < expandedPins.Length; expandedPinIndex++)
                        {
                            expandedPinData.Add((pinValue & (1 << expandedPinIndex)) != 0);
                        }
                    }
                    expandedInputPins.AddRange(expandedPins);
                }
                else
                {
                    expandedPinData.Add(portData[pinIndex] > 0);
                    expandedInputPins.Add(pinOrPinGroup);
                }
            }
        }

        /// <summary>
        /// Converts the individual pin data values into a byte values based on the pin groups corresponding to the DUT's digital output ports.
        /// </summary>
        /// <remarks>
        /// Note that this conversion is necessary since the channel mapping of the DUT's port pins may not be exactly aligned with the ports of R series device.
        /// This just so happens to be the case in this example's pin map.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutDigitalOutputPorts">The pin group names corresponding to the DUT digital output ports.</param>
        /// <param name="perPinData">Per site data values for each pin within the DUT's digital output ports</param>
        /// <returns>A new <see cref="PinSiteData{T}"/> object of <see cref="byte"/>s representing the port values corresponding to the DUT's digital output ports.</returns>
        private static PinSiteData<byte> ConvertGroupedChannelDataToByte(ISemiconductorModuleContext tsmContext, string[] dutDigitalOutputPorts, PinSiteData<bool> perPinData)
        {
            Dictionary<string, IDictionary<int, byte>> results = new Dictionary<string, IDictionary<int, byte>>();
            foreach (var targetPinGroup in dutDigitalOutputPorts)
            {
                string[] pinsInTargetPinGroup = tsmContext.GetPinsInPinGroup(targetPinGroup);

                foreach (string pin in perPinData.PinNames)
                {
                    if (pinsInTargetPinGroup.Contains(pin))
                    {
                        int bitIndex = Array.IndexOf(pinsInTargetPinGroup, pin);
                        if (!results.TryGetValue(targetPinGroup, out var perSiteStates))
                        {
                            perSiteStates = new Dictionary<int, byte>();
                            results.Add(targetPinGroup, perSiteStates);
                        }

                        foreach (var site in perPinData.SiteNumbers)
                        {
                            bool value = perPinData.GetValue(site, pin);

                            if (!perSiteStates.TryGetValue(site, out byte state))
                            {
                                // Initialize internal port state
                                state = 0;
                                results[targetPinGroup].Add(site, state);
                            }

                            results[targetPinGroup][site] = UpdateBitInByte(state, value, bitIndex);
                        }
                    }
                }
            }
            return new PinSiteData<byte>(results);
        }
    }
}
