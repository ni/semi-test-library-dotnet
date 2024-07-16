using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes an NI DAQmx Analog Input Task associated with the pin map.
        /// The value of <paramref name="taskType"/> string must match that of the DAQmx task definition within the pin map (default ="AI").
        /// Note that the task will only be configured upon exiting this step, it will not be running.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="taskType">The NI DAQmx task type.</param>
        /// <param name="maxiumValue">The maximum voltage value.</param>
        /// <param name="minimumValue">The minimum voltage value.</param>
        /// <param name="inputTerminalConfiguration">The terminal configuration of the analog input channel.</param>
        public static void SetupNIDAQmxAIVoltageTask(
            ISemiconductorModuleContext tsmContext,
            string taskType = DefaultDAQmxTaskTypeStrings.AnalogInput,
            double maxiumValue = 10,
            double minimumValue = -10,
            DAQmxTerminalConfiguration inputTerminalConfiguration = DAQmxTerminalConfiguration.Default)
        {
            try
            {
                InitializeAndClose.CreateDAQmxAIVoltageTasks(tsmContext, taskType, minimumValue, maxiumValue, inputTerminalConfiguration);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
