using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for DCPower measurements.
    /// </summary>
    public static class Measure
    {
        #region methods on DCPowerSessionsBundle

        /// <summary>
        /// Configures one or more measure settings based on the values populated within a <see  cref="DCPowerMeasureSettings"/> object.
        /// Accepts a scalar input of type <see  cref="DCPowerMeasureSettings"/>.
        /// With overrides for <see cref="SiteData{DCPowerMeasureSettings}"/>, and <see cref="PinSiteData{DCPowerMeasureSettings}"/> input.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The measure settings to configure.</param>
        public static void ConfigureMeasureSettings(this DCPowerSessionsBundle sessionsBundle, DCPowerMeasureSettings settings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AbortAndConfigure((channelString, modelString) =>
                {
                    sessionInfo.Session.ConfigureMeasureSettings(channelString, modelString, sessionInfo.PowerLineFrequency, settings);
                });
            });
        }

        /// <inheritdoc cref="ConfigureMeasureSettings(DCPowerSessionsBundle, DCPowerMeasureSettings)"/>
        public static void ConfigureMeasureSettings(this DCPowerSessionsBundle sessionsBundle, SiteData<DCPowerMeasureSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.ConfigureMeasureSettings(sitePinInfo.IndividualChannelString, sitePinInfo.ModelString, sessionInfo.PowerLineFrequency, settings.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <inheritdoc cref="ConfigureMeasureSettings(DCPowerSessionsBundle, DCPowerMeasureSettings)"/>
        public static void ConfigureMeasureSettings(this DCPowerSessionsBundle sessionsBundle, PinSiteData<DCPowerMeasureSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.ConfigureMeasureSettings(sitePinInfo.IndividualChannelString, sitePinInfo.ModelString, sessionInfo.PowerLineFrequency, settings.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName));
            });
        }

        /// <summary>
        /// Configures <see cref="DCPowerMeasureSettings"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The specific settings to configure.</param>
        public static void ConfigureMeasureSettings(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, DCPowerMeasureSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.ConfigureMeasureSettings(sitePinInfo.IndividualChannelString, sitePinInfo.ModelString, sessionInfo.PowerLineFrequency, settings[sitePinInfo.PinName]);
            });
        }

        /// <summary>
        /// Configures the DCPower MeasurementWhen property.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="measureWhen">The MeasurementWhen property to set.</param>
        public static void ConfigureMeasureWhen(this DCPowerSessionsBundle sessionsBundle, DCPowerMeasurementWhen measureWhen)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AbortAndConfigure((channelString, modelString) =>
                {
                    sessionInfo.Session.ConfigureMeasureWhen(channelString, modelString, measureWhen);
                });
            });
        }

        /// <summary>
        /// Configures the power line frequency in Hz (double).
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="frequency">The power line frequency in Hz to set.</param>
        /// <remarks>
        /// This method should only be invoked once when driver sessions are first initialized.
        /// </remarks>
        public static void ConfigurePowerLineFrequency(this DCPowerSessionsBundle sessionsBundle, double frequency)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AbortAndConfigure((channelString, modelString) =>
                {
                    sessionInfo.ConfigurePowerLineFrequency(channelString, modelString, frequency);
                });
            });
        }

        /// <summary>
        /// Configures the measurement sense.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sense">The measurement sense to set.</param>
        public static void ConfigureMeasurementSense(this DCPowerSessionsBundle sessionsBundle, DCPowerMeasurementSense sense)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AbortAndConfigure((channelString, modelString) =>
                {
                    sessionInfo.Session.ConfigureMeasurementSense(channelString, modelString, sense);
                });
            });
        }

        /// <summary>
        /// Gets aperture time in seconds.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="maximumApertureTime">Returns the maximum aperture time.</param>
        /// <returns>The per-site per-pin aperture times.</returns>
        public static PinSiteData<double> GetApertureTimeInSeconds(this DCPowerSessionsBundle sessionsBundle, out double maximumApertureTime)
        {
            var apertureTimes = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                switch (sitePinInfo.ModelString)
                {
                    case DCPowerModelStrings.PXI_4110:
                    case DCPowerModelStrings.PXI_4130:
                        // The 4110 and 4130 use samples to average and have a fixed sample rate of 3 kHz, convert this to an aperture time in seconds.
                        return channelOutput.Measurement.SamplesToAverage / 3000.0;

                    case DCPowerModelStrings.PXIe_4154:
                        // The 4154 uses samples to average and has a fixed sample rate of 300 kHz, convert this to an aperture time in seconds.
                        return channelOutput.Measurement.SamplesToAverage / 300000.0;

                    default:
                        var apertureTime = channelOutput.Measurement.ApertureTime;
                        var apertureTimeUnits = channelOutput.Measurement.ApertureTimeUnits;
                        if (apertureTimeUnits == DCPowerMeasureApertureTimeUnits.PowerLineCycles)
                        {
                            apertureTime /= channelOutput.Measurement.PowerLineFrequency;
                        }
                        return apertureTime;
                }
            });
            maximumApertureTime = apertureTimes.SiteNumbers.Select(siteNumber => apertureTimes.ExtractSite(siteNumber).Values.Max()).Max();
            return apertureTimes;
        }

        /// <summary>
        /// Gets the power line frequency.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The per-site per-pin power line frequencies.</returns>
        public static PinSiteData<double> GetPowerLineFrequency(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                switch (sitePinInfo.ModelString)
                {
                    case DCPowerModelStrings.PXI_4110:
                    case DCPowerModelStrings.PXI_4130:
                    case DCPowerModelStrings.PXIe_4154:
                        return sessionInfo.PowerLineFrequency;

                    default:
                        return channelOutput.Measurement.PowerLineFrequency;
                }
            });
        }

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
        /// Measures the voltage on the target pin(s) and returns a pin- and site-aware data object.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The per-pin per-site voltage measurements.</returns>
        public static PinSiteData<double> MeasureVoltage(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo => sessionInfo.MeasureVoltageAndCurrent().Item1);
        }

        /// <summary>
        /// Measures the current on the target pin(s) and returns a pin- and site-aware data object.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The per-pin per-site voltage measurements.</returns>
        public static PinSiteData<double> MeasureCurrent(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults(sessionInfo => sessionInfo.MeasureVoltageAndCurrent().Item2);
        }

        /// <summary>
        /// Measures the voltage on the target pin(s) and immediately publishes the results using the <paramref name="publishedDataId"/> passed in.
        /// </summary>
        /// <remarks>
        /// Use this method for the fastest test time if the measurement results are not needed for any other operations.
        /// Otherwise, use the override for this method that returns PinSiteData.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="publishedDataId">The unique data id to use when publishing.</param>
        /// <param name="voltageMeasurements">The returned voltage measurements.</param>
        public static void MeasureAndPublishVoltage(this DCPowerSessionsBundle sessionsBundle, string publishedDataId, out double[][] voltageMeasurements)
        {
            voltageMeasurements = sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.MeasureVoltageAndCurrent().Item1, publishedDataId);
        }

        /// <summary>
        /// Measures the voltage on the target pin(s) and immediately publishes the results using the <paramref name="publishedDataId"/> passed in.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="publishedDataId">The unique data id to use when publishing.</param>
        /// <returns>The pin-site aware voltage measurements.</returns>
        public static PinSiteData<double> MeasureAndPublishVoltage(this DCPowerSessionsBundle sessionsBundle, string publishedDataId)
        {
            MeasureAndPublishVoltage(sessionsBundle, publishedDataId, out var voltageMeasurements);
            return sessionsBundle.InstrumentSessions.PerInstrumentPerChannelResultsToPinSiteData(voltageMeasurements);
        }

        /// <summary>
        /// Measures the current on the target pin(s) and immediately publishes the results using the <paramref name="publishedDataId"/> passed in.
        /// </summary>
        /// <remarks>
        /// Use this method for the fastest test time if the measurement results do not needed for any other operations.
        /// Otherwise, use the override for this method that returns PinSiteData.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="publishedDataId">The unique data id to use when publishing.</param>
        /// <param name="currentMeasurements">The returned current measurements.</param>
        public static void MeasureAndPublishCurrent(this DCPowerSessionsBundle sessionsBundle, string publishedDataId, out double[][] currentMeasurements)
        {
            currentMeasurements = sessionsBundle.DoAndPublishResults(sessionInfo => sessionInfo.MeasureVoltageAndCurrent().Item2, publishedDataId);
        }

        /// <summary>
        /// Measures the current on the target pin(s) and immediately publishes the results using the <paramref name="publishedDataId"/> passed in.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="publishedDataId">The unique data id to use when publishing.</param>
        /// <returns>The pin-site aware current measurements.</returns>
        public static PinSiteData<double> MeasureAndPublishCurrent(this DCPowerSessionsBundle sessionsBundle, string publishedDataId)
        {
            MeasureAndPublishCurrent(sessionsBundle, publishedDataId, out var currentMeasurements);
            return sessionsBundle.InstrumentSessions.PerInstrumentPerChannelResultsToPinSiteData(currentMeasurements);
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
                return GetOriginalSettings(sessionInfo.Session, sitePinInfo.IndividualChannelString);
            });
            sessionsBundle.Do(sessionInfo =>
            {
                ConfigureAndInitiate(sessionInfo.Session, sessionInfo.AllChannelsString, sampleRate, bufferLength);
            });
            return originalSettings;
        }

        /// <summary>
        /// Fetches waveform acquisition results as a pin- and site-aware data object.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="fetchWaveformLength">The waveform length in seconds to fetch.</param>
        /// <param name="originalSettings">The original settings for the channels that do waveform acquisition. This is the return value of the ConfigureAndStartWaveformAcquisition method.</param>
        /// <returns>The per-site per-pin waveform results.</returns>
        public static PinSiteData<DCPowerWaveformResults> FinishWaveformAcquisition(this DCPowerSessionsBundle sessionsBundle, double fetchWaveformLength, PinSiteData<DCPowerWaveformAcquisitionSettings> originalSettings)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var results = Fetch(sessionInfo.Session, sitePinInfo.IndividualChannelString, fetchWaveformLength);
                ApplyOriginalSettings(sessionInfo.Session, sessionInfo.AllChannelsString, originalSettings.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName));
                return results;
            });
        }

        /// <summary>
        /// Configures and acquires waveforms synchronized across the pins.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="apertureTimeInSeconds">The measurement aperture time in seconds.</param>
        /// <param name="measurementTimeInSeconds">The measurement time in seconds.</param>
        /// <returns>The per-site per-pin waveform results.</returns>
        public static PinSiteData<DCPowerFetchResult> AcquireSynchronizedWaveforms(this DCPowerSessionsBundle sessionsBundle, double apertureTimeInSeconds = 0, double measurementTimeInSeconds = 0)
        {
            var masterChannelOutput = sessionsBundle.GetPrimaryOutput(TriggerType.MeasureTrigger.ToString(), out string measureTrigger);
            var originalApertureTimes = new Dictionary<string, double>();
            var originalSourceDelays = new Dictionary<string, PrecisionTimeSpan>();
            var originalMeasureWhen = new Dictionary<string, DCPowerMeasurementWhen>();
            var originalMeasureTriggerTypes = new Dictionary<string, DCPowerMeasureTriggerType>();
            var originalMeasureTriggerTerminalNames = new Dictionary<string, DCPowerDigitalEdgeMeasureTriggerInputTerminal>();
            var measureRecordLength = new Dictionary<string, int>();

            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                var perChannelString = sitePinInfo.IndividualChannelString;
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

                // Read back actual measure record delta time, configure measure record length and calculate buffer size.
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
                var perChannelString = sitePinInfo.IndividualChannelString;
                return sessionInfo.Session.Measurement.Fetch(perChannelString, PrecisionTimeSpan.FromSeconds(measurementTimeInSeconds + 1), measureRecordLength[perChannelString]);
            });

            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                var perChannelString = sitePinInfo.IndividualChannelString;
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

        /// <summary>
        /// Fetches results from a previous measurement.
        /// </summary>
        /// <remarks>
        /// This method should not be used when the MeasureWhen property is OnDemand.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="pointsToFetch">The number of points to Fetch.</param>
        /// <param name="timeoutInSeconds">The time to wait before the operation is aborted.</param>
        /// <returns>A <see cref="PinSiteData{T}"/> object that contains an array of <see cref="SingleDCPowerFetchResult"/> values,
        /// where each <see cref="SingleDCPowerFetchResult"/> object contains the voltage, current, and inCompliance result for a simple sample/point from the previous measurement.</returns>
        public static PinSiteData<SingleDCPowerFetchResult[]> FetchMeasurement(this DCPowerSessionsBundle sessionsBundle, int pointsToFetch = 1, double timeoutInSeconds = 10)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, pinSiteInfo) =>
            {
                var measureResult = sessionInfo.Session.Measurement.Fetch(pinSiteInfo.IndividualChannelString, PrecisionTimeSpan.FromSeconds(timeoutInSeconds), pointsToFetch);
                var samples = new SingleDCPowerFetchResult[pointsToFetch];
                for (int i = 0; i < pointsToFetch; i++)
                {
                    samples[i] = new SingleDCPowerFetchResult(measureResult.VoltageMeasurements[i], measureResult.CurrentMeasurements[i], measureResult.InCompliance[i]);
                }
                return samples;
            });
        }

        #endregion methods on DCPowerSessionsBundle

        #region methods on NIDCPower session

        /// <summary>
        /// Configures the aperture time.
        /// </summary>
        /// <param name="session">The <see cref="NIDCPower"/> object.</param>
        /// <param name="channelString">The channel string.</param>
        /// <param name="modelString">The DCPower instrument model.</param>
        /// <param name="powerLineFrequency">The power line frequency used to calculate aperture time value from power line cycles to seconds. This is used just for PXI-4110, PXI-4130, and PXIe-4154 models since they don't support power line frequency property.</param>
        /// <param name="apertureTime">The aperture time to set.</param>
        /// <param name="apertureTimeUnits">The aperture time units to set.</param>
        public static void ConfigureApertureTime(this NIDCPower session, string channelString, string modelString, double powerLineFrequency, double apertureTime, DCPowerMeasureApertureTimeUnits? apertureTimeUnits)
        {
            switch (modelString)
            {
                case DCPowerModelStrings.PXI_4110:
                case DCPowerModelStrings.PXI_4130:
                case DCPowerModelStrings.PXIe_4154:
                    // Use seconds as the default aperture time units if it's not specified.
                    var units = apertureTimeUnits ?? DCPowerMeasureApertureTimeUnits.Seconds;
                    double apertureTimeInSeconds = units == DCPowerMeasureApertureTimeUnits.PowerLineCycles
                            ? apertureTime / powerLineFrequency
                            : apertureTime;
                    // The 4154 has a fixed sample rate of 300kHz, while 4110 and 4130 have a fixed sample rate of 3kHz.
                    double sampleRate = modelString == DCPowerModelStrings.PXIe_4154 ? 300000.0 : 3000.0;
                    // These models use samples to average instead of aperture time.
                    session.Outputs[channelString].Measurement.SamplesToAverage = Convert.ToInt32(sampleRate * apertureTimeInSeconds);
                    break;

                default:
                    session.Outputs[channelString].Measurement.ApertureTime = apertureTime;
                    break;
            }
        }

        /// <summary>
        /// Configures the aperture time units.
        /// </summary>
        /// <param name="session">The <see cref="NIDCPower"/> object.</param>
        /// <param name="channelString">The channel string.</param>
        /// <param name="modelString">The DCPower instrument model.</param>
        /// <param name="apertureTimeUnits">The aperture time units to set.</param>
        public static void ConfigureApertureTimeUnits(this NIDCPower session, string channelString, string modelString, DCPowerMeasureApertureTimeUnits apertureTimeUnits)
        {
            if (modelString == DCPowerModelStrings.PXI_4110
                || modelString == DCPowerModelStrings.PXI_4130
                || modelString == DCPowerModelStrings.PXIe_4154)
            {
                return;
            }

            session.Outputs[channelString].Measurement.ApertureTimeUnits = apertureTimeUnits;
        }

        /// <summary>
        /// Configures the MeasurementWhen property.
        /// </summary>
        /// <param name="session">The <see cref="NIDCPower"/> object.</param>
        /// <param name="channelString">The channel string.</param>
        /// <param name="modelString">The DCPower instrument model.</param>
        /// <param name="measureWhen">The measurement when to set.</param>
        public static void ConfigureMeasureWhen(this NIDCPower session, string channelString, string modelString, DCPowerMeasurementWhen measureWhen)
        {
            if (modelString == DCPowerModelStrings.PXI_4110
                || modelString == DCPowerModelStrings.PXI_4130
                || session.Outputs[channelString].Measurement.MeasureWhen == measureWhen)
            {
                // The 4110 and 4130 support OnDemand only.
                return;
            }
            session.Outputs[channelString].Measurement.MeasureWhen = measureWhen;
        }

        /// <summary>
        /// Configures the measurement sense.
        /// </summary>
        /// <param name="session">The <see cref="NIDCPower"/> object.</param>
        /// <param name="channelString">The channel string.</param>
        /// <param name="modelString">The DCPower instrument model.</param>
        /// <param name="sense">The measurement sense to set.</param>
        public static void ConfigureMeasurementSense(this NIDCPower session, string channelString, string modelString, DCPowerMeasurementSense sense)
        {
            switch (modelString)
            {
                case DCPowerModelStrings.PXI_4110: // local sense only.
                case DCPowerModelStrings.PXIe_4112: // remote sense only.
                case DCPowerModelStrings.PXIe_4113: // remote sense only.
                    break;

                case DCPowerModelStrings.PXI_4130:
                    // channel 0 is local sense only.
                    string updatedChannelString = channelString.ExcludeSpecificChannel("0");
                    if (!string.IsNullOrEmpty(updatedChannelString))
                    {
                        session.Outputs[updatedChannelString].Measurement.Sense = sense;
                    }
                    break;

                default:
                    session.Outputs[channelString].Measurement.Sense = sense;
                    break;
            }
        }

        #endregion methods on NIDCPower session

        #region methods on DCPowerSessionInformation

        /// <summary>
        /// Measures the voltage and current.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <returns>The measurements. Item1 is voltage measurements, Item2 is current measurements.</returns>
        public static Tuple<double[], double[]> MeasureVoltageAndCurrent(this DCPowerSessionInformation sessionInfo)
        {
            var session = sessionInfo.Session;
            var lockObject = new object();

            int channelCount = sessionInfo.AssociatedSitePinList.Count;
            var voltageMeasurements = new double[channelCount];
            var currentMeasurements = new double[channelCount];

            IList<int> onDemandChannelIndexes = new List<int>();
            IList<string> onDemandChannelStrings = new List<string>();
            for (int i = 0; i < channelCount; i++)
            {
                string individualChannelString = sessionInfo.AssociatedSitePinList[i].IndividualChannelString;
                if (session.Outputs[individualChannelString].Measurement.MeasureWhen == DCPowerMeasurementWhen.OnDemand)
                {
                    onDemandChannelIndexes.Add(i);
                    onDemandChannelStrings.Add(individualChannelString);
                }
            }

            InvokeInParallel(
                () =>
                {
                    if (onDemandChannelIndexes.Any())
                    {
                        // Measure all channels that are configured to measure on demand as a single driver call to optimize test time.
                        var measureResult = session.Measurement.Measure(string.Join(",", onDemandChannelStrings));
                        for (int i = 0; i < onDemandChannelIndexes.Count; i++)
                        {
                            int index = onDemandChannelIndexes[i];
                            lock (lockObject)
                            {
                                voltageMeasurements[index] = measureResult.VoltageMeasurements[i];
                                currentMeasurements[index] = measureResult.CurrentMeasurements[i];
                            }
                        }
                    }
                },
                () =>
                {
                    Parallel.For(0, channelCount, channelIndex =>
                    {
                        var sitePinInfo = sessionInfo.AssociatedSitePinList[channelIndex];
                        var dcOutput = session.Outputs[sitePinInfo.IndividualChannelString];

                        switch (dcOutput.Measurement.MeasureWhen)
                        {
                            case DCPowerMeasurementWhen.OnMeasureTrigger:
                                if (sitePinInfo.ModelString == DCPowerModelStrings.PXI_4110)
                                {
                                    break;
                                }
                                // Make sure to clear previous results before fetching again.
                                session.Measurement.Fetch(sitePinInfo.IndividualChannelString, new PrecisionTimeSpan(20), dcOutput.Measurement.FetchBacklog);
                                dcOutput.Triggers.MeasureTrigger.SendSoftwareEdgeTrigger();
                                goto case DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete;

                            case DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete:
                                if (sitePinInfo.ModelString == DCPowerModelStrings.PXI_4110)
                                {
                                    break;
                                }
                                var fetchResult = session.Measurement.Fetch(sitePinInfo.IndividualChannelString, new PrecisionTimeSpan(20), 1);
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
                });
            return new Tuple<double[], double[]>(voltageMeasurements, currentMeasurements);
        }

        #endregion methods on DCPowerSessionInformation

        #region private methods

        private static void AbortAndConfigure(this DCPowerSessionInformation sessionInfo, Action<string, string> configure)
        {
            if (sessionInfo.AllInstrumentsAreTheSameModel)
            {
                sessionInfo.Session.Control.Abort();
                configure(sessionInfo.AllChannelsString, sessionInfo.ModelString);
            }
            else
            {
                foreach (var sitePinInfo in sessionInfo.AssociatedSitePinList)
                {
                    sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Abort();
                    configure(sitePinInfo.IndividualChannelString, sitePinInfo.ModelString);
                }
            }
        }

        private static void ConfigureMeasureSettings(this NIDCPower session, string channelString, string modelString, double powerLineFrequency, DCPowerMeasureSettings settings)
        {
            if (settings.ApertureTime.HasValue)
            {
                session.ConfigureApertureTime(channelString, modelString, powerLineFrequency, settings.ApertureTime.Value, settings.ApertureTimeUnits);
            }
            if (settings.ApertureTimeUnits.HasValue)
            {
                session.ConfigureApertureTimeUnits(channelString, modelString, settings.ApertureTimeUnits.Value);
            }
            if (settings.MeasureWhen.HasValue)
            {
                session.ConfigureMeasureWhen(channelString, modelString, settings.MeasureWhen.Value);
            }
            if (settings.Sense.HasValue)
            {
                session.ConfigureMeasurementSense(channelString, modelString, settings.Sense.Value);
            }
            if (settings.RecordLength.HasValue)
            {
                session.Outputs[channelString].Measurement.RecordLength = settings.RecordLength.Value;
            }
        }

        private static void ConfigurePowerLineFrequency(this DCPowerSessionInformation sessionInfo, string channelString, string modelString, double frequency)
        {
            switch (modelString)
            {
                case DCPowerModelStrings.PXI_4110:
                case DCPowerModelStrings.PXI_4130:
                case DCPowerModelStrings.PXIe_4154:
                    sessionInfo.PowerLineFrequency = frequency;
                    break;

                default:
                    sessionInfo.Session.Outputs[channelString].Measurement.PowerLineFrequency = frequency;
                    break;
            }
        }

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
}
