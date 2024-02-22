using System.Collections.Generic;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.MixedSignalLibrary.Common;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines methods for writing DAQmx samples.
    /// </summary>
    public static class Write
    {
        /// <summary>
        /// Writes a double static state to all channels in all tasks.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="staticState">The static state to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteSingleAnalogSample(this DAQmxTasksBundle tasksBundle, double staticState, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = Enumerable.Repeat(staticState, taskInfo.AssociatedSitePinList.Count).ToArray();
                channel.WriteSingleSample(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a single analog sample.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="perSitePerPinData">The per-site per-pin data to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteSingleAnalogSample(this DAQmxTasksBundle tasksBundle, IDictionary<int, Dictionary<string, double>> perSitePerPinData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, perSitePerPinData, defaultValue: 0);
                channel.WriteSingleSample(autoStart, data);
            });
        }

        /// <summary>
        /// Writes analog waveform samples.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="perSitePerPinWaveform">The per-site per-pin waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteAnalogWaveformsSamples(this DAQmxTasksBundle tasksBundle, IDictionary<int, Dictionary<string, AnalogWaveform<double>>> perSitePerPinWaveform, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, perSitePerPinWaveform, defaultValue: new AnalogWaveform<double>(perSitePerPinWaveform.First().Value.First().Value.SampleCount));
                channel.WriteWaveform(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a boolean static state to all channels in all tasks.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="staticState">The static state to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteSingleDigitalSample(this DAQmxTasksBundle tasksBundle, bool staticState, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = Enumerable.Repeat(staticState, taskInfo.AssociatedSitePinList.Count).ToArray();
                channel.WriteSingleSampleSingleLine(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a uint static state to all channels in all tasks.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="staticState">The static state to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteSingleDigitalSample(this DAQmxTasksBundle tasksBundle, uint staticState, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = Enumerable.Repeat(staticState, taskInfo.AssociatedSitePinList.Count).ToArray();
                channel.WriteSingleSamplePort(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a single digital boolean sample.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="perSitePerPinData">The per-site per-pin data to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteSingleDigitalSample(this DAQmxTasksBundle tasksBundle, IDictionary<int, Dictionary<string, bool>> perSitePerPinData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, perSitePerPinData, defaultValue: false);
                channel.WriteSingleSampleSingleLine(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a single digital uint sample.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="perSitePerPinData">The per-site per-pin data to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteSingleDigitalSample(this DAQmxTasksBundle tasksBundle, IDictionary<int, Dictionary<string, uint>> perSitePerPinData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, perSitePerPinData, defaultValue: (uint)0);
                channel.WriteSingleSamplePort(autoStart, data);
            });
        }

        /// <summary>
        /// Writes digital waveform samples.
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="perSitePerPinWaveform">The per-site per-pin waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteDigitalWaveformSamples(this DAQmxTasksBundle tasksBundle, IDictionary<int, Dictionary<string, DigitalWaveform>> perSitePerPinWaveform, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, perSitePerPinWaveform, defaultValue: new DigitalWaveform(perSitePerPinWaveform.First().Value.First().Value.Samples.Count, perSitePerPinWaveform.First().Value.First().Value.Signals.Count));
                channel.WriteWaveform(autoStart, data);
            });
        }

        private static T[] BuildData<T>(DAQmxTaskInformation taskInfo, IDictionary<int, Dictionary<string, T>> data, T defaultValue)
        {
            return taskInfo.AssociatedSitePinList.Select(sitePinInfo =>
            {
                if (sitePinInfo.SiteNumber.HasValue && data.TryGetValue(sitePinInfo.SiteNumber.Value, out var perSiteData))
                {
                    if (perSiteData.TryGetValue(sitePinInfo.PinName, out var perSitePerPinData))
                    {
                        return perSitePerPinData;
                    }
                }
                return defaultValue;
            }).ToArray();
        }
    }
}
