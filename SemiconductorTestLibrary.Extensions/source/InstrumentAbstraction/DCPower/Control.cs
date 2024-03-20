using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for get set DCPower configurations.
    /// </summary>
    public static class Control
    {
        #region methods on DCPowerSessionsBundle

        /// <summary>
        /// Configures <see cref="DCPowerSettings"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The specific settings to configure.</param>
        public static void ConfigureDCPowerSettings(this DCPowerSessionsBundle sessionsBundle, DCPowerSettings settings)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.ConfigureDCPowerSettings(settings);
            });
        }

        /// <summary>
        /// Configures <see cref="DCPowerSettings"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-pin settings to configure.</param>
        public static void ConfigureDCPowerSettings(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, DCPowerSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString].Control.Abort();
                sessionInfo.ConfigureDCPowerSettings(settings[sitePinInfo.PinName], sitePinInfo.InstrumentChannelString);
            });
        }

        /// <summary>
        /// Sets DCPower measure when.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="measureWhen">The measure when to set.</param>
        public static void SetMeasureWhen(this DCPowerSessionsBundle sessionsBundle, DCPowerMeasurementWhen measureWhen)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.Session.SetMeasureWhen(sessionInfo.ChannelString, sessionInfo.ModelString, measureWhen);
            });
        }

        /// <summary>
        /// Sets power line frequency
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="frequency">The power line frequency to set.</param>
        public static void SetPowerLineFrequency(this DCPowerSessionsBundle sessionsBundle, double frequency)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                switch (sessionInfo.ModelString)
                {
                    case DCPowerModelStrings.PXI_4110:
                    case DCPowerModelStrings.PXI_4130:
                    case DCPowerModelStrings.PXIe_4154:
                        sessionInfo.PowerLineFrequency = frequency;
                        break;

                    default:
                        sessionInfo.ChannelOutput.Measurement.PowerLineFrequency = frequency;
                        break;
                }
            });
        }

        /// <summary>
        /// Sets measurement sense.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sense">The measurement sense to set.</param>
        public static void SetMeasurementSense(this DCPowerSessionsBundle sessionsBundle, DCPowerMeasurementSense sense)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.Session.SetMeasurementSense(sessionInfo.ChannelString, sessionInfo.ModelString, sense);
            });
        }

        /// <summary>
        /// Sets current limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLimit">The current limit to set.</param>
        /// <param name="currentLimitRange">The current limit range to set. Use the absolute value of current limit to set current limit range when this parameter is not specified.</param>
        public static void SetCurrentLimit(this DCPowerSessionsBundle sessionsBundle, double currentLimit, double? currentLimitRange = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.ChannelOutput.SetCurrentLimit(currentLimit, currentLimitRange);
            });
        }

        /// <summary>
        /// Sets current limits.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLimits">The per-pin current limits to set.</param>
        /// <param name="currentLimitRanges">The current limit ranges to set. Use the absolute value of current limit to set current limit range when this parameter is not specified.</param>
        public static void SetCurrentLimits(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, double> currentLimits, IDictionary<string, double> currentLimitRanges = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString];
                channelOutput.Control.Abort();
                channelOutput.SetCurrentLimit(currentLimits[sitePinInfo.PinName], currentLimitRanges?[sitePinInfo.PinName]);
            });
        }

        /// <summary>
        /// Sets voltage or current sequence.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The voltage or current sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence is run after initiation.</param>
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        public static void SetSequence(this DCPowerSessionsBundle sessionsBundle, double[] sequence, int sequenceLoopCount, double? sequenceStepDeltaTimeInSeconds = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.ChannelOutput.SetSequence(sequence, sequenceLoopCount, sequenceStepDeltaTimeInSeconds);
            });
        }

        /// <summary>
        /// Gets aperture times.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="maximumApertureTime">Returns the maximum aperture time.</param>
        /// <returns>The per-site per-pin aperture times.</returns>
        public static PinSiteData<double> GetApertureTimesInSeconds(this DCPowerSessionsBundle sessionsBundle, out double maximumApertureTime)
        {
            var apertureTimes = sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString];
                switch (sessionInfo.ModelString)
                {
                    case DCPowerModelStrings.PXI_4110:
                    case DCPowerModelStrings.PXI_4130:
                        // The 4110 and 4130 use samples to average and have a fixed sample rate of 3kHz, convert this to an aperture time in seconds.
                        return channelOutput.Measurement.SamplesToAverage / 3000.0;

                    case DCPowerModelStrings.PXIe_4154:
                        // The 4154 uses samples to average and has a fixed sample rate of 300kHz, convert this to an aperture time in seconds.
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
        /// Gets power line frequencies.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The per-site per-pin power line frequencies.</returns>
        public static PinSiteData<double> GetPowerLineFrequencies(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString];
                switch (sessionInfo.ModelString)
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
        /// Gets current limits.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The per-site per-pin current limits.</returns>
        public static PinSiteData<double> GetCurrentLimits(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                return sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString].Source.Voltage.CurrentLimit;
            });
        }

        /// <summary>
        /// Checks voltage mode and levels.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="failedChannels">returns the channels that fail the check.</param>
        /// <param name="expectedVoltages">The expected per-pin voltages.</param>
        /// <returns>Whether all channels pass the check.</returns>
        public static bool CheckVoltageModeAndLevels(this DCPowerSessionsBundle sessionsBundle, out IEnumerable<string> failedChannels, IDictionary<string, double> expectedVoltages = null)
        {
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.InstrumentChannelString];
                bool pass = sessionInfo.Session.Source.Mode == DCPowerSourceMode.SinglePoint
                    && channelOutput.Source.Output.Function == DCPowerSourceOutputFunction.DCVoltage
                    && (expectedVoltages is null || channelOutput.Source.Voltage.VoltageLevel == expectedVoltages[sitePinInfo.PinName]);
                return pass ? string.Empty : sessionInfo.ChannelString;
            });
            var flatternedResults = results.SelectMany(r => r);
            failedChannels = flatternedResults.Where(r => !string.IsNullOrEmpty(r));
            return !failedChannels.Any();
        }

        #endregion methods on DCPowerSessionsBundle

        #region methods on DCPowerOutput

        /// <summary>
        /// Sets aperture time.
        /// </summary>
        /// <param name="output">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="modelString">The DCPower instrument model.</param>
        /// <param name="powerLineFrequency">The power line frequency used to calculate aperture time value from power line cycles to seconds. This is used just for PXI-4110, PXI-4130 and PXIe-4154 models since they don't support power line frequency property.</param>
        /// <param name="apertureTime">The aperture time to set.</param>
        /// <param name="apertureTimeUnits">The aperture time units to set.</param>
        public static void SetApertureTime(this DCPowerOutput output, string modelString, double powerLineFrequency, double apertureTime, DCPowerMeasureApertureTimeUnits apertureTimeUnits)
        {
            switch (modelString)
            {
                case DCPowerModelStrings.PXI_4110:
                case DCPowerModelStrings.PXI_4130:
                case DCPowerModelStrings.PXIe_4154:
                    double apertureTimeInSeconds = apertureTimeUnits == DCPowerMeasureApertureTimeUnits.PowerLineCycles
                            ? apertureTime / powerLineFrequency
                            : apertureTime;
                    // The 4154 has a fixed sample rate of 300kHz, while 4110 and 4130 have a fixed sample rate of 3kHz.
                    double sampleRate = modelString == DCPowerModelStrings.PXIe_4154 ? 300000.0 : 3000.0;
                    // These models use samples to average instead of aperture time.
                    output.Measurement.SamplesToAverage = Convert.ToInt32(sampleRate * apertureTimeInSeconds);
                    break;

                default:
                    output.Measurement.ApertureTime = apertureTime;
                    output.Measurement.ApertureTimeUnits = apertureTimeUnits;
                    break;
            }
        }

        /// <summary>
        /// Sets current limit.
        /// </summary>
        /// <param name="output">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="currentLimit">The current limit to set.</param>
        /// <param name="currentLimitRange">The current limit range to set. Use the absolute value of current limit to set current limit range when this parameter is not specified.</param>
        public static void SetCurrentLimit(this DCPowerOutput output, double currentLimit, double? currentLimitRange = null)
        {
            output.Source.Voltage.CurrentLimit = currentLimit;
            output.Source.Voltage.CurrentLimitRange = currentLimitRange ?? Math.Abs(currentLimit);
        }

        /// <summary>
        /// Sets voltage or current sequence.
        /// </summary>
        /// <param name="output">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="sequence">The voltage or current sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence is run after initiation.</param>
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        public static void SetSequence(this DCPowerOutput output, double[] sequence, int sequenceLoopCount, double? sequenceStepDeltaTimeInSeconds = null)
        {
            output.Source.Mode = DCPowerSourceMode.Sequence;
            output.Source.SequenceLoopCount = sequenceLoopCount;
            output.Source.SetSequence(sequence);
            if (sequenceStepDeltaTimeInSeconds.HasValue)
            {
                output.Source.SequenceStepDeltaTimeEnabled = true;
                output.Source.SequenceStepDeltaTime = PrecisionTimeSpan.FromSeconds(sequenceStepDeltaTimeInSeconds.Value);
            }
        }

        #endregion methods on DCPowerOutput

        #region methods on NIDCPower session

        /// <summary>
        /// Sets measurement when.
        /// </summary>
        /// <param name="session">The <see cref="NIDCPower"/> object.</param>
        /// <param name="channelString">The channel string.</param>
        /// <param name="modelString">The DCPower instrument model.</param>
        /// <param name="measureWhen">The measurement when to set.</param>
        public static void SetMeasureWhen(this NIDCPower session, string channelString, string modelString, DCPowerMeasurementWhen measureWhen)
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
        /// Sets transient response.
        /// </summary>
        /// <param name="session">The <see cref="NIDCPower"/> object.</param>
        /// <param name="channelString">The channel string.</param>
        /// <param name="modelString">The DCPower instrument model <see cref="DCPowerModelStrings"/>.</param>
        /// <param name="transientResponse">The transient response to set.</param>
        public static void SetTransientResponse(this NIDCPower session, string channelString, string modelString, DCPowerSourceTransientResponse transientResponse)
        {
            switch (modelString)
            {
                case DCPowerModelStrings.PXI_4110:
                case DCPowerModelStrings.PXI_4130:
                case DCPowerModelStrings.PXI_4132:
                case DCPowerModelStrings.PXIe_4112:
                case DCPowerModelStrings.PXIe_4113:
                    break;

                case DCPowerModelStrings.PXIe_4154:
                    // channel 1 does not support this property.
                    string updatedChannelString = channelString.ExcludeSpecificChannel("1");
                    if (!string.IsNullOrEmpty(updatedChannelString))
                    {
                        session.Outputs[channelString].Source.TransientResponse = transientResponse;
                    }
                    break;

                default:
                    session.Outputs[channelString].Source.TransientResponse = transientResponse;
                    break;
            }
        }

        /// <summary>
        /// Sets measurement sense.
        /// </summary>
        /// <param name="session">The <see cref="NIDCPower"/> object.</param>
        /// <param name="channelString">The channel string.</param>
        /// <param name="modelString">The DCPower instrument model.</param>
        /// <param name="sense">The measurement sense to set.</param>
        public static void SetMeasurementSense(this NIDCPower session, string channelString, string modelString, DCPowerMeasurementSense sense)
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
        /// Configures <see cref="DCPowerSettings"/>.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="settings">The settings to configure.</param>
        /// <param name="channelString">The channel string. Empty string means all channels in the session.</param>
        public static void ConfigureDCPowerSettings(this DCPowerSessionInformation sessionInfo, DCPowerSettings settings, string channelString = "")
        {
            if (string.IsNullOrEmpty(channelString))
            {
                channelString = sessionInfo.ChannelString;
            }
            if (settings.SourceSettings != null)
            {
                sessionInfo.ConfigureDCPowerSourceSettings(settings.SourceSettings, channelString);
            }
            if (settings.MeasureSettings != null)
            {
                sessionInfo.ConfigureDCPowerMeasureSettings(settings.MeasureSettings, channelString);
            }
        }

        /// <summary>
        /// Configures <see cref="DCPowerSourceSettings"/>.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="settings">The source settings to configure.</param>
        /// <param name="channelString">The channel string. Empty string means all channels in the session.</param>
        public static void ConfigureDCPowerSourceSettings(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, string channelString = "")
        {
            var channelOutput = string.IsNullOrEmpty(channelString) ? sessionInfo.ChannelOutput : sessionInfo.Session.Outputs[channelString];
            channelOutput.Source.Mode = DCPowerSourceMode.SinglePoint;
            channelOutput.Source.ComplianceLimitSymmetry = settings.LimitSymmetry;
            channelOutput.Source.Output.Function = settings.OutputFunction;
            if (settings.SourceDelayInSeconds.HasValue)
            {
                channelOutput.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(settings.SourceDelayInSeconds.Value);
            }
            if (settings.TransientResponse.HasValue)
            {
                sessionInfo.Session.SetTransientResponse(channelString, sessionInfo.ModelString, settings.TransientResponse.Value);
            }
            if (settings.OutputFunction.Equals(DCPowerSourceOutputFunction.DCVoltage))
            {
                ConfigureVoltageSettings(channelOutput, settings);
            }
            else
            {
                ConfigureCurrentSettings(channelOutput, settings);
            }
        }

        /// <summary>
        /// Configures <see cref="DCPowerMeasureSettings"/>.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="settings">The measure settings to configure.</param>
        /// <param name="channelString">The channel string. Empty string means all channels in the session.</param>
        public static void ConfigureDCPowerMeasureSettings(this DCPowerSessionInformation sessionInfo, DCPowerMeasureSettings settings, string channelString = "")
        {
            var channelOutput = string.IsNullOrEmpty(channelString) ? sessionInfo.ChannelOutput : sessionInfo.Session.Outputs[channelString];
            if (settings.ApertureTime.HasValue && settings.ApertureTimeUnits.HasValue)
            {
                channelOutput.SetApertureTime(sessionInfo.ModelString, sessionInfo.PowerLineFrequency, settings.ApertureTime.Value, settings.ApertureTimeUnits.Value);
            }
            if (settings.MeasureWhen.HasValue)
            {
                sessionInfo.Session.SetMeasureWhen(channelString, sessionInfo.ModelString, settings.MeasureWhen.Value);
            }
            if (settings.Sense.HasValue)
            {
                sessionInfo.Session.SetMeasurementSense(channelString, sessionInfo.ModelString, settings.Sense.Value);
            }
            if (settings.RecordLength.HasValue)
            {
                channelOutput.Measurement.RecordLength = settings.RecordLength.Value;
            }
        }

        #endregion methods on DCPowerSessionInformation

        #region private methods

        private static void ConfigureVoltageSettings(DCPowerOutput dcOutput, DCPowerSourceSettings settings)
        {
            dcOutput.Source.Voltage.VoltageLevel = settings.Level;
            if (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric)
            {
                dcOutput.Source.Voltage.CurrentLimit = settings.Limit.Value;
            }
            else
            {
                dcOutput.Source.Voltage.CurrentLimitHigh = settings.LimitHigh.Value;
                dcOutput.Source.Voltage.CurrentLimitLow = settings.LimitLow.Value;
            }
            dcOutput.Source.Voltage.VoltageLevelRange = settings.LevelRange ?? Math.Abs(settings.Level);
            dcOutput.Source.Voltage.CurrentLimitRange = settings.LimitRange ?? CalculateLimitRangeFromLimit(settings);
        }

        private static void ConfigureCurrentSettings(DCPowerOutput dcOutput, DCPowerSourceSettings settings)
        {
            dcOutput.Source.Current.CurrentLevel = settings.Level;
            if (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric)
            {
                dcOutput.Source.Current.VoltageLimit = settings.Limit.Value;
            }
            else
            {
                dcOutput.Source.Current.VoltageLimitHigh = settings.LimitHigh.Value;
                dcOutput.Source.Current.VoltageLimitLow = settings.LimitLow.Value;
            }
            dcOutput.Source.Current.CurrentLevelRange = settings.LevelRange ?? Math.Abs(settings.Level);
            dcOutput.Source.Current.VoltageLimitRange = settings.LimitRange ?? CalculateLimitRangeFromLimit(settings);
        }

        private static double CalculateLimitRangeFromLimit(DCPowerSourceSettings settings)
        {
            return settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric
                ? Math.Abs(settings.Limit.Value)
                : Math.Max(Math.Abs(settings.LimitHigh.Value), Math.Abs(settings.LimitLow.Value));
        }

        private static string ExcludeSpecificChannel(this string channelString, string channelToExclude)
        {
            return string.Join(",", channelString.Split(',').Where(s => !s.Contains($"/{channelToExclude}")));
        }

        #endregion private methods
    }
}
