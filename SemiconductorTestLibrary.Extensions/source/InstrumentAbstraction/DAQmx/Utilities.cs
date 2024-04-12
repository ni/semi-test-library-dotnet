using System.Collections;
using System.Globalization;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    internal static class Utilities
    {
        public static void VerifyTaskType(this DAQmxTaskInformation taskInformation, DAQmxTaskType expectedTaskType)
        {
            if (!taskInformation.TaskType.Equals(expectedTaskType))
            {
                throw new NIMixedSignalException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DAQmx_NoChannelsToRead, expectedTaskType));
            }
        }

        public static bool HasSingleChannel(this ICollection channels)
        {
            return channels.Count == 1;
        }
    }
}
