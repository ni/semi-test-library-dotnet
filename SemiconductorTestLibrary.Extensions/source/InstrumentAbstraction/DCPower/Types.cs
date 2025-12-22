using NationalInstruments.ModularInstruments.NIDCPower;

namespace NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower
{
    /// <summary>
    /// Defines DCPower settings.
    /// </summary>
    public class DCPowerSettings
    {
        /// <summary>
        /// The source settings.
        /// </summary>
        public DCPowerSourceSettings SourceSettings { get; set; }

        /// <summary>
        /// The measure settings.
        /// </summary>
        public DCPowerMeasureSettings MeasureSettings { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCPowerSettings()
        {
        }
    }

    /// <summary>
    /// Defines DCPower source settings.
    /// </summary>
    public class DCPowerSourceSettings
    {
        /// <summary>
        /// The output function.
        /// </summary>
        public DCPowerSourceOutputFunction? OutputFunction { get; set; } = DCPowerSourceOutputFunction.DCVoltage;

        /// <summary>
        /// The limit symmetry.
        /// </summary>
        public DCPowerComplianceLimitSymmetry? LimitSymmetry { get; set; } = DCPowerComplianceLimitSymmetry.Symmetric;

        /// <summary>
        /// The voltage or current level.
        /// </summary>
        public double? Level { get; set; }

        /// <summary>
        /// The current or voltage limit.
        /// </summary>
        public double? Limit { get; set; }

        /// <summary>
        /// The current or voltage high limit.
        /// </summary>
        public double? LimitHigh { get; set; }

        /// <summary>
        /// The current or voltage low limit.
        /// </summary>
        public double? LimitLow { get; set; }

        /// <summary>
        /// The voltage or current level range.
        /// </summary>
        public double? LevelRange { get; set; }

        /// <summary>
        /// The current or voltage limit range.
        /// </summary>
        public double? LimitRange { get; set; }

        /// <summary>
        /// The source delay in seconds.
        /// </summary>
        public double? SourceDelayInSeconds { get; set; }

        /// <summary>
        /// The transient response.
        /// </summary>
        public DCPowerSourceTransientResponse? TransientResponse { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCPowerSourceSettings()
        {
        }
    }

    /// <summary>
    /// Defines DCPower measure settings.
    /// </summary>
    public class DCPowerMeasureSettings
    {
        /// <summary>
        /// The aperture time.
        /// </summary>
        public double? ApertureTime { get; set; }

        /// <summary>
        /// The unit of aperture time.
        /// </summary>
        public DCPowerMeasureApertureTimeUnits? ApertureTimeUnits { get; set; }

        /// <summary>
        /// The measure when.
        /// </summary>
        public DCPowerMeasurementWhen? MeasureWhen { get; set; }

        /// <summary>
        /// The measurement sense.
        /// </summary>
        public DCPowerMeasurementSense? Sense { get; set; }

        /// <summary>
        /// The record length.
        /// </summary>
        public int? RecordLength { get; set; }

        /// <summary>
        /// Default constructor.
        /// </summary>
        public DCPowerMeasureSettings()
        {
        }
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

        /// <summary>
        /// Creates an object to define and set DCPower waveform acquisition specific settings.
        /// This object is used to store waveform acquisition settings of a DCPower channel. <see cref="Measure.ConfigureAndStartWaveformAcquisition(DCPowerSessionsBundle, double, double)"/>
        /// </summary>
        public DCPowerWaveformAcquisitionSettings()
        {
        }
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

    /// <summary>
    /// Defines per-step properties for DCPower advanced sequence configuration.
    /// Each nullable property corresponds to a DCPowerAdvancedSequenceProperty that can vary per step.
    /// Properties are organized by functional category for ease of use.
    /// </summary>
    public class DCPowerAdvancedSequenceStepProperties
    {
        #region Measurement Properties

        /// <summary>
        /// Specifies the measurement aperture time, in seconds, for the channel configuration.
        /// </summary>
        public double? ApertureTime { get; set; }

        /// <summary>
        /// Specifies whether the hardware automatically selects the best range to measure the signal.
        /// </summary>
        public DCPowerMeasurementAutorange? Autorange { get; set; }

        /// <summary>
        /// Specifies whether the aperture time used for the measurement autorange algorithm is determined automatically or customized.
        /// </summary>
        public DCPowerMeasurementAutorangeApertureTimeMode? AutorangeApertureTimeMode { get; set; }

