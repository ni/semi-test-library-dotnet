using NationalInstruments.DAQmx;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations for performing low-level task control.
    /// </summary>
    public static class TaskControl
    {
        /// <summary>
        /// Aborts execution of the task, immediately terminating any currently active operation, such as a read or a write.
        /// </summary>
        /// <remarks>
        /// This is a low-level driver control method that is not recommended for general use.
        /// Aborting a task puts the task into an unstable but recoverable state.
        /// To recover the task, use <see cref="Start"/>,
        /// to restart the task or use <see cref="Stop"/> to reset the task
        /// without starting it.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void Abort(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Abort);
            });
        }

        /// <summary>
        /// Programs the hardware with all parameters from the task properties previously configured.
        /// </summary>
        /// <remarks>
        /// This is a low-level driver control method that is not recommended for general use.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void Commit(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Commit);
            });
        }

        /// <summary>
        /// Reserves the hardware resources that are needed for the task.
        /// </summary>
        /// <remarks>
        /// This is a low-level driver control method that is not recommended for general use.
        /// It marks the hardware resources that are needed for the task as in use.
        /// No other tasks can reserve these same resources.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void Reserve(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Reserve);
            });
        }

        /// <summary>
        /// Starts the task.
        /// </summary>
        /// <remarks>
        /// This is a low-level driver control method that is not recommended for general use.
        /// It transitions the task to the running state, which begins device input or output.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void Start(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Start);
            });
        }

        /// <summary>
        /// Stops the task.
        /// </summary>
        /// <remarks>
        /// This is a low-level driver control method that is not recommended for general use.
        /// It transitions the task from the running state to the committed state,
        /// which ends device input or output.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void Stop(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Stop);
            });
        }

        /// <summary>
        /// Releases all previously reserved resources.
        /// </summary>
        /// <remarks>
        /// This is a low-level driver control method that is not recommended for general use.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void Unreserve(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Unreserve);
            });
        }

        /// <summary>
        /// Verifies that all task parameters are valid for the hardware.
        /// </summary>
        /// <remarks>
        /// This is a low-level driver control method that is not recommended for general use.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void Verify(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.Control(TaskAction.Verify);
            });
        }

        /// <summary>
        /// Waits for the measurement or generation to complete, regardless of the amount of time needed,
        /// and returns whether the execution is complete.
        /// </summary>
        /// <remarks>
        /// NationalInstruments.DAQmx.Task.WaitUntilDone waits for the task to finish acquiring
        /// or generating the number of samples per channel specified on the NationalInstruments.DAQmx.Timing
        /// class. NationalInstruments.DAQmx.Task.WaitUntilDone does not wait for pending
        /// asynchronous operations to complete. Use the methods and properties on System.IAsyncResult
        /// to verify that asynchronous reads and writes are complete.
        /// </remarks>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <exception cref="DaqException">The underling driver session returned an error.</exception>
        public static void WaitUntilDone(this DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.Do(taskInfo =>
            {
                taskInfo.Task.WaitUntilDone();
            });
        }
    }
}
