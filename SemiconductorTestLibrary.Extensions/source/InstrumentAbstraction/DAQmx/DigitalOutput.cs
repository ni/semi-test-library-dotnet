using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations on digital output channels.
    /// </summary>
    public static class DigitalOutput
    {
        /// <summary>
        /// Writes a single Boolean sample to the specified pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="staticState">The static state to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteDigital(this DAQmxTasksBundle tasksBundle, bool staticState, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = Enumerable.Repeat(staticState, taskInfo.AssociatedSitePinList.Count).ToArray();
                channel.WriteSingleSampleSingleLine(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a single Boolean sample to the specified pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="siteData">The per-site data to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteDigital(this DAQmxTasksBundle tasksBundle, SiteData<bool> siteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, siteData, defaultValue: false);
                channel.WriteSingleSampleSingleLine(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a single Boolean sample to the specified pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="pinSiteData">The per-site per-pin data to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteDigital(this DAQmxTasksBundle tasksBundle, PinSiteData<bool> pinSiteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, pinSiteData, defaultValue: false);
                channel.WriteSingleSampleSingleLine(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a DigitalWaveform (multiple Boolean samples over time) to the specified pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="waveform">The per-site per-pin waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteDigitalWaveform(this DAQmxTasksBundle tasksBundle, DigitalWaveform waveform, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var data = Enumerable.Repeat(waveform, taskInfo.AssociatedSitePinList.Count).ToArray();
                channel.WriteWaveform(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a DigitalWaveform (multiple Boolean samples over time) to the specified pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="pinSiteData">The per-site waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteDigitalWaveform(this DAQmxTasksBundle tasksBundle, SiteData<DigitalWaveform> pinSiteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var singleWaveform = GetSingleWaveform(pinSiteData);
                var data = BuildData(taskInfo, pinSiteData, defaultValue: new DigitalWaveform(singleWaveform.Samples.Count, singleWaveform.Signals.Count));
                channel.WriteWaveform(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a DigitalWaveform (multiple Boolean samples over time) to the specified pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="pinSiteData">The per-site per-pin waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteDigitalWaveform(this DAQmxTasksBundle tasksBundle, PinSiteData<DigitalWaveform> pinSiteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var channel = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                var singleWaveform = GetSingleWaveform(pinSiteData);
                var data = BuildData(taskInfo, pinSiteData, defaultValue: new DigitalWaveform(singleWaveform.Samples.Count, singleWaveform.Signals.Count));
                channel.WriteWaveform(autoStart, data);
            });
        }
    }
}
