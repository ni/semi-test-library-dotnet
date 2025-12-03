using System;
using NationalInstruments.Example.CustomInstrument.RSeries7822DriverAPI;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.CustomInstrument;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.CustomInstrument.MyCustomInstrument
{
    /// <summary>
    /// Concrete implementation of ICustomInstrument Interface.
    /// </summary>
    /// <remarks>
    /// Users must create separate concrete implementations for different Custom Instrument types.
    /// </remarks>
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
        /// Creates new driver session and stores it in InstrumentDriverSession.
        /// </summary>
        /// <param name="instrumentName">Instrument Name as defined in the Pin Map</param>
        /// <param name="channelGroupId">Channel Group Id as defined in the Pin Map</param>
        /// <param name="channelList">Channel List as defined in the Pin Map</param>
        public RSeries7822R(string instrumentName, string channelGroupId, string channelList)
        {
            // Initialize your driver based on the instrument name and channel data and update 'InstrumentDriverSession'.
            ChannelGroupId = channelGroupId;
            InstrumentName = instrumentName;
            ChannelList = channelList;
            ResourceName = InstrumentName;
            string bitFilePath = RSeriesCAPI.BitFilePath();
            _status = RSeriesCAPI.OpenFPGA(ResourceName, bitFilePath, out ulong fpgaRef);
            _referenceId = fpgaRef;
            ValidateStatus($"Error in OpenFPGA method, Error Code:{_status}, Resource Name:{ResourceName}, Ref:{_referenceId}, Bitfile:{bitFilePath}");
        }

        /// <summary>
        /// Closes the custom instrument session.
        /// </summary>
        public void Close()
        {
            _status = RSeriesCAPI.CloseFPGA(_referenceId);
            ValidateStatus($"Error in CloseFPGA method, ErrorCode:{_status}");
        }

        /// <summary>
        /// Resets the custom instrument.
        /// </summary>
        /// <remarks>
        /// Reset operation is about resetting to a known state, stopping current operation, clear faults and errors, reinitialize properties, etc. Not all instruments have reset API.
        /// </remarks>
        public void Reset()
        {
            // If instrument does not support reset operation, you can make it NOP.
            // NOP
        }

        /// <summary>
        /// RSeries card set LoopBack mode.
        /// </summary>
        /// <param name="mode">Status of LoopBack mode.</param>
        /// <exception cref="Exception">Thrown when 'EnableLoopBack' fails. </exception>
        public void SetLoopBackMode(bool mode)
        {
            int value = mode ? 1 : 0;
            _status = RSeriesCAPI.EnableLoopBack(_referenceId, (ulong)value);
            ValidateStatus($"Error in EnableLoopBack method, ErrorCode:{_status}");
        }

        /// <summary>
        /// RSeries card write channel data operation.
        /// </summary>
        /// <param name="channelString">ChannelName.</param>
        /// <param name="pinSiteSpecificData">ChannelData.</param>
        /// <exception cref="Exception">Thrown when FPGA 'WriteData' fails.</exception>
        public void WriteChannelData(string channelString, double pinSiteSpecificData)
        {
            _status = RSeriesCAPI.WriteData(_referenceId, channelString, (byte)pinSiteSpecificData);
            ValidateStatus($"Error in WriteData method, ErrorCode:{_status}, Channel Name:{channelString}, Channel Data:{pinSiteSpecificData}");
        }


        /// <summary>
        /// RSeries card read channel data operation.
        /// </summary>
        /// <param name="channelString">Channel name.</param>
        /// <returns>Channel data.</returns>
        /// <exception cref="Exception">Thrown when FPGA 'ReadData' fails.</exception>
        public double MeasureChannelData(string channelString)
        {
            _status = RSeriesCAPI.ReadData(_referenceId, channelString, out byte data);
            ValidateStatus($"Error in ReadData method, ErrorCode:{_status}, Channel name:{channelString}");
            return data;
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
