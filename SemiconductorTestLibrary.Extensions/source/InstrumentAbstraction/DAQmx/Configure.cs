using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Configures NI-DAQmx tasks properties.
    /// </summary>
    public static class Configure
    {
        /// <summary>
        /// Configures the sample clock for all tasks in the bundle.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="sampleClockRate">Specifies the sample clock rate.</param>
        /// <param name="sampleQuantityMode">Specifies the sample quantity mode.</param>
        /// <param name="samplesPerChannel">Specifies samples per-channel.</param>
        public static void ConfigureSampleClock(this DAQmxTasksBundle tasksBundle, double sampleClockRate = 1000.0, SampleQuantityMode sampleQuantityMode = SampleQuantityMode.FiniteSamples, int samplesPerChannel = 1000)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Verify);
                taskInfo.Task.Timing.ConfigureSampleClock(taskInfo.Task.Timing.SampleClockSource, sampleClockRate, taskInfo.Task.Timing.SampleClockActiveEdge, sampleQuantityMode, samplesPerChannel);
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }

        /// <summary>
        /// Configures the sample clock for the task.
        /// </summary>
        /// <param name="task">The DAQmx <see cref="Task"/> object.</param>
        /// <param name="sampleClockRate">Specifies the sample clock rate.</param>
        /// <param name="sampleQuantityMode">Specifies the sample quantity mode.</param>
        /// <param name="samplesPerChannel">Specifies samples per-channel.</param>
        public static void ConfigureSampleClock(this Task task, double sampleClockRate = 1000.0, SampleQuantityMode sampleQuantityMode = SampleQuantityMode.FiniteSamples, int samplesPerChannel = 1000)
        {
            task.Control(TaskAction.Verify);
            task.Timing.ConfigureSampleClock(task.Timing.SampleClockSource, sampleClockRate, task.Timing.SampleClockActiveEdge, sampleQuantityMode, samplesPerChannel);
            task.Control(TaskAction.Commit);
        }
    }
}
