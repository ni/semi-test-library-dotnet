using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.RSeries.RSeries7822RCustomInstrument
{
    /// <summary>
    /// Concrete implementation of ICustomInstrument Interface.
    /// </summary>
    public class RSeries7822R : ICustomInstrument
    {
        private readonly ulong _referenceId;
        private int _status;

        /// <summary>
        /// Instrument name that matches the one defined in the Pinmap file.
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
        /// Opens FPGA reference of the RIO instrument and stores it in '_referenceId'.
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

            // Open FPGA reference by deploying BitFile on the RIO device of the given Instrument.
            string bitFilePath = RSeries7822RDriverAPI.BitFilePath();
            _status = RSeries7822RDriverAPI.OpenFPGA(ResourceName, bitFilePath, out ulong fpgaRef);
            _referenceId = fpgaRef;
            ValidateStatus($"Error in OpenFPGA method, Error Code:{_status}, Resource Name:{ResourceName}, Ref:{_referenceId}, Bitfile:{bitFilePath}");
        }

        /// <summary>
        /// Closes FPGA reference of the RIO instrument.
        /// </summary>
        public void Close()
        {
            _status = RSeries7822RDriverAPI.CloseFPGA(_referenceId);
            ValidateStatus($"Error in CloseFPGA method, ErrorCode:{_status}");
        }

        /// <summary>
        /// Resets the RIO instrument.
        /// </summary>
        public void Reset()
        {
            // Not supported, NOP.
        }

        /// <summary>
        /// Enables LoopBack mode.
        /// </summary>
        /// <param name="enable">Whether to enable or disable LoopBack mode.</param>
        /// <exception cref="Exception">Thrown when 'EnableLoopBack' fails. </exception>
        public void EnableLoopBack(bool enable)
        {
            ulong value = enable ? 1UL : 0UL;
            _status = RSeries7822RDriverAPI.EnableLoopBack(_referenceId, value);
            ValidateStatus($"Error in EnableLoopBack method, ErrorCode:{_status}");
        }

        /// <summary>
        /// RSeries card write port data operation.
        /// </summary>
        /// <param name="portName">Port name.</param>
        /// <param name="portData">Channel data to write.</param>
        /// <exception cref="Exception">Thrown when FPGA 'WriteData' fails.</exception>
        public void WritePortData(string portName, double portData)
        {
            _status = RSeries7822RDriverAPI.WriteData(_referenceId, portName, (byte)portData);
            ValidateStatus($"Error in WriteData method, ErrorCode:{_status}, Port Name:{portName}, Port Data:{portData}");
        }

        /// <summary>
        /// RSeries card read port data operation.
        /// </summary>
        /// <param name="portName">Port name.</param>
        /// <returns>Port data.</returns>
        /// <exception cref="Exception">Thrown when FPGA 'ReadData' fails.</exception>
        public double MeasurePortData(string portName)
        {
            _status = RSeries7822RDriverAPI.ReadData(_referenceId, portName, out byte portData);
            ValidateStatus($"Error in ReadData method, ErrorCode:{_status}, Port name:{portName}");
            return portData;
        }

        private void ValidateStatus(string exceptionMessage)
        {
            if (_status != 0)
            {
                throw new Exception(exceptionMessage);
            }
        }
    }
}
