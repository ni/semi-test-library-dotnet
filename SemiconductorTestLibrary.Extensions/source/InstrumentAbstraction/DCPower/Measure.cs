using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for DCPower measurements.
    /// </summary>
    public static class Measure
    {
        /// <summary>
        /// Measures and returns per-instrument per-channel results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The measurements in per-instrument per-channel format. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<double[][], double[][]> MeasureAndReturnPerInstrumentPerChannelResults(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerInstrumentPerChannelResults(sessionInfo => sessionInfo.MeasureVoltageAndCurrent());
        }

        /// <summary>
        /// Measures and returns per-site per-pin results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The measurements in per-site per-pin format. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<PinSiteData<double>, PinSiteData<double>> MeasureAndReturnPerSitePerPinResults(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo => sessionInfo.MeasureVoltageAndCurrent());
        }

        /// <summary>
        /// Does measure on DCPower devices.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <returns>The measurements. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<double[], double[]> MeasureVoltageAndCurrent(this DCPowerSessionInformation sessionInfo)
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
                            // The measurement arrays are shared among threads, lock them when updating since arrays are not thread safe.
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

        /// <summary>
        /// Configures and starts a waveform acquisition.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sampleRate">The sample rate.</param>
        /// <param name="bufferLength">The buffer length in seconds.</param>
        /// <returns>The original settings for the channels that do waveform acquisition.</returns>
        public static PinSiteData<DCPowerWaveformAcquisitionSettings> ConfigureAndStartWaveformAcquisition(this DCPowerSessionsBundle sessionsBundle, double sampleRate, double bufferLength)
        {
            var originalSettings = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                return GetOriginalSettings(sessionInfo.Session, sitePinInfo.InstrumentChannelString);
            });
            sessionsBundle.Do(sessionInfo =>
            {
                ConfigureAndInitiate(sessionInfo.Session, sessionInfo.ChannelString, sampleRate, bufferLength);
            });
            return originalSettings;
        }

        /// <summary>
        /// Fetches waveform acquisition results.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="fetchWaveformLength">The waveform length in seconds to fetch.</param>
        /// <param name="originalSettings">The original settings for the channels that do waveform acquisition. This is the return value of the ConfigureAndStartWaveformAcquisition method.</param>
        /// <returns>The per-site per-pin waveform results.</returns>
        public static PinSiteData<DCPowerWaveformResults> FinishWaveformAcquisition(this DCPowerSessionsBundle sessionsBundle, double fetchWaveformLength, PinSiteData<DCPowerWaveformAcquisitionSettings> originalSettings)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var results = Fetch(sessionInfo.Session, sitePinInfo.InstrumentChannelString, fetchWaveformLength);
                ApplyOriginalSettings(sessionInfo.Session, sessionInfo.ChannelString, originalSettings.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName));
                return results;
            });
        }

        /// <summary>
        /// Acquires the synchronized waveforms.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="apertureTimeInSeconds">The measure aperture time in seconds.</param>
        /// <param name="measurementTimeInSeconds">The measurement time in seconds.</param>
        /// <returns>The per-site per-pin waveform results.</returns>
        public static PinSiteData<DCPowerFetchResult> AcquireSynchronizedWaveforms(this DCPowerSessionsBundle sessionsBundle, double apertureTimeInSeconds = 0, double measurementTimeInSeconds = 0)
        {
            var masterChannelOutput = sessionsBundle.GetMasterChannelOutput(TriggerType.MeasureTrigger.ToString(), out string measureTrigger);
            var originalApertureTimes = new Dictionary<string, double>();
            var originalSourceDelays = new Dictionary<string, PrecisionTimeSpan>();
            var originalMeasureWhen = new Dictionary<string, DCPowerMeasurementWhen>();
            var originalMeasureTriggerTypes = new Dictionary<string, DCPowerMeasureTriggerType>();
            var originalMeasureTriggerTerminalNames = new Dictionary<string, DCPowerDigitalEdgeMeasureTriggerInputTerminal>();
            var measureRecordLength = new Dictionary<string, int>();

            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                var perChannelString = sitePinInfo.InstrumentChannelString;
                var channelOutput = sessionInfo.Session.Outputs[perChannelString];
                channelOutput.Control.Abort();

                // Cache original settings to restore after the waveform acquisition.
                originalApertureTimes[perChannelString] = channelOutput.Measurement.ApertureTime;
                originalSourceDelays[perChannelString] = channelOutput.Source.SourceDelay;
                originalMeasureWhen[perChannelString] = channelOutput.Measurement.MeasureWhen;
                originalMeasureTriggerTypes[perChannelString] = channelOutput.Triggers.MeasureTrigger.Type;
                originalMeasureTriggerTerminalNames[perChannelString] = channelOutput.Triggers.MeasureTrigger.DigitalEdge.InputTerminal;

                channelOutput.Measurement.ApertureTime = apertureTimeInSeconds;
                channelOutput.Source.SourceDelay = PrecisionTimeSpan.Zero;
                channelOutput.Measurement.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
                if (sessionIndex == 0 && sitePinInfo.IsFirstChannelOfSession(sessionInfo))
                {
                    // Master channel uses software edge trigger.
                    channelOutput.Triggers.MeasureTrigger.ConfigureSoftwareEdgeTrigger();
                }
                else
                {
                    // Set slave channel measure trigger to be the master channel terminal name.
                    channelOutput.Triggers.MeasureTrigger.DigitalEdge.Configure(measureTrigger, DCPowerTriggerEdge.Rising);
                }

                // Read back actual measure record delta time, configure measure record length and calculate buffer sizes.
                channelOutput.Control.Commit();
                measureRecordLength[perChannelString] = (int)Math.Ceiling(measurementTimeInSeconds / channelOutput.Measurement.RecordDeltaTime);
                channelOutput.Measurement.RecordLength = measureRecordLength[perChannelString];
                if (channelOutput.Measurement.BufferSize < channelOutput.Measurement.RecordLength)
                {
                    channelOutput.Measurement.BufferSize = channelOutput.Measurement.RecordLength;
                }

                channelOutput.Control.Initiate();
            });

            masterChannelOutput.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
            var results = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var perChannelString = sitePinInfo.InstrumentChannelString;
                return sessionInfo.Session.Measurement.Fetch(perChannelString, PrecisionTimeSpan.FromSeconds(measurementTimeInSeconds + 1), measureRecordLength[perChannelString]);
            });

            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                var perChannelString = sitePinInfo.InstrumentChannelString;
                var channelOutput = sessionInfo.Session.Outputs[perChannelString];
                channelOutput.Control.Abort();

                // Restore original settings.
                channelOutput.Measurement.ApertureTime = originalApertureTimes[perChannelString];
                channelOutput.Source.SourceDelay = originalSourceDelays[perChannelString];
                channelOutput.Measurement.MeasureWhen = originalMeasureWhen[perChannelString];
                channelOutput.Triggers.MeasureTrigger.Type = originalMeasureTriggerTypes[perChannelString];
                channelOutput.Triggers.MeasureTrigger.DigitalEdge.InputTerminal = originalMeasureTriggerTerminalNames[perChannelString];
                channelOutput.Control.Initiate();
            });

            return results;
        }

        #region private methods

        private static DCPowerWaveformAcquisitionSettings GetOriginalSettings(NIDCPower session, string channelString)
        {
            var channelOutput = session.Outputs[channelString];
            return new DCPowerWaveformAcquisitionSettings()
            {
                ApertureTime = channelOutput.Measurement.ApertureTime,
                ApertureTimeUnits = channelOutput.Measurement.ApertureTimeUnits,
                MeasureWhen = channelOutput.Measurement.MeasureWhen,
                MeasureTriggerType = channelOutput.Triggers.MeasureTrigger.Type
            };
        }

        private static void ApplyOriginalSettings(NIDCPower session, string channelString, DCPowerWaveformAcquisitionSettings settings)
        {
            var channelOutput = session.Outputs[channelString];
            channelOutput.Control.Abort();
            channelOutput.Measurement.ApertureTime = settings.ApertureTime;
            channelOutput.Measurement.ApertureTimeUnits = settings.ApertureTimeUnits;
            channelOutput.Measurement.IsRecordLengthFinite = true;
            channelOutput.Measurement.MeasureWhen = settings.MeasureWhen;
            channelOutput.Triggers.MeasureTrigger.Type = settings.MeasureTriggerType;
            channelOutput.Control.Initiate();
        }

        private static void ConfigureAndInitiate(NIDCPower session, string channelString, double sampleRate, double bufferLength)
        {
            var channelOutput = session.Outputs[channelString];
            channelOutput.Control.Abort();

            channelOutput.Measurement.ApertureTime = 1 / sampleRate;
            channelOutput.Measurement.ApertureTimeUnits = DCPowerMeasureApertureTimeUnits.Seconds;
            channelOutput.Measurement.IsRecordLengthFinite = false;
            channelOutput.Measurement.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
            channelOutput.Triggers.MeasureTrigger.Type = DCPowerMeasureTriggerType.SoftwareEdge;

            channelOutput.Control.Commit();

            int requiredBufferSize = Convert.ToInt32(Math.Ceiling(bufferLength / channelOutput.Measurement.RecordDeltaTime));
            if (channelOutput.Measurement.BufferSize < requiredBufferSize)
            {
                channelOutput.Measurement.BufferSize = requiredBufferSize;
            }

            channelOutput.Control.Initiate();
            channelOutput.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
        }

        private static DCPowerWaveformResults Fetch(NIDCPower session, string channelString, double fetchWaveformLength)
        {
            var channelOutput = session.Outputs[channelString];
            var deltaTime = channelOutput.Measurement.RecordDeltaTime;
            int pointsToFetch = fetchWaveformLength == 0 ? channelOutput.Measurement.FetchBacklog : Convert.ToInt32(Math.Round(fetchWaveformLength / deltaTime));
            var result = session.Measurement.Fetch(channelString, timeout: PrecisionTimeSpan.FromSeconds(fetchWaveformLength + 1), pointsToFetch);
            return new DCPowerWaveformResults(result, deltaTime);
        }

        #endregion private methods
    }

    /// <summary>
    /// Defines DCPower waveform acquisition settings.
    /// </summary>
    public class DCPowerWaveformAcquisitionSettings
    {
        /// <summary>
        /// The aperture time.
        /// </summary>
        public double ApertureTime { get; set; }

        /// <summary>
        /// The aperture time units.
        /// </summary>
        public DCPowerMeasureApertureTimeUnits ApertureTimeUnits { get; set; }

        /// <summary>
        /// The measure when.
        /// </summary>
        public DCPowerMeasurementWhen MeasureWhen { get; set; }

        /// <summary>
        /// The measure trigger type.
        /// </summary>
        public DCPowerMeasureTriggerType MeasureTriggerType { get; set; }
    }

    /// <summary>
    /// Defines DCPower waveform results.
    /// </summary>
    public class DCPowerWaveformResults
    {
        /// <summary>
        /// The DCPower fetch result.
        /// </summary>
        public DCPowerFetchResult Result { get; }

        /// <summary>
        /// The measurement record delta time.
        /// </summary>
        public double DeltaTime { get; }

        /// <summary>
        /// Constructs a DCPower waveform results object.
        /// </summary>
        /// <param name="result">The DCPower fetch result.</param>
        /// <param name="deltaTime">The measurement record delta time.</param>
        public DCPowerWaveformResults(DCPowerFetchResult result, double deltaTime)
        {
            Result = result;
            DeltaTime = deltaTime;
        }
    }
}
