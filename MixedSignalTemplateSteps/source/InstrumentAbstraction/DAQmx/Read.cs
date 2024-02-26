using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using NationalInstruments.DAQmx;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.DataAbstraction;
using static NationalInstruments.MixedSignalLibrary.Common.ParallelExecution;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines methods for reading DAQmx samples.
    /// </summary>
    public static class Read
    {
        /// <summary>
        /// Reads analog samples.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin floating-point samples.</returns>
        public static PinSiteData<double[]> ReadAnalogSamples(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.AIChannels.VerifyChannelsExist(channelTypeString: "AI", operationString: "read");
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
        /// Reads analog waveform samples.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin analog waveform samples.</returns>
        public static PinSiteData<AnalogWaveform<double>> ReadAnalogWaveformSamples(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.AIChannels.VerifyChannelsExist(channelTypeString: "AI", operationString: "read");
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

        /// <summary>
        /// Reads uint samples.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin unsigned integer samples.</returns>
        public static PinSiteData<uint[]> ReadU32Samples(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.DIChannels.VerifyChannelsExist(channelTypeString: "DI", operationString: "read");
                if (taskInfo.Task.DIChannels.HasSingleChannel())
                {
                    var channel = new DigitalSingleChannelReader(taskInfo.Task.Stream);
                    return new uint[][] { channel.ReadMultiSamplePortUInt32(samplesToRead) };
                }
                else
                {
                    var channel = new DigitalMultiChannelReader(taskInfo.Task.Stream);
                    return channel.ReadMultiSamplePortUInt32(samplesToRead).ToJaggedArray();
                }
            });
        }

        /// <summary>
        /// Reads digital waveform samples.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin digital waveform samples.</returns>
        public static PinSiteData<DigitalWaveform> ReadDigitalWaveformSamples(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.DIChannels.VerifyChannelsExist(channelTypeString: "DI", operationString: "read");
                if (taskInfo.Task.DIChannels.HasSingleChannel())
                {
                    var channel = new DigitalSingleChannelReader(taskInfo.Task.Stream);
                    return new DigitalWaveform[] { channel.ReadWaveform(samplesToRead) };
                }
                else
                {
                    var channel = new DigitalMultiChannelReader(taskInfo.Task.Stream);
                    return channel.ReadWaveform(samplesToRead);
                }
            });
        }

        /// <summary>
        /// Reads boolean samples.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <returns>Per-site per-pin boolean samples.</returns>
        public static PinSiteData<bool> ReadBooleanSamples(this DAQmxTasksBundle tasksBundle)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.DIChannels.VerifyChannelsExist(channelTypeString: "DI", operationString: "read");
                if (taskInfo.Task.DIChannels.HasSingleChannel())
                {
                    var channel = new DigitalSingleChannelReader(taskInfo.Task.Stream);
                    return new bool[] { channel.ReadSingleSampleSingleLine() };
                }
                else
                {
                    var channel = new DigitalMultiChannelReader(taskInfo.Task.Stream);
                    return channel.ReadSingleSampleSingleLine();
                }
            });
        }

        /// <summary>
        /// Reads counter samples.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="samplesToRead">The number of samples to read.</param>
        /// <returns>Per-site per-pin floating-point samples.</returns>
        public static PinSiteData<double[]> ReadCounterSamplesFromSingleChannel(this DAQmxTasksBundle tasksBundle, int samplesToRead = -1)
        {
            return tasksBundle.DoAndReturnPerSitePerPinResults(taskInfo =>
            {
                taskInfo.Task.CIChannels.VerifyChannelsExist(channelTypeString: "CI", operationString: "read");
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

        private static void VerifyChannelsExist(this ICollection channels, string channelTypeString, string operationString)
        {
            if (channels.Count == 0)
            {
                throw new MixedSignalException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DAQmx_NoChannels, channelTypeString, operationString));
            }
        }

        private static bool HasSingleChannel(this ICollection channels)
        {
            return channels.Count == 1;
        }
    }
}