        /// <summary>
        /// Specifies the algorithm the hardware uses for measurement autoranging.
        /// </summary>
        public DCPowerMeasurementAutorangeBehavior? AutorangeBehavior { get; set; }

        /// <summary>
        /// Balances between settling time and maximum measurement time by specifying the maximum time delay between when a range change occurs and when measurements resume.
        /// </summary>
        public double? AutorangeMaximumDelayAfterRangeChange { get; set; }

        /// <summary>
        /// Specifies the autorange aperture time.
        /// </summary>
        public double? AutorangeMinimumApertureTime { get; set; }

        /// <summary>
        /// Specifies the lowest current range used during measurement autoranging.
        /// </summary>
        public double? AutorangeMinimumCurrentRange { get; set; }

        /// <summary>
        /// Specifies the lowest voltage range used during measurement autoranging.
        /// </summary>
        public double? AutorangeMinimumVoltageRange { get; set; }

        /// <summary>
        /// Specifies the relative weighting of samples in a measurement.
        /// </summary>
        public DCPowerMeasurementDCNoiseRejection? DCNoiseRejection { get; set; }

        /// <summary>
        /// Specifies how many measurements compose a measure record.
        /// </summary>
        public int? MeasureRecordLength { get; set; }

        /// <summary>
        /// Specifies the type of remote sensing for the specified channels.
        /// </summary>
        public DCPowerMeasurementSense? Sense { get; set; }

        #endregion

        #region DC Voltage Source Properties

        /// <summary>
        /// Specifies the voltage level, in volts, that the device attempts to generate on the specified channels.
        /// </summary>
        public double? VoltageLevel { get; set; }

        /// <summary>
        /// Specifies the voltage level range, in volts, for the specified channels.
        /// </summary>
        public double? VoltageLevelRange { get; set; }

        /// <summary>
        /// Specifies the voltage limit for the output to not exceed when generating the desired current level.
        /// </summary>
        public double? VoltageLimit { get; set; }

        /// <summary>
        /// Specifies the voltage limit high for the output to not exceed when generating the desired current level.
        /// </summary>
        public double? VoltageLimitHigh { get; set; }

        /// <summary>
        /// Specifies the voltage limit low for the output to not exceed when generating the desired current level.
        /// </summary>
        public double? VoltageLimitLow { get; set; }

        /// <summary>
        /// Specifies the voltage limit range, in volts, for the specified channels.
        /// </summary>
        public double? VoltageLimitRange { get; set; }

        /// <summary>
        /// Specifies the frequency at which a pole-zero pair is added to the system.
        /// </summary>
        public double? VoltageCompensationFrequency { get; set; }

        /// <summary>
        /// Specifies the frequency at which the unloaded loop gain extrapolates to 0 dB.
        /// </summary>
        public double? VoltageGainBandwidth { get; set; }

        /// <summary>
        /// Specifies the ratio of the pole frequency to the zero frequency.
        /// </summary>
        public double? VoltagePoleZeroRatio { get; set; }

        #endregion

        #region DC Current Source Properties

        /// <summary>
        /// Specifies the current level, in amperes, that the device attempts to generate on the specified channels.
        /// </summary>
        public double? CurrentLevel { get; set; }

        /// <summary>
        /// Specifies the current level range, in amperes, for the specified channels.
        /// </summary>
        public double? CurrentLevelRange { get; set; }

        /// <summary>
        /// Specifies the current limit, in amperes, for the output not to exceed when generating the desired voltage level.
        /// </summary>
        public double? CurrentLimit { get; set; }

        /// <summary>
        /// Specifies the current limit high, in amperes, for the output not to exceed when generating the desired voltage level.
        /// </summary>
        public double? CurrentLimitHigh { get; set; }

        /// <summary>
        /// Specifies the current limit low, in amperes, for the output not to exceed when generating the desired voltage level.
        /// </summary>
        public double? CurrentLimitLow { get; set; }

        /// <summary>
        /// Specifies the current limit range, in amperes, for the specified channels.
        /// </summary>
        public double? CurrentLimitRange { get; set; }

        /// <summary>
        /// Specifies the frequency at which a pole-zero pair is added to the system.
        /// </summary>
        public double? CurrentCompensationFrequency { get; set; }

