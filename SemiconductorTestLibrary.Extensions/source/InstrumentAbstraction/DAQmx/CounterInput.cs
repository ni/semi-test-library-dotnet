using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations on counter input channels.
    /// </summary>
    public static class CounterInput
    {
        /// <summary>
        /// Reads one or more samples from a Counter.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin floating-point samples.</returns>
        public static PinSiteData<double[]> ReadCounter(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.CIChannels.VerifyChannelsExist(DAQmxChannelType.CounterInput);
                if (taskInfo.Task.CIChannels.HasSingleChannel())
                {
                    var channel = new CounterSingleChannelReader(taskInfo.Task.Stream);
                    return new double[][] { channel.ReadMultiSampleDouble(samplesToRead) };
                }
                else
                {
                    var channel = new CounterMultiChannelReader(taskInfo.Task.Stream);
                    return channel.ReadMultiSampleDouble(samplesToRead).ToJaggedArray();
                }
            });
        }
    }
}
