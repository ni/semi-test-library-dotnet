using System;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.DataAbstraction;
using NationalInstruments.ModularInstruments.NIDCPower;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for waveform acquisition.
    /// </summary>
    public static class WaveformAcquisition
    {
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
                ApplyOriginalSettings(sessionInfo.Session, sessionInfo.ChannelString, originalSettings.Values[sitePinInfo.SiteNumber.Value][sitePinInfo.PinName]);
                return results;
            });
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
