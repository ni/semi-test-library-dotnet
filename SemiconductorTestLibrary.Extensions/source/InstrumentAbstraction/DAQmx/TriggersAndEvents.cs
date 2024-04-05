using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations for configuring triggers.
    /// </summary>
    public static class TriggersAndEvents
    {
        /// <summary>
        /// Configures the start trigger for the task.
        /// </summary>
        /// <remarks>
        /// Configures the task to acquiring or generating samples based on either the rising or falling edge of the digital signal source.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="source">The name of a terminal where there is a digital signal to use as the source of the trigger.</param>
        /// <param name="edge">The edge of the digital signal to start acquiring or generating samples.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void ConfigureStartTriggerDigitalEdge(this DAQmxTasksBundle tasksBundle, string source, DigitalEdgeStartTriggerEdge edge)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Verify);
                taskInfo.Task.Triggers.StartTrigger.ConfigureDigitalEdgeTrigger(source, edge);
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }

        /// <summary>
        /// Disables the start trigger for the task.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void DisableStartTrigger(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Verify);
                taskInfo.Task.Triggers.StartTrigger.Type = StartTriggerType.None;
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }

        /// <summary>
        /// Routes a control signal to the specified terminal.
        /// </summary>
        /// <remarks>
        /// Because the output terminal can reside on the device that generates the control
        /// signal or on a different device, you can use this method to share clocks and
        /// triggers between multiple devices.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="signal">The trigger, clock, or event to export.</param>
        /// <param name="outputTerminal">
        /// The destination of the exported signal.
        /// You can also specify a comma-delimited list for multiple terminal names.
        /// </param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void ExportSignal(this DAQmxTasksBundle tasksBundle, ExportSignal signal, string outputTerminal)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Verify);
                taskInfo.Task.ExportSignals.ExportHardwareSignal(signal, outputTerminal);
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }

        /// <summary>
        /// Routes a control signal to the specified terminal and gets the fully qualifed string of the exported signal.
        /// </summary>
        /// <remarks>
        /// Because the output terminal can reside on the device that generates the control
        /// signal or on a different device, you can use this method to share clocks and
        /// triggers between multiple devices.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="signal"> The trigger, clock, or event to export.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void ExportSignal(this DAQmxTasksBundle tasksBundle, ExportSignal signal)
        {
            tasksBundle.Do(taskInfo =>
            {
                /// Using the first insturment in the task as the primary.
                var instrumentAlias = taskInfo.Task.Devices[0];
                var chType = taskInfo.GetTaskType().ToDAQmxChannelType();
                var fullyQualifiedOutputTerminal = BuildFullyQualifiedDAQmxOutputTerminal(instrumentAlias, chType, signal);
                taskInfo.Task.Control(TaskAction.Verify);
                taskInfo.Task.ExportSignals.ExportHardwareSignal(signal, fullyQualifiedOutputTerminal);
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }
    }
}
