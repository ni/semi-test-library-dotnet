using System;

namespace NationalInstruments.Example.CustomInstrument.RSeries7822DriverAPI
{
    /// <summary>
    /// RSeries driver class which provides .NET APIs for RSeries card driver operations.
    /// </summary>
    public class RSeriesDriver
    {
        private readonly ulong _referenceId;
        private int _status;

        /// <summary>
        /// Initializes RSeries card driver session.
        /// </summary>
        /// <param name="resourceName">InstrumentName.</param>
        /// <exception cref="Exception">Thrown when 'OpenFPGA' fails.</exception>
        public RSeriesDriver(string resourceName)
        {
            string bitFilePath = RSeriesCAPI.BitFilePath();
            _status = RSeriesCAPI.OpenFPGA(resourceName, bitFilePath, out ulong fpgaRef);
            _referenceId = fpgaRef;
            ValidateStatus($"Error in OpenFPGA method, Error Code:{_status}, Resource Name:{resourceName}, Ref:{_referenceId}, Bitfile:{bitFilePath}");
        }

        /// <summary>
        /// Close RSeries card driver session.
        /// </summary>
        /// <exception cref="Exception">Thrown when 'CloseFPGA' fails.</exception>
        public void Close()
        {
            _status = RSeriesCAPI.CloseFPGA(_referenceId);
            ValidateStatus($"Error in CloseFPGA method, ErrorCode:{_status}");
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

        /// <summary>
        /// RSeries card driver method for configure.
        /// </summary>
        /// <param name="operationMode">Operation mode.</param>
        /// <exception cref="Exception">Thrown when 'EnableLoopBack' fails. </exception>
        public void ConfigureMode(string operationMode)
        {
            if (operationMode == "LoopBack")
            {
                _status = RSeriesCAPI.EnableLoopBack(_referenceId, 1);
            }
            else
            {
                _status = RSeriesCAPI.EnableLoopBack(_referenceId, 0);
            }
            ValidateStatus($"Error in EnableLoopBack method, ErrorCode:{_status}");
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