        /// <summary>
        /// Specifies the frequency at which the unloaded loop gain extrapolates to 0 dB.
        /// </summary>
        public double? CurrentGainBandwidth { get; set; }

        /// <summary>
        /// Specifies the ratio of the pole frequency to the zero frequency.
        /// </summary>
        public double? CurrentPoleZeroRatio { get; set; }

        /// <summary>
        /// Specifies the rate of increase, in amps per microsecond, to apply to the absolute magnitude of the current level of a channel.
        /// </summary>
        public double? CurrentLevelRisingSlewRate { get; set; }

        /// <summary>
        /// Specifies the rate of decrease, in amps per microsecond, to apply to the absolute magnitude of the current level of a channel.
        /// </summary>
        public double? CurrentLevelFallingSlewRate { get; set; }

        #endregion

        #region Pulse Voltage Source Properties

        /// <summary>
        /// Specifies the voltage level, in volts, that the device attempts to generate during the on phase of a pulse.
        /// </summary>
        public double? PulseVoltageLevel { get; set; }

        /// <summary>
        /// Specifies the pulse voltage level range, in volts, for the specified channels.
        /// </summary>
        public double? PulseVoltageLevelRange { get; set; }

        /// <summary>
        /// Specifies the pulse voltage limit, in volts, that the output cannot exceed when generating the desired pulse current during the on phase.
        /// </summary>
        public double? PulseVoltageLimit { get; set; }

        /// <summary>
        /// Specifies the pulse voltage limit high, in volts, that the output cannot exceed when generating the desired pulse current during the on phase.
        /// </summary>
        public double? PulseVoltageLimitHigh { get; set; }

        /// <summary>
        /// Specifies the pulse voltage limit low, in volts, that the output cannot exceed when generating the desired pulse current during the on phase.
        /// </summary>
        public double? PulseVoltageLimitLow { get; set; }

        /// <summary>
        /// Specifies the pulse voltage limit range, in volts, for the specified channels.
        /// </summary>
        public double? PulseVoltageLimitRange { get; set; }

        /// <summary>
        /// Specifies the voltage level, in volts, that the device attempts to generate during the off phase of a pulse.
        /// </summary>
        public double? PulseBiasVoltageLevel { get; set; }

        /// <summary>
        /// Specifies the pulse voltage limit, in volts, that the output cannot exceed when generating the desired current during the off phase.
        /// </summary>
        public double? PulseBiasVoltageLimit { get; set; }

        /// <summary>
        /// Specifies the pulse voltage limit high, in volts, that the output cannot exceed during the off phase.
        /// </summary>
        public double? PulseBiasVoltageLimitHigh { get; set; }

        /// <summary>
        /// Specifies the pulse voltage limit low, in volts, that the output cannot exceed during the off phase.
        /// </summary>
        public double? PulseBiasVoltageLimitLow { get; set; }

        #endregion

        #region Pulse Current Source Properties

        /// <summary>
        /// Specifies the pulse current level, in amperes, that the device attempts to generate during the on phase of a pulse.
        /// </summary>
        public double? PulseCurrentLevel { get; set; }

        /// <summary>
        /// Specifies the pulse current level range, in amperes, for the specified channels.
        /// </summary>
        public double? PulseCurrentLevelRange { get; set; }

        /// <summary>
        /// Specifies the current limit, in amps, that the output cannot exceed when generating the desired voltage during the on phase.
        /// </summary>
        public double? PulseCurrentLimit { get; set; }

        /// <summary>
        /// Specifies the current limit high, in amps, that the output cannot exceed when generating the desired voltage during the on phase.
        /// </summary>
        public double? PulseCurrentLimitHigh { get; set; }

        /// <summary>
        /// Specifies the current limit low, in amps, that the output cannot exceed when generating the desired voltage during the on phase.
        /// </summary>
        public double? PulseCurrentLimitLow { get; set; }

        /// <summary>
        /// Specifies the current limit range, in amperes, for the specified channels.
        /// </summary>
        public double? PulseCurrentLimitRange { get; set; }

        /// <summary>
        /// Specifies the pulse bias current level, in amperes, that the device attempts to generate during the off phase of a pulse.
        /// </summary>
        public double? PulseBiasCurrentLevel { get; set; }

        /// <summary>
        /// Specifies the current limit, in amperes, that the output cannot exceed when generating the desired voltage during the off phase.
        /// </summary>
        public double? PulseBiasCurrentLimit { get; set; }

