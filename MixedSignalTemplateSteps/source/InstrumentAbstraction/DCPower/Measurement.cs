using System;
using System.Threading.Tasks;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.DataAbstraction;
using NationalInstruments.ModularInstruments.NIDCPower;
using static NationalInstruments.MixedSignalLibrary.Common.ParallelExecution;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for DCPower measurements.
    /// </summary>
    public static class Measurement
    {
        /// <summary>
        /// Measures and returns per-instrument per-channel results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The measurements in per-instrument per-channel format. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<double[][], double[][]> MeasureAndReturnPerInstrumentPerChannelResults(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.Measure());
        }

        /// <summary>
        /// Measures and returns per-site per-pin results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The measurements in per-site per-pin format. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<PinSiteData<double>, PinSiteData<double>> MeasureAndReturnPerSitePerPinResults(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo => sessionInfo.Measure());
        }

        /// <summary>
        /// Does measure on DCPower devices.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <returns>The measurements. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<double[], double[]> Measure(this DCPowerSessionInformation sessionInfo)
        {
            var session = sessionInfo.Session;
            var modelString = sessionInfo.ModelString;
            var lockObject = new object();

            int channelCount = sessionInfo.AssociatedSitePinList.Count;
            var voltageMeasurements = new double[channelCount];
            var currentMeasurements = new double[channelCount];
            Parallel.For(0, channelCount, channelIndex =>
            {
                string channelString = sessionInfo.AssociatedSitePinList[channelIndex].InstrumentChannelString;
                var dcOutput = session.Outputs[channelString];

                switch (dcOutput.Measurement.MeasureWhen)
                {
                    case DCPowerMeasurementWhen.OnDemand:
                        var measureResult = session.Measurement.Measure(channelString);
                        lock (lockObject)
                        {
                            voltageMeasurements[channelIndex] = measureResult.VoltageMeasurements[0];
                            currentMeasurements[channelIndex] = measureResult.CurrentMeasurements[0];
                        }
                        break;

                    case DCPowerMeasurementWhen.OnMeasureTrigger:
                        if (modelString == DCPowerModelStrings.PXI_4110)
                        {
                            break;
                        }
                        // Make sure to clear previous results before fetching again.
                        session.Measurement.Fetch(channelString, new PrecisionTimeSpan(20), dcOutput.Measurement.FetchBacklog);
                        dcOutput.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
                        goto case DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete;

                    case DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete:
                        if (modelString == DCPowerModelStrings.PXI_4110)
                        {
                            break;
                        }
                        var fetchResult = session.Measurement.Fetch(channelString, new PrecisionTimeSpan(20), 1);
                        lock (lockObject)
                        {
                            voltageMeasurements[channelIndex] = fetchResult.VoltageMeasurements[0];
                            currentMeasurements[channelIndex] = fetchResult.CurrentMeasurements[0];
                        }
                        break;

                    default:
                        break;
                }
            });
            return new Tuple<double[], double[]>(voltageMeasurements, currentMeasurements);
        }
    }
}
