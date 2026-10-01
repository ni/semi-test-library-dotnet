using System;
using System.Globalization;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Contains obsolete source methods for the <see cref="DCPowerSessionsBundle"/> class.
    /// </summary>
    public static partial class Source
    {
        #region Obsolete methods on DCPowerSessionsBundle

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
        /// <param name="currentLimit">The current limit to use for the sequence.</param>
        /// <param name="voltageLevelRange">The voltage level range to use for the sequence.</param>
        /// <param name="currentLimitRange">The current limit range to use for the sequence.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="waitForSequenceCompletion">True to block until the sequence engine completes (waits on SequenceEngineDone event); false to return immediately.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion when <paramref name="waitForSequenceCompletion"/> is <see langword="true"/>.</param>
        [Obsolete("This method is deprecated. Use overload without waitForSequenceCompletion parameter instead.")]
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            double[] voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceVoltageSequence(
                voltageSequence,
                sequenceTimeoutInSeconds,
                currentLimit,
                voltageLevelRange,
                currentLimitRange,
                sequenceLoopCount);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureVoltageSequence(DCPowerSessionsBundle, string, SiteData{double[]}, int, double?, bool, UpdateMode)"/>
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
        [Obsolete("This method is deprecated. Use overload without waitForSequenceCompletion parameter instead.")]
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceVoltageSequence(
                voltageSequence,
                sequenceTimeoutInSeconds,
                currentLimit,
                voltageLevelRange,
                currentLimitRange,
                sequenceLoopCount);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureVoltageSequence(DCPowerSessionsBundle, string, PinSiteData{double[]}, int, double?, bool, UpdateMode)"/>
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
        [Obsolete("This method is deprecated. Use overload without waitForSequenceCompletion parameter instead.")]
        public static void ForceVoltageSequence(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceVoltageSequence(
                voltageSequence,
                sequenceTimeoutInSeconds,
                currentLimit,
                voltageLevelRange,
                currentLimitRange,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            double[] voltageSequence,
            double? currentLimit = null,
            double? voltageLevelRange = null,
            double? currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceVoltageSequenceSynchronized(
                voltageSequence,
                sequenceTimeoutInSeconds,
                currentLimit,
                voltageLevelRange,
                currentLimitRange,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> voltageSequence,
            SiteData<double> currentLimit = null,
            SiteData<double> voltageLevelRange = null,
            SiteData<double> currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceVoltageSequenceSynchronized(
                voltageSequence,
                sequenceTimeoutInSeconds,
                currentLimit,
                voltageLevelRange,
                currentLimitRange,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceVoltageSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> voltageSequence,
            PinSiteData<double> currentLimit = null,
            PinSiteData<double> voltageLevelRange = null,
            PinSiteData<double> currentLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceVoltageSequenceSynchronized(
                voltageSequence,
                sequenceTimeoutInSeconds,
                currentLimit,
                voltageLevelRange,
                currentLimitRange,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            double[] currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceCurrentSequenceSynchronized(
                currentSequence,
                sequenceTimeoutInSeconds,
                voltageLimit,
                currentLevelRange,
                voltageLimitRange,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> currentSequence,
            SiteData<double> voltageLimit = null,
            SiteData<double> currentLevelRange = null,
            SiteData<double> voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceCurrentSequenceSynchronized(
                currentSequence,
                sequenceTimeoutInSeconds,
                voltageLimit,
                currentLevelRange,
                voltageLimitRange,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceCurrentSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> currentSequence,
            PinSiteData<double> voltageLimit = null,
            PinSiteData<double> currentLevelRange = null,
            PinSiteData<double> voltageLimitRange = null,
            double? sourceDelayInSeconds = null,
            DCPowerSourceTransientResponse? transientResponse = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceCurrentSequenceSynchronized(
                currentSequence,
                sequenceTimeoutInSeconds,
                voltageLimit,
                currentLevelRange,
                voltageLimitRange,
                sourceDelayInSeconds,
                transientResponse,
                sequenceLoopCount);
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
        /// <param name="sequenceTimeoutInSeconds">The timeout, in seconds, to wait for sequence completion.</param>
        [Obsolete("This method has been deprecated. Use the ForceAdvancedSequenceSynchronized() overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerSourceSettings[] sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0)
        {
            sessionsBundle.ForceAdvancedSequenceSynchronized(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerSourceSettings[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0)
        {
            sessionsBundle.ForceAdvancedSequenceSynchronized(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerSourceSettings[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0)
        {
            sessionsBundle.ForceAdvancedSequenceSynchronized(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount);
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
        /// <param name="sequenceTimeoutInSeconds">The timeout, in seconds, to wait for sequence completion.</param>
        /// <param name="pointsToFetch">The number of points to fetch.</param>
        /// <param name="measurementTimeoutInSeconds">The time to wait before the fetch measurement operation is aborted.</param>
        /// <returns>A <see cref="PinSiteData{T}"/> object that contains an array of <see cref="SingleDCPowerFetchResult"/> values,
        /// where each <see cref="SingleDCPowerFetchResult"/> object contains the voltage, current, and inCompliance result for a simple sample or point from the previous measurement.</returns>
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerSourceSettings[] sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            return sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double, int?, double)"/>
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerSourceSettings[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            return sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerSourceSettings[], int, bool, double, int?, double)"/>
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerSourceSettings[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            return sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount,
                pointsToFetch,
                measurementTimeoutInSeconds);
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
        /// <param name="sequenceTimeoutInSeconds">The timeout, in seconds, to wait for sequence completion.</param>
        /// <param name="pointsToFetch">The number of points to fetch.</param>
        /// <param name="measurementTimeoutInSeconds">The time to wait before the fetch measurement operation is aborted.</param>
        /// <returns>A <see cref="PinSiteData{T}"/> object that contains an array of <see cref="SingleDCPowerFetchResult"/> values,
        /// where each <see cref="SingleDCPowerFetchResult"/> object contains the voltage, current, and inCompliance result for a simple sample or point from the previous measurement.</returns>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerAdvancedSequenceStepProperties[] sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            return sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], int, bool, double, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            return sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount,
                pointsToFetch,
                measurementTimeoutInSeconds);
        }

        /// <inheritdoc cref="ForceAdvancedSequenceSynchronizedAndFetch(DCPowerSessionsBundle, DCPowerAdvancedSequenceStepProperties[], int, bool, double, int?, double)"/>
        public static PinSiteData<SingleDCPowerFetchResult[]> ForceAdvancedSequenceSynchronizedAndFetch(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0,
            int? pointsToFetch = null,
            double measurementTimeoutInSeconds = 10)
        {
            return sessionsBundle.ForceAdvancedSequenceSynchronizedAndFetch(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount,
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
        /// <param name="sequenceTimeoutInSeconds">The timeout, in seconds, to wait for sequence completion.</param>
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            DCPowerAdvancedSequenceStepProperties[] sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0)
        {
            sessionsBundle.ForceAdvancedSequenceSynchronized(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0)
        {
            sessionsBundle.ForceAdvancedSequenceSynchronized(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount);
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceAdvancedSequenceSynchronized(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<DCPowerAdvancedSequenceStepProperties[]> sequence,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = 5.0)
        {
            sessionsBundle.ForceAdvancedSequenceSynchronized(
                sequence,
                sequenceTimeoutInSeconds,
                sequenceLoopCount);
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
        /// <param name="voltageLimit">Voltage limit for the sequence.</param>
        /// <param name="currentLevelRange">Current level range.</param>
        /// <param name="voltageLimitRange">Voltage limit range.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="waitForSequenceCompletion">True to block until the sequence engine completes (waits on SequenceEngineDone event); false to return immediately.</param>
        /// <param name="sequenceTimeoutInSeconds">Maximum time to wait for completion when <paramref name="waitForSequenceCompletion"/> is true.</param>
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            double[] currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceCurrentSequence(
                currentSequence,
                sequenceTimeoutInSeconds,
                voltageLimit,
                currentLevelRange,
                voltageLimitRange,
                sequenceLoopCount);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureCurrentSequence(DCPowerSessionsBundle, string, SiteData{double[]}, int, double?, bool, UpdateMode)"/>
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            SiteData<double[]> currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceCurrentSequence(
                currentSequence,
                sequenceTimeoutInSeconds,
                voltageLimit,
                currentLevelRange,
                voltageLimitRange,
                sequenceLoopCount);
        }

        /// <remarks>
        /// This method does not support taking measurements during sequence execution, regardless of the state of the <see cref="DCPowerMeasurementWhen"/> property.<br/>
        /// If measurements are required, call <see cref="ConfigureCurrentSequence(DCPowerSessionsBundle, string, PinSiteData{double[]}, int, double?, bool, UpdateMode)"/>
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
        [Obsolete("This method has been deprecated. Use the overload without the waitForSequenceCompletion parameter instead.")]
        public static void ForceCurrentSequence(
            this DCPowerSessionsBundle sessionsBundle,
            PinSiteData<double[]> currentSequence,
            double? voltageLimit = null,
            double? currentLevelRange = null,
            double? voltageLimitRange = null,
            int sequenceLoopCount = 1,
            bool waitForSequenceCompletion = true,
            double sequenceTimeoutInSeconds = DefaultTimeout)
        {
            sessionsBundle.ForceCurrentSequence(
                currentSequence,
                sequenceTimeoutInSeconds,
                voltageLimit,
                currentLevelRange,
                voltageLimitRange,
                sequenceLoopCount);
        }

        /// <summary>
        /// Configures a hardware-timed sequence of values.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <param name="sequence">The voltage or current sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        [Obsolete("Using both simple sequencing and advanced sequencing for the same channel within the same session is not supported. For this reason it is better to just use advanced sequencing. This method does not support configuring ganged pin groups for sequencing. Consider using either ConfigureVoltageSequence or ConfigureCurrentSequence instead.", error: false)]
        public static void ConfigureSequence(this DCPowerSessionsBundle sessionsBundle, double[] sequence, int sequenceLoopCount, double? sequenceStepDeltaTimeInSeconds = null)
        {
            sessionsBundle.ValidateNoChannelGanged();
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Control.Abort();
                sessionInfo.AllChannelsOutput.ConfigureSequence(sequence, sequenceLoopCount, sequenceStepDeltaTimeInSeconds);
            });
        }

        /// <summary>
        /// Gets the current limit of all pins.
        /// </summary>
        /// <param name="sessionsBundle">The <see cref="DCPowerSessionsBundle"/> object.</param>
        /// <returns>The per-site per-pin current limits.</returns>
         [Obsolete("This method has been deprecated. Use the GetCurrentLimit method instead.")]
        public static PinSiteData<double> GetCurrentLimits(this DCPowerSessionsBundle sessionsBundle)
        {
            return sessionsBundle.GetCurrentLimit();
        }

        #endregion Obsolete methods on DCPowerSessionsBundle

        #region Obsolete methods on DCPowerOutput

        /// <summary>
        /// Configures a hardware-timed sequence of values.
        /// </summary>
        /// <param name="output">The <see cref="DCPowerOutput"/> object.</param>
        /// <param name="sequence">The voltage or current sequence to set.</param>
        /// <param name="sequenceLoopCount">The number of loops a sequence runs after initiation.</param>
        /// <param name="sequenceStepDeltaTimeInSeconds">The delta time between the start of two consecutive steps in a sequence.</param>
        /// <param name="sitePinInfo">The <see cref="SitePinInfo"/> object.</param>
        [Obsolete("Using both simple sequencing and advanced sequencing for the same channel within the same session is not supported. For this reason it is better to just use advanced sequencing. This method does not support configuring ganged pin groups for sequencing. Consider using the high-level ConfigureVoltageSequence or ConfigureCurrentSequence methods instead.", error: false)]
        public static void ConfigureSequence(
            this DCPowerOutput output,
            double[] sequence,
            int sequenceLoopCount,
            double? sequenceStepDeltaTimeInSeconds = null,
            SitePinInfo sitePinInfo = null)
        {
            if (sitePinInfo?.CascadingInfo is GangingInfo)
            {
                throw new NISemiconductorTestException(string.Format(CultureInfo.InvariantCulture, ResourceStrings.DCPower_GangedPinGroupDetected));
            }

            output.Source.Mode = DCPowerSourceMode.Sequence;
            output.Source.SequenceLoopCount = sequenceLoopCount;
            output.Source.SetSequence(sequence);
            if (sequenceStepDeltaTimeInSeconds.HasValue)
            {
                output.Source.SequenceStepDeltaTimeEnabled = true;
                output.Source.SequenceStepDeltaTime = PrecisionTimeSpan.FromSeconds(sequenceStepDeltaTimeInSeconds.Value);
            }
        }

        #endregion Obsolete methods on DCPowerOutput
    }
}
