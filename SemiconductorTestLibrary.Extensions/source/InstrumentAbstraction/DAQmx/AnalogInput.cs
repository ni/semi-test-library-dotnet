using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations on analog input channels.
    /// </summary>
    public static class AnalogInput
    {
        /// <summary>
        /// Reads multiple samples and returns pin and site aware object of an array of doubles, where each element in the array represents one sample read.
        /// By default, the value of samplesToRead is set to -1 and will result in reading in all available samples at the time the method is invoked.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin floating-point samples.</returns>
        public static PinSiteData<double[]> ReadAnalogMultiSample(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.AIChannels.VerifyChannelsExist(DAQmxChannelType.AnalogInput);
                if (taskInfo.Task.AIChannels.HasSingleChannel())
                {
                    var channel = new AnalogSingleChannelReader(taskInfo.Task.Stream);
                    return new double[][] { channel.ReadMultiSample(samplesToRead) };
                }
                else
                {
                    var channel = new AnalogMultiChannelReader(taskInfo.Task.Stream);
                    return channel.ReadMultiSample(samplesToRead).ToJaggedArray();
                }
            });
        }

        /// <summary>
        /// Reads the samples and returns pin and site aware object of double AnalogWaveform, where each double value in the waveform represents one sample read.
        /// By default, all available samples will be returned, unless otherwise specified by passing in value via the samplesToRead argument.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin analog waveform samples.</returns>
        public static PinSiteData<AnalogWaveform<double>> ReadAnalogWaveform(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.AIChannels.VerifyChannelsExist(DAQmxChannelType.AnalogInput);
                if (taskInfo.Task.AIChannels.HasSingleChannel())
                {
                    var channel = new AnalogSingleChannelReader(taskInfo.Task.Stream);
                    return new AnalogWaveform<double>[] { channel.ReadWaveform(samplesToRead) };
                }
                else
                {
                    var channel = new AnalogMultiChannelReader(taskInfo.Task.Stream);
                    return channel.ReadWaveform(samplesToRead);
                }
            });
        }
    }
}
