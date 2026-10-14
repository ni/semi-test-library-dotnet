using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines methods for DCPower voltage/current hardware level sequencing.
    /// </summary>
    public static partial class Source
    {
        private const int AttributeIdNotRecognized = -1074135028;

        #region Custom Delegates

        /// <summary>
        /// Provides an array of items of type T based on the specified site pin information.
        /// </summary>
        /// <typeparam name="T">The type of items returned in the array.</typeparam>
        /// <param name="sitePinInfo">The site pin information that is used to generate the array.</param>
        /// <returns>An array of type T that contains the generated sequence based on the provided site pin information.</returns>
        private delegate T[] ArrayProvider<T>(SitePinInfo sitePinInfo);

        /// <summary>
        /// Provides an IEnumerable of items of type T based on the specified site pin information.
        /// </summary>
        /// <typeparam name="T">The type of items returned in the IEnumerable.</typeparam>
        /// <param name="sitePinInfo">The site pin information used to generate the IEnumerable.</param>
        /// <returns>An IEnumerable of type T generated based on the provided site pin information.</returns>
        private delegate IEnumerable<T> IEnumerableProvider<T>(SitePinInfo sitePinInfo);

        /// <summary>
        /// Delegate to retrieve a single double value (for example, limit or range) for a given site-pin pair.
        /// </summary>
        private delegate double? ValueProvider(SitePinInfo sitePinInfo);

        #endregion

        private static string BuildSequenceName()
        {
            return $"STL_AdvSeq_{DateTime.UtcNow.Ticks}_{Guid.NewGuid().ToString("N", CultureInfo.InvariantCulture).Substring(0, 8)}";
        }

        private static void ConfigureTriggersForCascadedSequencing(this DCPowerSessionInformation sessionInfo, SitePinInfo sitePinInfo)
        {
            var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
            output.ConfigureSourceTriggerForCascading(sitePinInfo);
            output.ConfigureStartTriggerForCascadedSequencing(sitePinInfo);
            output.ConfigureSequenceAdvanceTriggerForCascadedSequencing(sitePinInfo);
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

        private static bool IsPrimaryOutput(int sessionIndex, SitePinInfo sitePinInfo, DCPowerSessionInformation sessionInfo)
        {
            return sessionIndex == 0 && sitePinInfo.IsFirstChannelOfSession(sessionInfo);
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

        /// <summary>
        /// Forces a hardware-timed sequence of voltage values on the targeted pins.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureVoltageSequence(DCPowerSessionsBundle, string, double[], int, double?, bool, UpdateMode)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageSequence">Array of voltage values to force in the sequence.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion.</param>
        /// <param name="currentLimit">The current limit to use for the sequence.</param>
        /// <param name="voltageLevelRange">The voltage level range to use for the sequence.</param>
        /// <param name="currentLimitRange">The current limit range to use for the sequence.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            double[] voltageSequence,
            double sequenceTimeoutInSeconds,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1)
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
                sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion: true, sequenceTimeoutInSeconds);
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
                       sequenceTimeoutInSeconds,
                       setAsActiveSequence: true);
                });
            }

            sessionsBundle.ReleaseAdvancedSequenceResources(advancedSequenceName);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureVoltageSequence(DCPowerSessionsBundle, string, SiteData{double[]}, int, double?, bool, UpdateMode)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceVoltageSequence(DCPowerSessionsBundle, double[], double, double?, double?, double?, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="voltageSequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="currentLimit"/>
        /// <param name="voltageLevelRange"/>
        /// <param name="currentLimitRange"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> voltageSequence,
            double sequenceTimeoutInSeconds,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1)
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
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion: true, sequenceTimeoutInSeconds);

            sessionsBundle.ReleaseAdvancedSequenceResources(advancedSequenceName);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureVoltageSequence(DCPowerSessionsBundle, string, PinSiteData{double[]}, int, double?, bool, UpdateMode)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceVoltageSequence(DCPowerSessionsBundle, double[], double, double?, double?, double?, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="voltageSequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="currentLimit"/>
        /// <param name="voltageLevelRange"/>
        /// <param name="currentLimitRange"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequence,
            double sequenceTimeoutInSeconds,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1)
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
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion: true, sequenceTimeoutInSeconds);

            sessionsBundle.ReleaseAdvancedSequenceResources(advancedSequenceName);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of voltage outputs, ensuring synchronized output across all specified target pins.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="voltageSequence">Sequence of voltage values to force.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion.</param>
        /// <param name="currentLimit">The current limit to use for the sequence.</param>
        /// <param name="voltageLevelRange">The voltage level range to use for the sequence.</param>
        /// <param name="currentLimitRange">The current limit range to use for the sequence.</param>
        /// <param name="sourceDelayInSeconds">Optional source delay to use uniformly for synchronization.</param>
        /// <param name="transientResponse">Transient response.</param>
        /// <param name="sequenceLoopCount">The number of times to force the sequence.</param>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            double[] voltageSequence,
            double sequenceTimeoutInSeconds,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<double> getVoltageSequence = _ => voltageSequence;
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
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, SiteData{ DCPowerSourceSettings[] }, double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceVoltageSequenceSynchronized(DCPowerSessionsBundle, double[], double, double?, double?, double?, double?, DCPowerSourceTransientResponse?, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="voltageSequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="currentLimit"/>
        /// <param name="voltageLevelRange"/>
        /// <param name="currentLimitRange"/>
        /// <param name="sourceDelayInSeconds"/>
        /// <param name="transientResponse"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> voltageSequence,
            double sequenceTimeoutInSeconds,
            SiteData<double> currentLimit = null,
            SiteData<double> voltageLevelRange = null,
            SiteData<double> currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<double> getVoltageSequenceForSite = sitePinInfo => voltageSequence?.GetValue(sitePinInfo.SiteNumber);
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
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, PinSiteData{ DCPowerSourceSettings[] }, double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceVoltageSequenceSynchronized(DCPowerSessionsBundle, double[], double, double?, double?, double?, double?, DCPowerSourceTransientResponse?, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="voltageSequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="currentLimit"/>
        /// <param name="voltageLevelRange"/>
        /// <param name="currentLimitRange"/>
        /// <param name="sourceDelayInSeconds"/>
        /// <param name="transientResponse"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequence,
            double sequenceTimeoutInSeconds,
            PinSiteData<double> currentLimit = null,
            PinSiteData<double> voltageLevelRange = null,
            PinSiteData<double> currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<double> getVoltageSequenceForSitePin = sitePinInfo => voltageSequence?.GetValue(sitePinInfo);
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
                sequenceTimeoutInSeconds);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of current outputs, ensuring synchronized output across all specified target pins.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentSequence">Sequence of current values to force.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion.</param>
        /// <param name="voltageLimit">Voltage limit for the sequence.</param>
        /// <param name="currentLevelRange">Current level range.</param>
        /// <param name="voltageLimitRange">Voltage limit range.</param>
        /// <param name="sourceDelayInSeconds">Optional source delay to use uniformly for synchronization.</param>
        /// <param name="transientResponse">Transient response.</param>
        /// <param name="sequenceLoopCount">The number of times to force the sequence.</param>
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            double[] currentSequence,
            double sequenceTimeoutInSeconds,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<double> getCurrentSequence = _ => currentSequence;
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
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, SiteData{ DCPowerSourceSettings[] }, double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceCurrentSequenceSynchronized(DCPowerSessionsBundle, double[], double, double?, double?, double?, double?, DCPowerSourceTransientResponse?, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="currentSequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="voltageLimit"/>
        /// <param name="currentLevelRange"/>
        /// <param name="voltageLimitRange"/>
        /// <param name="sourceDelayInSeconds"/>
        /// <param name="transientResponse"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> currentSequence,
            double sequenceTimeoutInSeconds,
            SiteData<double> voltageLimit = null,
            SiteData<double> currentLevelRange = null,
            SiteData<double> voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<double> getCurrentSequenceForSite = sitePinInfo => currentSequence.GetValue(sitePinInfo.SiteNumber);
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
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, PinSiteData{ DCPowerSourceSettings[] }, double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceCurrentSequenceSynchronized(DCPowerSessionsBundle, double[], double, double?, double?, double?, double?, DCPowerSourceTransientResponse?, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="currentSequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="voltageLimit"/>
        /// <param name="currentLevelRange"/>
        /// <param name="voltageLimitRange"/>
        /// <param name="sourceDelayInSeconds"/>
        /// <param name="transientResponse"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> currentSequence,
            double sequenceTimeoutInSeconds,
            PinSiteData<double> voltageLimit = null,
            PinSiteData<double> currentLevelRange = null,
            PinSiteData<double> voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<double> getCurrentSequenceForSitePin = sitePinInfo => currentSequence.GetValue(sitePinInfo);
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
                sequenceTimeoutInSeconds);
        }

        /// <summary>
        /// Forces a hardware-timed sequence of current/voltage outputs, ensuring synchronized output across all specified target pins.
        /// </summary>
        private static void ForceSequenceSynchronizedCore(
            this DCPowerSessionsBundle sessionsBundle,
            ArrayProvider<double> fetchLevelSequence,
            DCPowerSourceOutputFunction outputFunction,
            ValueProvider fetchLimit,
            ValueProvider fetchLevelRange,
            ValueProvider fetchLimitRange,
            double? sourceDelayInSeconds,
            DCPowerSourceTransientResponse? transientResponse,
            int sequenceLoopCount,
            double sequenceTimeoutInSeconds)
        {
            sessionsBundle.ValidateNoChannelGanged();
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
                sessionInfo.ConfigureTransientResponse(settings, perChannelString);

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

            primaryOutput.Events.SequenceEngineDoneEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(sequenceTimeoutInSeconds));

            sessionsBundle.ReleaseSynchronizedAdvancedSequenceResources(sequenceName);
        }

        /// <summary>
        /// Synchronizes and forces an advanced sequence across all sessions in the bundle.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The sequence of source settings to apply.</param>
        /// <param name="sequenceTimeoutInSeconds">The timeout, in seconds, to wait for sequence completion.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the sequence.</param>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerSourceSettings[] sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<DCPowerSourceSettings> getSequence = _ => sequence;

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, SiteData{ DCPowerSourceSettings[] }, double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerSourceSettings[], double, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerSourceSettings[]> sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<DCPowerSourceSettings> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, PinSiteData{ DCPowerSourceSettings[] }, double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerSourceSettings[], double, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerSourceSettings[]> sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<DCPowerSourceSettings> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo);

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
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
        /// <param name="sequenceTimeoutInSeconds">The timeout, in seconds, to wait for sequence completion.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the voltage sequence.</param>
        /// <param name="pointsToFetch">The number of points to fetch.</param>
        /// <param name="measurementTimeoutInSeconds">The time to wait before the fetch measurement operation is aborted.</param>
        /// <returns>A <see cref="PinSiteData{T}"/> object that contains an array of <see cref="SingleDCPowerFetchResult"/> values,
        /// where each <see cref="SingleDCPowerFetchResult"/> object contains the voltage, current, and inCompliance result for a simple sample or point from the previous measurement.</returns>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerSourceSettings[] sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            ArrayProvider<DCPowerSourceSettings> getSequence = _ => sequence;

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], double, int, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerSourceSettings[]> sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            ArrayProvider<DCPowerSourceSettings> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo.SiteNumber);

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], double, int, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerSourceSettings[]> sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            ArrayProvider<DCPowerSourceSettings> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo);

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
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
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The sequence of <see cref="DCPowerAdvancedSequenceStepProperties"/> to apply.</param>
        /// <param name="sequenceTimeoutInSeconds">The timeout, in seconds, to wait for sequence completion.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the sequence.</param>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerAdvancedSequenceStepProperties[] sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<DCPowerAdvancedSequenceStepProperties> getSequence = _ => sequence;

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, SiteData{ DCPowerAdvancedSequenceStepProperties[] }, double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], double, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<DCPowerAdvancedSequenceStepProperties> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo.SiteNumber);

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, consider using the <see cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, PinSiteData{ DCPowerAdvancedSequenceStepProperties[] }, double, int, int?, double)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceAdvancedSequenceSynchronized(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], double, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="sequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1)
        {
            ArrayProvider<DCPowerAdvancedSequenceStepProperties> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo);

            sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
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
        /// <param name="sequenceTimeoutInSeconds">The timeout, in seconds, to wait for sequence completion.</param>
        /// <param name="sequenceLoopCount">The number of times to loop through the voltage sequence.</param>
        /// <param name="pointsToFetch">The number of points to fetch.</param>
        /// <param name="measurementTimeoutInSeconds">The time to wait before the fetch measurement operation is aborted.</param>
        /// <returns>A <see cref="PinSiteData{T}"/> object that contains an array of <see cref="SingleDCPowerFetchResult"/> values,
        /// where each <see cref="SingleDCPowerFetchResult"/> object contains the voltage, current, and inCompliance result for a simple sample or point from the previous measurement.</returns>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerAdvancedSequenceStepProperties[] sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            ArrayProvider<DCPowerAdvancedSequenceStepProperties> getSequence = _ => sequence;

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], double, int, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            ArrayProvider<DCPowerAdvancedSequenceStepProperties> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo.SiteNumber);

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], double, int, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            double sequenceTimeoutInSeconds,
            int sequenceLoopCount = 1,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            ArrayProvider<DCPowerAdvancedSequenceStepProperties> getSequence = sitePinInfo => sequence.GetValue(sitePinInfo);

            return sessionsBundle.ForceAdvancedSequenceSynchronizedCore(
                getSequence,
                sequenceLoopCount,
                sequenceTimeoutInSeconds,
                fetchResult: true,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        private static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedCore<T>(
            this DCPowerSessionsBundle sessionsBundle,
            ArrayProvider<T> getSequence,
            int sequenceLoopCount,
            double sequenceTimeoutInSeconds,
            bool fetchResult = false,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10) where T : class
        {
            sessionsBundle.ValidateNoChannelGanged();
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

            primaryOutput.Events.SequenceEngineDoneEvent.WaitForEvent(PrecisionTimeSpan.FromSeconds(sequenceTimeoutInSeconds));

            if (fetchResult)
            {
                result = sessionsBundle.FetchMeasurement(pointsToFetch.Value, measurementTimeoutInSeconds);
            }

            sessionsBundle.ReleaseSynchronizedAdvancedSequenceResources(sequenceName);

            return result;
        }

        /// <summary>
        /// Forces a hardware-timed sequence of current values on the targeted pins.
        /// </summary>
        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureCurrentSequence(DCPowerSessionsBundle, string, double[], int, double?, bool, UpdateMode)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="currentSequence">Sequence of current values to force.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion.</param>
        /// <param name="voltageLimit">Voltage limit for the sequence.</param>
        /// <param name="currentLevelRange">Current level range.</param>
        /// <param name="voltageLimitRange">Voltage limit range.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            double[] currentSequence,
            double sequenceTimeoutInSeconds,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1)
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
                sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion: true, sequenceTimeoutInSeconds);
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
                        sequenceTimeoutInSeconds,
                        setAsActiveSequence: true);
                });
            }

            sessionsBundle.ReleaseAdvancedSequenceResources(sequenceName);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureCurrentSequence(DCPowerSessionsBundle, string, SiteData{double[]}, int, double?, bool, UpdateMode)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceCurrentSequence(DCPowerSessionsBundle, double[], double, double?, double?, double?, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="currentSequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="voltageLimit"/>
        /// <param name="currentLevelRange"/>
        /// <param name="voltageLimitRange"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> currentSequence,
            double sequenceTimeoutInSeconds,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1)
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
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion: true, sequenceTimeoutInSeconds);

            sessionsBundle.ReleaseAdvancedSequenceResources(sequenceName);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureCurrentSequence(DCPowerSessionsBundle, string, PinSiteData{double[]}, int, double?, bool, UpdateMode)"/>
        /// followed by <see cref="Control.Initiate(DCPowerSessionsBundle)"/> instead.<br/>
        /// This method will set the Source Mode back to SinglePoint mode upon returning.
        /// </remarks>
        /// <inheritdoc cref="ForceCurrentSequence(DCPowerSessionsBundle, double[], double, double?, double?, double?, int)"/>
        /// <param name="sessionsBundle"/>
        /// <param name="currentSequence"/>
        /// <param name="sequenceTimeoutInSeconds"/>
        /// <param name="voltageLimit"/>
        /// <param name="currentLevelRange"/>
        /// <param name="voltageLimitRange"/>
        /// <param name="sequenceLoopCount"/>
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> currentSequence,
            double sequenceTimeoutInSeconds,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1)
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
            sessionsBundle.InitiateGangedLeaderAndNonGangedChannels(waitForSourceCompletion: true, sequenceTimeoutInSeconds);

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
            sessionInfo.ConfigureSequenceForCascadingCore(
                sequenceName: sequenceName,
                sequence: levelSequence,
                sequenceLoopCount: sequenceLoopCount,
                outputFunction: (DCPowerSourceOutputFunction)settings.OutputFunction,
                sitePinInfo: sitePinInfo,
                needDataAdjustment: needDataAdjustment,
                setAsActiveSequence: setAsActiveSequence);
            var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
            channelOutput.ConfigureLevelsAndLimits(settings, sitePinInfo, needDataAdjustment);
            if (IsFollowerOfGangedChannels(sitePinInfo.CascadingInfo))
            {
                channelOutput.InitiateChannels(waitForCompletion: false, timeoutInSeconds: DefaultTimeout);
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
        /// <param name="updateMode">Specifies when the configured settings are applied.</param>
        public static void ConfigureVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            double[] sequence,
            int sequenceLoopCount = 1,
            double? sequenceStepDeltaTimeInSeconds = null,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
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
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureVoltageSequence(DCPowerSessionsBundle,string, double[], int, double?, bool, UpdateMode)"/>
        public static void ConfigureVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            SiteData<double[]> sequence,
            int sequenceLoopCount = 1,
            double? sequenceStepDeltaTimeInSeconds = null,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo.SiteNumber),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                    sitePinInfo: sitePinInfo,
                    sequenceStepDeltaTimeInSeconds: sequenceStepDeltaTimeInSeconds,
                    setAsActiveSequence: setAsActiveSequence);
            });
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureVoltageSequence(DCPowerSessionsBundle,string, double[], int, double?, bool, UpdateMode)"/>
        public static void ConfigureVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            PinSiteData<double[]> sequence,
            int sequenceLoopCount = 1,
            double? sequenceStepDeltaTimeInSeconds = null,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            var hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, sequence);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                    sitePinInfo: sitePinInfo,
                    sequenceStepDeltaTimeInSeconds,
                    setAsActiveSequence: setAsActiveSequence);
            });
            sessionsBundle.ApplyUpdateMode(updateMode);
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
        /// <param name="updateMode">Specifies when the configured settings are applied.</param>
        public static void ConfigureCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            double[] sequence,
            int sequenceLoopCount = 1,
            double? sequenceStepDeltaTimeInSeconds = null,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
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
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureCurrentSequence(DCPowerSessionsBundle,string, double[], int, double?, bool, UpdateMode)"/>
        public static void ConfigureCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            SiteData<double[]> sequence,
            int sequenceLoopCount = 1,
            double? sequenceStepDeltaTimeInSeconds = null,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo.SiteNumber),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                    sitePinInfo: sitePinInfo,
                    sequenceStepDeltaTimeInSeconds,
                    setAsActiveSequence: setAsActiveSequence);
            });
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureCurrentSequence(DCPowerSessionsBundle,string, double[], int, double?, bool, UpdateMode)"/>
        public static void ConfigureCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            PinSiteData<double[]> sequence,
            int sequenceLoopCount = 1,
            double? sequenceStepDeltaTimeInSeconds = null,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
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
            sessionsBundle.ApplyUpdateMode(updateMode);
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
        /// <param name="updateMode">Specifies when the configured settings are applied.</param>
        public static void ConfigureAdvancedSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            IList<DCPowerAdvancedSequenceStepProperties> perStepProperties,
            bool setAsActiveSequence = false,
            bool commitFirstElementAsInitialState = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            IEnumerableProvider<DCPowerAdvancedSequenceStepProperties> getPerStepProperties = _ => perStepProperties;
            sessionsBundle.ConfigureAdvancedSequenceCore(sequenceName, getPerStepProperties, setAsActiveSequence, commitFirstElementAsInitialState);
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureAdvancedSequence(DCPowerSessionsBundle, string, IList{DCPowerAdvancedSequenceStepProperties}, bool, bool, UpdateMode)"/>
        public static void ConfigureAdvancedSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            SiteData<IList<DCPowerAdvancedSequenceStepProperties>> perStepProperties,
            bool setAsActiveSequence = false,
            bool commitFirstElementAsInitialState = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            IEnumerableProvider<DCPowerAdvancedSequenceStepProperties> getPerStepProperties = sitePinInfo => perStepProperties.GetValue(sitePinInfo.SiteNumber);
            sessionsBundle.ConfigureAdvancedSequenceCore(sequenceName, getPerStepProperties, setAsActiveSequence, commitFirstElementAsInitialState);
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureAdvancedSequence(DCPowerSessionsBundle, string, IList{DCPowerAdvancedSequenceStepProperties}, bool, bool, UpdateMode)"/>
        public static void ConfigureAdvancedSequence(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            PinSiteData<IList<DCPowerAdvancedSequenceStepProperties>> perStepProperties,
            bool setAsActiveSequence = false,
            bool commitFirstElementAsInitialState = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            IEnumerableProvider<DCPowerAdvancedSequenceStepProperties> getPerStepProperties = sitePinInfo => perStepProperties.GetValue(sitePinInfo);
            sessionsBundle.ConfigureAdvancedSequenceCore(sequenceName, getPerStepProperties, setAsActiveSequence, commitFirstElementAsInitialState);
            sessionsBundle.ApplyUpdateMode(updateMode);
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
        /// <param name="updateMode">Specifies when the configured settings are applied.</param>
        public static void ConfigureVoltageSequenceWithSourceDelays(
           this DCPowerSessionsBundle sessionsBundle,
           string sequenceName,
           double[] sequence,
           double[] sourceDelaysInSeconds,
           int sequenceLoopCount = 1,
           bool setAsActiveSequence = false,
           UpdateMode updateMode = UpdateMode.Deferred)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
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
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureVoltageSequenceWithSourceDelays(DCPowerSessionsBundle, string, double[], double[], int, bool, UpdateMode)"/>
        public static void ConfigureVoltageSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            SiteData<double[]> sequence,
            SiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo.SiteNumber),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                    sitePinInfo: sitePinInfo,
                    sourceDelay: sourceDelaysInSeconds.GetValue(sitePinInfo.SiteNumber),
                    setAsActiveSequence: setAsActiveSequence);
            });
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureVoltageSequenceWithSourceDelays(DCPowerSessionsBundle, string, double[], double[], int, bool, UpdateMode)"/>
        public static void ConfigureVoltageSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            PinSiteData<double[]> sequence,
            PinSiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            var hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, sequence);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, sourceDelaysInSeconds);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCVoltage,
                    sitePinInfo: sitePinInfo,
                    sourceDelay: sourceDelaysInSeconds.GetValue(sitePinInfo),
                    setAsActiveSequence: setAsActiveSequence);
            });
            sessionsBundle.ApplyUpdateMode(updateMode);
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
        /// <param name="updateMode">Specifies when the configured settings are applied.</param>
        public static void ConfigureCurrentSequenceWithSourceDelays(
           this DCPowerSessionsBundle sessionsBundle,
           string sequenceName,
           double[] sequence,
           double[] sourceDelaysInSeconds,
           int sequenceLoopCount = 1,
           bool setAsActiveSequence = false,
           UpdateMode updateMode = UpdateMode.Deferred)
        {
            if (sessionsBundle.HasGangedChannels)
            {
                sessionsBundle.ValidatePinsForGanging(hasGangedChannels: true);
                sessionsBundle.Do((sessionInfo, sitePinInfo) =>
                {
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
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureCurrentSequenceWithSourceDelays(DCPowerSessionsBundle, string, double[], double[], int, bool, UpdateMode)"/>
        public static void ConfigureCurrentSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            SiteData<double[]> sequence,
            SiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            sessionsBundle.ValidatePinsForGanging(sessionsBundle.HasGangedChannels);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                sessionInfo.ConfigureSequenceForCascadingCore(
                    sequenceName: sequenceName,
                    sequence: sequence.GetValue(sitePinInfo.SiteNumber),
                    sequenceLoopCount: sequenceLoopCount,
                    outputFunction: DCPowerSourceOutputFunction.DCCurrent,
                    sitePinInfo: sitePinInfo,
                    sourceDelay: sourceDelaysInSeconds.GetValue(sitePinInfo.SiteNumber),
                    setAsActiveSequence: setAsActiveSequence);
            });
            sessionsBundle.ApplyUpdateMode(updateMode);
        }

        /// <inheritdoc cref="ConfigureCurrentSequenceWithSourceDelays(DCPowerSessionsBundle, string, double[], double[], int, bool, UpdateMode)"/>
        public static void ConfigureCurrentSequenceWithSourceDelays(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            PinSiteData<double[]> sequence,
            PinSiteData<double[]> sourceDelaysInSeconds,
            int sequenceLoopCount = 1,
            bool setAsActiveSequence = false,
            UpdateMode updateMode = UpdateMode.Deferred)
        {
            var hasGangedChannels = sessionsBundle.HasGangedChannels;
            sessionsBundle.ValidatePinsForGanging(hasGangedChannels);
            sessionsBundle.ValidatePinValuesForCascading(hasGangedChannels, sourceDelaysInSeconds);
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
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
            sessionsBundle.ApplyUpdateMode(updateMode);
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

        private static void ConfigureAdvancedSequenceCore(
            this DCPowerSessionsBundle sessionsBundle,
            string sequenceName,
            IEnumerableProvider<DCPowerAdvancedSequenceStepProperties> getPerStepProperties,
            bool setAsActiveSequence,
            bool commitFirstElementAsInitialState)
        {
            sessionsBundle.ValidateNoChannelGanged();
            sessionsBundle.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                var perStepProperties = getPerStepProperties(sitePinInfo);
                channelOutput.Control.Abort();
                channelOutput.ConfigureAdvancedSequenceCore(
                    sequenceName,
                    sitePinInfo.ModelString,
                    perStepProperties,
                    setAsActiveSequence: setAsActiveSequence,
                    commitFirstElementAsInitialState: commitFirstElementAsInitialState);
            });
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
            Source.ValidateChannelOutputAndSitePinInfoPair(sitePinInfo, output.Name);
            output.Control.Abort();
            output.Source.Output.Function = outputFunction;
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
        /// Core implementation for forcing a current or voltage sequence.
        /// </summary>
        private static void ForceSequenceCore(
            this DCPowerOutput channelOutput,
            DCPowerSourceSettings settings,
            string sequenceName,
            double[] levelSequence,
            int sequenceLoopCount,
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

            channelOutput.InitiateChannels(waitForCompletion: true, sequenceTimeoutInSeconds);
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
    }
}
