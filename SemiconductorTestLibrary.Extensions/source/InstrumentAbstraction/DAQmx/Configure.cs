using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations for configuring timing and other task properties.
    /// </summary>
    public static class Configure
    {
        /// <summary>
        /// Configures the terminal configuration for the analog output channels within the task,
        /// refer to <see cref="AOTerminalConfiguration"/> for more information.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="terminalConfiguration">Specifies the terminal configuration mode.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void ConfigureTerminalConfiguration(this DAQmxTasksBundle tasksBundle, AOTerminalConfiguration terminalConfiguration)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Verify);
                taskInfo.Task.AOChannels.All.TerminalConfiguration = terminalConfiguration;
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }

        /// <summary>
        /// Configures the terminal configuration for the analog input channels within the task.
        /// refer to <see cref="AITerminalConfiguration"/> for more information.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="terminalConfiguration">Specifies the terminal configuration mode.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void ConfigureTerminalConfiguration(this DAQmxTasksBundle tasksBundle, AITerminalConfiguration terminalConfiguration)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Verify);
                taskInfo.Task.AIChannels.All.TerminalConfiguration = terminalConfiguration;
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }

        /// <summary>
        /// Configures the timing for the task. The capture mode (FiniteSamples/ContinuousSamples), rate of the Sample Clock, and the number of samples to acquire or generate.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="timingSettings">Specifies the timing settings.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void ConfigureTiming(this DAQmxTasksBundle tasksBundle, DAQmxTimingSampleClockSettings timingSettings)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Verify);
                // Setting the following properties are equivalent to invoking taskInfo.Task.Timing.ConfigureSampleClock().
                // This is adventegous as it allows the user to set the properties in a single call,
                // BUT also allows the user to update the properties individually at any part of thier code,
                // without having to worry about previous settings being changed or needing to be reset.
                taskInfo.Task.Timing.SampleTimingType = timingSettings.SampleTimingType;
                if (timingSettings.SampleQuantityMode.HasValue)
                {
                    taskInfo.Task.Timing.SampleQuantityMode = timingSettings.SampleQuantityMode.Value;
                }
                if (timingSettings.SampleClockActiveEdge.HasValue)
                {
                    taskInfo.Task.Timing.SampleClockActiveEdge = timingSettings.SampleClockActiveEdge.Value;
                }
                if (timingSettings.SampleClockRate.HasValue)
                {
                    taskInfo.Task.Timing.SampleClockRate = timingSettings.SampleClockRate.Value;
                }
                if (!string.IsNullOrEmpty(timingSettings.SampleClockSource))
                {
                    taskInfo.Task.Timing.SampleClockSource = timingSettings.SampleClockSource;
                }
                if (timingSettings.SamplesPerChannel.HasValue)
                {
                    taskInfo.Task.Timing.SamplesPerChannel = timingSettings.SamplesPerChannel.Value;
                }
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }

        /// <summary>
        /// Gets the actual sample clock rate (Hz).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <returns>Sample clock rate, one value per underlying instrument session session.</returns>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static double[] GetSampleClockRates(this DAQmxTasksBundle tasksBundle)
        {
            return tasksBundle.DoAndReturnPerInstrumentPerChannelResults(taskInfo =>
            {
                return taskInfo.Task.Timing.SampleClockRate;
            });
        }

        /// <summary>
        /// <inheritdoc cref="Timing.SampleClockRate"/>/>
        /// </summary>
        /// <remarks>
        /// This method is ths same as <see cref="GetSampleClockRates"/>,
        /// except it also checks to confirm if the flag state is the values are the same across all sessions in the bundle.
        /// If the values are indeed the same, it will return the single double value.
        /// Otheriwse, it will throw an exception.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <returns>Sample clock rate.</returns>
        /// <exception cref="NIMixedSignalException">The value for the sample clock rate is not the same for all underlying instrument sessions.</exception>
        public static double GetSampleClockRate(this DAQmxTasksBundle tasksBundle)
        {
            var perInstrumentResults = tasksBundle.DoAndReturnPerInstrumentPerChannelResults(taskInfo =>
            {
                return taskInfo.Task.Timing.SampleClockRate;
            });
            var result = perInstrumentResults.Distinct().ToArray();
            if (result.Length > 1)
            {
                throw new NIMixedSignalException($"The value for the sample clock rate is not the same for all underlying instrument sessions.");
            }
            return result[0];
        }
    }
}
