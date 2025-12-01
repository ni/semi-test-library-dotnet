using System;

namespace NationalInstruments.Example.CustomInstrument.RSeries7822DriverAPI
{
    /// <summary>
    /// RSeries driver class which provides .NET APIs for RSeries card driver operations.
    /// </summary>
    public class RSeriesDriver
    {
        private ulong ReferenceID { get; }
        private int Status { get; set; }

        /// <summary>
        /// Initializes RSeries card driver session.
        /// </summary>
        /// <param name="resourceName">InstrumentName.</param>
        /// <exception cref="Exception">Thrown when 'OpenFPGA' fails.</exception>
        public RSeriesDriver(string resourceName)
        {
            string bitFilePath = RSeriesCAPI.BitFilePath();
            Status = RSeriesCAPI.OpenFPGA(resourceName, bitFilePath, out ulong fpgaRef);
            ReferenceID = fpgaRef;
            if (Status != 0)
            {
                throw new Exception($"Error in OpenFPGA Ref, ErrorCode:{Status}, ResourceName:{resourceName}, Ref:{ReferenceID}, Bitfile:{bitFilePath}");
            }
        }

        /// <summary>
        /// Close RSeries card driver session.
        /// </summary>
        /// <exception cref="Exception">Thrown when 'CloseFPGA' fails.</exception>
        public void Close()
        {
            Status = RSeriesCAPI.CloseFPGA(ReferenceID);
            if (Status != 0)
            {
                throw new Exception($"Error in CloseFPGA Ref, ErrorCode:{Status}");
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
        /// RSeries card write channel data operation.
        /// </summary>
        /// <param name="channelString">ChannelName.</param>
        /// <param name="pinSiteSpecificData">ChannelData.</param>
        /// <exception cref="Exception">Thrown when FPGA 'WriteData' fails.</exception>
        public void WriteChannelData(string channelString, double pinSiteSpecificData)
        {
            Status = RSeriesCAPI.WriteData(ReferenceID, channelString, (byte)pinSiteSpecificData);
            if (Status != 0)
            {
                throw new Exception($"Error in Write channel data, ErrorCode:{Status}, channel name:{channelString}");
            }
        }

        /// <summary>
        /// RSeries card read channel data operation.
        /// </summary>
        /// <param name="channelString">Channel name.</param>
        /// <returns>Channel data.</returns>
        /// <exception cref="Exception">Thrown when FPGA 'ReadData' fails.</exception>
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
        /// RSeries card driver method for configure.
        /// </summary>
        /// <param name="operationMode">Operation mode.</param>
        /// <exception cref="Exception">Thrown when 'EnableLoopBack' fails. </exception>
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
