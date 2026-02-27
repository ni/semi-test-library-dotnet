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

        private const double DefaultSequenceTimeout = 5.0;
        private const int AttributeIdNotRecognized = -1074135028;

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
        public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, SiteData<DCPowerSourceSettings> settings, bool applyToIndividualPins = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings.GetValue(sitePinInfo.SiteNumber), channelOutput, sitePinInfo, applyToIndividualPins);
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
                sessionInfo.ConfigureSourceSettings(settings.GetValue(sitePinInfo, out bool isGroupdata), channelOutput, sitePinInfo, !isGroupdata);
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
                var perPinSettings = settings.GetValue(sitePinInfo, out bool isGroupData);
                sessionInfo.ConfigureSourceSettings(perPinSettings, channelOutput, sitePinInfo, !isGroupData);
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
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
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
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, applyToIndividualPins: false);
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
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, applyToIndividualPins: false);
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
        /// <param name="applyToIndividualPins">Indicates whether the provided current limit and current range are intended to be applied to individual pins within a ganged group (cascading pin data), which affects how channels are configured and initiated for ganged groups. This should be set to true when providing pin-specific current limits in a scenario where some of the targeted pins are part of a ganged group, and false otherwise.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, PinSiteData<double> voltageLevels, double? currentLimit = null, double? voltageLevelRange = null, double? currentLimitRange = null, bool waitForSourceCompletion = false, bool applyToIndividualPins = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
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
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, applyToIndividualPins);
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
        /// <param name="applyToIndividualPins">Indicates whether the provided settings are intended to be applied to individual pins within a ganged group (cascading pin data), which affects how channels are configured and initiated for ganged groups. This should be set to true when providing pin-unique settings in a scenario where some of the targeted pins are part of a ganged group, and false when providing site-unique or uniform settings for pins that may be in a ganged group.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, DCPowerSourceSettings settings, bool waitForSourceCompletion = false, bool applyToIndividualPins = false)
        {
            settings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, applyToIndividualPins);
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
        /// <param name="applyToIndividualPins">Indicates whether the provided settings are intended to be applied to individual pins within a ganged group (cascading pin data), which affects how channels are configured and initiated for ganged groups. This should be set to true when providing pin-unique settings in a scenario where some of the targeted pins are part of a ganged group, and false when providing site-unique or uniform settings for pins that may be in a ganged group.</param>
        public static void ForceVoltage(this DCPowerSessionsBundle sessionsBundle, SiteData<DCPowerSourceSettings> settings, bool waitForSourceCompletion = false, bool applyToIndividualPins = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSiteSettings = settings.GetValue(sitePinInfo.SiteNumber);
                perSiteSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perSiteSettings, sitePinInfo, applyToIndividualPins);
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
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perPinSettings = settings.GetValue(sitePinInfo, out bool isGroupData);
                perPinSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perPinSettings, sitePinInfo, !isGroupData);
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
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSitePinPairSettings = settings.GetValue(sitePinInfo, out bool isGroupData);
                perSitePinPairSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perSitePinPairSettings, sitePinInfo, !isGroupData);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of voltage values on the targeted pin(s).
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageSequence">The array of voltage values to force in sequence.</param>
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
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                ForceSequenceCore(
                    sessionInfo.AllChannelsOutput,
                    DCPowerSourceOutputFunction.DCVoltage,
                    voltageSequence,
                    currentLimit,
                    voltageLevelRange,
                    currentLimitRange,
                    sequenceLoopCount,
                    waitForSequenceCompletion,
                    sequenceTimeoutInSeconds);
            });
        }

        /// <inheritdoc cref="ForceVoltageSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/>
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var sequence = voltageSequence.GetValue(pinSiteInfo.SiteNumber);
                var channelOutput = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];

                ForceSequenceCore(
                    channelOutput,
                    DCPowerSourceOutputFunction.DCVoltage,
                    sequence,
                    currentLimit,
                    voltageLevelRange,
                    currentLimitRange,
                    sequenceLoopCount,
                    waitForSequenceCompletion,
                    sequenceTimeoutInSeconds);
            });
        }

        /// <inheritdoc cref="ForceVoltageSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/>
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            sessionsBundle.Do((sessionInfo, pinSiteInfo) =>
            {
                var sequence = voltageSequence.GetValue(pinSiteInfo);
                var channelOutput = sessionInfo.Session.Outputs[pinSiteInfo.IndividualChannelString];

                ForceSequenceCore(
                    channelOutput,
                    DCPowerSourceOutputFunction.DCVoltage,
                    sequence,
                    currentLimit,
                    voltageLevelRange,
                    currentLimitRange,
                    sequenceLoopCount,
                    waitForSequenceCompletion,
                    sequenceTimeoutInSeconds);
            });
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
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageSequence">The voltage sequence to force for all site-pin pairs.</param>
        /// <param name="currentLimit">Current limit for the sequence.</param>
        /// <param name="voltageLevelRange">Voltage level range.</param>
        /// <param name="currentLimitRange">Current limit range.</param>
        /// <param name="sourceDelayinSeconds">Optional source delay to use uniformly for synchronization.</param>
        /// <param name="transientResponse">Transient response.</param>
        /// <param name="sequenceLoopCount">The number of times to force the sequence.</param>
        /// <param name="waitForSequenceCompletion">True to block until the sequence engine completes (waits on SequenceEngineDone event); false to return immediately.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion when <paramref name="waitForSequenceCompletion"/> is true.</param>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            double[] voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            double? sourceDelayinSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
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
                sourceDelayinSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceVoltageSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> voltageSequences,
            SiteData<double> currentLimits = null,
            SiteData<double> voltageLevelRanges = null,
            SiteData<double> currentLimitRanges = null,
            double? sourceDelayinSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            SequenceProvider<double> getVoltageSequenceForSite = sitePinInfo => voltageSequences?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getCurrentLimitForSite = sitePinInfo => currentLimits?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getVoltageLevelRangeForSite = sitePinInfo => voltageLevelRanges?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getCurrentLimitRangeForSite = sitePinInfo => currentLimitRanges?.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceSequenceSynchronizedCore(
                getVoltageSequenceForSite,
                DCPowerSourceOutputFunction.DCVoltage,
                getCurrentLimitForSite,
                getVoltageLevelRangeForSite,
                getCurrentLimitRangeForSite,
                sourceDelayinSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceVoltageSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequences,
            PinSiteData<double> currentLimits = null,
            PinSiteData<double> voltageLevelRanges = null,
            PinSiteData<double> currentLimitRanges = null,
            double? sourceDelayinSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            SequenceProvider<double> getVoltageSequenceForSitePin = sitePinInfo => voltageSequences?.GetValue(sitePinInfo);
            ValueProvider getCurrentLimitForSitePin = sitePinInfo => currentLimits?.GetValue(sitePinInfo);
            ValueProvider getVoltageLevelRangeForSitePin = sitePinInfo => voltageLevelRanges?.GetValue(sitePinInfo);
            ValueProvider getCurrentLimitRangeForSitePin = sitePinInfo => currentLimitRanges?.GetValue(sitePinInfo);

            sessionsBundle.ForceSequenceSynchronizedCore(
                getVoltageSequenceForSitePin,
                DCPowerSourceOutputFunction.DCVoltage,
                getCurrentLimitForSitePin,
                getVoltageLevelRangeForSitePin,
                getCurrentLimitRangeForSitePin,
                sourceDelayinSeconds,
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
                var currentLevel = currentLevels.GetValue(sitePinInfo, out bool isGroupData);
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = currentLevel,
                    Limit = voltageLimit,
                    LevelRange = currentLevelRange,
                    LimitRange = voltageLimitRange
                };
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, !isGroupData);
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
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, applyToIndividualPins: false);
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
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, !isGroupData);
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
        /// <param name="applyToIndividualPins">Set this to true if the settings contains Data for Pin which belongs to cascaded channels. This will ensure the method treats the settings as pin-unique when configuring channels of ganged groups, which is necessary to properly handle scenarios where pins within a ganged group have different settings provided through pin-unique data.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, DCPowerSourceSettings settings, bool waitForSourceCompletion = false, bool applyToIndividualPins = false)
        {
            settings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
                    sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(settings, sitePinInfo, applyToIndividualPins);
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
        /// <param name="applyToIndividualPins">Set this to true if the settings contains Data for Pin which belongs to cascaded channels. This will ensure the method treats the settings as pin-unique when configuring channels of ganged groups, which is necessary to properly handle scenarios where pins within a ganged group have different settings provided through pin-unique data.</param>
        public static void ForceCurrent(this DCPowerSessionsBundle sessionsBundle, SiteData<DCPowerSourceSettings> settings, bool waitForSourceCompletion = false, bool applyToIndividualPins = false)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSiteSettings = settings.GetValue(sitePinInfo.SiteNumber);
                perSiteSettings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perSiteSettings, sitePinInfo, applyToIndividualPins);
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
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perPinSettings, sitePinInfo, !isGroupData);
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
                sessionInfo.ConfigureAllChannelsAndInitiateGangedFollowerChannels(perSitePinPairSettings, sitePinInfo, !isGroupData);
            });
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of current outputs, ensuring synchronized output across all specified target pins.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentSequence">The current sequence to force for all site-pin pairs.</param>
        /// <param name="voltageLimit">Voltage limit for the sequence.</param>
        /// <param name="currentLevelRange">Current level range.</param>
        /// <param name="voltageLimitRange">Voltage limit range.</param>
        /// <param name="sourceDelayinSeconds">Optional source delay to use uniformly for synchronization.</param>
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
            double? sourceDelayinSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
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
                sourceDelayinSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceCurrentSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> currentSequences,
            SiteData<double> voltageLimits = null,
            SiteData<double> currentLevelRanges = null,
            SiteData<double> voltageLimitRanges = null,
            double? sourceDelayinSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            SequenceProvider<double> getCurrentSequenceForSite = sitePinInfo => currentSequences.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getVoltageLimitForSite = sitePinInfo => voltageLimits?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getCurrentLevelRangeForSite = sitePinInfo => currentLevelRanges?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getVoltageLimitRangeForSite = sitePinInfo => voltageLimitRanges?.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceSequenceSynchronizedCore(
                getCurrentSequenceForSite,
                DCPowerSourceOutputFunction.DCCurrent,
                getVoltageLimitForSite,
                getCurrentLevelRangeForSite,
                getVoltageLimitRangeForSite,
                sourceDelayinSeconds,
                transientResponse,
                sequenceLoopCount,
                waitForSequenceCompletion,
                sequenceTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceCurrentSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> currentSequences,
            PinSiteData<double> voltageLimits = null,
            PinSiteData<double> currentLevelRanges = null,
            PinSiteData<double> voltageLimitRanges = null,
            double? sourceDelayinSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            SequenceProvider<double> getCurrentSequenceForSitePin = sitePinInfo => currentSequences.GetValue(sitePinInfo);
            ValueProvider getVoltageLimitForSitePin = sitePinInfo => voltageLimits?.GetValue(sitePinInfo);
            ValueProvider getCurrentLevelRangeForSitePin = sitePinInfo => currentLevelRanges?.GetValue(sitePinInfo);
            ValueProvider getVoltageLimitRangeForSitePin = sitePinInfo => voltageLimitRanges?.GetValue(sitePinInfo);

            sessionsBundle.ForceSequenceSynchronizedCore(
                getCurrentSequenceForSitePin,
                DCPowerSourceOutputFunction.DCCurrent,
                getVoltageLimitForSitePin,
                getCurrentLevelRangeForSitePin,
                getVoltageLimitRangeForSitePin,
                sourceDelayinSeconds,
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
            var masterChannelOutput = sessionsBundle.GetPrimaryOutput(TriggerType.StartTrigger.ToString(), out string startTrigger);

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
                channelOutput.ConfigureSequence(fetchLevelSequence(sitePinInfo), sequenceLoopCount);
                channelOutput.ConfigureLevelsAndLimits(settings);
                channelOutput.Source.SourceDelay = sourceDelayInSeconds.HasValue
                    ? PrecisionTimeSpan.FromSeconds(sourceDelayInSeconds.Value)
                    : PrecisionTimeSpan.Zero;
                sessionInfo.ConfigureTransientResponce(settings, perChannelString);

                if (sessionIndex == 0 && sitePinInfo.IsFirstChannelOfSession(sessionInfo))
                {
                    // Master channel does not need a start trigger
                    channelOutput.Triggers.StartTrigger.Disable();
                    channelOutput.Control.Commit();
                }
                else
                {
                    // Slave channels start on master's start trigger
                    channelOutput.Triggers.StartTrigger.DigitalEdge.Configure(startTrigger, DCPowerTriggerEdge.Rising);
                    channelOutput.Control.Initiate();
                }
            });

            // Start master
            masterChannelOutput.Control.Initiate();

            if (waitForSequenceCompletion)
            {
                masterChannelOutput.Events.SequenceEngineDoneEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(sequenceTimeoutInSeconds));
            }
        }

        /// <summary>
        /// Synchronizes and forces an advanced sequence across all sessions in the bundle.
        /// </summary>
        /// <param name="sessionsBundle">The bundle of DC power sessions to synchronize.</param>
        /// <param name="sequence">The sequence of voltage source settings to apply.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the voltage sequence.</param>
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

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double)"/>
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

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double)"/>
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
        /// <param name="sessionsBundle">The bundle of DC power sessions to synchronize.</param>
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
        /// <param name="sessionsBundle">The bundle of DC power sessions to synchronize.</param>
        /// <param name="sequence">The sequence of voltage source settings to apply.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the voltage sequence.</param>
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

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double)"/>
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

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double)"/>
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
        /// <param name="sessionsBundle">The bundle of DC power sessions to synchronize.</param>
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
            var masterChannelOutput = sessionsBundle.GetPrimaryOutput(TriggerType.StartTrigger.ToString(), out string startTrigger);
            var sequenceName = $"STL_AdvSeq_{DateTime.UtcNow.Ticks}_{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(0, 8)}";
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
                ConfigureAdvanceSequenceCore(sequenceName, channelOutput, sitePinInfo.ModelString, validProperties, setAsActiveSequence: true, commitFirstElementAsInitialState: false);
                if (sessionIndex == 0 && sitePinInfo.IsFirstChannelOfSession(sessionInfo))
                {
                    channelOutput.Triggers.StartTrigger.Disable();
                    channelOutput.Control.Commit();
                }
                else
                {
                    // Slave channels start on master's start trigger
                    channelOutput.Triggers.StartTrigger.DigitalEdge.Configure(startTrigger, DCPowerTriggerEdge.Rising);
                    channelOutput.Control.Initiate();
                }
            });

            masterChannelOutput.Control.Initiate();

            if (waitForSequenceCompletion)
            {
                masterChannelOutput.Events.SequenceEngineDoneEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(sequenceTimeoutInSeconds));
            }

            if (fetchResult)
            {
                result = sessionsBundle.FetchMeasurement(pointsToFetch.Value, measurementTimeoutInSeconds);
            }

            // deleting the advanced sequence after use
            sessionsBundle.DeleteAdvancedSequence(sequenceName);

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
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentSequence">Array of current levels to source step-by-step.</param>
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
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                ForceSequenceCore(
                     sessionInfo.AllChannelsOutput,
                     DCPowerSourceOutputFunction.DCCurrent,
                     currentSequence,
                     voltageLimit,
                     currentLevelRange,
                     voltageLimitRange,
                     sequenceLoopCount,
                     waitForSequenceCompletion,
                     sequenceTimeoutInSeconds);
            });
        }

        /// <inheritdoc cref="ForceCurrentSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/>
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> currentSequences,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var sequence = currentSequences.GetValue(sitePinInfo.SiteNumber);
                var channelString = sitePinInfo.IndividualChannelString;
                var channelOutput = sessionInfo.Session.Outputs[channelString];

                ForceSequenceCore(
                    channelOutput,
                    DCPowerSourceOutputFunction.DCCurrent,
                    sequence,
                    voltageLimit,
                    currentLevelRange,
                    voltageLimitRange,
                    sequenceLoopCount,
                    waitForSequenceCompletion,
                    sequenceTimeoutInSeconds);
            });
        }

        /// <inheritdoc cref="ForceCurrentSequence(DCPowerSessionsBundle, double[], double?, double?, double?, int, bool, double)"/>
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> currentSequences,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var sequence = currentSequences.GetValue(sitePinInfo);
                var channelString = sitePinInfo.IndividualChannelString;
                var channelOutput = sessionInfo.Session.Outputs[channelString];

                ForceSequenceCore(
                    channelOutput,
                    DCPowerSourceOutputFunction.DCCurrent,
                    sequence,
                    voltageLimit,
                    currentLevelRange,
                    voltageLimitRange,
                    sequenceLoopCount,
                    waitForSequenceCompletion,
                    sequenceTimeoutInSeconds);
            });
        }

        /// <summary>
        /// Core implementation for forcing a current/voltage sequence.
        /// </summary>
        private static void ForceSequenceCore(
            DCPowerOutput channelOutput,
            DCPowerSourceOutputFunction outputFunction,
            double[] levelSequence,
            double? limit,
            double? levelRange,
            double? limitRange,
            int sequenceLoopCount,
            bool waitForSequenceCompletion,
            double sequenceTimeoutInSeconds)
        {
            var settings = new DCPowerSourceSettings()
            {
                OutputFunction = outputFunction,
                LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                Limit = limit,
                LevelRange = levelRange,
                LimitRange = limitRange
            };

            channelOutput.Control.Abort();
            channelOutput.ConfigureSequence(levelSequence, sequenceLoopCount);
            channelOutput.ConfigureLevelsAndLimits(settings);
            channelOutput.InitiateChannels(waitForSequenceCompletion, sequenceTimeoutInSeconds);
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
        public static void ConfigureSequence(this DCPowerSessionsBundle sessionsBundle, double[] sequence, int sequenceLoopCount, double? sequenceStepDeltaTimeInSeconds = null)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.AllChannelsOutput.ConfigureSequence(sequence, sequenceLoopCount, sequenceStepDeltaTimeInSeconds);
            });
        }

        /// <inheritdoc cref="ConfigureSequence(DCPowerSessionsBundle, double[], int, double?)"/>
        public static void ConfigureSequence(this DCPowerSessionsBundle sessionsBundle, SiteData<double[]> sequence, int sequenceLoopCount = 1, double? sequenceStepDeltaTimeInSeconds = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.ConfigureSequence(sequence.GetValue(sitePinInfo.SiteNumber), sequenceLoopCount, sequenceStepDeltaTimeInSeconds);
            });
        }

        /// <inheritdoc cref="ConfigureSequence(DCPowerSessionsBundle, double[], int, double?)"/>
        public static void ConfigureSequence(this DCPowerSessionsBundle sessionsBundle, PinSiteData<double[]> sequence, int sequenceLoopCount = 1, double? sequenceStepDeltaTimeInSeconds = null)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.ConfigureSequence(sequence.GetValue(sitePinInfo), sequenceLoopCount, sequenceStepDeltaTimeInSeconds);
            });
        }

        /// <summary>
        /// Creates and configures an advanced sequence with per-step property configurations.
        /// </summary>
        /// <param name="sessionsBundle">The DCPower sessions bundle.</param>
        /// <param name="sequenceName">The name of the advanced sequence to create.</param>
        /// <param name="perStepProperties">A list of property configurations for each step in the sequence.</param>
        /// <param name="setAsActiveSequence">If true, leaves the sequence active after configuration. If false (default), clears the active sequence to allow configuring multiple sequences without activation. Default is false.</param>
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
                ConfigureAdvanceSequenceCore(sequenceName, channelOutput, sitePinInfo.ModelString, perStepProperties, setAsActiveSequence, commitFirstElementAsInitialState);
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
                ConfigureAdvanceSequenceCore(sequenceName, channelOutput, sitePinInfo.ModelString, stepProperties, setAsActiveSequence, commitFirstElementAsInitialState);
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
                ConfigureAdvanceSequenceCore(sequenceName, channelOutput, sitePinInfo.ModelString, stepProperties, setAsActiveSequence, commitFirstElementAsInitialState);
            });
        }

        private static void ConfigureAdvanceSequenceCore(
            string sequenceName,
            DCPowerOutput channelOutput,
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
        /// Configures a hardware-timed sequence of values with per-step source delays.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The voltage or current sequence to set.</param>
        /// <param name="sourceDelaysInSeconds">The array of source delays in seconds for each step in the sequence.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        public static void ConfigureSequenceWithSourceDelays(
           this DCPowerSessionsBundle sessionsBundle,
           double[] sequence,
           double[] sourceDelaysInSeconds,
           int sequenceLoopCount = 1)
        {
            sessionsBundle.Do(sessinInfo =>
            {
                sessinInfo.AllChannelsOutput.Control.Abort();
                sessinInfo.AllChannelsOutput.ConfigureSequence(sequence, sequenceLoopCount, sourceDelaysInSeconds);
            });
        }

        /// <inheritdoc cref="ConfigureSequenceWithSourceDelays(DCPowerSessionsBundle, double[], double[], int)"/>
        public static void ConfigureSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> sequence,
            SiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.ConfigureSequence(sequence.GetValue(sitePinInfo.SiteNumber), sequenceLoopCount, sourceDelaysInSeconds.GetValue(sitePinInfo.SiteNumber));
            });
        }

        /// <inheritdoc cref="ConfigureSequenceWithSourceDelays(DCPowerSessionsBundle, double[], double[], int)"/>
        public static void ConfigureSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> sequence,
            PinSiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.ConfigureSequence(sequence.GetValue(sitePinInfo), sequenceLoopCount, sourceDelaysInSeconds.GetValue(sitePinInfo));
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
        /// Deletes the advanced sequence with the specified name from all sessions in the <see cref="DCPowerSessionsBundle"/>.
        /// </summary>
        /// <param name = "sessionsBundle" > The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequenceName">The name of the advanced sequence to delete.</param>
        public static void DeleteAdvancedSequence(this DCPowerSessionsBundle sessionsBundle, string sequenceName)
        {
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.AllChannelsOutput.Control.Abort();
                sessionInfo.AllChannelsOutput.Source.AdvancedSequencing.DeleteAdvancedSequence(sequenceName);
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
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        public static void ConfigureSequence(this DCPowerOutput output, double[] sequence, int sequenceLoopCount, double? sequenceStepDeltaTimeInSeconds = null)
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

        /// <summary>
        /// Configures a hardware-timed sequence of values with per-step source delays.
        /// </summary>
        /// <param name="output">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="sequence">The voltage or current sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="sourceDelaysInSeconds">The array of source delays in seconds for each step in the sequence.</param>
        public static void ConfigureSequence(this DCPowerOutput output, double[] sequence, int sequenceLoopCount, double[] sourceDelaysInSeconds)
        {
            output.Source.Mode = DCPowerSourceMode.Sequence;
            output.Source.SequenceLoopCount = sequenceLoopCount;
            var sourceDelays = sourceDelaysInSeconds.Select(d => PrecisionTimeSpan.FromSeconds(d)).ToArray();
            output.Source.SetSequence(sequence, sourceDelays);
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
        /// <param name="applyToIndividualPins">Whether the provided site pin info is for cascading pin data. Default is false.</param>
        public static void ConfigureSourceSettings(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, string channelString = "", bool applyToIndividualPins = false)
        {
            var channelOutput = string.IsNullOrEmpty(channelString) ? sessionInfo.AllChannelsOutput : sessionInfo.Session.Outputs[channelString];
            sessionInfo.ConfigureSourceSettings(settings, channelOutput, sitePinInfo: null, applyToIndividualPins);
        }

        /// <summary>
        /// Configures <see cref="DCPowerSourceSettings"/>.
        /// </summary>
        /// <param name="sessionInfo">The <see cref="DCPowerSessionInformation"/> object.</param>
        /// <param name="settings">The source settings to configure.</param>
        /// <param name="channelOutput">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="sitePinInfo">The <see cref="SitePinInfo"/> object.</param>
        /// <param name="applyToIndividualPins">Whether the provided site pin info is for cascading pin data. Default is false.</param>
        public static void ConfigureSourceSettings(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, DCPowerOutput channelOutput, SitePinInfo sitePinInfo, bool applyToIndividualPins)
        {
            string channelString = string.IsNullOrEmpty(channelOutput.Name) ? sessionInfo.AllChannelsString : channelOutput.Name;
            if (sitePinInfo != null && channelString.Split(',').Length > 1)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_MultipleChannelOutputsDetected, channelString));
            }

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
                    channelOutput.ConfigureLevelsAndLimits(settings, sitePin, applyToIndividualPins);
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

        private static DCPowerAdvancedSequenceProperty[] GetAdvancedSequencePropertiesToConfigure(IEnumerable<DCPowerAdvancedSequenceStepProperties> perStepProperties)
        {
            var result = new HashSet<DCPowerAdvancedSequenceProperty>();
            foreach (var stepProperties in perStepProperties)
            {
                foreach (var (property, enumValue) in Utilities.PropertyMappingsCache)
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

        private static void ConfigureLevelsAndLimits(this DCPowerOutput channelOutput, DCPowerSourceSettings settings, SitePinInfo sitePinInfo = null, bool applyToIndividualPins = false)
        {
            if (settings.LimitSymmetry.HasValue)
            {
                channelOutput.Source.ComplianceLimitSymmetry = settings.LimitSymmetry.Value;
            }
            if (settings.OutputFunction.Equals(DCPowerSourceOutputFunction.DCVoltage))
            {
                ConfigureVoltageSettings(channelOutput, settings, sitePinInfo, applyToIndividualPins);
            }
            else if (settings.OutputFunction.Equals(DCPowerSourceOutputFunction.DCCurrent))
            {
                ConfigureCurrentSettings(channelOutput, settings, sitePinInfo, applyToIndividualPins);
            }
        }

        private static void Force(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, bool waitForSourceCompletion = false, bool applyToIndividualPins = false)
        {
            var channelString = sessionInfo.AllChannelsString;
            var channelOutput = sessionInfo.Session.Outputs[channelString];
            sessionInfo.ConfigureChannels(settings, channelOutput, sitePinInfo: null, applyToIndividualPins);
            channelOutput.InitiateChannels(waitForSourceCompletion);
        }

        private static void ConfigureChannels(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, DCPowerOutput channelOutput, SitePinInfo sitePinInfo, bool applyToIndividualPins)
        {
            channelOutput.Control.Abort();
            sessionInfo.ConfigureSourceSettings(settings, channelOutput, sitePinInfo, applyToIndividualPins);
            if (sitePinInfo != null)
            {
                sessionInfo.ConfigureMeasureWhen(sitePinInfo, sitePinInfo.ModelString, DCPowerMeasurementWhen.OnMeasureTrigger);
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

        private static void InitiateGangedLeaderAndNonGangedChannels(this DCPowerSessionsBundle sessionsBundle, bool waitForSourceCompletion = false)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                if (!IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
                {
                    var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                    channelOutput.InitiateChannels(waitForSourceCompletion);
                }
            });
        }

        private static void ConfigureAllChannelsAndInitiateGangedFollowerChannels(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, SitePinInfo sitePinInfo, bool applyToIndividualPins)
        {
            var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
            sessionInfo.ConfigureChannels(settings, channelOutput, sitePinInfo, applyToIndividualPins);
            if (IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
            {
                channelOutput.InitiateChannels();
            }
        }

        private static void InitiateChannels(this DCPowerOutput channelOutput, bool waitForSourceCompletion = false, double timeoutInSeconds = 5)
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

        internal static string BuildTerminalName(this DCPowerSessionInformation sessionInfo, string channelString, string triggerOrEventTypeName)
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

        internal static bool IsFirstChannelOfSession(this SitePinInfo sitePinInfo, DCPowerSessionInformation sessionInfo)
        {
            return sessionInfo.AllChannelsString.StartsWith(sitePinInfo.IndividualChannelString, StringComparison.InvariantCulture);
        }

        private static void ConfigureVoltageSettings(DCPowerOutput dcOutput, DCPowerSourceSettings settings, SitePinInfo sitePinInfo, bool applyToIndividualPins)
        {
            var currentLimitDivisor = (sitePinInfo?.CascadingInfo as GangingInfo)?.ChannelsCount ?? 1;
            if (applyToIndividualPins)
            {
                currentLimitDivisor = 1;
            }
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

        private static void ConfigureCurrentSettings(DCPowerOutput dcOutput, DCPowerSourceSettings settings, SitePinInfo sitePinInfo, bool applyToIndividualPins)
        {
            var currentLevelDivisor = (sitePinInfo?.CascadingInfo as GangingInfo)?.ChannelsCount ?? 1;
            if (applyToIndividualPins)
            {
                currentLevelDivisor = 1;
            }
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

        private static void ConfigureTriggerForGanging(this DCPowerOutput channelOutput, SitePinInfo sitePinInfo)
        {
            if (IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
            {
                channelOutput.Triggers.SourceTrigger.Type = DCPowerSourceTriggerType.DigitalEdge;
                channelOutput.Triggers.SourceTrigger.DigitalEdge.Configure((sitePinInfo.CascadingInfo as GangingInfo).SourceTriggerName, DCPowerTriggerEdge.Rising);
            }
        }

        #endregion private and internal methods
    }
}
