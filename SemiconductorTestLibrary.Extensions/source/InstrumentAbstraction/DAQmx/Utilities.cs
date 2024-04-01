using System.Collections;
using System.Globalization;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    internal static class Utilities
    {
        public static void VerifyChannelsExist(this ICollection channels, DAQmxChannelType channelType)
        {
            if (channels.Count == 0)
            {
                throw new NIMixedSignalException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DAQmx_NoChannelsToRead, channelType.ToDefaultDAQmxTaskTypeString()));
            }
        }

        public static bool HasSingleChannel(this ICollection channels)
        {
            return channels.Count == 1;
        }

        private static string ToDefaultDAQmxTaskTypeString(this DAQmxChannelType channelType)
        {
            switch (channelType)
            {
                case DAQmxChannelType.AnalogInput:
                    return DefaultDAQmxTaskTypeStrings.AnalogInput;
                case DAQmxChannelType.AnalogOutput:
                    return DefaultDAQmxTaskTypeStrings.AnalogOutput;
                case DAQmxChannelType.DigitalInput:
                    return DefaultDAQmxTaskTypeStrings.DigitalInput;
                case DAQmxChannelType.DigitalOutput:
                    return DefaultDAQmxTaskTypeStrings.DigitalOutput;
                case DAQmxChannelType.CounterInput:
                    return DefaultDAQmxTaskTypeStrings.CounterInput;
                case DAQmxChannelType.CounterOutput:
                    return DefaultDAQmxTaskTypeStrings.CounterOutput;
                case DAQmxChannelType.AnalogOutputFunctionGeneration:
                    return DefaultDAQmxTaskTypeStrings.AnalogOutputFunctionGeneration;
                default:
                    return string.Empty;
            }
        }
    }

    internal enum DAQmxChannelType
    {
        AnalogInput,
        AnalogOutput,
        DigitalInput,
        DigitalOutput,
        CounterInput,
        CounterOutput,
        AnalogOutputFunctionGeneration
    }
}
