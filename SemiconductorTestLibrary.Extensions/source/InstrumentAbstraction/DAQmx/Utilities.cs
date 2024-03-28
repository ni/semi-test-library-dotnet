using System.Collections;
using System.Globalization;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

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
