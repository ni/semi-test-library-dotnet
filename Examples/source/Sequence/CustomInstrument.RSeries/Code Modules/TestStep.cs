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
        /// Demonstrates the use of extension methods to perform a digital read/write test by writing data to the DUT digital input ports,
        /// reading from the digital output ports, and publishing the resulting output data from the DUT.
        /// Data is published as port values, where the published data id matches the dutDigitalOutputPorts values.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutDigitalInputPorts">The pin group names corresponding to the DUT digital input ports.</param>
        /// <param name="dutDigitalOutputPorts">The pin group names corresponding to the DUT digital output ports.</param>
        /// <param name="portData">The byte data to be written to the DUT's digital input ports.</param>
        public static void DigitalReadWriteTest(
            ISemiconductorModuleContext tsmContext,
            string[] dutDigitalInputPorts,
            string[] dutDigitalOutputPorts,
            byte[] portData)
        {
            // The amount of time in seconds to wait for the DUT's output to settle.
            double settlingTime = 0.2;

            // Get active sites and create PinSiteData object for digital input data.
            int[] sites = tsmContext.SiteNumbers.ToArray();

            // Need to expand the pin groups representing the DUT's digital input ports into individual pins as well as their associated data,
            // which is necessary to construct the appropriate PinSiteData object.
            ExpandPortDataToPinData(tsmContext, dutDigitalInputPorts, portData, out bool[] expandedPinData, out string[] expandedInputPins);
            var portDataToWrite = new PinSiteData<bool>(expandedInputPins, sites, expandedPinData);

            // Create TSM session manager.
            var sessionManager = new TSMSessionManager(tsmContext);

            // Create sessions bundle for DUT digital input pins and sessions bundle for digital output pins.
            var digitalInputBundle = sessionManager.CustomInstrument(RSeries7822RFactory.CustomInstrumentTypeId, dutDigitalInputPorts);
            var digitalOutputBundle = sessionManager.CustomInstrument(RSeries7822RFactory.CustomInstrumentTypeId, dutDigitalOutputPorts);

            // Write data to DUT's digital input pins, wait for settling time and read data on digital output pins.
            digitalInputBundle.WriteData(portDataToWrite);
            Utilities.PreciseWait(settlingTime);

            // Read data from DUT's digital output pins
            PinSiteData<bool> perPinData = digitalOutputBundle.ReadData();

            // Packs per-pin output values into a single byte based on pin groups passed in via the digitalOutputPins input parameter,
            PinSiteData<byte> perPortData = ConvertDUTPortPinDataToByte(tsmContext, dutDigitalOutputPorts, perPinData);

            // Publish port-based data.
            foreach (var portName in dutDigitalOutputPorts)
            {
                // TSM cannot publish a single value for all pins in a pin group nor can it publish byte data types directly.
                // Therefore, the site data must first be extracted by the pin group name and cast as a int before publishing,
                // which is done using the ExtractPin and Select methods, respectively.
                // This must be repeated for each port's data.
                tsmContext.PublishResults(perPortData.ExtractPin(portName).Select(x => (int)x), portName);
            }
        }

        /// <summary>
        /// This method expands the pin groups representing the DUT's digital input ports into individual pins as well as their associated data.
        /// </summary>
        /// <remarks>
        /// The elements of the dutDigitalInputPorts input parameter must be passed as valid pin group names.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutDigitalInputPorts">The pin group names corresponding to the DUT digital input ports.</param>
        /// <param name="portData">The byte data to be written to the DUT's digital input ports.</param>
        /// <param name="expandedPinData">The expanded data values for each pin of the DUT's digital input ports.</param>
        /// <param name="expandedInputPins">The expanded pin names of each pin group corresponding to the DUT's digital input ports.</param>
        /// <exception cref="ArgumentException">
        /// Returned if one or more elements of the dutDigitalInputPorts input parameter is an individual pin or not a valid pin group name.
        /// </exception>
        private static void ExpandPortDataToPinData(
            ISemiconductorModuleContext tsmContext,
            string[] dutDigitalInputPorts,
            byte[] portData,
            out bool[] expandedPinData,
            out string[] expandedInputPins)
        {
            var expandedPinDataList = new List<bool>();
            var expandedInputPinsList = new List<string>();
            for (int portIndex = 0; portIndex < dutDigitalInputPorts.Length; portIndex++)
            {
                int portValue = portData[portIndex];
                string pinGroup = dutDigitalInputPorts[portIndex];

                var expandedPins = tsmContext.FilterPinsByInstrumentType(new[] { pinGroup }, RSeries7822RFactory.CustomInstrumentTypeId);
                // Throw an exception if the Length of expandedPins returned by FilterPinsByInstrumentType is 0 or 1.
                // This implies that either the element of dutDigitalInputPorts is either an individual pin (if 1),
                // or is not a valid pin group (if 0).
                if (expandedPins.Length < 2)
                {
                    throw new ArgumentException(
                        $"{pinGroup} is not a valid pin group." +
                        $"Each element of the dutDigitalInputPorts array input parameter must be valid pin group." +
                        $"Using individual pin names are not valid.");
                }
                for (int expandedPinIndex = 0; expandedPinIndex < expandedPins.Length; expandedPinIndex++)
                {
                    expandedPinDataList.Add(GetBitFromByte((byte)portValue, expandedPinIndex));
                }
                expandedInputPinsList.AddRange(expandedPins);
            }
            expandedPinData = expandedPinDataList.ToArray();
            expandedInputPins = expandedInputPinsList.ToArray();
        }

        /// <summary>
        /// Converts the individual DUT pin data values into a byte values based on the pin groups corresponding to the DUT's digital output ports.
        /// </summary>
        /// <remarks>
        /// Note that this conversion is necessary as the channel mapping of the DUT's port pins may not be exactly aligned with the ports of R series device.
        /// </remarks>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutDigitalOutputPorts">The pin group names corresponding to the DUT digital output ports.</param>
        /// <param name="perPinData">Per site data values for each pin within the DUT's digital output ports</param>
        /// <returns>
        /// A new <see cref="PinSiteData{T}"/> object of <see cref="byte"/>s representing the port values corresponding to the DUT's digital output ports.
        /// </returns>
        private static PinSiteData<byte> ConvertDUTPortPinDataToByte(
            ISemiconductorModuleContext tsmContext,
            string[] dutDigitalOutputPorts,
            PinSiteData<bool> perPinData)
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
                            results[targetPinGroup] = perSiteStates;
                        }

                        foreach (var site in perPinData.SiteNumbers)
                        {
                            bool value = perPinData.GetValue(site, pin);
                            // The state value will default to 0 if absent.
                            perSiteStates.TryGetValue(site, out byte state);
                            perSiteStates[site] = UpdateBitInByte(state, value, bitIndex);
                        }
                    }
                }
            }
            return new PinSiteData<byte>(results);
        }
    }
}
