using System;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes an NI DAQmx Digital Output Task associated with the pin map.
        /// The value of <paramref name="taskType"/> string must match that of the DAQmx task definition within the pin map (default ="DO").
        /// Note that the task will only be configured upon exiting this step, it will not be running.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="taskType">The NI DAQmx task type.</param>
        /// <param name="channelLineGrouping">How digital lines are grouped.</param>
        public static void SetupNIDAQmxDOTask(
            ISemiconductorModuleContext tsmContext,
            string taskType = DefaultDAQmxTaskTypeStrings.DigitalOutput,
            ChannelLineGrouping channelLineGrouping = ChannelLineGrouping.OneChannelForEachLine)
        {
            try
            {
                InitializeAndClose.CreateDAQmxDOTasks(tsmContext, taskType, channelLineGrouping);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
