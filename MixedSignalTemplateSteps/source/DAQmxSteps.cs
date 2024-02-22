using System;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.MixedSignalLibrary.Common.ParallelExecution;

namespace NationalInstruments.MixedSignalLibrary
{
    /// <summary>
    /// Defines entry points for DAQmx steps.
    /// </summary>
    public static class DAQmxSteps
    {
        /// <summary>
        /// Create NI-DAQmx tasks that measure voltage on analog input channels.
        ///
        /// This method creates tasks for the analog input voltage task type you specify. It adds and
        /// configures analog input channels to the new tasks to measure voltage. Modify the utility method
        /// <see cref="CreateAIVoltageTask"/> to create other types of channels, or to configure the
        /// channels and the task differently.
        /// </summary>
        /// <param name="tsmContext">The Semiconductor Module context.</param>
        /// <param name="analogInputVoltageTaskType">The task type associated with the analog input voltage tasks in the pin map.</param>
        /// <param name="sampleClockRate">The rate in samples per second at which to read samples.</param>
        /// <param name="minimumVoltage">The expected minimum voltage to read.</param>
        /// <param name="maximumVoltage">The expected maximum voltage to read.</param>
        public static void CreateDAQmxAIVoltageTasks(
            ISemiconductorModuleContext tsmContext,
            string analogInputVoltageTaskType = "Analog Input Voltage",
            double sampleClockRate = 1000.0,
            double minimumVoltage = -10.0,
            double maximumVoltage = 10.0)
        {
            try
            {
                SetupAndCleanup.CreateAndConfigureDAQmxAIVoltageTasks(
                    tsmContext,
                    analogInputVoltageTaskType,
                    sampleClockRate,
                    minimumVoltage: minimumVoltage,
                    maximumVoltage: maximumVoltage,
                    aiTerminalConfiguration: (AITerminalConfiguration)(-1));
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Clear and dispose all NI-DAQmx tasks in the Semiconductor Module context.
        /// </summary>
        /// <param name="tsmContext">The Semiconductor Module context.</param>
        public static void ClearDAQmxTasks(ISemiconductorModuleContext tsmContext)
        {
            try
            {
                InitializeAndClose.ClearAllDAQmxTasks(tsmContext);
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }

        /// <summary>
        /// Acquire waveforms on the NI-DAQmx tasks and channels connected to pins and pin groups in the pin map.
        ///
        /// This method performs a simple example analysis on the measurements and publishes the results.
        /// Modify this method to meet the needs of your own application.
        ///
        /// You do not incur a performance penalty if you pass the same configuration values for
        /// <see cref="samplesPerChannel"/>, <see cref="sampleClockRate"/>, <see cref="minimumVoltage"/> and
        /// <see cref="maximumVoltage"/> that you passed to <see cref="CreateNIDAQmxAIVoltageTasks"/> because
        /// the tasks and channels do not have to be reconfigured before the acquisition.
        /// </summary>
        /// <param name="tsmContext">The Semiconductor Module context.</param>
        /// <param name="pinsAndPinGroups">An array of pins and pin groups connected to NI-DAQmx tasks and channels.</param>
        /// <param name="samplesPerChannel">The number of samples to read on each NI-DAQmx channel.</param>
        public static void AcquireAIVoltageWaveforms(ISemiconductorModuleContext tsmContext, string[] pinsAndPinGroups, int samplesPerChannel = 1000)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var tasksBundle = sessionManager.GetDAQmxTasksBundle(pinsAndPinGroups);
                tasksBundle.DoAndPublishResults(
                    taskInfo =>
                    {
                        var task = taskInfo.Task;
                        task.Start();
                        var reader = new AnalogMultiChannelReader(task.Stream);
                        var waveforms = reader.ReadWaveform(samplesPerChannel);
                        task.Stop();

                        // Compute the minimums and maximums of the acquired waveforms.
                        // Modify these operations to meet the needs of your own application.
                        var minimums = Array.ConvertAll(waveforms, w => w.Samples.Select(s => s.Value).Min());
                        var maximums = Array.ConvertAll(waveforms, w => w.Samples.Select(s => s.Value).Max());
                        return new Tuple<double[], double[]>(minimums, maximums);
                    },
                    publishedDataId1: "Minimum",
                    publishedDataId2: "Maximum");
            }
            catch (Exception e)
            {
                MixedSignalException.Throw(e);
            }
        }
    }
}
