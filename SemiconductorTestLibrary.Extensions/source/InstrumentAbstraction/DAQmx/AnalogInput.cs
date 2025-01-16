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
        /// Reads multiple samples and returns pin- and site-aware objects of an array of doubles, where each element in the array represents one sample read.
        /// By default, the value of samplesToRead is -1. In this case, all available samples are read when this method is invoked.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin floating-point samples.</returns>
        public static PinSiteData<double[]> ReadAnalogMultiSample(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.VerifyTaskType(DAQmxTaskType.AnalogInput);

                double[][] analogSample = default;
                var availableChannels = taskInfo.Task.Stream.ChannelsToRead.Clone();

                taskInfo.Task.Stream.ChannelsToRead = taskInfo.ChannelList;
                if (taskInfo.Task.AIChannels.HasSingleChannel())
                {
                    var reader = new AnalogSingleChannelReader(taskInfo.Task.Stream);
                    analogSample = new double[][] { reader.ReadMultiSample(samplesToRead) };
                }
                else
                {
                    var reader = new AnalogMultiChannelReader(taskInfo.Task.Stream);
                    analogSample = reader.ReadMultiSample(samplesToRead).ToJaggedArray();
                }
                taskInfo.Task.Stream.ChannelsToRead = (string)availableChannels;

                return analogSample;
            });
        }

        /// <summary>
        /// Reads the samples and returns pin- and site-aware objects of double AnalogWaveform, where each double value in the waveform represents one sample read.
        /// By default, all available samples are returned, unless otherwise specified by passing in value via the samplesToRead argument.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin analog waveform samples.</returns>
        public static PinSiteData<AnalogWaveform<double>> ReadAnalogWaveform(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.VerifyTaskType(DAQmxTaskType.AnalogInput);

                AnalogWaveform<double>[] analogSample = default;
                var availableChannels = taskInfo.Task.Stream.ChannelsToRead.Clone();

                taskInfo.Task.Stream.ChannelsToRead = taskInfo.ChannelList;
                if (taskInfo.Task.AIChannels.HasSingleChannel())
                {
                    var reader = new AnalogSingleChannelReader(taskInfo.Task.Stream);
                    analogSample = new AnalogWaveform<double>[] { reader.ReadWaveform(samplesToRead) };
                }
                else
                {
                    var reader = new AnalogMultiChannelReader(taskInfo.Task.Stream);
                    analogSample = reader.ReadWaveform(samplesToRead);
                }
                taskInfo.Task.Stream.ChannelsToRead = (string)availableChannels;

                return analogSample;
            });
        }
    }
}
