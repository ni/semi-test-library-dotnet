using System.Collections;
using System.Globalization;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    internal static class Utilities
    {
        public static void VerifyTaskType(this DAQmxTaskInformation taskInformation, DAQmxTaskType expectedTaskType)
        {
            if (!taskInformation.GetTaskType().Equals(expectedTaskType))
            {
                throw new NIMixedSignalException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DAQmx_NoChannelsToRead, expectedTaskType.ToString()));
            }
        }

        public static bool HasSingleChannel(this ICollection channels)
        {
            return channels.Count == 1;
        }

        internal static ChannelType ToDAQmxChannelType(this DAQmxTaskType taskType)
        {
            switch (taskType)
            {
                case DAQmxTaskType.AnalogInput:
                    return ChannelType.AI;
                case DAQmxTaskType.AnalogOutput:
                    return ChannelType.AO;
                case DAQmxTaskType.DigitalInput:
                    return ChannelType.DI;
                case DAQmxTaskType.DigitalOutput:
                    return ChannelType.DO;
                case DAQmxTaskType.CounterInput:
                    return ChannelType.CI;
                case DAQmxTaskType.CounterOutput:
                    return ChannelType.CO;
                case DAQmxTaskType.AnalogOutputFunctionGeneration:
                    return ChannelType.AO;
                default:
                    throw new NIMixedSignalException($"Cannot Determine ChannelType for the specified task type ({taskType}).");
            }
        }

        internal static string ToDefaultTaskTypeString(this DAQmxTaskType taskType)
        {
            if (taskType.Equals(DAQmxTaskType.AnalogOutputFunctionGeneration))
            {
                return $"AOFGEN";
            }
            return taskType.ToDAQmxChannelType().ToString();
        }

        internal static DAQmxTaskType GetTaskType(this DAQmxTaskInformation taskInfo)
        {
            if (!taskInfo.Task.AIChannels.Count.Equals(0))
            {
                return DAQmxTaskType.AnalogInput;
            }
            if (!taskInfo.Task.AOChannels.Count.Equals(0))
            {
                if (taskInfo.Task.AOChannels.All.OutputType.Equals(AOOutputType.FunctionGeneration))
                {
                    return DAQmxTaskType.AnalogOutputFunctionGeneration;
                }
                return DAQmxTaskType.AnalogOutput;
            }
            if (!taskInfo.Task.DOChannels.Count.Equals(0))
            {
                return DAQmxTaskType.DigitalOutput;
            }
            if (!taskInfo.Task.DIChannels.Count.Equals(0))
            {
                return DAQmxTaskType.DigitalInput;
            }
            if (!taskInfo.Task.CIChannels.Count.Equals(0))
            {
                return DAQmxTaskType.CounterInput;
            }
            if (!taskInfo.Task.COChannels.Count.Equals(0))
            {
                return DAQmxTaskType.CounterOutput;
            }
            throw new NIMixedSignalException("Cannot determine the type task. The task may be improperly initialized.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1308:Normalize strings to uppercase", Justification = "Not applicable, driver expects lowercase")]
        internal static string BuildFullyQualifiedDAQmxOutputTerminal(string deviceAlias, ChannelType channelType, ExportSignal signal)
        {
            string exportedSignalString;
            switch (signal)
            {
                case ExportSignal.ReferenceTrigger:
                case ExportSignal.SampleClock:
                case ExportSignal.StartTrigger:
                    if (signal.Equals(ExportSignal.ReferenceTrigger) && !channelType.Equals(ChannelType.DI))
                    {
                        throw new NIMixedSignalException("ReferenceTrigger is not supported for DigitalInput channels.");
                    }
                    exportedSignalString = $"{channelType.ToString().ToLowerInvariant()}/{signal}";
                    break;
                case ExportSignal.Timebase20MHz:
                    exportedSignalString = $"Timebase20MHz";
                    break;
                case ExportSignal.ReferenceClock10MHz:
                    exportedSignalString = $"ReferenceClock10MHz";
                    break;
                case ExportSignal.WatchdogExpiredEvent:
                    exportedSignalString = $"WatchdogExpiredEvent";
                    break;
                case ExportSignal.ChangeDetectionEvent:
                    exportedSignalString = $"ChangeDetectionEvent";
                    break;
                default:
                    throw new NIMixedSignalException($"ExportSignal not supported: {signal}");
            }
            return $"/{deviceAlias}/{exportedSignalString}";
        }
    }

    internal enum DAQmxTaskType
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
