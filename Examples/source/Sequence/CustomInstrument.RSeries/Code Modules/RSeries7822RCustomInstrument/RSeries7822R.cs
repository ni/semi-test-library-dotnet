using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.Common.Utilities;
using static NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument.Types;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Concrete implementation of ICustomInstrument Interface for controlling a PXIe-7822R R series device.
    /// </summary>
    public class RSeries7822R : ICustomInstrument
    {
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
        /// </remarks>
        internal Dictionary<string, ChannelInfo> ChannelInfoMap { get; }

        /// <summary>
        /// The internal tracked states of all ports configured as output ports.
        /// </summary>
        /// <remarks>
        /// Store the current state of each port. This information is needed to perform driver operation on specific port.
        /// </remarks>
        internal Dictionary<ChannelInfo, byte> OutputPortStates { get; private set; }

        /// <summary>
        /// The configuration of each port of the device, which can either be set as an Output or an Input.
        /// </summary>
        internal Dictionary<(int, int), PortConfiguration> PortConfigurations { get; private set; }

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
            OutputPortStates = new Dictionary<ChannelInfo, byte>();
            PortConfigurations = new Dictionary<(int, int), PortConfiguration>();
            string[] channels = ChannelList.Split(',');
            var connectorStringId = "Connector";
            var channelStringId = "DIO";
            for (int i = 0; i < channels.Length; i++)
            {
                // The format of the channel strings is expected to have been validated prior to this point.
                string channelInfoString = channels[i];
                var channelInfoSegments = channels[i].Split('_').Select(x => x.Trim());
                string channelString = channelInfoSegments.FirstOrDefault(s => s.StartsWith(channelStringId, StringComparison.Ordinal));
                int channelNumber = int.Parse(
                    channelString.Substring(channelStringId.Length),
                    NumberStyles.Integer,
                    NumberFormatInfo.InvariantInfo);
                string connectorString = channelInfoSegments.FirstOrDefault(s => s.StartsWith(connectorStringId, StringComparison.Ordinal));
                int connectorNumber = int.Parse(
                    connectorString.Substring(connectorStringId.Length),
                    NumberStyles.Integer,
                    NumberFormatInfo.InvariantInfo);
                // Ports are 8 bits wide
                int portNumber = channelNumber / 8;
                // This is the specific bit in port that the channel maps to.
                int portIndex = channelNumber % 8;

                ChannelInfoMap.Add(channelInfoString, new ChannelInfo(connectorNumber, portNumber, channelNumber, portIndex));
                // Statically set port configurations such that the first two ports of each connector are designated Output ports,
                // while the second two ports of each connector are designated Input ports.
                // This could otherwise be exposed as a configuration method for this class but is outside of the scope of the example.
                bool portAlreadyConfigured = PortConfigurations.TryGetValue((connectorNumber, portNumber), out var portConfiguration);
                if (!portAlreadyConfigured)
                {
                    portConfiguration = (portNumber < 2) ? PortConfiguration.Output : PortConfiguration.Input;
                }
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
                string portName = $"{portState.Key.ConnectorNumber}_{portState.Key.PortNumber}";
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
            byte state;
            if (!OutputPortStates.TryGetValue(channelInfo, out state))
            {
                // Initialize port state
                state = 0;
            }
            byte newState = UpdateBitInByte(state, value,  channelInfo.PortIndex);

            OutputPortStates[channelInfo] = state;
        }

        /// <summary>
        /// RSeries card read channel data operation.
        /// </summary>
        /// <returns>Port data, where the first dimension represents the connector and the second dimension represents the ports.</returns>
        /// <exception cref="Exception">Thrown when FPGA 'ReadData' fails.</exception>
        public byte[][] ReadPortData()
        {
            var numberOfConnectors = ChannelInfoMap.Values.Select(x => x.ConnectorNumber).Distinct().Count();
            var portsData = new byte[numberOfConnectors][];

            var inputPorts = PortConfigurations
                .Where(kvp => kvp.Value == PortConfiguration.Input)
                .Select(kvp => kvp.Key)
                .ToList();

            for (int i = 0; i < inputPorts.Count; i++)
            {
                var key = PortConfigurations.ElementAt(i).Key;
                int connectorNumber = key.Item2;
                int portNumber = key.Item2;
                string portName = $"{connectorNumber}_{portNumber}";
                _status = RSeries7822RDriverAPI.ReadData(_referenceId, portName, out byte data);
                ValidateStatus($"Error in ReadData method, ErrorCode:{_status}, PortNumber:{portNumber}");
                portsData[connectorNumber][portNumber] = data;
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
    }
}
