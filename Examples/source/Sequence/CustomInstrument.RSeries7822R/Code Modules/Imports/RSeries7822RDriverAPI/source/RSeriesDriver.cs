using System;

namespace NationalInstruments.Example.CustomInstrument.RSeries7822DriverAPI
{
    /// <summary>
    /// RSeries driver class which provides .NET APIs for Rseries card driver operations.
    /// </summary>
    public class RSeriesDriver
    {
        private ulong ReferenceID { get; }
        private int Status { get; set; }

        /// <summary>
        /// Initializes Rseries card driver session.
        /// </summary>
        /// <param name="resourceName">InstrumentName.</param>
        /// <exception cref="Exception"></exception>
        public RSeriesDriver(string resourceName)
        {
            string bitFilePath = RSeriesCAPI.BitFilePath();
            Status = RSeriesCAPI.OpenFPGA(resourceName, bitFilePath, out ulong fpgaRef);
            ReferenceID = fpgaRef;
            if (Status != 0)
            {
                throw new Exception($"Error in Open FPGA Ref, ErrorCode:{Status}, ResourceName:{resourceName}, Ref:{ReferenceID}, Bitfile:{bitFilePath}");
            }
        }

        /// <summary>
        /// Close Rseries card driver session.
        /// </summary>
        /// <exception cref="Exception">Exception message.</exception>
        public void Close()
        {
            Status = RSeriesCAPI.CloseFPGA(ReferenceID);
            if (Status != 0)
            {
                throw new Exception($"Error in Close FPGA Ref, ErrorCode:{Status}");
            }
        }

        /// <summary>
        /// Reset RSeries card.
        /// </summary>
        public void Reset()
        {
            // Driver code.
        }

        /// <summary>
        /// Rseries card write channel data operation.
        /// </summary>
        /// <param name="channelString">ChannelName.</param>
        /// <param name="pinSiteSpecificData">ChannelData.</param>
        /// <exception cref="Exception"></exception>
        public void WriteChannelData(string channelString, double pinSiteSpecificData)
        {
            Status = RSeriesCAPI.WriteData(ReferenceID, channelString, (byte)pinSiteSpecificData);
            if (Status != 0)
            {
                throw new Exception($"Error in Write channel data, ErrorCode:{Status}, channel name:{channelString}");
            }
        }

        /// <summary>
        /// Rseries card read channel data operation.
        /// </summary>
        /// <param name="channelString">Channel name.</param>
        /// <returns>Channel data.</returns>
        /// <exception cref="Exception"></exception>
        public double MeasureChannelData(string channelString)
        {
            Status = RSeriesCAPI.ReadData(ReferenceID, channelString, out byte data);
            if (Status != 0)
            {
                throw new Exception($"Error in read channel data, ErrorCode:{Status}, channel name:{channelString}, channel data:{data}");
            }
            return data;
        }

        /// <summary>
        /// Rseries card driver method for configure.
        /// </summary>
        /// <param name="operationMode">Operation mode.</param>
        /// <exception cref="Exception"></exception>
        public void ConfigureMode(string operationMode)
        {
            if (operationMode == "LoopBack")
            {
                Status = RSeriesCAPI.EnableLoopBack(ReferenceID, 1);
            }
            else
            {
                Status = RSeriesCAPI.EnableLoopBack(ReferenceID, 0);
            }
            if (Status != 0)
            {
                throw new Exception("Error in Disabling Loopback");
            }
        }
    }
}
