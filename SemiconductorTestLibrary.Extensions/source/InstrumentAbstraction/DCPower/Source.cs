using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for DCPower voltage/current sourcing.
    /// </summary>
    public static class Source
    {
        #region Custom Delegates

        /// <summary>
        /// Provides a sequence of items of type T based on the specified site pin information.
        /// </summary>
        /// <typeparam name="T">The type of items returned in the sequence.</typeparam>
        /// <param name="sitePinInfo">The site pin information used to generate the sequence.</param>
        /// <returns>An array of type T containing the generated sequence based on the provided site pin information.</returns>
        private delegate T[] SequenceProvider<T>(SitePinInfo sitePinInfo);

        /// <summary>
        /// Delegate to retrieve a single double value (limit, range, etc.) for a given site-pin pair.
        /// </summary>
        private delegate double? ValueProvider(SitePinInfo sitePinInfo);

        #endregion

        private const double DefaultTimeout = 5.0;
        private const int AttributeIdNotRecognized = -1074135028;

        private static string BuildSequenceName()
        {
            return $"STL_AdvSeq_{DateTime.UtcNow.Ticks}_{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(0, 8)}";
        }

        #region methods on DCPowerSessionsBundle

        /// <summary>
        /// Configures one or more source settings based on values populated within a <see  cref="DCPowerSourceSettings"/> object.
        /// Accepts a scalar input of type <see  cref="DCPowerSourceSettings"/>.
        /// With overrides for <see cref="SiteData{DCPowerSourceSettings}"/>, and <see cref="PinSiteData{DCPowerSourceSettings}"/> input.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The source settings to configure.</param>
        public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, DCPowerSourceSettings settings)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings);
            });
        }

        /// <inheritdoc cref="ConfigureSourceSettings(DCPowerSessionsBundle, DCPowerSourceSettings)"/>
        public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, SiteData<DCPowerSourceSettings> settings)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings.GetValue(sitePinInfo.SiteNumber), channelOutput, sitePinInfo);
            });
        }

        /// <inheritdoc cref="ConfigureSourceSettings(DCPowerSessionsBundle, DCPowerSourceSettings)"/>
        public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, PinSiteData<DCPowerSourceSettings> settings)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings.GetValue(sitePinInfo, out bool isGroupData), channelOutput, sitePinInfo, isGroupData);
            });
        }

        /// <summary>
        /// Configures <see cref="DCPowerSourceSettings"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The specific settings to configure.</param>
        public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, DCPowerSourceSettings> settings)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings.GetValue(sitePinInfo, out bool isGroupData), channelOutput, sitePinInfo, isGroupData);
            });
        }

        /// <summary>
        /// Forces voltage on the target pins at the specified level. Must at least provide a level value, and the method will assume all other properties that have been previously set. Optionally, can also provide a specific current limit, current limit range, voltage level range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevel">The voltage level to force.</param>
        /// <param name="currentLimit">The current limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, double voltageLevel, double? currentLimit = null, double? voltageLevelRange = null, double? currentLimitRange = null, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Level = voltageLevel,
                Limit = currentLimit,
                LevelRange = voltageLevelRange,
                LimitRange = currentLimitRange
            };
            sessionsBundle.ForceVoltage(settings, waitForSourceCompletion);
        }

        /// <summary>
        /// Forces voltage on the target pins at the specified pin-unique level. Must at least provide a level value, and the method will assume all other properties that have been previously set.  Optionally, can also provide a specific current limit, current limit range, voltage level range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels to force for different pins.</param>
        /// <param name="currentLimit">The current limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, double> voltageLevels, double? currentLimit = null, double? voltageLevelRange = null, double? currentLimitRange = null, bool waitForSourceCompletion = false)
        {
            bool hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, voltageLevels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = voltageLevels.GetValue(sitePinInfo, out _),
                    Limit = currentLimit,
                    LevelRange = voltageLevelRange,
                    LimitRange = currentLimitRange
                };
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces voltage on the target pins at the specified site-unique level. Must at least provide a level value, and the method will assume all other properties that have been previously set. Optionally, can also provide a specific current limit, current limit range, voltage level range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels to force for different sites.</param>
        /// <param name="currentLimit">The current limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, SiteData<double> voltageLevels, double? currentLimit = null, double? voltageLevelRange = null, double? currentLimitRange = null, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = voltageLevels.GetValue(sitePinInfo.SiteNumber),
                    Limit = currentLimit,
                    LevelRange = voltageLevelRange,
                    LimitRange = currentLimitRange
                };
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces voltage on the target pins at the specified pin- and site-unique level. Must at least provide a level value, and the method will assume all other properties that have been previously set. Optionally, can also provide a specific current limit, current limit range, voltage level range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevels">The voltage levels to force for different site-pin pairs.</param>
        /// <param name="currentLimit">The current limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, PinSiteData<double> voltageLevels, double? currentLimit = null, double? voltageLevelRange = null, double? currentLimitRange = null, bool waitForSourceCompletion = false)
        {
            bool hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, voltageLevels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = voltageLevels.GetValue(sitePinInfo),
                    Limit = currentLimit,
                    LevelRange = voltageLevelRange,
                    LimitRange = currentLimitRange
                };
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces voltage using specified source settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The settings to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, DCPowerSourceSettings settings, bool waitForSourceCompletion = false)
        {
            settings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo);
                });
                sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.Force(settings, waitForSourceCompletion);
                });
            }
        }

        /// <summary>
        /// Forces voltage using specified site-unique source settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-site settings to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, SiteData<DCPowerSourceSettings> settings, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSiteSettings = settings.GetValue(sitePinInfo.SiteNumber);
                perSiteSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perSiteSettings, sitePinInfo);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces voltage using specified pin-unique source settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-pin settings to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, DCPowerSourceSettings> settings, bool waitForSourceCompletion = false)
        {
            bool hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, settings);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perPinSettings = settings.GetValue(sitePinInfo, out bool isGroupData);
                perPinSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perPinSettings, sitePinInfo, isGroupData);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces voltage using specified pin- and site-unique source settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-site-pin-pair settings to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, PinSiteData<DCPowerSourceSettings> settings, bool waitForSourceCompletion = false)
        {
            bool hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, settings);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSitePinPairSettings = settings.GetValue(sitePinInfo, out bool isGroupData);
                perSitePinPairSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perSitePinPairSettings, sitePinInfo, isGroupData);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of voltage values on the targeted pin(s).
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureVoltageSequence(DCPowerSessionsBundle, string, double[], int, double?, bool)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageSequence">Sequence of voltage values to force.</param>
        /// <param name="currentLimit">The current limit to use for the sequence.</param>
        /// <param name="voltageLevelRange">The voltage level range to use for the sequence.</param>
        /// <param name="currentLimitRange">The current limit range to use for the sequence.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="waitForSequenceCompletion">True to block until the sequence engine completes (waits on SequenceEngineDone event); false to return immediately.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion when <paramref name="waitForSequenceCompletion"/> is <see langword="true"/>.</param>
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            double[] voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            var advancedSequenceName = BuildSequenceName();
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Limit = currentLimit,
                LevelRange = voltageLevelRange,
                LimitRange = currentLimitRange
            };
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    sessionInfo.ConfigureAllChannelsForSequenceModeAndInitiateGangedFollowerChannels(
                        sitePinInfo,
                        settings,
                        advancedSequenceName,
                        voltageSequence,
                        sequenceLoopCount,
                        setAsActiveSequence: true);
                });
                sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSequenceCompletion, sequenceTimeoutInSeconds);
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.ForceSequenceCore(
                       settings,
                       advancedSequenceName,
                       voltageSequence,
                       sequenceLoopCount,
                       waitForSequenceCompletion,
                       sequenceTimeoutInSeconds,
                       setAsActiveSequence: true);
                });
            }

            sessionsBundle.ReleaseAdvancedSequenceResources(advancedSequenceName);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureVoltageSequence(DCPowerSessionsBundle, string, SiteData{double[]}, int, double?, bool)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceVoltageSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="voltageSequence"/>
        /// <param name="currentLimit"/>
        /// <param name="voltageLevelRange"/>
        /// <param name="currentLimitRange"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            var advancedSequenceName = BuildSequenceName();
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Limit = currentLimit,
                LevelRange = voltageLevelRange,
                LimitRange = currentLimitRange
            };
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var sequence = voltageSequence.GetValue(pinSiteInfo.SiteNumber);
                var channelOutput = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                sessionInfo.ConfigureAllChannelsForSequenceModeAndInitiateGangedFollowerChannels(
                    pinSiteInfo,
                    settings,
                    advancedSequenceName,
                    sequence,
                    sequenceLoopCount,
                    setAsActiveSequence: true);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSequenceCompletion, sequenceTimeoutInSeconds);

            sessionsBundle.ReleaseAdvancedSequenceResources(advancedSequenceName);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureVoltageSequence(DCPowerSessionsBundle, string, PinSiteData{double[]}, int, double?, bool)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceVoltageSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="voltageSequence"/>
        /// <param name="currentLimit"/>
        /// <param name="voltageLevelRange"/>
        /// <param name="currentLimitRange"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            var advancedSequenceName = BuildSequenceName();
            var hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, voltageSequence);
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Limit = currentLimit,
                LevelRange = voltageLevelRange,
                LimitRange = currentLimitRange
            };
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var sequence = voltageSequence.GetValue(pinSiteInfo);
                var channelOutput = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];
                sessionInfo.ConfigureAllChannelsForSequenceModeAndInitiateGangedFollowerChannels(
                    pinSiteInfo,
                    settings,
                    advancedSequenceName,
                    sequence,
                    sequenceLoopCount,
                    setAsActiveSequence: true);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSequenceCompletion, sequenceTimeoutInSeconds);

            sessionsBundle.ReleaseAdvancedSequenceResources(advancedSequenceName);
        }

        /// <summary>
        /// Behaves the same as the ForceVoltage() method, but as two current limit inputs for setting separate high and low current limits.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageLevel">The voltage level to force.</param>
        /// <param name="currentLimitHigh">The current high limit to use.</param>
        /// <param name="currentLimitLow">The current low limit to use.</param>
        /// <param name="voltageLevelRange">The voltage level range to use.</param>
        /// <param name="currentLimitRange">The current limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceVoltageAsymmetricLimit(this DCPowerSessionsBundle sessionsBundle, double voltageLevel, double currentLimitHigh, double currentLimitLow, double? voltageLevelRange = null, double? currentLimitRange = null, bool waitForSourceCompletion = false)
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
            sessionsBundle.ForceVoltage(settings, waitForSourceCompletion);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of voltage outputs, ensuring synchronized output across all specified target pins.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageSequence">Sequence of voltage values to force.</param>
        /// <param name="currentLimit">The current limit to use for the sequence.</param>
        /// <param name="voltageLevelRange">The voltage level range to use for the sequence.</param>
        /// <param name="currentLimitRange">The current limit range to use for the sequence.</param>
        /// <param name="sourceDelayInSeconds">Optional source delay to use uniformly for synchronization.</param>
        /// <param name="transientResponse">Transient response.</param>
        /// <param name="sequenceLoopCount">The number of times to force the sequence.</param>
        /// <param name="waitForSequenceCompletion">True to block until the sequence engine completes (waits on SequenceEngineDone event); false to return immediately.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion when <paramref name="waitForSequenceCompletion"/> is <see langword="true"/>.</param>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            double[] voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            SequenceProvider<double> getVoltageSequence = _ => voltageSequence;
            ValueProvider getCurrentLimit = _ => currentLimit;
            ValueProvider getVoltageLevelRange = _ => voltageLevelRange;
            ValueProvider getCurrentLimitRange = _ => currentLimitRange;

            sessionsBundle.ForceSequenceSynchronizedCore(
                getVoltageSequence,
                DCPowerSourceOutputFunction.DCVoltage,
                getCurrentLimit,
                getVoltageLevelRange,
                getCurrentLimitRange,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, SiteData{ DCPowerSourceSettings[] }, int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceVoltageSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="voltageSequence"/>
        /// <param name="currentLimit"/>
        /// <param name="voltageLevelRange"/>
        /// <param name="currentLimitRange"/>
        /// <param name="sourceDelayInSeconds"/>
        /// <param name="transientResponse"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> voltageSequence,
            SiteData<double> currentLimit = null,
            SiteData<double> voltageLevelRange = null,
            SiteData<double> currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            SequenceProvider<double> getVoltageSequenceForSite = sitePinInfo => voltageSequence?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getCurrentLimitForSite = sitePinInfo => currentLimit?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getVoltageLevelRangeForSite = sitePinInfo => voltageLevelRange?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getCurrentLimitRangeForSite = sitePinInfo => currentLimitRange?.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceSequenceSynchronizedCore(
                getVoltageSequenceForSite,
                DCPowerSourceOutputFunction.DCVoltage,
                getCurrentLimitForSite,
                getVoltageLevelRangeForSite,
                getCurrentLimitRangeForSite,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, PinSiteData{ DCPowerSourceSettings[] }, int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceVoltageSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="voltageSequence"/>
        /// <param name="currentLimit"/>
        /// <param name="voltageLevelRange"/>
        /// <param name="currentLimitRange"/>
        /// <param name="sourceDelayInSeconds"/>
        /// <param name="transientResponse"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequence,
            PinSiteData<double> currentLimit = null,
            PinSiteData<double> voltageLevelRange = null,
            PinSiteData<double> currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            SequenceProvider<double> getVoltageSequenceForSitePin = sitePinInfo => voltageSequence?.GetValue(sitePinInfo);
            ValueProvider getCurrentLimitForSitePin = sitePinInfo => currentLimit?.GetValue(sitePinInfo);
            ValueProvider getVoltageLevelRangeForSitePin = sitePinInfo => voltageLevelRange?.GetValue(sitePinInfo);
            ValueProvider getCurrentLimitRangeForSitePin = sitePinInfo => currentLimitRange?.GetValue(sitePinInfo);

            sessionsBundle.ForceSequenceSynchronizedCore(
                getVoltageSequenceForSitePin,
                DCPowerSourceOutputFunction.DCVoltage,
                getCurrentLimitForSitePin,
                getVoltageLevelRangeForSitePin,
                getCurrentLimitRangeForSitePin,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <summary>
        /// Forces current on the target pins at the specified level. Must at least provide a level value, and the method will assume all other properties that have been previously set. Optionally, can also provide a specific voltage limit, current level range, voltage limit range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLevel">The current level to force.</param>
        /// <param name="voltageLimit">The voltage limit to use.</param>
        /// <param name="currentLevelRange">The current level range to use.</param>
        /// <param name="voltageLimitRange">The voltage limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, double currentLevel, double? voltageLimit = null, double? currentLevelRange = null, double? voltageLimitRange = null, bool waitForSourceCompletion = false)
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
            sessionsBundle.ForceCurrent(settings, waitForSourceCompletion);
        }

        /// <summary>
        /// Forces current on the target pins at the specified pin-unique level. Must at least provide a level value, and the method will assume all other properties that have been previously set. Optionally, can also provide a specific voltage limit, current level range, voltage limit range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLevels">The current level to force for different pins.</param>
        /// <param name="voltageLimit">The voltage limit to use.</param>
        /// <param name="currentLevelRange">The current level range to use.</param>
        /// <param name="voltageLimitRange">The voltage limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, double> currentLevels, double? voltageLimit = null, double? currentLevelRange = null, double? voltageLimitRange = null, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = currentLevels.GetValue(sitePinInfo, out bool isGroupData),
                    Limit = voltageLimit,
                    LevelRange = currentLevelRange,
                    LimitRange = voltageLimitRange
                };
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, isGroupData);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces current on the target pins at the specified site-unique level. Must at least provide a level value, and the method will assume all other properties that have been previously set. Optionally, can also provide a specific voltage limit, current level range, voltage limit range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLevels">The current level to force for different sites.</param>
        /// <param name="voltageLimit">The voltage limit to use.</param>
        /// <param name="currentLevelRange">The current level range to use.</param>
        /// <param name="voltageLimitRange">The voltage limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, SiteData<double> currentLevels, double? voltageLimit = null, double? currentLevelRange = null, double? voltageLimitRange = null, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = currentLevels.GetValue(sitePinInfo.SiteNumber),
                    Limit = voltageLimit,
                    LevelRange = currentLevelRange,
                    LimitRange = voltageLimitRange
                };
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces current on the target pins at the specified pin- and site-unique level. Must at least provide a level value, and the method will assume all other properties that have been previously set. Optionally, can also provide a specific voltage limit, current level range, voltage limit range values directly.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLevels">The current level to force for different site-pin pairs.</param>
        /// <param name="voltageLimit">The voltage limit to use.</param>
        /// <param name="currentLevelRange">The current level range to use.</param>
        /// <param name="voltageLimitRange">The voltage limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, PinSiteData<double> currentLevels, double? voltageLimit = null, double? currentLevelRange = null, double? voltageLimitRange = null, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = currentLevels.GetValue(sitePinInfo, out bool isGroupData),
                    Limit = voltageLimit,
                    LevelRange = currentLevelRange,
                    LimitRange = voltageLimitRange
                };
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, isGroupData);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces current using specified source settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The settings to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, DCPowerSourceSettings settings, bool waitForSourceCompletion = false)
        {
            settings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo);
                });
                sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.Force(settings, waitForSourceCompletion);
                });
            }
        }

        /// <summary>
        /// Forces current using specified site-unique source settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-site settings to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, SiteData<DCPowerSourceSettings> settings, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSiteSettings = settings.GetValue(sitePinInfo.SiteNumber);
                perSiteSettings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perSiteSettings, sitePinInfo);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces current using specified pin-unique source settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-pin settings to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, DCPowerSourceSettings> settings, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perPinSettings = settings.GetValue(sitePinInfo, out bool isGroupData);
                perPinSettings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perPinSettings, sitePinInfo, isGroupData);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces current using specified pin- and site-unique source settings.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The per-site-pin-pair settings to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, PinSiteData<DCPowerSourceSettings> settings, bool waitForSourceCompletion = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSitePinPairSettings = settings.GetValue(sitePinInfo, out bool isGroupData);
                perSitePinPairSettings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perSitePinPairSettings, sitePinInfo, isGroupData);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of current outputs, ensuring synchronized output across all specified target pins.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentSequence">Sequence of current values to force.</param>
        /// <param name="voltageLimit">Voltage limit for the sequence.</param>
        /// <param name="currentLevelRange">Current level range.</param>
        /// <param name="voltageLimitRange">Voltage limit range.</param>
        /// <param name="sourceDelayInSeconds">Optional source delay to use uniformly for synchronization.</param>
        /// <param name="transientResponse">Transient response.</param>
        /// <param name="sequenceLoopCount">The number of times to force the sequence.</param>
        /// <param name="waitForSequenceCompletion">True to block until the sequence engine completes (waits on SequenceEngineDone event); false to return immediately.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion when <paramref name="waitForSequenceCompletion"/> is true.</param>
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            double[] currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            SequenceProvider<double> getCurrentSequence = _ => currentSequence;
            ValueProvider getVoltageLimit = _ => voltageLimit;
            ValueProvider getCurrentLevelRange = _ => currentLevelRange;
            ValueProvider getVoltageLimitRange = _ => voltageLimitRange;

            sessionsBundle.ForceSequenceSynchronizedCore(
                getCurrentSequence,
                DCPowerSourceOutputFunction.DCCurrent,
                getVoltageLimit,
                getCurrentLevelRange,
                getVoltageLimitRange,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, SiteData{ DCPowerSourceSettings[] }, int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceCurrentSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="currentSequence"/>
        /// <param name="voltageLimit"/>
        /// <param name="currentLevelRange"/>
        /// <param name="voltageLimitRange"/>
        /// <param name="sourceDelayInSeconds"/>
        /// <param name="transientResponse"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> currentSequence,
            SiteData<double> voltageLimit = null,
            SiteData<double> currentLevelRange = null,
            SiteData<double> voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            SequenceProvider<double> getCurrentSequenceForSite = sitePinInfo => currentSequence.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getVoltageLimitForSite = sitePinInfo => voltageLimit?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getCurrentLevelRangeForSite = sitePinInfo => currentLevelRange?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getVoltageLimitRangeForSite = sitePinInfo => voltageLimitRange?.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceSequenceSynchronizedCore(
                getCurrentSequenceForSite,
                DCPowerSourceOutputFunction.DCCurrent,
                getVoltageLimitForSite,
                getCurrentLevelRangeForSite,
                getVoltageLimitRangeForSite,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, PinSiteData{ DCPowerSourceSettings[] }, int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceCurrentSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="currentSequence"/>
        /// <param name="voltageLimit"/>
        /// <param name="currentLevelRange"/>
        /// <param name="voltageLimitRange"/>
        /// <param name="sourceDelayInSeconds"/>
        /// <param name="transientResponse"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> currentSequence,
            PinSiteData<double> voltageLimit = null,
            PinSiteData<double> currentLevelRange = null,
            PinSiteData<double> voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            SequenceProvider<double> getCurrentSequenceForSitePin = sitePinInfo => currentSequence.GetValue(sitePinInfo);
            ValueProvider getVoltageLimitForSitePin = sitePinInfo => voltageLimit?.GetValue(sitePinInfo);
            ValueProvider getCurrentLevelRangeForSitePin = sitePinInfo => currentLevelRange?.GetValue(sitePinInfo);
            ValueProvider getVoltageLimitRangeForSitePin = sitePinInfo => voltageLimitRange?.GetValue(sitePinInfo);

            sessionsBundle.ForceSequenceSynchronizedCore(
                getCurrentSequenceForSitePin,
                DCPowerSourceOutputFunction.DCCurrent,
                getVoltageLimitForSitePin,
                getCurrentLevelRangeForSitePin,
                getVoltageLimitRangeForSitePin,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of current/voltage outputs, ensuring synchronized output across all specified target pins.
        /// </summary>
        private static void ForceSequenceSynchronizedCore(
            this DCPowerSessionsBundle sessionsBundle,
            SequenceProvider<double> fetchLevelSequence,
            DCPowerSourceOutputFunction outputFunction,
            ValueProvider fetchLimit,
            ValueProvider fetchLevelRange,
            ValueProvider fetchLimitRange,
            double? sourceDelayInSeconds,
            DCPowerSourceTransientResponse? transientResponse,
            int sequenceLoopCount,
            bool waitForSequenceCompletion,
            double sequenceTimeoutInSeconds)
        {
            var sequenceName = BuildSequenceName();
            // The output of a designated primary channel within the bundle is needed to synchronize all other channels together.
            var primaryOutput = sessionsBundle.GetPrimaryOutput(TriggerType.StartTrigger.ToString(), out string startTrigger);

            // Configure all channels
            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = outputFunction,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Limit = fetchLimit(sitePinInfo),
                    LevelRange = fetchLevelRange(sitePinInfo),
                    LimitRange = fetchLimitRange(sitePinInfo),
                    TransientResponse = transientResponse
                };

                var perChannelString = sitePinInfo.IndividualChannelString;
                var channelOutput = sessionInfo.Session.Outputs[perChannelString];
                channelOutput.Control.Abort();
                channelOutput.ConfigureLevelsAndLimits(settings);
                channelOutput.ConfigureSequenceCore(
                    sequenceName: sequenceName,
                    sequence: fetchLevelSequence(sitePinInfo),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: outputFunction,
                    setAsActiveSequence: true);
                channelOutput.Source.SourceDelay = sourceDelayInSeconds.HasValue
                    ? PrecisionTimeSpan.FromSeconds(sourceDelayInSeconds.Value)
                    : PrecisionTimeSpan.Zero;
                sessionInfo.ConfigureTransientResponce(settings, perChannelString);

                if (IsPrimaryOutput(sessionIndex, sitePinInfo, sessionInfo))
                {
                    // Primary channel does not need a start trigger
                    channelOutput.Triggers.StartTrigger.Disable();
                    channelOutput.Control.Commit();
                }
                else
                {
                    // All other channels start on primary channel's start trigger
                    channelOutput.Triggers.StartTrigger.DigitalEdge.Configure(startTrigger, DCPowerTriggerEdge.Rising);
                    channelOutput.Control.Initiate();
                }
            });

            // Start Primary
            primaryOutput.Control.Initiate();

            if (waitForSequenceCompletion)
            {
                primaryOutput.Events.SequenceEngineDoneEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(sequenceTimeoutInSeconds));
            }

            sessionsBundle.ReleaseSynchronizedAdvancedSequenceResources(sequenceName);
        }

        /// <summary>
        /// Synchronizes and forces an advanced sequence across all sessions in the bundle.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The sequence of source settings to apply.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the sequence.</param>
        /// <param name="waitForSequenceCompletion">Indicates whether to wait for the sequence to complete before returning.</param>
        /// <param name="sequenceTimeoutInSeconds">The timeout in seconds to wait for sequence completion.</param>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerSourceSettings[] sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0)
        {
            SequenceProvider<DCPowerSourceSettings> getSequence = _ => sequence;

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, SiteData{ DCPowerSourceSettings[] }, int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sequence"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerSourceSettings[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0)
        {
            SequenceProvider<DCPowerSourceSettings> getVoltageSequence = sitePinInfo => sequence.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getVoltageSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, PinSiteData{ DCPowerSourceSettings[] }, int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sequence"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerSourceSettings[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0)
        {
            SequenceProvider<DCPowerSourceSettings> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo);

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <summary>
        /// Synchronizes and forces an advanced sequence across all sessions in the bundle and return measurements.
        /// </summary>
        /// <remarks>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The sequence of source settings to apply.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the voltage sequence.</param>
        /// <param name="waitForSequenceCompletion">Indicates whether to wait for the sequence to complete before returning.</param>
        /// <param name="sequenceTimeoutInSeconds">The timeout in seconds to wait for sequence completion.</param>
        /// <param name="pointsToFetch">The number of points to Fetch.</param>
        /// <param name="measurementTimeoutInSeconds">The time to wait before the fetch measurement operation is aborted.</param>
        /// <returns>A <see cref="PinSiteData{T}"/> object that contains an array of <see cref="SingleDCPowerFetchResult"/> values,
        /// where each <see cref="SingleDCPowerFetchResult"/> object contains the voltage, current, and inCompliance result for a simple sample/point from the previous measurement.</returns>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerSourceSettings[] sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            SequenceProvider<DCPowerSourceSettings> getSequence = _ => sequence;

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerSourceSettings[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            SequenceProvider<DCPowerSourceSettings> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo.SiteNumber);

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerSourceSettings[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            SequenceProvider<DCPowerSourceSettings> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo);

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <summary>
        /// Synchronizes and forces an advanced sequence across all sessions in the bundle.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The sequence of <see cref="DCPowerAdvancedSequenceStepProperties"/> to apply.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the sequence.</param>
        /// <param name="waitForSequenceCompletion">Indicates whether to wait for the sequence to complete before returning.</param>
        /// <param name="sequenceTimeoutInSeconds">The timeout in seconds to wait for sequence completion.</param>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerAdvancedSequenceStepProperties[] sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0)
        {
            SequenceProvider<DCPowerAdvancedSequenceStepProperties> getSequence = _ => sequence;

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, SiteData{ DCPowerAdvancedSequenceStepProperties[] }, int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sequence"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0)
        {
            SequenceProvider<DCPowerAdvancedSequenceStepProperties> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, PinSiteData{ DCPowerAdvancedSequenceStepProperties[] }, int, bool, double, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sequence"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0)
        {
            SequenceProvider<DCPowerAdvancedSequenceStepProperties> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo);

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <summary>
        /// Synchronizes and forces an advanced sequence across all sessions in the bundle and return measurements.
        /// </summary>
        /// <remarks>
        /// This function will switch the Source Mode back to SinglePoint.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The sequence of <see cref="DCPowerAdvancedSequenceStepProperties"/> to apply.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the voltage sequence.</param>
        /// <param name="waitForSequenceCompletion">Indicates whether to wait for the sequence to complete before returning.</param>
        /// <param name="sequenceTimeoutInSeconds">The timeout in seconds to wait for sequence completion.</param>
        /// <param name="pointsToFetch">The number of points to Fetch.</param>
        /// <param name="measurementTimeoutInSeconds">The time to wait before the fetch measurement operation is aborted.</param>
        /// <returns>A <see cref="PinSiteData{T}"/> object that contains an array of <see cref="SingleDCPowerFetchResult"/> values,
        /// where each <see cref="SingleDCPowerFetchResult"/> object contains the voltage, current, and inCompliance result for a simple sample/point from the previous measurement.</returns>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerAdvancedSequenceStepProperties[] sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            SequenceProvider<DCPowerAdvancedSequenceStepProperties> getSequence = _ => sequence;

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], int, bool, double, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            SequenceProvider<DCPowerAdvancedSequenceStepProperties> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo.SiteNumber);

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], int, bool, double, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            SequenceProvider<DCPowerAdvancedSequenceStepProperties> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo);

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        private static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedCore<T>(
            this DCPowerSessionsBundle sessionsBundle,
            SequenceProvider<T> getSequence,
            int sequenceLoopCount,
            bool waitForSequenceCompletion,
            double sequenceTimeoutInSeconds,
            bool fetchResult = false,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10) where T : class
        {
            // The output of a designated primary channel within the bundle is needed to synchronize all other channels together.
            var primaryOutput = sessionsBundle.GetPrimaryOutput(TriggerType.StartTrigger.ToString(), out string startTrigger);
            var sequenceName = BuildSequenceName();
            PinSiteData<SingleDCPowerFetchResult[]> result = null;

            sessionsBundle.Do((sessionInfo, sessionIndex, sitePinInfo) =>
            {
                var perSitePinSequence = getSequence(sitePinInfo);
                var validProperties = GetValidProperties(perSitePinSequence);
                var perChannelString = sitePinInfo.IndividualChannelString;
                var channelOutput = sessionInfo.Session.Outputs[perChannelString];
                channelOutput.Control.Abort();
                if (fetchResult)
                {
                    if (pointsToFetch == null)
                    {
                        pointsToFetch = perSitePinSequence.Length;
                    }
                    channelOutput.Measurement.MeasureWhen = DCPowerMeasurementWhen.AutomaticallyAfterSourceComplete;
                }
                channelOutput.Source.SequenceLoopCount = sequenceLoopCount;
                channelOutput.ConfigureAdvancedSequenceCore(
                    sequenceName,
                    sitePinInfo.ModelString,
                    validProperties,
                    setAsActiveSequence: true,
                    commitFirstElementAsInitialState: false);
                if (IsPrimaryOutput(sessionIndex, sitePinInfo, sessionInfo))
                {
                    channelOutput.Triggers.StartTrigger.Disable();
                    channelOutput.Control.Commit();
                }
                else
                {
                     // All other channels start on primary channel's start trigger
                    channelOutput.Triggers.StartTrigger.DigitalEdge.Configure(startTrigger, DCPowerTriggerEdge.Rising);
                    channelOutput.Control.Initiate();
                }
            });

            primaryOutput.Control.Initiate();

            if (waitForSequenceCompletion)
            {
                primaryOutput.Events.SequenceEngineDoneEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(sequenceTimeoutInSeconds));
            }

            if (fetchResult)
            {
                result = sessionsBundle.FetchMeasurement(pointsToFetch.Value, measurementTimeoutInSeconds);
            }

            sessionsBundle.ReleaseSynchronizedAdvancedSequenceResources(sequenceName);

            return result;
        }

        /// <summary>
        /// Behaves the same as the ForceCurrent() method, but has two voltage limit inputs for setting separate high and low voltage limits.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLevel">The current level to force.</param>
        /// <param name="voltageLimitHigh">The voltage high limit to use.</param>
        /// <param name="voltageLimitLow">The voltage low limit to use.</param>
        /// <param name="currentLevelRange">The current level range to use.</param>
        /// <param name="voltageLimitRange">The voltage limit range to use.</param>
        /// <param name="waitForSourceCompletion">Setting this to True will wait until sourcing is complete before continuing, which includes the set amount of source delay.
        /// Otherwise, the source delay amount is not directly accounted for by this method and the WaitForEvent must be manually invoked in proceeding code.</param>
        public static void ForceCurrentAsymmetricLimit(this DCPowerSessionsBundle sessionsBundle, double currentLevel, double voltageLimitHigh, double voltageLimitLow, double? currentLevelRange = null, double? voltageLimitRange = null, bool waitForSourceCompletion = false)
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
            sessionsBundle.ForceCurrent(settings, waitForSourceCompletion);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of current values on the targeted pin(s).
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureCurrentSequence(DCPowerSessionsBundle, string, double[], int, double?, bool)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentSequence">Sequence of current values to force.</param>
        /// <param name="voltageLimit">Voltage limit for the sequence.</param>
        /// <param name="currentLevelRange">Current level range.</param>
        /// <param name="voltageLimitRange">Voltage limit range.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="waitForSequenceCompletion">True to block until the sequence engine completes (waits on SequenceEngineDone event); false to return immediately.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion when <paramref name="waitForSequenceCompletion"/> is true.</param>
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            double[] currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            var sequenceName = BuildSequenceName();
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Limit = voltageLimit,
                LevelRange = currentLevelRange,
                LimitRange = voltageLimitRange
            };

            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    sessionInfo.ConfigureAllChannelsForSequenceModeAndInitiateGangedFollowerChannels(
                        sitePinInfo,
                        settings,
                        sequenceName,
                        currentSequence,
                        sequenceLoopCount,
                        setAsActiveSequence: true);
                });
                sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSequenceCompletion, sequenceTimeoutInSeconds);
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.ForceSequenceCore(
                        settings,
                        sequenceName,
                        currentSequence,
                        sequenceLoopCount,
                        waitForSequenceCompletion,
                        sequenceTimeoutInSeconds,
                        setAsActiveSequence: true);
                });
            }

            sessionsBundle.ReleaseAdvancedSequenceResources(sequenceName);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureCurrentSequence(DCPowerSessionsBundle, string, SiteData{double[]}, int, double?, bool)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceCurrentSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="currentSequence"/>
        /// <param name="voltageLimit"/>
        /// <param name="currentLevelRange"/>
        /// <param name="voltageLimitRange"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            var sequenceName = BuildSequenceName();
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Limit = voltageLimit,
                LevelRange = currentLevelRange,
                LimitRange = voltageLimitRange
            };
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var sequence = currentSequence.GetValue(sitePinInfo.SiteNumber);
                sessionInfo.ConfigureAllChannelsForSequenceModeAndInitiateGangedFollowerChannels(
                    sitePinInfo,
                    settings,
                    sequenceName,
                    sequence,
                    sequenceLoopCount,
                    setAsActiveSequence: true);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSequenceCompletion, sequenceTimeoutInSeconds);

            sessionsBundle.ReleaseAdvancedSequenceResources(sequenceName);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureCurrentSequence(DCPowerSessionsBundle, string, PinSiteData{double[]}, int, double?, bool)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceCurrentSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="currentSequence"/>
        /// <param name="voltageLimit"/>
        /// <param name="currentLevelRange"/>
        /// <param name="voltageLimitRange"/>
        /// <param name="sequenceLoopCount"/>
        /// <param name="waitForSequenceCompletion"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            var sequenceName = BuildSequenceName();
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Limit = voltageLimit,
                LevelRange = currentLevelRange,
                LimitRange = voltageLimitRange
            };
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var sequence = currentSequence.GetValue(sitePinInfo, out bool isGroupData);
                sessionInfo.ConfigureAllChannelsForSequenceModeAndInitiateGangedFollowerChannels(
                    sitePinInfo,
                    settings,
                    sequenceName,
                    sequence,
                    sequenceLoopCount,
                    isGroupData,
                    setAsActiveSequence: true);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSequenceCompletion, sequenceTimeoutInSeconds);

            sessionsBundle.ReleaseAdvancedSequenceResources(sequenceName);
        }

        private static void ConfigureAllChannelsForSequenceModeAndInitiateGangedFollowerChannels(
            this DCPowerSessionInformation sessionInfo,
            SitePinInfo sitePinInfo,
            DCPowerSourceSettings settings,
            string sequenceName,
            double[] levelSequence,
            int sequenceLoopCount,
            bool needDataAdjustment = true,
            bool setAsActiveSequence = false)
        {
            var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
            channelOutput.Control.Abort();
            channelOutput.ConfigureLevelsAndLimits(settings, sitePinInfo, needDataAdjustment);
            sessionInfo.ConfigureSequenceForCascadingCore(
                sequenceName: sequenceName,
                sequence: levelSequence,
                sequenceLoopCount: sequenceLoopCount,
                outputFunction: (DCPowerSourceOutputFunction)settings.OutputFunction,
                sitePinInfo: sitePinInfo,
                needDataAdjustment: needDataAdjustment,
                setAsActiveSequence: setAsActiveSequence);
            if (IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
            {
                channelOutput.InitiateChannels();
            }
        }

        /// <summary>
        /// Powers down the channel by disabling output generation on the underlying device channel(s).
        /// </summary>
        /// <Remarks>
        /// This method is similar to <see cref="ConfigureOutputEnabled(DCPowerSessionsBundle,bool)"/> but will automatically initiate the underlying driver session.
        /// <para>
        /// This method does not physically disconnect the output channel.
        /// Use the <see cref="ConfigureOutputConnected(DCPowerSessionsBundle, bool)"/> method to physically disconnect the connected output channel on supported instruments.
        /// </para>
        /// </Remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settlingTime">The settling time. Null means no need to wait for the turn off operation to settle.</param>
        public static void PowerDown(this DCPowerSessionsBundle sessionsBundle, double? settlingTime = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Abort();
                var originalSourceMode = sessionInfo.AllChannelsOutput.Source.Mode;
                sessionInfo.AllChannelsOutput.Source.Mode = DCPowerSourceMode.SinglePoint;
                sessionInfo.AllChannelsOutput.Source.Output.Enabled = false;
                sessionInfo.AllChannelsOutput.Control.Initiate();
                sessionInfo.AllChannelsOutput.Control.Abort();
                sessionInfo.AllChannelsOutput.Source.Mode = originalSourceMode;
            });
            PreciseWait(settlingTime);
        }

        /// <summary>
        /// Configures the current limit.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLimit">The current limit to set.</param>
        /// <param name="currentLimitRange">The current limit range to set. Use the absolute value of current limit to set current limit range when this parameter is not specified.</param>
        public static void ConfigureCurrentLimit(this DCPowerSessionsBundle sessionsBundle, double currentLimit, double? currentLimitRange = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.AllChannelsOutput.ConfigureCurrentLimit(currentLimit, currentLimitRange);
            });
        }

        /// <summary>
        /// Configures the current limits.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentLimits">The per-pin current limits to set.</param>
        /// <param name="currentLimitRanges">The current limit ranges to set. Use the absolute value of current limit to set current limit range when this parameter is not specified.</param>
        public static void ConfigureCurrentLimits(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, double> currentLimits, IDictionary<string, double> currentLimitRanges = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.ConfigureCurrentLimit(currentLimits[sitePinInfo.PinName], currentLimitRanges?[sitePinInfo.PinName]);
            });
        }

        /// <summary>
        /// Configures a hardware-timed sequence of values.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The voltage or current sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        [Obsolete("Using both simple sequencing and advanced sequencing for the same channel within the same session is not supported. For this reason it is better to just use advanced sequencing. Consider using either ConfigureVoltageSequence or ConfigureCurrentSequence instead.", error: false)]
        public static void ConfigureSequence(this DCPowerSessionsBundle sessionsBundle, double[] sequence, int sequenceLoopCount, double? sequenceStepDeltaTimeInSeconds = null)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    channelOutput.Control.Abort();
                    channelOutput.ConfigureSequence(
                        sequence,
                        sequenceLoopCount,
                        sequenceStepDeltaTimeInSeconds,
                        sitePinInfo: sitePinInfo);
                });
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.Session.Control.Abort();
                    sessionInfo.AllChannelsOutput.ConfigureSequence(sequence, sequenceLoopCount, sequenceStepDeltaTimeInSeconds);
                });
            }
        }

        /// <summary>
        /// Configures a hardware-timed voltage sequence.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequenceName">The name of the advanced sequence to create.</param>
        /// <param name="sequence">The voltage sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        /// <param name="setAsActiveSequence">
        /// If <see langword="true"/>, sets the configured sequence as the active sequence.
        /// If <see langword="false"/> (default), clears the active sequence to allow configuring multiple sequences before initiating.
        /// </param>
        public static void ConfigureVoltageSequence(this DCPowerSessionsBundle sessionsBundle, string sequenceName, double[] sequence, int sequenceLoopCount = 1, double? sequenceStepDeltaTimeInSeconds = null, bool setAsActiveSequence = false)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    channelOutput.Control.Abort();
                    channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                    sessionInfo.ConfigureSequenceForCascadingCore(
                        sequenceName: sequenceName,
                        sequence: sequence,
                        sequenceLoopCount: sequenceLoopCount,
                        outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                        sitePinInfo: sitePinInfo,
                        sequenceStepDeltaTimeInSeconds,
                        setAsActiveSequence: setAsActiveSequence);
                });
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.Session.Control.Abort();
                    sessionInfo.AllChannelsOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                    sessionInfo.AllChannelsOutput.ConfigureSequenceCore(
                        sequenceName: sequenceName,
                        sequence: sequence,
                        sequenceLoopCount: sequenceLoopCount,
                        outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                        sequenceStepDeltaTimeInSeconds: sequenceStepDeltaTimeInSeconds,
                        setAsActiveSequence: setAsActiveSequence);
                });
            }
        }

        /// <inheritdoc cref="ConfigureVoltageSequence(DCPowerSessionsBundle,string, double[], int, double?, bool)"/>
        public static void ConfigureVoltageSequence(this DCPowerSessionsBundle sessionsBundle, string sequenceName, SiteData<double[]> sequence, int sequenceLoopCount = 1, double? sequenceStepDeltaTimeInSeconds = null, bool setAsActiveSequence = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo.SiteNumber),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                    sitePinInfo: sitePinInfo,
                    sequenceStepDeltaTimeInSeconds: sequenceStepDeltaTimeInSeconds,
                    setAsActiveSequence: setAsActiveSequence);
            });
        }

        /// <inheritdoc cref="ConfigureVoltageSequence(DCPowerSessionsBundle,string, double[], int, double?, bool)"/>
        public static void ConfigureVoltageSequence(this DCPowerSessionsBundle sessionsBundle, string sequenceName, PinSiteData<double[]> sequence, int sequenceLoopCount = 1, double? sequenceStepDeltaTimeInSeconds = null, bool setAsActiveSequence = false)
        {
            var hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, sequence);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                    sitePinInfo: sitePinInfo,
                    sequenceStepDeltaTimeInSeconds,
                    setAsActiveSequence: setAsActiveSequence);
            });
        }

        /// <summary>
        /// Configures a hardware-timed current sequence.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequenceName">The name of the advanced sequence to create.</param>
        /// <param name="sequence">The current sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        /// <param name="setAsActiveSequence">
        /// If <see langword="true"/>, sets the configured sequence as the active sequence.
        /// If <see langword="false"/> (default), clears the active sequence to allow configuring multiple sequences before initiating.
        /// </param>
        public static void ConfigureCurrentSequence(this DCPowerSessionsBundle sessionsBundle, string sequenceName, double[] sequence, int sequenceLoopCount = 1, double? sequenceStepDeltaTimeInSeconds = null, bool setAsActiveSequence = false)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    channelOutput.Control.Abort();
                    channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                    sessionInfo.ConfigureSequenceForCascadingCore(
                        sequenceName: sequenceName,
                        sequence: sequence,
                        sequenceLoopCount: sequenceLoopCount,
                        outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                        sitePinInfo: sitePinInfo,
                        sequenceStepDeltaTimeInSeconds: sequenceStepDeltaTimeInSeconds,
                        setAsActiveSequence: setAsActiveSequence);
                });
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.Session.Control.Abort();
                    sessionInfo.AllChannelsOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                    sessionInfo.AllChannelsOutput.ConfigureSequenceCore(
                        sequenceName: sequenceName,
                        sequence: sequence,
                        sequenceLoopCount: sequenceLoopCount,
                        outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                        sequenceStepDeltaTimeInSeconds: sequenceStepDeltaTimeInSeconds,
                        setAsActiveSequence: setAsActiveSequence);
                });
            }
        }

        /// <inheritdoc cref="ConfigureCurrentSequence(DCPowerSessionsBundle,string, double[], int, double?, bool)"/>
        public static void ConfigureCurrentSequence(this DCPowerSessionsBundle sessionsBundle, string sequenceName, SiteData<double[]> sequence, int sequenceLoopCount = 1, double? sequenceStepDeltaTimeInSeconds = null, bool setAsActiveSequence = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo.SiteNumber),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                    sitePinInfo: sitePinInfo,
                    sequenceStepDeltaTimeInSeconds,
                    setAsActiveSequence: setAsActiveSequence);
            });
        }

        /// <inheritdoc cref="ConfigureCurrentSequence(DCPowerSessionsBundle,string, double[], int, double?, bool)"/>
        public static void ConfigureCurrentSequence(this DCPowerSessionsBundle sessionsBundle, string sequenceName, PinSiteData<double[]> sequence, int sequenceLoopCount = 1, double? sequenceStepDeltaTimeInSeconds = null, bool setAsActiveSequence = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo, out bool isGroupData),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                    sitePinInfo: sitePinInfo,
                    sequenceStepDeltaTimeInSeconds: sequenceStepDeltaTimeInSeconds,
                    needDataAdjustment: isGroupData,
                    setAsActiveSequence: setAsActiveSequence);
            });
        }

        /// <summary>
        /// Creates and configures an advanced sequence with per-step property configurations.
        /// </summary>
        /// <param name="sessionsBundle">The DCPower sessions bundle.</param>
        /// <param name="sequenceName">The name of the advanced sequence to create.</param>
        /// <param name="perStepProperties">A list of property configurations for each step in the sequence.</param>
        /// <param name="setAsActiveSequence">
        /// If <see langword="true"/>, sets the configured sequence as the active sequence.
        /// If <see langword="false"/> (default), clears the active sequence to allow configuring multiple sequences before initiating.
        /// </param>
        /// <param name="commitFirstElementAsInitialState">If true, uses the first element in perStepProperties as a commit step. Default is false.</param>
        public static void ConfigureAdvancedSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            IList<DCPowerAdvancedSequenceStepProperties> perStepProperties,
            bool setAsActiveSequence = false,
            bool commitFirstElementAsInitialState = false)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.ConfigureAdvancedSequenceCore(
                    sequenceName,
                    sitePinInfo.ModelString,
                    perStepProperties,
                    setAsActiveSequence,
                    commitFirstElementAsInitialState);
            });
        }

        /// <inheritdoc cref="ConfigureAdvancedSequence(DCPowerSessionsBundle, string, IList{DCPowerAdvancedSequenceStepProperties}, bool, bool)"/>
        public static void ConfigureAdvancedSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            SiteData<IList<DCPowerAdvancedSequenceStepProperties>> perStepProperties,
            bool setAsActiveSequence = false,
            bool commitFirstElementAsInitialState = false)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var stepProperties = perStepProperties.GetValue(sitePinInfo.SiteNumber);
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.ConfigureAdvancedSequenceCore(
                    sequenceName,
                    sitePinInfo.ModelString,
                    stepProperties,
                    setAsActiveSequence,
                    commitFirstElementAsInitialState);
            });
        }

        /// <inheritdoc cref="ConfigureAdvancedSequence(DCPowerSessionsBundle, string, IList{DCPowerAdvancedSequenceStepProperties}, bool, bool)"/>
        public static void ConfigureAdvancedSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            PinSiteData<IList<DCPowerAdvancedSequenceStepProperties>> perStepProperties,
            bool setAsActiveSequence = false,
            bool commitFirstElementAsInitialState = false)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var stepProperties = perStepProperties.GetValue(sitePinInfo);
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.ConfigureAdvancedSequenceCore(
                    sequenceName,
                    sitePinInfo.ModelString,
                    stepProperties,
                    setAsActiveSequence,
                    commitFirstElementAsInitialState);
            });
        }

        /// <summary>
        /// Gets the current limits.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The per-site per-pin current limits.</returns>
        public static PinSiteData<double> GetCurrentLimits(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                return sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Source.Voltage.CurrentLimit;
            });
        }

        /// <summary>
        /// Checks if the output function is set to DCVoltage and the level(s) are set to the expected values.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="failedChannels">Returns the channels that fail the check.</param>
        /// <param name="expectedVoltages">The expected per-pin voltages.</param>
        /// <returns>Whether all channels pass the check.</returns>
        public static bool CheckDCVoltageModeAndLevels(this DCPowerSessionsBundle sessionsBundle, out IEnumerable<string> failedChannels, IDictionary<string, double> expectedVoltages = null)
        {
            var results = sessionsBundle.DoAndReturnPerInstrumentPerChannelResults((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                bool pass = sessionInfo.Session.Source.Mode == DCPowerSourceMode.SinglePoint
                    && channelOutput.Source.Output.Function == DCPowerSourceOutputFunction.DCVoltage
                    && (expectedVoltages is null || channelOutput.Source.Voltage.VoltageLevel == expectedVoltages[sitePinInfo.PinName]);
                return pass ? string.Empty : sessionInfo.AllChannelsString;
            });
            var flatternedResults = results.SelectMany(r => r);
            failedChannels = flatternedResults.Where(r => !string.IsNullOrEmpty(r));
            return !failedChannels.Any();
        }

        /// <summary>
        /// Configures the output relay of the underlying device channel (s) to either be connected (closed) or disconnected (open).
        /// Accepts a scalar input of type <see  cref="bool"/>.
        /// With overrides for <see cref="SiteData{Boolean}" />, and <see cref="PinSiteData{Boolean}"/> input.
        /// <para>Pass this method a false value to physically disconnect the output terminal from the front panel.</para>
        /// <remarks>
        /// Excessive connecting and disconnecting of the output can cause premature wear on the relay.
        /// Disconnect the output only if physically disconnecting is necessary for your application.
        /// For example, a battery connected to the output terminal might discharge unless the relay is disconnected.
        /// <para>
        /// This method will configure the <see cref="DCPowerOutputSourceOutput.Connected"/> property, which is not supported by all devices.
        /// Refer to the Supported Properties by Device topic in the NI-DCPower User Manual for information about supported devices.
        /// </para>
        /// <para>
        /// This method is independent from the <see cref="ConfigureOutputEnabled(DCPowerSessionsBundle, bool)"/> method.
        /// It does not affect the <see cref="DCPowerOutputSourceOutput.Enabled"/> property.
        /// </para>
        /// </remarks>
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="connectOutput">The boolean value to either connect (true) or disconnect (false) the output terminal.</param>
        public static void ConfigureOutputConnected(this DCPowerSessionsBundle sessionsBundle, bool connectOutput)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.AllChannelsOutput.Source.Output.Connected = connectOutput;
            });
        }

        /// <inheritdoc cref="ConfigureOutputConnected(DCPowerSessionsBundle, bool)"/>
        public static void ConfigureOutputConnected(this DCPowerSessionsBundle sessionsBundle, SiteData<bool> connectOutput)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Source.Output.Connected = connectOutput.GetValue(pinSiteInfo.SiteNumber);
            });
        }

        /// <inheritdoc cref="ConfigureOutputConnected(DCPowerSessionsBundle, bool)"/>
        public static void ConfigureOutputConnected(this DCPowerSessionsBundle sessionsBundle, PinSiteData<bool> connectOutput)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Source.Output.Connected = connectOutput.GetValue(pinSiteInfo);
            });
        }

        /// <summary>
        /// Configures whether to enable (true) or disable (false) output generation on the underlying device channel(s).
        /// </summary>
        /// <remarks>
        /// Note: This method is independent from the <see cref="ConfigureOutputConnected(DCPowerSessionsBundle, bool)"/> method. It does not affect the <see cref="DCPowerOutputSourceOutput.Connected"/> property.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="enableOutput">The boolean value to either enable (true) or disable (false) the output .</param>
        public static void ConfigureOutputEnabled(this DCPowerSessionsBundle sessionsBundle, bool enableOutput)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Abort();
                sessionInfo.AllChannelsOutput.Source.Output.Enabled = enableOutput;
            });
        }

        /// <inheritdoc cref="ConfigureOutputEnabled(DCPowerSessionsBundle, bool)"/>
        public static void ConfigureOutputEnabled(this DCPowerSessionsBundle sessionsBundle, SiteData<bool> enableOutput)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Source.Output.Enabled = enableOutput.GetValue(pinSiteInfo.SiteNumber);
            });
        }

        /// <inheritdoc cref="ConfigureOutputEnabled(DCPowerSessionsBundle, bool)"/>
        public static void ConfigureOutputEnabled(this DCPowerSessionsBundle sessionsBundle, PinSiteData<bool> enableOutput)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Source.Output.Enabled = enableOutput.GetValue(pinSiteInfo);
            });
        }

        /// <summary>
        /// Configures the source delay.
        /// With overrides for <see cref="SiteData{Double}" />, and <see cref="PinSiteData{Double}"/> input.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sourceDelayInSeconds">The double value of the source delay in seconds.</param>
        public static void ConfigureSourceDelay(this DCPowerSessionsBundle sessionsBundle, double sourceDelayInSeconds)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Abort();
                sessionInfo.AllChannelsOutput.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(sourceDelayInSeconds);
            });
        }

        /// <inheritdoc cref="ConfigureSourceDelay(DCPowerSessionsBundle, double)"/>
        public static void ConfigureSourceDelay(this DCPowerSessionsBundle sessionsBundle, SiteData<double> sourceDelayInSeconds)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(sourceDelayInSeconds.GetValue(pinSiteInfo.SiteNumber));
            });
        }

        /// <inheritdoc cref="ConfigureSourceDelay(DCPowerSessionsBundle, double)"/>
        public static void ConfigureSourceDelay(this DCPowerSessionsBundle sessionsBundle, PinSiteData<double> sourceDelayInSeconds)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Control.Abort();
                sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Source.SourceDelay = PrecisionTimeSpan.FromSeconds(sourceDelayInSeconds.GetValue(pinSiteInfo));
            });
        }

        /// <summary>
        /// Configures a hardware-timed voltage sequence with per-step source delays.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequenceName">The name of the advanced sequence to create.</param>
        /// <param name="sequence">The voltage sequence to set.</param>
        /// <param name="sourceDelaysInSeconds">The array of source delays in seconds for each step in the sequence.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="setAsActiveSequence">
        /// If <see langword="true"/>, sets the configured sequence as the active sequence.
        /// If <see langword="false"/> (default), clears the active sequence to allow configuring multiple sequences before initiating.
        /// </param>
        public static void ConfigureVoltageSequenceWithSourceDelays(
           this DCPowerSessionsBundle sessionsBundle,
           string sequenceName,
           double[] sequence,
           double[] sourceDelaysInSeconds,
           int sequenceLoopCount = 1,
           bool setAsActiveSequence = false)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    channelOutput.Control.Abort();
                    channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                    sessionInfo.ConfigureSequenceForCascadingCore(
                        sequenceName: sequenceName,
                        sequence: sequence,
                        sequenceLoopCount: sequenceLoopCount,
                        outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                        sitePinInfo: sitePinInfo,
                        sourceDelay: sourceDelaysInSeconds,
                        setAsActiveSequence: setAsActiveSequence);
                });
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Control.Abort();
                    sessionInfo.AllChannelsOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                    sessionInfo.AllChannelsOutput.ConfigureSequenceCore(
                        sequenceName: sequenceName,
                        sequence: sequence,
                        sequenceLoopCount: sequenceLoopCount,
                        outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                        sourceDelaysInSeconds: sourceDelaysInSeconds,
                        setAsActiveSequence: setAsActiveSequence);
                });
            }
        }

        /// <inheritdoc cref="ConfigureVoltageSequenceWithSourceDelays(DCPowerSessionsBundle, string, double[], double[], int, bool)"/>
        public static void ConfigureVoltageSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            SiteData<double[]> sequence,
            SiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1,
            bool setAsActiveSequence = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo.SiteNumber),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                    sitePinInfo: sitePinInfo,
                    sourceDelay: sourceDelaysInSeconds.GetValue(sitePinInfo.SiteNumber),
                    setAsActiveSequence: setAsActiveSequence);
            });
        }

        /// <inheritdoc cref="ConfigureVoltageSequenceWithSourceDelays(DCPowerSessionsBundle, string, double[], double[], int, bool)"/>
        public static void ConfigureVoltageSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            PinSiteData<double[]> sequence,
            PinSiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1,
            bool setAsActiveSequence = false)
        {
            var hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, sequence);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, sourceDelaysInSeconds);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                    sitePinInfo: sitePinInfo,
                    sourceDelay: sourceDelaysInSeconds.GetValue(sitePinInfo),
                    setAsActiveSequence: setAsActiveSequence);
            });
        }

        /// <summary>
        /// Configures a hardware-timed current sequence with per-step source delays.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequenceName">The name of the advanced sequence to create.</param>
        /// <param name="sequence">The current sequence to set.</param>
        /// <param name="sourceDelaysInSeconds">The array of source delays in seconds for each step in the sequence.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="setAsActiveSequence">
        /// If <see langword="true"/>, sets the configured sequence as the active sequence.
        /// If <see langword="false"/> (default), clears the active sequence to allow configuring multiple sequences before initiating.
        /// </param>
        public static void ConfigureCurrentSequenceWithSourceDelays(
           this DCPowerSessionsBundle sessionsBundle,
           string sequenceName,
           double[] sequence,
           double[] sourceDelaysInSeconds,
           int sequenceLoopCount = 1,
           bool setAsActiveSequence = false)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    channelOutput.Control.Abort();
                    channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                    sessionInfo.ConfigureSequenceForCascadingCore(
                        sequenceName: sequenceName,
                        sequence: sequence,
                        sequenceLoopCount: sequenceLoopCount,
                        outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                        sitePinInfo: sitePinInfo,
                        sourceDelay: sourceDelaysInSeconds,
                        setAsActiveSequence: setAsActiveSequence);
                });
            }
            else
            {
                sessionsBundle.Do(sessionInfo =>
                {
                    sessionInfo.AllChannelsOutput.Control.Abort();
                    sessionInfo.AllChannelsOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                    sessionInfo.AllChannelsOutput.ConfigureSequenceCore(
                        sequenceName: sequenceName,
                        sequence: sequence,
                        sequenceLoopCount: sequenceLoopCount,
                        outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                        sourceDelaysInSeconds: sourceDelaysInSeconds,
                        setAsActiveSequence: setAsActiveSequence);
                });
            }
        }

        /// <inheritdoc cref="ConfigureCurrentSequenceWithSourceDelays(DCPowerSessionsBundle, string, double[], double[], int, bool)"/>
        public static void ConfigureCurrentSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            SiteData<double[]> sequence,
            SiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1,
            bool setAsActiveSequence = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo.SiteNumber),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                    sitePinInfo: sitePinInfo,
                    sourceDelay: sourceDelaysInSeconds.GetValue(sitePinInfo.SiteNumber),
                    setAsActiveSequence: setAsActiveSequence);
            });
        }

        /// <inheritdoc cref="ConfigureCurrentSequenceWithSourceDelays(DCPowerSessionsBundle, string, double[], double[], int, bool)"/>
        public static void ConfigureCurrentSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            PinSiteData<double[]> sequence,
            PinSiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1,
            bool setAsActiveSequence = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(sessionsBundle.HasGangedChannels, sourceDelaysInSeconds);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo, out bool isGroupData),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                    sitePinInfo: sitePinInfo,
                    sequenceStepDeltaTimeInSeconds: null,
                    needDataAdjustment: isGroupData,
                    sourceDelay: sourceDelaysInSeconds.GetValue(sitePinInfo),
                    setAsActiveSequence: setAsActiveSequence);
            });
        }

        /// <summary>
        /// Gets the source delay in seconds for each of the underlying device channel(s), per-pin and per-site.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns> The source delay in seconds (<see cref="PinSiteData{T}"/>, where T is of type <see cref="double"/>).</returns>
        public static PinSiteData<double> GetSourceDelayInSeconds(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.DoAndReturnPerSitePerPinResults((sessionInfo, pinSiteInfo) =>
            {
                return sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString].Source.SourceDelay.TotalSeconds;
            });
        }

        /// <summary>
        /// Clears the active advanced sequence for all channels in the specified sessions bundle.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        public static void ClearActiveAdvancedSequence(this DCPowerSessionsBundle sessionsBundle)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Abort();
                sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = string.Empty;
            });
        }

        /// <summary>
        /// Deletes one or more advanced sequences by name from all sessions in the <see cref="DCPowerSessionsBundle"/>.
        /// </summary>
        /// <remarks>
        /// This function will also switch the Source Mode back to SinglePoint.<br/>
        /// Note that you can pass one or more sequence names via the <paramref name="sequenceNames"/> parameter.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequenceNames">The names of the advanced sequences to delete.</param>
        public static void DeleteAdvancedSequence(this DCPowerSessionsBundle sessionsBundle, params string[] sequenceNames)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Abort();
                foreach (string sequenceName in sequenceNames)
                {
                    sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.DeleteAdvancedSequence(sequenceName);
                }
                sessionInfo.AllChannelsOutput.Source.Mode = DCPowerSourceMode.SinglePoint;
            });
        }

        #endregion methods on DCPowerSessionsBundle

        #region methods on DCPowerOutput

        /// <summary>
        /// Configures the current limit.
        /// </summary>
        /// <param name="output">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="currentLimit">The current limit to set.</param>
        /// <param name="currentLimitRange">The current limit range to set. Use the absolute value of current limit to set current limit range when this parameter is not specified.</param>
        public static void ConfigureCurrentLimit(this DCPowerOutput output, double currentLimit, double? currentLimitRange = null)
        {
            output.Source.Voltage.CurrentLimit = currentLimit;
            output.Source.Voltage.CurrentLimitRange = currentLimitRange ?? Math.Abs(currentLimit);
        }

        /// <summary>
        /// Configures a hardware-timed sequence of values.
        /// </summary>
        /// <param name="output">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="sequence">The voltage or current sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="sourceDelaysInSeconds">The array of source delays in seconds for each step in the sequence.</param>
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        /// <param name="sitePinInfo">The <see cref="SitePinInfo"/> object.</param>
        /// <param name="needDataAdjustment">Indicates if the sequence values should be divided in case of Ganging.</param>
        [Obsolete("Using both simple sequencing and advanced sequencing for the same channel within the same session is not supported. For this reason it is better to just use advanced sequencing. Consider using the high-level ConfigureVoltageSequence or ConfigureCurrentSequence methods instead.", error: false)]
        public static void ConfigureSequence(
            this DCPowerOutput output,
            double[] sequence,
            int sequenceLoopCount,
            double? sequenceStepDeltaTimeInSeconds = null,
            double[] sourceDelaysInSeconds = null,
            SitePinInfo sitePinInfo = null,
            bool needDataAdjustment = true)
        {
            ValidateChannelOutputAndSitePinInfoPair(sitePinInfo, output.Name);
            var outputFunction = output.Source.Output.Function;
            sequence = DivideSequenceForCascading(outputFunction, sitePinInfo, needDataAdjustment, sequence);
            output.Source.Mode = DCPowerSourceMode.Sequence;
            output.Source.SequenceLoopCount = sequenceLoopCount;
            var sourceDelays = sourceDelaysInSeconds?.Select(d => PrecisionTimeSpan.FromSeconds(d)).ToArray();
            if (sourceDelays != null)
            {
                output.Source.SetSequence(sequence, sourceDelays);
            }
            else
            {
                output.Source.SetSequence(sequence);
            }

            output.ConfigureSourceTriggerForCascading(sitePinInfo);
            output.ConfigureStartTriggerForCascadedSequencing(sitePinInfo);
            if (sequenceStepDeltaTimeInSeconds.HasValue)
            {
                output.Source.SequenceStepDeltaTimeEnabled = true;
                output.Source.SequenceStepDeltaTime = PrecisionTimeSpan.FromSeconds(sequenceStepDeltaTimeInSeconds.Value);
            }
        }

        #endregion methods on DCPowerOutput

        #region methods on NIDCPower session

        /// <summary>
        /// Configures the transient response.
        /// </summary>
        /// <param name="session">The <see cref="NIDCPower"/> object.</param>
        /// <param name="channelString">The channel string.</param>
        /// <param name="modelString">The DCPower instrument model <see cref="DCPowerModelStrings"/>.</param>
        /// <param name="transientResponse">The transient response to set.</param>
        public static void ConfigureTransientResponse(this NIDCPower session, string channelString, string modelString, DCPowerSourceTransientResponse transientResponse)
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

        #endregion methods on NIDCPower session

        #region methods on DCPowerSessionInformation

        /// <summary>
        /// Configures <see cref="DCPowerSourceSettings"/>.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="settings">The source settings to configure.</param>
        /// <param name="channelString">The channel string. Empty string means all channels in the session.</param>
        public static void ConfigureSourceSettings(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, string channelString = "")
        {
            var channelOutput = string.IsNullOrEmpty(channelString) ? sessionInfo.AllChannelsOutput : sessionInfo.Session.Outputs[channelString];
            sessionInfo.ConfigureSourceSettings(settings, channelOutput, sitePinInfo: null);
        }

        /// <summary>
        /// Configures <see cref="DCPowerSourceSettings"/>.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="settings">The source settings to configure.</param>
        /// <param name="channelOutput">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="sitePinInfo">The <see cref="SitePinInfo"/> object.</param>
        /// <param name="needDataAdjustment">Indicates whether the provided current limit and range values should be divided equally to each individual pin within a ganged group. Set this to false to apply pin-specific values to pins in a ganged group, or true to apply values at the group level.</param>
        public static void ConfigureSourceSettings(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, DCPowerOutput channelOutput, SitePinInfo sitePinInfo, bool needDataAdjustment = true)
        {
            string channelString = string.IsNullOrEmpty(channelOutput.Name) ? sessionInfo.AllChannelsString : channelOutput.Name;
            ValidateChannelOutputAndSitePinInfoPair(sitePinInfo, channelString);

            channelOutput.Source.Mode = DCPowerSourceMode.SinglePoint;
            if (settings.SourceDelayInSeconds.HasValue)
            {
                channelOutput.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(settings.SourceDelayInSeconds.Value);
            }

            sessionInfo.ConfigureTransientResponce(settings, channelString);
            if (sessionInfo.HasGangedChannels)
            {
                var sitePinInfoList = sitePinInfo != null ? new List<SitePinInfo>() { sitePinInfo } : sessionInfo.AssociatedSitePinList.Where(sitePin => channelString.Contains(sitePin.IndividualChannelString));
                Parallel.ForEach(sitePinInfoList, sitePin =>
                {
                    channelOutput.ConfigureLevelsAndLimits(settings, sitePin, needDataAdjustment);
                    channelOutput.ConfigureSourceTriggerForCascading(sitePin);
                });
            }
            else
            {
                channelOutput.ConfigureLevelsAndLimits(settings);
            }
        }

        #endregion methods on DCPowerSessionInformation

        #region private and internal methods

        private static void ConfigureTriggersForCascadedSequencing(this DCPowerSessionInformation sessionInfo, SitePinInfo sitePinInfo)
        {
            var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
            output.ConfigureSourceTriggerForCascading(sitePinInfo);
            output.ConfigureStartTriggerForCascadedSequencing(sitePinInfo);
            sessionInfo.ConfigureMeasureWhen(sitePinInfo, sitePinInfo.ModelString, measureWhen: null);
            sessionInfo.ConfigureMeasureTriggerForCascading(sitePinInfo);
        }

        private static double[] DivideSequenceForCascading(DCPowerSourceOutputFunction outputFunction, SitePinInfo sitePinInfo, bool needDataAdjustment, double[] sequence)
        {
            if (outputFunction == DCPowerSourceOutputFunction.DCCurrent && needDataAdjustment && sitePinInfo?.CascadingInfo is GangingInfo gangingInfo)
            {
                sequence = sequence.Select(level => level / gangingInfo.ChannelsCount).ToArray();
            }
            return sequence;
        }

        private static DCPowerAdvancedSequenceProperty[] GetAdvancedSequencePropertiesToConfigure(IEnumerable<DCPowerAdvancedSequenceStepProperties> perStepProperties)
        {
            var result = new HashSet<DCPowerAdvancedSequenceProperty>();
            foreach (var stepProperties in perStepProperties)
            {
                foreach (var (property, enumValue) in Utilities.GetPropertyMappingsCache())
                {
                    if (property.GetValue(stepProperties) != null)
                    {
                        result.Add(enumValue);
                    }
                }
            }

            return result.ToArray();
        }

        private static void ConfigureTransientResponce(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, string channelString = "")
        {
            if (settings.TransientResponse.HasValue)
            {
                string channelStringToUse = string.IsNullOrEmpty(channelString) ? sessionInfo.AllChannelsString : channelString;
                if (sessionInfo.AllInstrumentsAreTheSameModel)
                {
                    sessionInfo.Session.ConfigureTransientResponse(channelStringToUse, sessionInfo.ModelString, settings.TransientResponse.Value);
                }
                else
                {
                    foreach (var sitePinInfo in sessionInfo.AssociatedSitePinList.Where(sitePin => channelStringToUse.Contains(sitePin.IndividualChannelString)))
                    {
                        sessionInfo.Session.ConfigureTransientResponse(sitePinInfo.IndividualChannelString, sitePinInfo.ModelString, settings.TransientResponse.Value);
                    }
                }
            }
        }

        private static void ConfigureLevelsAndLimits(this DCPowerOutput channelOutput, DCPowerSourceSettings settings, SitePinInfo sitePinInfo = null, bool needDataAdjustment = true)
        {
            if (settings.LimitSymmetry.HasValue)
            {
                channelOutput.Source.ComplianceLimitSymmetry = settings.LimitSymmetry.Value;
            }
            if (settings.OutputFunction.Equals(DCPowerSourceOutputFunction.DCVoltage))
            {
                ConfigureVoltageSettings(channelOutput, settings, sitePinInfo, needDataAdjustment);
            }
            else if (settings.OutputFunction.Equals(DCPowerSourceOutputFunction.DCCurrent))
            {
                ConfigureCurrentSettings(channelOutput, settings, sitePinInfo, needDataAdjustment);
            }
        }

        private static void Force(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, bool waitForSourceCompletion = false)
        {
            var channelString = sessionInfo.AllChannelsString;
            var channelOutput = sessionInfo.Session.Outputs[channelString];
            sessionInfo.ConfigureChannels(settings, channelOutput, sitePinInfo: null);
            channelOutput.InitiateChannels(waitForSourceCompletion);
        }

        private static void ConfigureChannels(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, DCPowerOutput channelOutput, SitePinInfo sitePinInfo, bool needDataAdjustment = true)
        {
            channelOutput.Control.Abort();
            sessionInfo.ConfigureSourceSettings(settings, channelOutput, sitePinInfo, needDataAdjustment);
            if (sitePinInfo != null)
            {
                sessionInfo.ConfigureMeasureWhen(sitePinInfo, sitePinInfo.ModelString, measureWhen: null);
                sessionInfo.ConfigureMeasureTriggerForCascading(sitePinInfo);
            }
            else
            {
                var channelString = channelOutput.Name;
                sessionInfo.ConfigureMeasureWhen(channelString, sessionInfo.ModelString, measureWhen: null);
                sessionInfo.ConfigureMeasureTriggerForCascading(channelString);
            }
            channelOutput.Source.Output.Enabled = true;
            channelOutput.Control.Commit();
        }

        private static void InitiateGangedLeaderAndNonGangedChannels(this DCPowerSessionsBundle sessionsBundle, bool waitForSourceCompletion = false, double timeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (!IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    channelOutput.InitiateChannels(waitForSourceCompletion, timeoutInSeconds);
                }
            });
        }

        private static void ConfigureAllChannelsAndInitiateGangedFollowerChannels(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, SitePinInfo sitePinInfo, bool needDataAdjustment = true)
        {
            var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
            sessionInfo.ConfigureChannels(settings, channelOutput, sitePinInfo, needDataAdjustment);
            if (IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
            {
                channelOutput.InitiateChannels();
            }
        }

        private static void InitiateChannels(this DCPowerOutput channelOutput, bool waitForSourceCompletion = false, double timeoutInSeconds = DefaultTimeout)
        {
            channelOutput.Control.Initiate();
            if (waitForSourceCompletion)
            {
                channelOutput.Events.SourceCompleteEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(timeoutInSeconds));
            }
        }

        internal static DCPowerOutput GetPrimaryOutput(this DCPowerSessionsBundle sessionsBundle, string triggerOrEventTypeName, out string terminalName)
        {
            var masterChannelSessionInfo = sessionsBundle.InstrumentSessions.First();
            var masterChannelString = masterChannelSessionInfo.AssociatedSitePinList.First().IndividualChannelString;
            terminalName = masterChannelSessionInfo.BuildTerminalName(masterChannelString, triggerOrEventTypeName);
            return masterChannelSessionInfo.Session.Outputs[masterChannelString];
        }

        private static bool IsPrimaryOutput(int sessionIndex, SitePinInfo sitePinInfo, DCPowerSessionInformation sessionInfo)
        {
            return sessionIndex == 0 && sitePinInfo.IsFirstChannelOfSession(sessionInfo);
        }

        internal static bool IsFirstChannelOfSession(this SitePinInfo sitePinInfo, DCPowerSessionInformation sessionInfo)
        {
            return sessionInfo.AllChannelsString.StartsWith(sitePinInfo.IndividualChannelString, StringComparison.InvariantCulture);
        }

        private static void ConfigureVoltageSettings(DCPowerOutput dcOutput, DCPowerSourceSettings settings, SitePinInfo sitePinInfo, bool needDataAdjustment)
        {
            var currentLimitDivisor = needDataAdjustment && sitePinInfo?.CascadingInfo is GangingInfo gangingInfo ? gangingInfo.ChannelsCount : 1;
            dcOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
            if (settings.Level.HasValue)
            {
                dcOutput.Source.Voltage.VoltageLevel = settings.Level.Value;
            }
            if (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric && settings.Limit.HasValue)
            {
                dcOutput.Source.Voltage.CurrentLimit = settings.Limit.Value / currentLimitDivisor;
            }
            else
            {
                if (settings.LimitHigh.HasValue)
                {
                    dcOutput.Source.Voltage.CurrentLimitHigh = settings.LimitHigh.Value / currentLimitDivisor;
                }
                if (settings.LimitLow.HasValue)
                {
                    dcOutput.Source.Voltage.CurrentLimitLow = settings.LimitLow.Value / currentLimitDivisor;
                }
            }
            if (settings.LevelRange.HasValue || settings.Level.HasValue)
            {
                dcOutput.Source.Voltage.VoltageLevelRange = settings.LevelRange ?? Math.Abs(settings.Level.Value);
            }
            if (settings.LimitRange.HasValue
                || (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric && settings.Limit.HasValue)
                || (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Asymmetric && (settings.LimitHigh.HasValue || settings.LimitLow.HasValue)))
            {
                dcOutput.Source.Voltage.CurrentLimitRange = (settings.LimitRange ?? CalculateLimitRangeFromLimit(settings)) / currentLimitDivisor;
            }
        }

        private static void ConfigureCurrentSettings(DCPowerOutput dcOutput, DCPowerSourceSettings settings, SitePinInfo sitePinInfo, bool needDataAdjustment)
        {
            var currentLevelDivisor = needDataAdjustment && sitePinInfo?.CascadingInfo is GangingInfo gangingInfo ? gangingInfo.ChannelsCount : 1;
            dcOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
            if (settings.Level.HasValue)
            {
                dcOutput.Source.Current.CurrentLevel = settings.Level.Value / currentLevelDivisor;
            }
            if (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric && settings.Limit.HasValue)
            {
                dcOutput.Source.Current.VoltageLimit = settings.Limit.Value;
            }
            else
            {
                if (settings.LimitHigh.HasValue)
                {
                    dcOutput.Source.Current.VoltageLimitHigh = settings.LimitHigh.Value;
                }
                if (settings.LimitLow.HasValue)
                {
                    dcOutput.Source.Current.VoltageLimitLow = settings.LimitLow.Value;
                }
            }
            if (settings.LevelRange.HasValue || settings.Level.HasValue)
            {
                dcOutput.Source.Current.CurrentLevelRange = (settings.LevelRange ?? Math.Abs(settings.Level.Value)) / currentLevelDivisor;
            }
            if (settings.LimitRange.HasValue
                || (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric && settings.Limit.HasValue)
                || (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Asymmetric && (settings.LimitHigh.HasValue || settings.LimitLow.HasValue)))
            {
                dcOutput.Source.Current.VoltageLimitRange = settings.LimitRange ?? CalculateLimitRangeFromLimit(settings);
            }
        }

        private static void ConfigureAdvancedSequenceCore(
            this DCPowerOutput channelOutput,
            string sequenceName,
            string modelString,
            IEnumerable<DCPowerAdvancedSequenceStepProperties> perStepProperties,
            bool setAsActiveSequence,
            bool commitFirstElementAsInitialState)
        {
            channelOutput.Source.Mode = DCPowerSourceMode.Sequence;
            var advancedSequenceProperties = GetAdvancedSequencePropertiesToConfigure(perStepProperties);
            try
            {
                channelOutput.Source.AdvancedSequencing.CreateAdvancedSequence(sequenceName, advancedSequenceProperties, setAsActiveSequence: true);
            }
            catch (Exception ex) when (ex is Ivi.Driver.OperationNotSupportedException operationNotSupported && operationNotSupported.InnerException is Ivi.Driver.IviCDriverException cDriverException && cDriverException.ErrorCode == AttributeIdNotRecognized)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPowerDeviceNotSupported, modelString), ex);
            }
            for (int i = 0; i < perStepProperties.Count(); i++)
            {
                if (i == 0 && commitFirstElementAsInitialState)
                {
                    channelOutput.Source.AdvancedSequencing.CreateAdvancedSequenceCommitStep(true);
                }
                else
                {
                    channelOutput.Source.AdvancedSequencing.CreateAdvancedSequenceStep(true);
                }
                perStepProperties.ElementAt(i).ApplyTo(channelOutput);
            }
            if (!setAsActiveSequence)
            {
                channelOutput.Source.AdvancedSequencing.ActiveAdvancedSequence = string.Empty;
            }
        }

        private static void ConfigureSequenceForCascadingCore(
            this DCPowerSessionInformation sessionInfo,
            string sequenceName,
            double[] sequence,
            int sequenceLoopCount,
            DCPowerSourceOutputFunction outputFunction,
            SitePinInfo sitePinInfo,
            double? sequenceStepDeltaTimeInSeconds = null,
            bool needDataAdjustment = true,
            double[] sourceDelay = null,
            bool setAsActiveSequence = false)
        {
            var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
            ValidateChannelOutputAndSitePinInfoPair(sitePinInfo, output.Name);
            sequence = DivideSequenceForCascading(outputFunction, sitePinInfo, needDataAdjustment, sequence);
            output.ConfigureSequenceCore(
                sequenceName: sequenceName,
                sequence: sequence,
                sequenceLoopCount: sequenceLoopCount,
                outputFunction: outputFunction,
                sequenceStepDeltaTimeInSeconds: sequenceStepDeltaTimeInSeconds,
                sourceDelaysInSeconds: sourceDelay,
                setAsActiveSequence: setAsActiveSequence);
            sessionInfo.ConfigureTriggersForCascadedSequencing(sitePinInfo);
        }

        private static void ConfigureSequenceCore(
            this DCPowerOutput output,
            string sequenceName,
            double[] sequence,
            int sequenceLoopCount,
            DCPowerSourceOutputFunction outputFunction,
            double? sequenceStepDeltaTimeInSeconds = null,
            double[] sourceDelaysInSeconds = null,
            bool setAsActiveSequence = false)
        {
            output.Source.Mode = DCPowerSourceMode.Sequence;
            output.Source.SequenceLoopCount = sequenceLoopCount;

            output.SetAdvancedSequence(
                advancedSequenceName: sequenceName,
                sequence: sequence,
                outputFunction: outputFunction,
                sourceDelay: sourceDelaysInSeconds,
                setAsActiveSequence: setAsActiveSequence);

            if (sequenceStepDeltaTimeInSeconds.HasValue)
            {
                output.Source.SequenceStepDeltaTimeEnabled = true;
                output.Source.SequenceStepDeltaTime = PrecisionTimeSpan.FromSeconds(sequenceStepDeltaTimeInSeconds.Value);
            }
        }

        private static void SetAdvancedSequence(this DCPowerOutput output, string advancedSequenceName, double[] sequence, DCPowerSourceOutputFunction outputFunction, double[] sourceDelay = null, bool setAsActiveSequence = false)
        {
            List<DCPowerAdvancedSequenceProperty> advancedSequenceProperties = new List<DCPowerAdvancedSequenceProperty>();

            Action<double> setLevel = null;
            if (outputFunction == DCPowerSourceOutputFunction.DCVoltage)
            {
                advancedSequenceProperties.Add(DCPowerAdvancedSequenceProperty.VoltageLevel);
                setLevel = value => output.Source.Voltage.VoltageLevel = value;
            }
            else if (outputFunction == DCPowerSourceOutputFunction.DCCurrent)
            {
                advancedSequenceProperties.Add(DCPowerAdvancedSequenceProperty.CurrentLevel);
                setLevel = value => output.Source.Current.CurrentLevel = value;
            }

            bool hasSourceDelay = sourceDelay != null;
            if (hasSourceDelay)
            {
                advancedSequenceProperties.Add(DCPowerAdvancedSequenceProperty.SourceDelay);
            }

            output.Source.AdvancedSequencing.CreateAdvancedSequence(advancedSequenceName, advancedSequenceProperties.ToArray(), setAsActiveSequence: true);

            for (int i = 0; i < sequence.Length; i++)
            {
                output.Source.AdvancedSequencing.CreateAdvancedSequenceStep(true);
                setLevel(sequence[i]);

                if (hasSourceDelay)
                {
                    output.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(sourceDelay[i]);
                }
            }
            if (!setAsActiveSequence)
            {
                output.Source.AdvancedSequencing.ActiveAdvancedSequence = string.Empty;
            }
        }

        /// <summary>
        /// Core implementation for forcing a current/voltage sequence.
        /// </summary>
        private static void ForceSequenceCore(
            this DCPowerOutput channelOutput,
            DCPowerSourceSettings settings,
            string sequenceName,
            double[] levelSequence,
            int sequenceLoopCount,
            bool waitForSequenceCompletion,
            double sequenceTimeoutInSeconds,
            bool setAsActiveSequence)
        {
            channelOutput.Control.Abort();
            channelOutput.ConfigureLevelsAndLimits(settings);
            channelOutput.ConfigureSequenceCore(
                sequenceName: sequenceName,
                sequence: levelSequence,
                sequenceLoopCount: sequenceLoopCount,
                outputFunction: (DCPowerSourceOutputFunction)settings.OutputFunction,
                setAsActiveSequence: setAsActiveSequence);

            channelOutput.InitiateChannels(waitForSequenceCompletion, sequenceTimeoutInSeconds);
        }

        private static double CalculateLimitRangeFromLimit(DCPowerSourceSettings settings)
        {
            return settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric
                ? Math.Abs(settings.Limit.Value)
                : Math.Max(Math.Abs(settings.LimitHigh.Value), Math.Abs(settings.LimitLow.Value));
        }

        internal static IEnumerable<DCPowerAdvancedSequenceStepProperties> GetValidProperties<T>(T[] dcPowerSettings) where T : class
        {
            ValidateAdvancedSequenceProperties(dcPowerSettings);
            foreach (var dcPowerSetting in dcPowerSettings)
            {
                if (dcPowerSetting is DCPowerSourceSettings sourceSetting)
                {
                    var dcPowerAdvancedSequenceStepProperties = new DCPowerAdvancedSequenceStepProperties
                    {
                        OutputFunction = sourceSetting.OutputFunction,
                        TransientResponse = sourceSetting.TransientResponse,
                        SourceDelay = sourceSetting.SourceDelayInSeconds
                    };
                    if (sourceSetting.OutputFunction == DCPowerSourceOutputFunction.DCVoltage)
                    {
                        dcPowerAdvancedSequenceStepProperties.VoltageLevel = sourceSetting.Level;
                        dcPowerAdvancedSequenceStepProperties.VoltageLevelRange = sourceSetting.LevelRange;
                        dcPowerAdvancedSequenceStepProperties.CurrentLimit = sourceSetting.Limit;
                        dcPowerAdvancedSequenceStepProperties.CurrentLimitHigh = sourceSetting.LimitHigh;
                        dcPowerAdvancedSequenceStepProperties.CurrentLimitLow = sourceSetting.LimitLow;
                        dcPowerAdvancedSequenceStepProperties.CurrentLimitRange = sourceSetting.LimitRange;
                    }
                    else if (sourceSetting.OutputFunction == DCPowerSourceOutputFunction.DCCurrent)
                    {
                        dcPowerAdvancedSequenceStepProperties.CurrentLevel = sourceSetting.Level;
                        dcPowerAdvancedSequenceStepProperties.CurrentLevelRange = sourceSetting.LevelRange;
                        dcPowerAdvancedSequenceStepProperties.VoltageLimit = sourceSetting.Limit;
                        dcPowerAdvancedSequenceStepProperties.VoltageLimitHigh = sourceSetting.LimitHigh;
                        dcPowerAdvancedSequenceStepProperties.VoltageLimitLow = sourceSetting.LimitLow;
                        dcPowerAdvancedSequenceStepProperties.VoltageLimitRange = sourceSetting.LimitRange;
                    }
                    yield return dcPowerAdvancedSequenceStepProperties;
                }
                else if (dcPowerSetting is DCPowerAdvancedSequenceStepProperties advancedProperties)
                {
                    yield return advancedProperties;
                }
            }
        }

        private static void ValidateAdvancedSequenceProperties<T>(T[] sequenceProperties) where T : class
        {
            var properties = typeof(T).GetProperties();

            var invalidProperties = new List<string>();

            foreach (var property in properties)
            {
                int state = 0; // 1 => when value is null, 2 =>when value is non-null; 3 => both (mixed)

                foreach (var setting in sequenceProperties)
                {
                    // below is the bitwise OR operation to set the state variable as 0|1 = 1, 0|2 = 2, 1|2 =3
                    state |= (property.GetValue(setting) is null) ? 1 : 2;

                    if (state == 3)
                    {
                        invalidProperties.Add(property.Name);
                        break;
                    }
                }
            }

            if (invalidProperties.Count > 0)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_InconsistentAdvancedSequenceProperties, string.Join(", ", invalidProperties.Select(p => $"\"{p}\""))));
            }
        }

        private static void ValidateChannelOutputAndSitePinInfoPair(SitePinInfo sitePinInfo, string channelString)
        {
            if (sitePinInfo != null && channelString.Split(',').Length > 1)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_MultipleChannelOutputsDetected, channelString));
            }
        }

        private static void ReleaseAdvancedSequenceResources(this DCPowerSessionsBundle sessionsBundle, string advancedSequenceName)
        {
            // Clearing the active advanced sequence after use.
            sessionsBundle.ClearActiveAdvancedSequence();
            // Deleting the advanced sequence after use to free up available sequences (limited to 100 per session).
            sessionsBundle.DeleteAdvancedSequence(advancedSequenceName);

            // Since ganged pins use the start trigger when operating in Sequence mode,
            // and DeleteAdvancedSequence method sets the Source Mode back to SinglePoint mode,
            // their start trigger must be disabled before any subsequent SinglePoint operations can be performed.
            // Also, since an Abort operation is preformed within DeleteAdvancedSequence
            // an Abort is not required before disabling the start trigger.
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (sitePinInfo?.CascadingInfo is GangingInfo)
                {
                    var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    output.Triggers.StartTrigger.Disable();
                }
            });
        }

        private static void ReleaseSynchronizedAdvancedSequenceResources(this DCPowerSessionsBundle sessionsBundle, string advancedSequenceName)
        {
            // Clearing the active advanced sequence after use.
            sessionsBundle.ClearActiveAdvancedSequence();
            // Deleting the advanced sequence after use to free up available sequences (limited to 100 per session).
            sessionsBundle.DeleteAdvancedSequence(advancedSequenceName);

            // The start trigger must be set to None before any subsequent SinglePoint operations can be performed.
            sessionsBundle.DisableTriggers(new[] { TriggerType.StartTrigger });
        }
        #endregion private and internal methods
    }
}
