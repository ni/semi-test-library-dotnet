using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.Common.Utilities;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Concrete implementation of ICustomInstrument Interface for controlling a PXIe-7822R R series device.
    /// </summary>
    public class RSeries7822R : ICustomInstrument
    {
        private const string ConnectorStringId = "Connector";
        private const string ChannelStringId = "DIO";
        private readonly ulong _referenceId;
        private int _status;

        /// <summary>
        /// Instrument name that matches the one defined in the pin map file.
        /// </summary>
        public string InstrumentName { get; }

        /// <summary>
        /// Channel group information.
        /// </summary>
        /// <remarks>
        /// Optionally, store channel group information for later use.
        /// </remarks>
        public string ChannelGroupId { get; }

        /// <summary>
        /// Channel list.
        /// </summary>
        /// <remarks>
        /// Store channel list for later use. This information is needed to perform driver operation on specific channel.
        /// </remarks>
        public string ChannelList { get; }

        /// <summary>
        /// Resource name which includes instrument name and channel information.
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// The channel information mapping.
        /// </summary>
        /// <remarks>
        /// Maps an individual channel string, from the pin map definition, to it's ConnectorNumber, PortNumber, and ChannelNumber.
        /// Includes the configuration of each channel of the device, which can either be an Output or an Input, depending on the port.
        /// </remarks>
        internal Dictionary<string, ChannelInfo> ChannelInfoMap { get; }

        /// <summary>
        /// The internal tracked states of all ports configured as output ports.
        /// </summary>
        /// <remarks>
        /// Store the current state of each port. This information is needed to perform driver operation on specific port.
        /// </remarks>
        private Dictionary<(int, int), byte> OutputPortStates { get; }

        /// <summary>
        /// The internal tracked input ports.
        /// </summary>
        private List<(int, int)> InputPorts { get; }

        /// <summary>
        /// The internal tracked number of connectors for input ports.
        /// </summary>
        private int InputConnectorCount { get; }

        /// <summary>
        /// The internal tracked number of connectors for input ports.
        /// </summary>
        private int InputPortCount { get; }

        /// <summary>
        /// Opens FPGA reference of the R series device.
        /// </summary>
        /// <param name="instrumentName">Instrument Name as defined in the Pin Map</param>
        /// <param name="channelGroupId">Channel Group Id as defined in the Pin Map</param>
        /// <param name="channelList">Channel List as defined in the Pin Map</param>
        public RSeries7822R(string instrumentName, string channelGroupId, string channelList)
        {
            // Store instrument name, channel and channel group information in the class object.
            InstrumentName = instrumentName;
            ChannelGroupId = channelGroupId;
            ChannelList = channelList;
            ResourceName = InstrumentName;
            // Organize internal tracking information of each channel based on port and connector.
            // This will be used for easy lookup within class methods.
            ChannelInfoMap = new Dictionary<string, ChannelInfo>();
            OutputPortStates = new Dictionary<(int, int), byte>();
            InputPorts = new List<(int, int)>();
            string[] channels = ChannelList.Split(',').Select(x => x.Trim()).ToArray();
            foreach (string channelInfoString in channels)
            {
                // The format of the channel strings is expected to have been validated prior to this point.
                var channelInfoSegments = channelInfoString.Split('_').Select(x => x.Trim());
                // This implementation intentionally allows the flexibility to specify the channels without or out the port.
                // Such that, the port can be inferred and both of these values are acceptable: "Connector0_Port1_DIO5", "Connector0_DIO5".
                string channelString = channelInfoSegments.First(s => s.StartsWith(ChannelStringId, StringComparison.Ordinal));
                int channelNumber = int.Parse(
                    channelString.Substring(ChannelStringId.Length),
                    NumberStyles.Integer,
                    NumberFormatInfo.InvariantInfo);
                string connectorString = channelInfoSegments.ElementAt(0);
                int connectorNumber = int.Parse(
                    connectorString.Substring(ConnectorStringId.Length),
                    NumberStyles.Integer,
                    NumberFormatInfo.InvariantInfo);
                // Ports are 8 bits wide
                int portNumber = channelNumber / 8;
                // This is the specific bit in port that the channel maps to.
                int portIndex = channelNumber % 8;

                // Statically defines the port mode such that the first two ports of each connector are designated Output ports,
                // while the second two ports of each connector are designated Input ports.
                // This could otherwise be exposed as a configuration method for this class but is outside of the scope of the example.
                PortMode portMode = (portNumber < 2) ? PortMode.Output : PortMode.Input;
                var connectorPort = (connectorNumber, portNumber);
                if (portMode == PortMode.Output && !OutputPortStates.ContainsKey(connectorPort))
                {
                    OutputPortStates.Add(connectorPort, 0);
                }
                if (portMode == PortMode.Input && !InputPorts.Contains(connectorPort))
                {
                    InputPorts.Add(connectorPort);
                }

                ChannelInfoMap.Add(channelInfoString, new ChannelInfo(connectorNumber, portNumber, channelNumber, portIndex, portMode));
            }

            // Cache the number of input connectors and ports so that this is only calculated only once, upfront,
            // and can be efficiently retrieved by each invocation of the ReadPortData method.
            InputConnectorCount = InputPorts.Select(x => x.Item1).Distinct().Count();
            InputPortCount = InputPorts.Select(x => x.Item2).Distinct().Count();

            // Open FPGA reference by deploying BitFile on the RIO device of the given Instrument.
            string bitFilePath = RSeries7822RDriverAPI.BitFilePath();
            _status = RSeries7822RDriverAPI.OpenFPGA(ResourceName, bitFilePath, out ulong fpgaRef);
            _referenceId = fpgaRef;
            ValidateStatus($"Error in OpenFPGA method, Error Code:{_status}, Resource Name:{ResourceName}, Ref:{_referenceId}, Bitfile:{bitFilePath}");
        }

        /// <summary>
        /// Closes FPGA reference of the R series device.
        /// </summary>
        /// <exception cref="Exception">Thrown when the low-level FPGA method 'CloseFPGA' fails.</exception>
        public void Close()
        {
            _status = RSeries7822RDriverAPI.CloseFPGA(_referenceId);
            ValidateStatus($"Error in CloseFPGA method, ErrorCode:{_status}");
        }

        /// <summary>
        /// Resets the R series device.
        /// </summary>
        public void Reset()
        {
            // Not supported, NOP.
        }

        /// <summary>
        /// Enables LoopBack mode.
        /// </summary>
        /// <param name="enable">Whether to enable or disable LoopBack mode.</param>
        /// <exception cref="Exception">Thrown when the low-level FPGA method 'EnableLoopBack' fails. </exception>
        public void EnableLoopBack(bool enable)
        {
            ulong value = enable ? 1UL : 0UL;
            _status = RSeries7822RDriverAPI.EnableLoopBack(_referenceId, value);
            ValidateStatus($"Error in EnableLoopBack method, ErrorCode:{_status}");
        }

        /// <summary>
        /// Writes data to all ports of the R series device.
        /// </summary>
        /// <exception cref="Exception">Thrown when the low-level FPGA method 'WriteData' fails.</exception>
        public void WritePortData()
        {
            foreach (var portState in OutputPortStates)
            {
                string portName = BuildPortName(portState.Key.Item1, portState.Key.Item2);
                _status = RSeries7822RDriverAPI.WriteData(_referenceId, portName, portState.Value);
                ValidateStatus($"Error in WriteData method, ErrorCode:{_status}, PortNumber:{portState.Key}, PortData:{portState.Value}");
            }
        }

        /// <summary>
        /// Sets the state of a specific channel.
        /// The state will be applied only when the WritePortData method is called.
        /// </summary>
        /// <param name="channel">The selected channel.</param>
        /// <param name="value">Value to set.</param>
        /// <exception cref="Exception">Thrown when the low-level FPGA method 'WriteData' fails.</exception>
        public void SetOutputState(string channel, bool value)
        {
            var channelInfo = ChannelInfoMap[channel];
            var key = (channelInfo.ConnectorNumber, channelInfo.PortNumber);
            byte currentState = OutputPortStates[key];
            byte newState = UpdateBitInByte(currentState, value, channelInfo.PortIndex);

            OutputPortStates[key] = newState;
        }

        /// <summary>
        /// RSeries card read channel data operation.
        /// </summary>
        /// <returns>Port data, where the first dimension represents the connector and the second dimension represents the ports.</returns>
        /// <exception cref="Exception">Thrown when FPGA 'ReadData' fails.</exception>
        public byte[][] ReadPortData()
        {
            // Initialize jagged array for storing port data.
            var portsData = new byte[InputConnectorCount][];

            // Read port date from R series device and store values in jagged array.
            for (int i = 0; i < InputPorts.Count; i++)
            {
                var key = InputPorts[i];
                int connectorNumber = key.Item1;
                int portNumber = key.Item2;
                string portName = BuildPortName(connectorNumber, portNumber);
                _status = RSeries7822RDriverAPI.ReadData(_referenceId, portName, out byte data);
                ValidateStatus($"Error in ReadData method, ErrorCode:{_status}, PortNumber:{portNumber}");

                // Allocate new byte array for each connector
                if (portsData[connectorNumber] == null)
                {
                    portsData[connectorNumber] = new byte[InputPortCount];
                }
                // Get the index of the connectorNumber and portNumber relative to the inputPorts array.
                int connectorNumberIndex = connectorNumber % InputConnectorCount;
                int portNumberIndex = portNumber % InputPortCount;
                // Fill the array with the specific port's data
                portsData[connectorNumberIndex][portNumberIndex] = data;
            }

            return portsData;
        }

        private void ValidateStatus(string exceptionMessage)
        {
            if (_status != 0)
            {
                throw new RSeries7822RDriverAPIException(exceptionMessage);
            }
        }

        private string BuildPortName(int connectorNumber, int portNumber)
        {
            return $"Connector{connectorNumber}_DIOPORT{portNumber}";
        }
    }

    internal struct ChannelInfo
    {
        public int ConnectorNumber;
        public int ChannelNumber;
        public int PortNumber;
        public int PortIndex;
        public PortMode Mode;

        public ChannelInfo(int connectorNumber, int portNumber, int channelNumber, int portIndex, PortMode mode)
        {
            ConnectorNumber = connectorNumber;
            PortNumber = portNumber;
            ChannelNumber = channelNumber;
            PortIndex = portIndex;
            Mode = mode;
        }
    }

    /// <summary>
    /// Represents the possible configuration modes for the digital ports of an R Series device, either Input or Output.
    /// </summary>
    public enum PortMode
    {
        /// <summary>
        /// Output mode.
        /// </summary>
        Output,
        /// <summary>
        /// Input mode.
        /// </summary>
        Input
    }
}
