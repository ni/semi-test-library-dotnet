using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.ModularInstruments.NIDCPower;
using static NationalInstruments.MixedSignalLibrary.Common.Utilities;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for DCPower voltage/current sourcing.
    /// </summary>
    public static class Source
    {
        #region methods on DCPowerSessionsBundle

        /// <summary>
        /// Forces voltage and specifies symmetric current limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevel">The voltage level to force.</param>
        /// <param name="currentLimit">The current limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        public static void ForceVoltageSymmetricLimit(this DCPowerSessionsBundle sessionsBundle, double voltageLevel, double currentLimit, double? voltageLevelRange = null, double? currentLimitRange = null)
        {
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = voltageLevel,
                Limit = currentLimit,
                LevelRange = voltageLevelRange,
                LimitRange = currentLimitRange
            };
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Force(new DCPowerSettings() { SourceSettings = settings });
            });
        }

        /// <summary>
        /// Forces voltage and specifies symmetric current limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels to force for different pins.</param>
        /// <param name="currentLimit">The current limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        public static void ForceVoltageSymmetricLimit(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, double> voltageLevels, double currentLimit, double? voltageLevelRange = null, double? currentLimitRange = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = voltageLevels[sitePinInfo.PinName],
                    Limit = currentLimit,
                    LevelRange = voltageLevelRange,
                    LimitRange = currentLimitRange
                };
                sessionInfo.Force(new DCPowerSettings() { SourceSettings = settings }, sitePinInfo.InstrumentChannelString);
            });
        }

        /// <summary>
        /// Forces voltage and specifies symmetric current limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels to force for different site-pin pairs.</param>
        /// <param name="currentLimit">The current limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        public static void ForceVoltageSymmetricLimit(this DCPowerSessionsBundle sessionsBundle, IDictionary<int, Dictionary<string, double>> voltageLevels, double currentLimit, double? voltageLevelRange = null, double? currentLimitRange = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = voltageLevels[sitePinInfo.SiteNumber.Value][sitePinInfo.PinName],
                    Limit = currentLimit,
                    LevelRange = voltageLevelRange,
                    LimitRange = currentLimitRange
                };
                sessionInfo.Force(new DCPowerSettings() { SourceSettings = settings }, sitePinInfo.InstrumentChannelString);
            });
        }

        /// <summary>
        /// Forces voltage and specifies symmetric current limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-pin settings to use.</param>
        public static void ForceVoltageSymmetricLimit(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, DCPowerSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                settings[sitePinInfo.PinName].SourceSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                settings[sitePinInfo.PinName].SourceSettings.LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric;
                sessionInfo.Force(settings[sitePinInfo.PinName], sitePinInfo.InstrumentChannelString);
            });
        }

        /// <summary>
        /// Forces voltage and specifies asymmetric current limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevel">The voltage level to force.</param>
        /// <param name="currentLimitHigh">The current high limit to use.</param>
        /// <param name="currentLimitLow">The current low limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        public static void ForceVoltageAsymmetricLimit(this DCPowerSessionsBundle sessionsBundle, double voltageLevel, double currentLimitHigh, double currentLimitLow, double? voltageLevelRange = null, double? currentLimitRange = null)
        {
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Asymmetric,
                Level = voltageLevel,
                LimitHigh = currentLimitHigh,
                LimitLow = currentLimitLow,
                LevelRange = voltageLevelRange,
                LimitRange = currentLimitRange
            };
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Force(new DCPowerSettings() { SourceSettings = settings });
            });
        }

        /// <summary>
        /// Forces current and specifies symmetric voltage limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLevel">The current level to force.</param>
        /// <param name="voltageLimit">The voltage limit to use.</param>
        /// <param name="currentLevelRange">The current level range to use.</param>
        /// <param name="voltageLimitRange">The voltage limit range to use.</param>
        public static void ForceCurrentSymmetricLimit(this DCPowerSessionsBundle sessionsBundle, double currentLevel, double voltageLimit, double? currentLevelRange = null, double? voltageLimitRange = null)
        {
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = currentLevel,
                Limit = voltageLimit,
                LevelRange = currentLevelRange,
                LimitRange = voltageLimitRange
            };
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Force(new DCPowerSettings() { SourceSettings = settings });
            });
        }

        /// <summary>
        /// Forces current and specifies symmetric voltage limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-pin settings to use.</param>
        public static void ForceCurrentSymmetricLimit(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, DCPowerSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                settings[sitePinInfo.PinName].SourceSettings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                settings[sitePinInfo.PinName].SourceSettings.LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric;
                sessionInfo.Force(settings[sitePinInfo.PinName], sitePinInfo.InstrumentChannelString);
            });
        }

        /// <summary>
        /// Forces current and specifies asymmetric voltage limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLevel">The current level to force.</param>
        /// <param name="voltageLimitHigh">The voltage high limit to use.</param>
        /// <param name="voltageLimitLow">The voltage low limit to use.</param>
        /// <param name="currentLevelRange">The current level range to use.</param>
        /// <param name="voltageLimitRange">The voltage limit range to use.</param>
        public static void ForceCurrentAsymmetricLimit(this DCPowerSessionsBundle sessionsBundle, double currentLevel, double voltageLimitHigh, double voltageLimitLow, double? currentLevelRange = null, double? voltageLimitRange = null)
        {
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Asymmetric,
                Level = currentLevel,
                LimitHigh = voltageLimitHigh,
                LimitLow = voltageLimitLow,
                LevelRange = currentLevelRange,
                LimitRange = voltageLimitRange
            };
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Force(new DCPowerSettings() { SourceSettings = settings });
            });
        }

        /// <summary>
        /// Forces voltage sequence synchronized on all instrument channels.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageSequences">The voltage sequence to force for different site-pin pairs.</param>
        /// <param name="currentLimits">The current limits to use for different pins.</param>
        /// <param name="currentLimitRanges">The current limit ranges to use for different pins.</param>
        /// <param name="sequenceLoopCount">The number of times to force the sequence.</param>
        /// <param name="transientResponse">The transient response to use.</param>
        /// <param name="sequenceTimeoutInSeconds">The maximum time used to force the sequence.</param>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            IDictionary<int, Dictionary<string, double[]>> voltageSequences,
            IDictionary<string, double> currentLimits,
            IDictionary<string, double> currentLimitRanges,
            int sequenceLoopCount = 1,
            DCPowerSourceTransientResponse transientResponse = DCPowerSourceTransientResponse.Fast,
            double sequenceTimeoutInSeconds = 5.0)
        {
            var masterChannelOutput = sessionsBundle.GetMasterChannelOutput(out string startTrigger);

            var originalSourceDelays = new Dictionary<string, PrecisionTimeSpan>();
            var originalMeasureWhens = new Dictionary<string, DCPowerMeasurementWhen>();
            var originalStartTriggerTypes = new Dictionary<string, DCPowerStartTriggerType>();
            var originalStartTriggerTerminalNames = new Dictionary<string, DCPowerDigitalEdgeStartTriggerInputTerminal>();

            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                var voltageSequence = voltageSequences[sitePinInfo.SiteNumber.Value][sitePinInfo.PinName];
                var settings = new DCPowerSettings()
                {
                    SourceSettings = new DCPowerSourceSettings()
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                        Level = voltageSequence[0],
                        Limit = currentLimits[sitePinInfo.PinName],
                        LevelRange = voltageSequence.Select(v => Math.Abs(v)).Max(),
                        LimitRange = currentLimitRanges[sitePinInfo.PinName],
                        TransientResponse = transientResponse
                    },
                };
                // Applies limits and ranges.
                var perChannelString = sitePinInfo.InstrumentChannelString;
                sessionInfo.Force(settings, perChannelString);

                var channelOutput = sessionInfo.Session.Outputs[perChannelString];
                channelOutput.Control.Abort();
                originalSourceDelays[perChannelString] = channelOutput.Source.SourceDelay;
                channelOutput.Source.SourceDelay = PrecisionTimeSpan.Zero;
                originalMeasureWhens[perChannelString] = channelOutput.Measurement.MeasureWhen;
                channelOutput.Measurement.MeasureWhen = DCPowerMeasurementWhen.OnMeasureTrigger;
                // Applies voltage sequence.
                channelOutput.SetSequence(voltageSequence, sequenceLoopCount);

                if (sessionIndex == 0 && sitePinInfo.IsFirstChannelOfSession(sessionInfo))
                {
                    // Master channel does not need a start trigger.
                    channelOutput.Triggers.StartTrigger.Disable();
                    channelOutput.Control.Commit();
                }
                else
                {
                    // Set slave channel start trigger to be the master channel terminal name.
                    originalStartTriggerTypes[perChannelString] = channelOutput.Triggers.StartTrigger.Type;
                    channelOutput.Triggers.StartTrigger.Type = DCPowerStartTriggerType.DigitalEdge;
                    originalStartTriggerTerminalNames[perChannelString] = channelOutput.Triggers.StartTrigger.DigitalEdge.InputTerminal;
                    channelOutput.Triggers.StartTrigger.DigitalEdge.Configure(startTrigger, DCPowerTriggerEdge.Rising);
                    channelOutput.Control.Initiate();
                }
            });

            masterChannelOutput.Control.Initiate();

            masterChannelOutput.Events.SequenceEngineDoneEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(sequenceTimeoutInSeconds));

            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                var perChannelString = sitePinInfo.InstrumentChannelString;
                var channelOutput = sessionInfo.Session.Outputs[perChannelString];
                channelOutput.Control.Abort();
                if (sessionIndex > 0 || (sessionIndex == 0 && !sitePinInfo.IsFirstChannelOfSession(sessionInfo)))
                {
                    channelOutput.Triggers.StartTrigger.Type = originalStartTriggerTypes[perChannelString];
                    channelOutput.Triggers.StartTrigger.DigitalEdge.InputTerminal = originalStartTriggerTerminalNames[perChannelString];
                }
                channelOutput.Measurement.MeasureWhen = originalMeasureWhens[perChannelString];
                channelOutput.Source.SourceDelay = originalSourceDelays[perChannelString];
                channelOutput.Source.Mode = DCPowerSourceMode.SinglePoint;
                channelOutput.Control.Initiate();
            });
        }

        /// <summary>
        /// Powers down the channel.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settlingTime">The settling time. Null means no need to wait for the turn off operation to settle.</param>
        public static void PowerDown(this DCPowerSessionsBundle sessionsBundle, double? settlingTime = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.ChannelOutput.Control.Abort();
                var originalSourceMode = sessionInfo.ChannelOutput.Source.Mode;
                sessionInfo.ChannelOutput.Source.Mode = DCPowerSourceMode.SinglePoint;
                sessionInfo.ChannelOutput.Source.Output.Enabled = false;
                sessionInfo.ChannelOutput.Control.Initiate();
                sessionInfo.ChannelOutput.Control.Abort();
                sessionInfo.ChannelOutput.Source.Mode = originalSourceMode;
            });
            PreciseWait(settlingTime);
        }

        #endregion methods on DCPowerSessionsBundle

        #region private methods

        private static void Force(this DCPowerSessionInformation sessionInfo, DCPowerSettings settings, string channelString = "")
        {
            var channelOutput = string.IsNullOrEmpty(channelString) ? sessionInfo.ChannelOutput : sessionInfo.Session.Outputs[channelString];
            channelOutput.Control.Abort();
            sessionInfo.ConfigureDCPowerSettings(settings, channelString);
            channelOutput.Source.Output.Enabled = true;
            channelOutput.Control.Initiate();
        }

        private static DCPowerOutput GetMasterChannelOutput(this DCPowerSessionsBundle sessionsBundle, out string startTrigger)
        {
            var masterChannelSessionInfo = sessionsBundle.InstrumentSessions.First();
            var masterChannelString = masterChannelSessionInfo.AssociatedSitePinList.First().InstrumentChannelString;
            startTrigger = masterChannelSessionInfo.BuildTerminalName(masterChannelString, TriggerType.StartTrigger.ToString());
            return masterChannelSessionInfo.Session.Outputs[masterChannelString];
        }

        private static string BuildTerminalName(this DCPowerSessionInformation sessionInfo, string channelString, string triggerOrEventTypeName)
        {
            string resourceDescriptor = $"/{sessionInfo.Session.DriverOperation.IOResourceDescriptor.Split('/')[0]}";
            string logicalTerminalName = $"/{triggerOrEventTypeName}";
            if (sessionInfo.ModelString == DCPowerModelStrings.PXI_4132)
            {
                return $"{resourceDescriptor}{logicalTerminalName}";
            }
            else
            {
                string channelName = $"/Engine{channelString.Split('/')[1]}";
                return $"{resourceDescriptor}{channelName}{logicalTerminalName}";
            }
        }

        private static bool IsFirstChannelOfSession(this SitePinInfo sitePinInfo, DCPowerSessionInformation sessionInfo)
        {
            return sessionInfo.ChannelString.StartsWith(sitePinInfo.InstrumentChannelString, StringComparison.InvariantCulture);
        }

        #endregion private methods
    }
}