        /// <summary>
        /// Specifies the current limit high, in amperes, that the output cannot exceed during the off phase.
        /// </summary>
        public double? PulseBiasCurrentLimitHigh { get; set; }

        /// <summary>
        /// Specifies the current limit low, in amperes, that the output cannot exceed during the off phase.
        /// </summary>
        public double? PulseBiasCurrentLimitLow { get; set; }

        #endregion

        #region Pulse Timing Properties

        /// <summary>
        /// Specifies the length, in seconds, of the on phase of a pulse.
        /// </summary>
        public double? PulseOnTime { get; set; }

        /// <summary>
        /// Specifies the length, in seconds, of the off phase of a pulse.
        /// </summary>
        public double? PulseOffTime { get; set; }

        /// <summary>
        /// Specifies the time, in seconds, when the device generates the PulseCompleteEvent.
        /// </summary>
        public double? PulseBiasDelay { get; set; }

        #endregion

        #region Source and Timing Properties

        /// <summary>
        /// Specifies the time, in seconds, when the device generates the SourceCompleteEvent.
        /// </summary>
        public double? SourceDelay { get; set; }

        /// <summary>
        /// Specifies the amount of time, in seconds, between the start of two consecutive steps in a sequence.
        /// </summary>
        public double? SequenceStepDeltaTime { get; set; }

        /// <summary>
        /// Specifies the transient response.
        /// </summary>
        public DCPowerSourceTransientResponse? TransientResponse { get; set; }

        #endregion

        #region Output and Compliance Properties

        /// <summary>
        /// Enables the output.
        /// </summary>
        public bool? OutputEnabled { get; set; }

        /// <summary>
        /// Specifies the method to generate on the specified channels.
        /// </summary>
        public DCPowerSourceOutputFunction? OutputFunction { get; set; }

        /// <summary>
        /// Specifies the output resistance that the device attempts to generate for the specified channels.
        /// </summary>
        public double? OutputResistance { get; set; }

        /// <summary>
        /// Specifies whether compliance limits are applied symmetrically about 0 V and 0 A or asymmetrically.
        /// </summary>
        public DCPowerComplianceLimitSymmetry? ComplianceLimitSymmetry { get; set; }

        #endregion

        #region OVP Properties

        /// <summary>
        /// Gets or sets whether to enable or disable overvoltage protection (OVP).
        /// </summary>
        public bool? OvpEnabled { get; set; }

        /// <summary>
        /// Determines the voltage limit, in volts, beyond which overvoltage protection (OVP) engages.
        /// </summary>
        public double? OvpLimit { get; set; }

        #endregion

        #region LCR Properties

        /// <summary>
        /// Specifies the amplitude, in amps (A RMS), of the AC current test signal applied to the DUT for LCR measurements.
        /// </summary>
        public double? LcrCurrentAmplitude { get; set; }

        /// <summary>
        /// Specifies the amplitude, in volts (V RMS), of the AC voltage test signal applied to the DUT for LCR measurements.
        /// </summary>
        public double? LcrVoltageAmplitude { get; set; }

        /// <summary>
        /// Specifies the frequency used by the AC stimulus in LCR mode.
        /// </summary>
        public double? LcrFrequency { get; set; }

        /// <summary>
        /// Specifies the bias current level in LCR mode in amps when DCBiasSource is set to Current.
        /// </summary>
        public double? LcrDcBiasCurrentLevel { get; set; }

        /// <summary>
        /// Specifies the bias voltage level in LCR mode in volts when DCBiasSource is set to Voltage.
        /// </summary>
        public double? LcrDcBiasVoltageLevel { get; set; }

        /// <summary>
        /// Specifies the impedance of the load when the ImpedanceAutoRange attribute is set to Off in LCR mode.
        /// </summary>
        public double? LcrImpedanceRange { get; set; }

        /// <summary>
        /// Specifies how the impedance range for LCR measurements is determined.
        /// </summary>
        public DCPowerLcrImpedanceRangeSource? LcrImpedanceRangeSource { get; set; }

        /// <summary>
        /// Specifies the aperture time of LCR measurements.
        /// </summary>
        public DCPowerLcrMeasurementTime? LcrMeasurementTime { get; set; }

