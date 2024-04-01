using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx
{
    /// <summary>
    /// Defines operations on analog output channels.
    /// </summary>
    public static class AnalogOutput
    {
        /// <summary>
        /// Writes a static DC output state to the pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="staticState">The static state to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteAnalogSingleSample(this DAQmxTasksBundle tasksBundle, double staticState, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var writer = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = Enumerable.Repeat(staticState, taskInfo.AssociatedSitePinList.Count).ToArray();
                writer.WriteSingleSample(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a static DC output state to the pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="siteData">The per-site data to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteAnalogSingleSample(this DAQmxTasksBundle tasksBundle, SiteData<double> siteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var writer = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, siteData, defaultValue: 0);
                writer.WriteSingleSample(autoStart, data);
            });
        }

        /// <summary>
        /// Writes a static DC output state to the pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="pinSiteData">The per-site per-pin data to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteAnalogSingleSample(this DAQmxTasksBundle tasksBundle, PinSiteData<double> pinSiteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var writer = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, pinSiteData, defaultValue: 0);
                writer.WriteSingleSample(autoStart, data);
            });
        }

        /// <summary>
        /// Writes the specified waveform to output on the pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="waveform">The waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteAnalogWaveform(this DAQmxTasksBundle tasksBundle, AnalogWaveform<double> waveform, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var writer = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = Enumerable.Repeat(waveform, taskInfo.AssociatedSitePinList.Count).ToArray();
                writer.WriteWaveform(autoStart, data);
            });
        }

        /// <summary>
        /// Writes the specified waveform to output on the pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="siteData">The per-site waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteAnalogWaveform(this DAQmxTasksBundle tasksBundle, SiteData<AnalogWaveform<double>> siteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var writer = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, siteData, defaultValue: new AnalogWaveform<double>(GetSingleWaveform(siteData).SampleCount));
                writer.WriteWaveform(autoStart, data);
            });
        }

        /// <summary>
        /// Writes the specified waveform to output on the pin(s).
        /// </summary>
        /// <param name="tasksBundle">The <see cref="DAQmxTasksBundle"/> object.</param>
        /// <param name="pinSiteData">The per-site per-pin waveform to write.</param>
        /// <param name="autoStart">Specifies whether to automatically start the tasks.</param>
        public static void WriteAnalogWaveform(this DAQmxTasksBundle tasksBundle, PinSiteData<AnalogWaveform<double>> pinSiteData, bool autoStart = true)
        {
            tasksBundle.Do(taskInfo =>
            {
                var writer = new AnalogMultiChannelWriter(taskInfo.Task.Stream);
                var data = BuildData(taskInfo, pinSiteData, defaultValue: new AnalogWaveform<double>(GetSingleWaveform(pinSiteData).SampleCount));
                writer.WriteWaveform(autoStart, data);
            });
        }
    }
}
