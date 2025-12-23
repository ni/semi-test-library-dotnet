using System;
using System.Collections.Generic;
using System.Linq;
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
        private const double DefaultSequenceTimeout = 5;

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
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings);
            });
        }

        /// <inheritdoc cref="ConfigureSourceSettings(DCPowerSessionsBundle, DCPowerSourceSettings)"/>
        public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, SiteData<DCPowerSourceSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings.GetValue(sitePinInfo.SiteNumber), sitePinInfo.IndividualChannelString);
            });
        }

        /// <inheritdoc cref="ConfigureSourceSettings(DCPowerSessionsBundle, DCPowerSourceSettings)"/>
        public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, PinSiteData<DCPowerSourceSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings.GetValue(sitePinInfo), sitePinInfo.IndividualChannelString);
            });
        }

        /// <summary>
        /// Configures <see cref="DCPowerSourceSettings"/>.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="settings">The specific settings to configure.</param>
        public static void ConfigureSourceSettings(this DCPowerSessionsBundle sessionsBundle, IDictionary<string, DCPowerSourceSettings> settings)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString].Control.Abort();
                sessionInfo.ConfigureSourceSettings(settings[sitePinInfo.PinName], sitePinInfo.IndividualChannelString);
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
                sessionInfo.Force(settings, sitePinInfo: null, waitForSourceCompletion);
            });
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
                sessionInfo.Force(settings, sitePinInfo, waitForSourceCompletion);
            });
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
                sessionInfo.Force(settings, sitePinInfo, waitForSourceCompletion);
            });
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
                sessionInfo.Force(settings, sitePinInfo, waitForSourceCompletion);
            });
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
            sessionsBundle.Do(sessionInfo =>
            {
                settings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.Force(settings, sitePinInfo: null, waitForSourceCompletion);
            });
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
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSiteSettings = settings.GetValue(sitePinInfo.SiteNumber);
                perSiteSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.Force(perSiteSettings, sitePinInfo, waitForSourceCompletion);
            });
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
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perPinSettings = settings[sitePinInfo.PinName];
                perPinSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.Force(perPinSettings, sitePinInfo, waitForSourceCompletion);
            });
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
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSitePinPairSettings = settings.GetValue(sitePinInfo);
                perSitePinPairSettings.OutputFunction = DCPowerSourceOutputFunction.DCVoltage;
                sessionInfo.Force(perSitePinPairSettings, sitePinInfo, waitForSourceCompletion);
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
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Force(settings, sitePinInfo: null, waitForSourceCompletion);
            });
        }

        /// <summary>
        /// Forces a hardware-timed sequence of voltage outputs, ensuring synchronized output across all specified target pins.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageSequence">The voltage sequence to force for all site-pin pairs.</param>
        /// <param name="currentLimit">Current limit for the sequence.</param>
        /// <param name="voltageLevelRange">Voltage level range.</param>
        /// <param name="currentLimitRange">current limit range.</param>
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
            SequenceProvider getVoltageSequence = _ => voltageSequence;
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
            SiteData<double[]> voltageSequence,
            SiteData<double> currentLimits = null,
            SiteData<double> voltageLevelRanges = null,
            SiteData<double> currentLimitRanges = null,
            double? sourceDelayinSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            SequenceProvider getVoltageSequenceForSite = sitePinInfo => voltageSequence?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getCurrentLimitsForSite = sitePinInfo => currentLimits?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getVoltageLevelRangesForSite = sitePinInfo => voltageLevelRanges?.GetValue(sitePinInfo.SiteNumber);
            ValueProvider getCurrentLimitRangesForSite = sitePinInfo => currentLimitRanges?.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceSequenceSynchronizedCore(
               getVoltageSequenceForSite,
               DCPowerSourceOutputFunction.DCVoltage,
               getCurrentLimitsForSite,
               getVoltageLevelRangesForSite,
               getCurrentLimitRangesForSite,
               sourceDelayinSeconds,
               transientResponse,
               sequenceLoopCount,
               waitForSequenceCompletion,
               sequenceTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceVoltageSequenceSynchronized(DCPowerSessionsBundle, double[], double?, double?, double?, double?, DCPowerSourceTransientResponse?, int, bool, double)"/>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequence,
            PinSiteData<double> currentLimits = null,
            PinSiteData<double> voltageLevelRanges = null,
            PinSiteData<double> currentLimitRanges = null,
            double? sourceDelayinSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            SequenceProvider getVoltageSequenceForSitePin = sitePinInfo => voltageSequence?.GetValue(sitePinInfo);
            ValueProvider getCurrentLimitsForSitePin = sitePinInfo => currentLimits?.GetValue(sitePinInfo);
            ValueProvider getVoltageLevelRangesForSitePin = sitePinInfo => voltageLevelRanges?.GetValue(sitePinInfo);
            ValueProvider getCurrentLimitRangesForSitePin = sitePinInfo => currentLimitRanges?.GetValue(sitePinInfo);

            sessionsBundle.ForceSequenceSynchronizedCore(
               getVoltageSequenceForSitePin,
               DCPowerSourceOutputFunction.DCVoltage,
               getCurrentLimitsForSitePin,
               getVoltageLevelRangesForSitePin,
               getCurrentLimitRangesForSitePin,
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
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Force(settings, sitePinInfo: null, waitForSourceCompletion);
            });
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
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = currentLevels[sitePinInfo.PinName],
                    Limit = voltageLimit,
                    LevelRange = currentLevelRange,
                    LimitRange = voltageLimitRange
                };
                sessionInfo.Force(settings, sitePinInfo, waitForSourceCompletion);
            });
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
                sessionInfo.Force(settings, sitePinInfo, waitForSourceCompletion);
            });
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
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var settings = new DCPowerSourceSettings()
                {
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                    LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric,
                    Level = currentLevels.GetValue(sitePinInfo),
                    Limit = voltageLimit,
                    LevelRange = currentLevelRange,
                    LimitRange = voltageLimitRange
                };
                sessionInfo.Force(settings, sitePinInfo, waitForSourceCompletion);
            });
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
            sessionsBundle.Do(sessionInfo =>
            {
                settings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.Force(settings, sitePinInfo: null, waitForSourceCompletion);
            });
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
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSiteSettings = settings.GetValue(sitePinInfo.SiteNumber);
                perSiteSettings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.Force(perSiteSettings, sitePinInfo, waitForSourceCompletion);
            });
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
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perPinSettings = settings[sitePinInfo.PinName];
                perPinSettings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.Force(perPinSettings, sitePinInfo, waitForSourceCompletion);
            });
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
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var perSitePinPairSettings = settings.GetValue(sitePinInfo);
                perSitePinPairSettings.OutputFunction = DCPowerSourceOutputFunction.DCCurrent;
                sessionInfo.Force(perSitePinPairSettings, sitePinInfo, waitForSourceCompletion);
            });
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
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Force(settings, waitForSourceCompletion: waitForSourceCompletion);
            });
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
            SiteData<double[]> currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var sequence = currentSequence.GetValue(sitePinInfo.SiteNumber);
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
            PinSiteData<double[]> currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = false,
            double sequenceTimeoutInSeconds = DefaultSequenceTimeout)
        {
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var sequence = currentSequence.GetValue(sitePinInfo);
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
        public static void ConfigureSourceSettings(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, string channelString = "")
        {
            var channelOutput = string.IsNullOrEmpty(channelString) ? sessionInfo.AllChannelsOutput : sessionInfo.Session.Outputs[channelString];
            channelOutput.Source.Mode = DCPowerSourceMode.SinglePoint;
            if (settings.SourceDelayInSeconds.HasValue)
            {
                channelOutput.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(settings.SourceDelayInSeconds.Value);
            }
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
            channelOutput.ConfigureLevelsAndLimits(settings);
        }

        #endregion methods on DCPowerSessionInformation

        #region private and internal methods

        private static void ConfigureLevelsAndLimits(this DCPowerOutput channelOutput, DCPowerSourceSettings settings)
        {
            if (settings.LimitSymmetry.HasValue)
            {
                channelOutput.Source.ComplianceLimitSymmetry = settings.LimitSymmetry.Value;
            }
            if (settings.OutputFunction.Equals(DCPowerSourceOutputFunction.DCVoltage))
            {
                ConfigureVoltageSettings(channelOutput, settings);
            }
            else if (settings.OutputFunction.Equals(DCPowerSourceOutputFunction.DCCurrent))
            {
                ConfigureCurrentSettings(channelOutput, settings);
            }
        }

        private static void Force(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, SitePinInfo sitePinInfo = null, bool waitForSourceCompletion = false)
        {
            var channelString = sitePinInfo?.IndividualChannelString ?? sessionInfo.AllChannelsString;
            var channelOutput = sessionInfo.Session.Outputs[channelString];
            sessionInfo.ConfigureChannels(settings, channelOutput, channelString, sitePinInfo);
            channelOutput.InitiateChannels(waitForSourceCompletion);
        }

        private static void ConfigureChannels(this DCPowerSessionInformation sessionInfo, DCPowerSourceSettings settings, DCPowerOutput channelOutput, string channelString = "", SitePinInfo sitePinInfo = null)
        {
            channelOutput.Control.Abort();
            sessionInfo.ConfigureSourceSettings(settings, channelString);
            channelOutput.Source.Output.Enabled = true;
            channelOutput.Control.Commit();
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

        private static void ConfigureVoltageSettings(DCPowerOutput dcOutput, DCPowerSourceSettings settings)
        {
            dcOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCVoltage;
            if (settings.Level.HasValue)
            {
                dcOutput.Source.Voltage.VoltageLevel = settings.Level.Value;
            }
            if (settings.LimitSymmetry == DCPowerComplianceLimitSymmetry.Symmetric && settings.Limit.HasValue)
            {
                dcOutput.Source.Voltage.CurrentLimit = settings.Limit.Value;
            }
            else
            {
                if (settings.LimitHigh.HasValue)
                {
                    dcOutput.Source.Voltage.CurrentLimitHigh = settings.LimitHigh.Value;
                }
                if (settings.LimitLow.HasValue)
                {
                    dcOutput.Source.Voltage.CurrentLimitLow = settings.LimitLow.Value;
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
                dcOutput.Source.Voltage.CurrentLimitRange = settings.LimitRange ?? CalculateLimitRangeFromLimit(settings);
            }
        }

        private static void ConfigureCurrentSettings(DCPowerOutput dcOutput, DCPowerSourceSettings settings)
        {
            dcOutput.Source.Output.Function = DCPowerSourceOutputFunction.DCCurrent;
            if (settings.Level.HasValue)
            {
                dcOutput.Source.Current.CurrentLevel = settings.Level.Value;
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
                dcOutput.Source.Current.CurrentLevelRange = settings.LevelRange ?? Math.Abs(settings.Level.Value);
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

        #endregion private and internal methods
    }
}
