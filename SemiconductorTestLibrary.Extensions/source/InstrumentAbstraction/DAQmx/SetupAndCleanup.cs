using NationalInstruments.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.InitializeAndClose;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines NI-DAQmx tasks setup and cleanup APIs.
    /// </summary>
    public static class SetupAndCleanup
    {
        /// <summary>
        /// Creates NI-DAQmx tasks in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <remarks>This method only creates tasks with type defined as "AI", "AO", "DI", and "DO" in pinmap.</remarks>
        public static void CreateDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            CreateDAQmxAIVoltageTasks(tsmContext);
            CreateDAQmxAOVoltageTasks(tsmContext);
            CreateDAQmxDITasks(tsmContext);
            CreateDAQmxDOTasks(tsmContext);
        }

        /// <summary>
        /// Creates and configures NI-DAQmx analog input voltage tasks in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="analogInputVoltageTaskType">Specifies the task type.</param>
        /// <param name="sampleClockRate">Specifies sample clock rate.</param>
        /// <param name="samplesPerChannel">Specifies samples per-channel.</param>
        /// <param name="minimumVoltage">Specifies minimum voltage value.</param>
        /// <param name="maximumVoltage">Specifies maximum voltage value.</param>
        /// <param name="aiTerminalConfiguration">Specifies input terminal configuration.</param>
        public static void CreateAndConfigureDAQmxAIVoltageTasks(
            ISemiconductorModuleContext tsmContext,
            string analogInputVoltageTaskType = "AI",
            double sampleClockRate = 1000.0,
            int samplesPerChannel = 1000,
            double minimumVoltage = -1.0,
            double maximumVoltage = 1.0,
            AITerminalConfiguration aiTerminalConfiguration = AITerminalConfiguration.Differential)
        {
            CreateDAQmxAIVoltageTasks(tsmContext, analogInputVoltageTaskType, minimumVoltage, maximumVoltage, aiTerminalConfiguration);

            foreach (var task in tsmContext.GetAllNIDAQmxTasks(analogInputVoltageTaskType))
            {
                task.Control(TaskAction.Verify);
                task.Timing.ConfigureSampleClock(task.Timing.SampleClockSource, sampleClockRate, task.Timing.SampleClockActiveEdge, SampleQuantityMode.FiniteSamples, samplesPerChannel);
                task.Control(TaskAction.Commit);
            }
        }

        /// <summary>
        /// Creates and configures NI-DAQmx analog output voltage tasks in the test system.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="analogOutputVoltageTaskType">Specifies the task type.</param>
        /// <param name="sampleClockRate">Specifies sample clock rate.</param>
        /// <param name="samplesPerChannel">Specifies samples per-channel.</param>
        /// <param name="minimumVoltage">Specifies minimum voltage value.</param>
        /// <param name="maximumVoltage">Specifies maximum voltage value.</param>
        /// <param name="aoIdleOutputBehavior">Specifies AO idle output behavior.</param>
        public static void CreateAndConfigureDAQmxAOVoltageTasks(
            ISemiconductorModuleContext tsmContext,
            string analogOutputVoltageTaskType = "AO",
            double sampleClockRate = 1000.0,
            int samplesPerChannel = 1000,
            double minimumVoltage = -1.0,
            double maximumVoltage = 1.0,
#pragma warning disable IDE0060 // Need to diagnose why this property cannot be configured
            AOIdleOutputBehavior aoIdleOutputBehavior = AOIdleOutputBehavior.ZeroVolts)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            CreateDAQmxAOVoltageTasks(tsmContext, analogOutputVoltageTaskType, minimumVoltage, maximumVoltage);

            foreach (var task in tsmContext.GetAllNIDAQmxTasks(analogOutputVoltageTaskType))
            {
                task.Control(TaskAction.Verify);
                task.Timing.ConfigureSampleClock(task.Timing.SampleClockSource, sampleClockRate, task.Timing.SampleClockActiveEdge, SampleQuantityMode.ContinuousSamples, samplesPerChannel);
                // Property not supported error is reported when setting IdleOutputBehavior (regardless of the behavior type). DevTools G implementation has the same issue.
                // task.AOChannels[aoChannelLists[i]].IdleOutputBehavior = aoIdleOutputBehavior;
                task.Control(TaskAction.Commit);
            }
        }

        /// <summary>
        /// Clears NI-DAQmx tasks.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <remarks>This method only clears tasks with type "AI", "AO", "DI", and "DO".</remarks>
        public static void ClearDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            ClearDAQmxAIVoltageTasks(tsmContext);
            ClearDAQmxAOVoltageTasks(tsmContext);
            ClearDAQmxDITasks(tsmContext);
            ClearDAQmxDOTasks(tsmContext);
        }
    }
}
