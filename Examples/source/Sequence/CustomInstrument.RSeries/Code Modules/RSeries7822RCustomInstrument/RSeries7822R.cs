using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.Common.Utilities;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Concrete implementation of ICustomInstrument Interface for controlling a PXIe-7822R R series device.
    /// </summary>
    public class RSeries7822R : ICustomInstrument
    {
        private const string ChannelStringPattern = @"^Connector(?<Connector>\d+)(?:_Port(?<Port>\d+))?_DIO(?<DIO>\d+)$";
        private const int ChannelsPerConnector = 32;
        private const int ChannelsPerPort = 8;
        private const int ConnectorsPerDevice = 2;
        private const int PortsPerConnector = ChannelsPerConnector / ChannelsPerPort;
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
        internal IDictionary<string, ChannelInfo> ChannelInfoMap { get; } = new Dictionary<string, ChannelInfo>();

        /// <summary>
        /// The internal tracked states of all ports configured as output ports.
        /// </summary>
        /// <remarks>
        /// Store the current state of each port. This information is needed to perform driver operation on specific port.
        /// </remarks>
        internal IDictionary<(int, int), byte> OutputPortStates { get; } = new Dictionary<(int, int), byte>();

        /// <summary>
        /// The internal tracked input ports.
        /// </summary>
        internal IList<(int, int)> InputPorts { get; } = new List<(int, int)>();

        /// <summary>
        /// Opens FPGA reference of the R series device.
        /// </summary>
        /// <param name="instrumentName">Instrument Name as defined in the Pin Map</param>
        /// <param name="channelGroupId">Channel Group Id as defined in the Pin Map</param>
        /// <param name="channelList">Channel List as defined in the Pin Map</param>
        public RSeries7822R(string instrumentName, string channelGroupId, string channelList)
        {
            if (string.IsNullOrEmpty(instrumentName) && string.IsNullOrEmpty(channelGroupId) && string.IsNullOrEmpty(channelList))
            {
                throw new ArgumentException(
                    $"One or more arguments passed into the {nameof(RSeries7822R)} constructor is either null or an empty string.");
            }

            // Store instrument name, channel and channel group information in the class object.
            InstrumentName = instrumentName;
            ChannelGroupId = channelGroupId;
            ChannelList = channelList;
            ResourceName = InstrumentName;
            // Organize internal tracking information of each channel based on port and connector.
            // This will be used for easy lookup within class methods.
            string[] channels = ChannelList.Split(',').Select(x => x.Trim()).ToArray();

            var regex = new Regex(ChannelStringPattern, RegexOptions.Compiled);
            foreach (string channelString in channels)
            {
                // The channelString must be parsed and validated to extract individual integer values for the connector, port, and channel.
                ParseAndValidateChannelString(channelString, regex, out int connectorNumber, out int portNumber, out int channelNumber);

                // This is the specific bit in port that the channel maps to.
                int portIndex = channelNumber % ChannelsPerPort;

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

                ChannelInfoMap.Add(
                    channelString,
                    new ChannelInfo(connectorNumber, portNumber, channelNumber, portIndex, portMode));
            }

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
        /// <returns>
        /// An <see cref="IDictionary{TKey, TValue}"/> containing a single byte value for each
        /// port of the R Series device. Each entry is keyed by a (connector, port) tuple,
        /// where the first item identifies the connector number and the second item
        /// identifies the port number.
        /// </returns>
        /// <exception cref="Exception">Thrown when FPGA 'ReadData' fails.</exception>
        public IDictionary<(int, int), byte> ReadPortData()
        {
            // Initialize dictionary object for storing port data.
            var portsData = new Dictionary<(int, int), byte>();

            // Read data from each input port of the R series device and add values to dictionary.
            foreach (var inputPort in InputPorts)
            {
                int connectorNumber = inputPort.Item1;
                int portNumber = inputPort.Item2;
                string portName = BuildPortName(connectorNumber, portNumber);
                _status = RSeries7822RDriverAPI.ReadData(_referenceId, portName, out byte data);
                ValidateStatus($"Error in ReadData method, ErrorCode:{_status}, PortNumber:{portNumber}");

                portsData.Add((connectorNumber, portNumber), data);
            }

            return portsData;
        }

        /// <summary>
        /// Parses and validates the channelString to extract the connectorNumber, portNumber, and channelNumber.
        /// </summary>
        /// <param name="channelString">
        /// The string value representing the information associated with the channel.
        /// The value of which is expected to be formatted as either:
        /// "Connector{connectorNumber}_Port{portNumber}_DIO{channelNumber}" or  "Connector{connectorNumber}_DIO{channelNumber}".
        /// </param>
        /// <param name="regex">The <see cref="Regex"/> object representing the immutable regular expression used to parse the channelString.</param>
        /// <param name="connectorNumber">The zero-based number that identifies the channel of the device within its associated connector.</param>
        /// <param name="portNumber">The zero-based number for the port associated with the channel.</param>
        /// <param name="channelNumber">The zero-based number that identifies the channel of the device within its associated connector.</param>
        private void ParseAndValidateChannelString(string channelString, Regex regex, out int connectorNumber, out int portNumber, out int channelNumber)
        {
            var matches = regex.Match(channelString);
            if (!matches.Groups["Connector"].Success && !matches.Groups["DIO"].Success)
            {
                throw new ArgumentException($"Invalid value in channelList ({channelString}). " +
                    "The channelList must be a comma separated string of values, " +
                    "where each value is formatted as either: \"Connector0_Port1_DIO5\" or  \"Connector0_DIO5\".");
            }

            connectorNumber = int.Parse(matches.Groups["Connector"].Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            var maxConnectorNumber = ConnectorsPerDevice - 1;
            if (connectorNumber > maxConnectorNumber)
            {
                throw new ArgumentException($"Invalid connector number ({connectorNumber}) specified in channelList value: {channelString}." +
                    $"The connector number must be zero indexed and less than or equal to {maxConnectorNumber}");
            }

            channelNumber = int.Parse(matches.Groups["DIO"].Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);
            var maxChannelNumber = ChannelsPerConnector - 1;
            if (channelNumber > maxChannelNumber)
            {
                throw new ArgumentException($"Invalid channel number ({channelNumber}) specified in channelList value: {channelString}." +
                    $"The channel number must be zero indexed and less than or equal to {maxChannelNumber}");
            }

            // Ports are expected to be 8 bits wide. This can be changed via the ChannelsPerPort constant.
            // This implementation intentionally allows the flexibility to specify the channels with or without the port.
            // Such that, the port can be inferred and both of these values are acceptable: "Connector0_Port1_DIO5", "Connector0_DIO5".
            // If the port string is not provided within the channelInfoString, the expected/inferred port number will be used,
            // which is based on how wide each port is.
            // If the port string is provided within the channelInfoString, it will be used if valid.
            int expectedPortNumber = channelNumber / ChannelsPerPort;
            portNumber = expectedPortNumber;
            if (matches.Groups["Port"].Success)
            {
                portNumber = int.Parse(matches.Groups["Port"].Value, NumberStyles.Integer, NumberFormatInfo.InvariantInfo);

                if (portNumber != expectedPortNumber || portNumber > (PortsPerConnector - 1))
                {
                    // Port string ID found in channelInfoSegments, but it is invalid.
                    throw new ArgumentException($"Invalid port number ({portNumber}) specified in channelList value: {channelString}.");
                }
            }
        }

        /// <summary>
        /// Validates the status returned by the lower-level P/Invoke methods of the RSeries7822RDriverAPI.
        /// Throws an exception with the exceptionMessage passed in when an error status is returned.
        /// </summary>
        /// <param name="exceptionMessage">The message to associate when an exception occurs.</param>
        /// <exception cref="RSeries7822RDriverAPIException"></exception>
        private void ValidateStatus(string exceptionMessage)
        {
            if (_status != 0)
            {
                throw new RSeries7822RDriverAPIException(exceptionMessage);
            }
        }

        /// <summary>
        /// Builds the connector-port name string required to pass to the lower-level P/Invoke methods of the RSeries7822RDriverAPI.
        /// </summary>
        /// <param name="connectorNumber">The connector number.</param>
        /// <param name="portNumber">The port number.</param>
        /// <returns>
        /// The connector-port name string.
        /// </returns>
        private string BuildPortName(int connectorNumber, int portNumber)
        {
            return $"Connector{connectorNumber}_DIOPORT{portNumber}";
        }
    }
}
