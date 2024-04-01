using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

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
                var writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                writer.WriteSingleSampleSingleLine(autoStart, SampleValuesCacher<bool>.Instance.GetUpdatedSampleValues(taskInfo, staticState));
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
                var writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                writer.WriteSingleSampleSingleLine(autoStart, SampleValuesCacher<bool>.Instance.GetUpdatedSampleValues(taskInfo, siteData));
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
                var writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                writer.WriteSingleSampleSingleLine(autoStart, SampleValuesCacher<bool>.Instance.GetUpdatedSampleValues(taskInfo, pinSiteData));
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
                var writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                writer.WriteWaveform(autoStart, SampleValuesCacher<DigitalWaveform>.Instance.GetUpdatedSampleValues(taskInfo, waveform));
            });
        }

        /// <summary>
        /// Writes a DigitalWaveform (multiple Boolean samples over time) to the specified pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="siteData">The per-site waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteDigitalWaveform(this DAQmxTasksBundle tasksBundle, SiteData<DigitalWaveform> siteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                writer.WriteWaveform(autoStart, SampleValuesCacher<DigitalWaveform>.Instance.GetUpdatedSampleValues(taskInfo, siteData));
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
                var writer = new DigitalMultiChannelWriter(taskInfo.Task.Stream);
                writer.WriteWaveform(autoStart, SampleValuesCacher<DigitalWaveform>.Instance.GetUpdatedSampleValues(taskInfo, pinSiteData));
            });
        }
    }
}
