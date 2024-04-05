using System.Collections;
using System.Globalization;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

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

        public static T[] BuildData<T>(DAQmxTaskInformation taskInfo, SiteData<T> data, T defaultValue)
        {
            return taskInfo.AssociatedSitePinList.Select(sitePinInfo =>
            {
                if (data.SiteNumbers.Contains(sitePinInfo.SiteNumber))
                {
                    return data.GetValue(sitePinInfo.SiteNumber);
                }
                return defaultValue;
            }).ToArray();
        }

        public static T[] BuildData<T>(DAQmxTaskInformation taskInfo, PinSiteData<T> data, T defaultValue)
        {
            return taskInfo.AssociatedSitePinList.Select(sitePinInfo =>
            {
                if (data.PinNames.Contains(sitePinInfo.PinName))
                {
                    var siteData = data.ExtractPin(sitePinInfo.PinName);
                    if (siteData.SiteNumbers.Contains(sitePinInfo.SiteNumber))
                    {
                        return siteData.GetValue(sitePinInfo.SiteNumber);
                    }
                }
                return defaultValue;
            }).ToArray();
        }

        public static T GetSingleWaveform<T>(SiteData<T> siteData)
        {
            return siteData.GetValue(siteData.SiteNumbers.First());
        }

        public static T GetSingleWaveform<T>(PinSiteData<T> pinSiteData)
        {
            var siteData = pinSiteData.ExtractPin(pinSiteData.PinNames.First());
            return GetSingleWaveform(siteData);
        }

        internal static ChannelType ToDAQmxChannelType(this DAQmxTaskType taskType)
        {
            switch (taskType)
            {
                case DAQmxTaskType.AnalogInput:
                    return ChannelType.AI;
                case DAQmxTaskType.AnalogOutput:
                    return ChannelType.DO;
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