        /// <summary>
        /// Specifies the LCR measurement aperture time for a channel, in seconds, when the MeasurementTime property is set to Custom.
        /// </summary>
        public double? LcrCustomMeasurementTime { get; set; }

        /// <summary>
        /// Specifies the LCR source aperture time for a channel, in seconds.
        /// </summary>
        public double? LcrSourceApertureTime { get; set; }

        /// <summary>
        /// Specifies whether AC Voltage or AC Current stimulus is used in LCR mode.
        /// </summary>
        public DCPowerLcrStimulusFunction? LcrStimulusFunction { get; set; }

        /// <summary>
        /// Specifies how to set the DC bias in LCR mode.
        /// </summary>
        public DCPowerLcrDcBiasSource? LcrDcBiasSource { get; set; }

        /// <summary>
        /// Specifies whether to apply open LCR compensation data to LCR measurements.
        /// </summary>
        public bool? LcrOpenCompensationEnabled { get; set; }

        /// <summary>
        /// Specifies whether to apply short LCR compensation data to LCR measurements.
        /// </summary>
        public bool? LcrShortCompensationEnabled { get; set; }

        /// <summary>
        /// Specifies whether to apply load LCR compensation data to LCR measurements.
        /// </summary>
        public bool? LcrLoadCompensationEnabled { get; set; }

        #endregion

        #region Instrument Mode

        /// <summary>
        /// Specifies the mode of operation for an instrument channel for instruments that support multiple modes.
        /// </summary>
        public DCPowerInstrumentMode? InstrumentMode { get; set; }

        #endregion

        /// <summary>
        /// Default constructor for DCPowerAdvancedSequenceStepProperties.
        /// </summary>
        public DCPowerAdvancedSequenceStepProperties()
        {
        }
    }

    /// <summary>
    /// Structure that defines the result of a single fetch operation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1815:Override equals and operator equals on value types", Justification = "Not needed now, may implement in future")]
    public readonly struct SingleDCPowerFetchResult
    {
        /// <summary>
        /// Constructs a SingleDCPowerFetchResult object.
        /// </summary>
        /// <param name="voltageMeasurement">Voltage measurement value</param>
        /// <param name="currentMeasurement">Current measurement value.</param>
        /// <param name="inCompliance">Whether the output is in compliance mode when measurement is taken.</param>
        public SingleDCPowerFetchResult(double voltageMeasurement, double currentMeasurement, bool inCompliance)
        {
            VoltageMeasurement = voltageMeasurement;
            CurrentMeasurement = currentMeasurement;
            InCompliance = inCompliance;
        }

        /// <summary>
        /// Voltage measurement value.
        /// </summary>
        public double VoltageMeasurement { get; }

        /// <summary>
        /// Current measurement value.
        /// </summary>
        public double CurrentMeasurement { get; }

        /// <summary>
        /// Whether the output was in compliance mode when measurement was taken.
        /// </summary>
        public bool InCompliance { get; }
    }

    /// <summary>
    /// Defines DCPower trigger type.
    /// </summary>
    public enum TriggerType
    {
        /// <summary>
        /// The measure trigger.
        /// </summary>
        MeasureTrigger,

        /// <summary>
        /// The pulse trigger.
        /// </summary>
        PulseTrigger,

        /// <summary>
        /// The sequence advance trigger.
        /// </summary>
        SequenceAdvanceTrigger,

        /// <summary>
        /// The source trigger.
        /// </summary>
        SourceTrigger,

        /// <summary>
        /// The start trigger.
        /// </summary>
        StartTrigger,

        /// <summary>
        /// The shutdown trigger.
        /// </summary>
        ShutdownTrigger
    }

    /// <summary>
    /// Defines DCPower event type.
    /// </summary>
    public enum EventType
    {
        /// <summary>
        /// The measure complete event.
        /// </summary>
        MeasureCompleteEvent,

        /// <summary>
        /// The pulse complete event.
        /// </summary>
        PulseCompleteEvent,

        /// <summary>
        /// The ready for pulse trigger event.
        /// </summary>
        ReadyForPulseTriggerEvent,

        /// <summary>
        /// The sequence engine done event.
        /// </summary>
        SequenceEngineDoneEvent,

        /// <summary>
        /// The sequence iteration complete event.
        /// </summary>
        SequenceIterationCompleteEvent,

        /// <summary>
        /// The source complete event.
        /// </summary>
        SourceCompleteEvent
    }
}
