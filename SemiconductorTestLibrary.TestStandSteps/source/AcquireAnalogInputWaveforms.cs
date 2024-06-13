using System;
using System.Linq;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    /// <summary>
    /// Defines entry points for semiconductor common steps.
    /// </summary>
    public static partial class CommonSteps
    {
        /// <summary>
        /// Acquires an analog waveform for each of the specified pins and pin groups. This step performs a simple acquisition of an analog signal,
        /// computes the minimums and maximums of the acquired waveforms, and then publishes the results.
        /// Only pins mapped to an analog input channel of a NI-DAQmx instrument are supported.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups.</param>
        /// <param name="samplesPerChannel">The number of samples to read on each NI-DAQmx analog input channel.</param>
        public static void AcquireAnalogInputWaveforms(ISemiconductorModuleContext tsmContext, string[] pinsOrPinGroups, int samplesPerChannel = 1000)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var daqmx = sessionManager.DAQmx(pinsOrPinGroups);
                daqmx.DoAndPublishResults(
                    taskInfo =>
                    {
                        var task = taskInfo.Task;
                        task.Start();
                        var reader = new AnalogMultiChannelReader(task.Stream);
                        var waveforms = reader.ReadWaveform(samplesPerChannel);
                        task.Stop();

                        // Compute the minimums and maximums of the acquired waveforms.
                        // Modify these operations to meet the needs of your own application.
                        var minimums = Array.ConvertAll(waveforms, w => w.Samples.Select(s => s.Value).Min());
                        var maximums = Array.ConvertAll(waveforms, w => w.Samples.Select(s => s.Value).Max());
                        return new Tuple<double[], double[]>(minimums, maximums);
                    },
                    publishedDataId1: "Minimum",
                    publishedDataId2: "Maximum");
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
