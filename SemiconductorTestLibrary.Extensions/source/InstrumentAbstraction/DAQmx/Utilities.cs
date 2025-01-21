using System.Collections;
using System.Globalization;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Provides utility methods for NI DAQmx instrument abstraction.
    /// </summary>
    public static class Utilities
    {
        /// <summary>
        /// Verifies whether an NI DAQmx task is of a specific type.
        /// </summary>
        /// <param name="taskInformation">The task to check.</param>
        /// <param name="expectedTaskType">The expected task type.</param>
        /// <exception cref="NISemiconductorTestException">Thrown when the NI DAQmx task is not the specific type.</exception>
        public static void VerifyTaskType(this DAQmxTaskInformation taskInformation, DAQmxTaskType expectedTaskType)
        {
            if (!taskInformation.TaskType.Equals(expectedTaskType))
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DAQmx_NoChannelsToRead, expectedTaskType));
            }
        }

        internal static bool HasSingleChannel(this ICollection channels)
        {
            return channels.Count == 1;
        }
    }
}
