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
        /// Clones the current DCPower source settings.
        /// </summary>
        public DCPowerSourceSettings Clone()
        {
            return (DCPowerSourceSettings)this.MemberwiseClone();
        }

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
        #region Class Properties

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
        /// Specifies the algorithm the hardware uses for measurement AutoRanging.
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
        public DCPowerMeasurementNoiseRejection? DCNoiseRejection { get; set; }

        /// <summary>
        /// Specifies how many measurements compose a measure record.
        /// </summary>
        public int? MeasureRecordLength { get; set; }

        /// <summary>
        /// Specifies the type of remote sensing for the specified channels.
        /// </summary>
        public DCPowerMeasurementSense? Sense { get; set; }

        /// <summary>
        /// Specifies the mode used to determine the threshold for autoranging during DC power measurement.
        /// </summary>
        public DCPowerMeasurementAutorangeThresholdMode? AutorangeThresholdMode { get; set; }

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
        /// Specifies whether compliance limits are applied symmetrically.
        /// </summary>
        public DCPowerComplianceLimitSymmetry? ComplianceLimitSymmetry { get; set; }

        /// <summary>
        /// Specifies the conduction voltage mode for DC power operations.
        /// </summary>
        public DCPowerConductionVoltageMode? ConductionVoltageMode { get; set; }

        /// <summary>
        /// Voltage threshold at which conduction is considered to be off.
        /// </summary>
        public double? ConductionVoltageOffThreshold { get; set; }

        /// <summary>
        /// Voltage threshold at which conduction is considered to be on.
        /// </summary>
        public double? ConductionVoltageOnThreshold { get; set; }

        /// <summary>
        /// Units for the minimum aperture time used during AutoRanging.
        /// </summary>
        public DCPowerMeasureApertureTimeUnits? AutorangeMinimumApertureTimeUnits { get; set; }

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
        /// Actual load reactance value for the LCR.
        /// </summary>
        public double? LcrActualLoadReactance { get; set; }

        /// <summary>
        /// Actual load resistance measured in the LCR.
        /// </summary>
        public double? LcrActualLoadResistance { get; set; }

        /// <summary>
        /// Current range value for LCR.
        /// </summary>
        public double? LcrCurrentRange { get; set; }

        /// <summary>
        /// DC bias current range for LCR.
        /// </summary>
        public double? LcrDcBiasCurrentRange { get; set; }

        /// <summary>
        /// DC bias transient response settings for LCR.
        /// </summary>
        public DCPowerLCRDCBiasTransientResponse? LcrDcBiasTransientResponse { get; set; }

        /// <summary>
        /// DC bias voltage range for LCR measurements.
        /// </summary>
        public double? LcrDcBiasVoltageRange { get; set; }

        /// <summary>
        /// Specifies whether the LCR impedance measurement uses AutoRanging.
        /// </summary>
        public DCPowerLCRImpedanceAutorange? LcrImpedanceAutoRange { get; set; }

        /// <summary>
        /// Specifies the load capacitance value for LCR.
        /// </summary>
        public double? LcrLoadCapacitance { get; set; }

        /// <summary>
        /// Gets or sets the inductance value of the LCR load.
        /// </summary>
        public double? LcrLoadInductance { get; set; }

        /// <summary>
        /// Load resistance value for the LCR.
        /// </summary>
        public double? LcrLoadResistance { get; set; }

        /// <summary>
        /// Specifies the measured load reactance for LCR.
        /// </summary>
        public double? LcrMeasuredLoadReactance { get; set; }

        /// <summary>
        /// Gets or sets the measured load resistance value from the LCR measurement.
        /// </summary>
        public double? LcrMeasuredLoadResistance { get; set; }

        /// <summary>
        /// Measured open conductance value from the LCR.
        /// </summary>
        public double? LcrOpenConductance { get; set; }

        /// <summary>
        /// Open-circuit susceptance value for the LCR component.
        /// </summary>
        public double? LcrOpenSusceptance { get; set; }

        /// <summary>
        /// Gets or sets the short-circuit reactance value for the LCR component.
        /// </summary>
        public double? LcrShortReactance { get; set; }

        /// <summary>
        /// Gets or sets the measured short resistance value from the LCR.
        /// </summary>
        public double? LcrShortResistance { get; set; }

        /// <summary>
        /// Gets or sets the delay mode for the LCR source.
        /// </summary>
        public DCPowerLCRSourceDelayMode? LcrSourceDelayMode { get; set; }

        /// <summary>
        /// Gets or sets the voltage range used for LCR measurements.
        /// </summary>
        public double? LcrVoltageRange { get; set; }
        /// <summary>
        /// Specifies how the impedance range for LCR measurements is determined.
        /// </summary>
        public DCPowerLCRImpedanceRangeSource? LcrImpedanceRangeSource { get; set; }

        /// <summary>
        /// Specifies the aperture time of LCR measurements.
        /// </summary>
        public DCPowerLCRMeasurementTime? LcrMeasurementTime { get; set; }

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
        public DCPowerLCRStimulusFunction? LcrStimulusFunction { get; set; }

        /// <summary>
        /// Specifies how to set the DC bias in LCR mode.
        /// </summary>
        public DCPowerLCRDCBiasSource? LcrDcBiasSource { get; set; }

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

        #endregion

        /// <summary>
        /// Default constructor for DCPowerAdvancedSequenceStepProperties.
        /// </summary>
        public DCPowerAdvancedSequenceStepProperties()
        {
        }

        #region Apply To Method

        internal void ApplyTo(DCPowerOutput channelOutput)
        {
            // Measurement Properties
            if (ApertureTime.HasValue)
            {
                channelOutput.Measurement.ApertureTime = ApertureTime.Value;
            }
            if (DCNoiseRejection.HasValue)
            {
                channelOutput.Measurement.NoiseRejection = DCNoiseRejection.Value;
            }
            if (Autorange.HasValue)
            {
                channelOutput.Measurement.Autorange = Autorange.Value;
            }
            if (Sense.HasValue)
            {
                channelOutput.Measurement.Sense = Sense.Value;
            }
            if (VoltageLevel.HasValue)
            {
                channelOutput.Source.Voltage.VoltageLevel = VoltageLevel.Value;
            }
            if (VoltageLevelRange.HasValue)
            {
                channelOutput.Source.Voltage.VoltageLevelRange = VoltageLevelRange.Value;
            }
            if (CurrentLimit.HasValue)
            {
                channelOutput.Source.Voltage.CurrentLimit = CurrentLimit.Value;
            }
            if (CurrentLimitRange.HasValue)
            {
                channelOutput.Source.Voltage.CurrentLimitRange = CurrentLimitRange.Value;
            }
            // DC Current Properties
            if (CurrentLevel.HasValue)
            {
                channelOutput.Source.Current.CurrentLevel = CurrentLevel.Value;
            }
            if (CurrentLevelRange.HasValue)
            {
                channelOutput.Source.Current.CurrentLevelRange = CurrentLevelRange.Value;
            }
            if (VoltageLimit.HasValue)
            {
                channelOutput.Source.Current.VoltageLimit = VoltageLimit.Value;
            }
            if (VoltageLimitRange.HasValue)
            {
                channelOutput.Source.Current.VoltageLimitRange = VoltageLimitRange.Value;
            }
            if (PulseVoltageLevel.HasValue)
            {
                channelOutput.Source.PulseVoltage.VoltageLevel = PulseVoltageLevel.Value;
            }
            if (PulseVoltageLevelRange.HasValue)
            {
                channelOutput.Source.PulseVoltage.VoltageLevelRange = PulseVoltageLevelRange.Value;
            }
            if (PulseVoltageLimit.HasValue)
            {
                channelOutput.Source.PulseVoltage.CurrentLimit = PulseVoltageLimit.Value;
            }
            if (PulseVoltageLimitRange.HasValue)
            {
                channelOutput.Source.PulseVoltage.CurrentLimitRange = PulseVoltageLimitRange.Value;
            }

            // Pulse Current Properties

            if (PulseCurrentLevel.HasValue)
            {
                channelOutput.Source.PulseCurrent.CurrentLevel = PulseCurrentLevel.Value;
            }
            if (PulseCurrentLevelRange.HasValue)
            {
                channelOutput.Source.PulseCurrent.CurrentLevelRange = PulseCurrentLevelRange.Value;
            }
            if (PulseCurrentLimit.HasValue)
            {
                channelOutput.Source.PulseCurrent.VoltageLimit = PulseCurrentLimit.Value;
            }
            if (PulseCurrentLimitRange.HasValue)
            {
                channelOutput.Source.PulseCurrent.VoltageLimitRange = PulseCurrentLimitRange.Value;
            }
            if (PulseBiasVoltageLevel.HasValue)
            {
                channelOutput.Source.PulseVoltage.BiasVoltageLevel = PulseBiasVoltageLevel.Value;
            }
            if (PulseBiasCurrentLevel.HasValue)
            {
                channelOutput.Source.PulseCurrent.BiasCurrentLevel = PulseBiasCurrentLevel.Value;
            }
            if (SourceDelay.HasValue)
            {
                channelOutput.Source.SourceDelay = PrecisionTimeSpan.FromSeconds(SourceDelay.Value);
            }
            if (TransientResponse.HasValue)
            {
                channelOutput.Source.TransientResponse = TransientResponse.Value;
            }
            if (MeasureRecordLength.HasValue)
            {
                channelOutput.Measurement.RecordLength = MeasureRecordLength.Value;
            }
            if (OutputEnabled.HasValue)
            {
                channelOutput.Source.Output.Enabled = OutputEnabled.Value;
            }
            if (OutputFunction.HasValue)
            {
                channelOutput.Source.Output.Function = OutputFunction.Value;
            }
            if (ComplianceLimitSymmetry.HasValue)
            {
                channelOutput.Source.ComplianceLimitSymmetry = ComplianceLimitSymmetry.Value;
            }
            if (OvpEnabled.HasValue)
            {
                channelOutput.Source.OvpEnabled = OvpEnabled.Value;
            }
            if (OvpLimit.HasValue)
            {
                channelOutput.Source.OvpLimit = OvpLimit.Value;
            }
            if (LcrCurrentAmplitude.HasValue)
            {
                channelOutput.LCR.CurrentAmplitude = LcrCurrentAmplitude.Value;
            }
            if (LcrVoltageAmplitude.HasValue)
            {
                channelOutput.LCR.VoltageAmplitude = LcrVoltageAmplitude.Value;
            }
            if (LcrFrequency.HasValue)
            {
                channelOutput.LCR.Frequency = LcrFrequency.Value;
            }
            if (LcrDcBiasCurrentLevel.HasValue)
            {
                channelOutput.LCR.DCBiasCurrentLevel = LcrDcBiasCurrentLevel.Value;
            }
            if (LcrDcBiasVoltageLevel.HasValue)
            {
                channelOutput.LCR.DCBiasVoltageLevel = LcrDcBiasVoltageLevel.Value;
            }
            if (LcrImpedanceRange.HasValue)
            {
                channelOutput.LCR.ImpedanceRange = LcrImpedanceRange.Value;
            }
            if (LcrImpedanceRangeSource.HasValue)
            {
                channelOutput.LCR.ImpedanceRangeSource = LcrImpedanceRangeSource.Value;
            }
            if (LcrMeasurementTime.HasValue)
            {
                channelOutput.LCR.MeasurementTime = LcrMeasurementTime.Value;
            }
            if (LcrCustomMeasurementTime.HasValue)
            {
                channelOutput.LCR.CustomMeasurementTime = LcrCustomMeasurementTime.Value;
            }
            if (LcrSourceApertureTime.HasValue)
            {
                channelOutput.LCR.Advanced.SourceApertureTime = LcrSourceApertureTime.Value;
            }
            if (LcrStimulusFunction.HasValue)
            {
                channelOutput.LCR.StimulusFunction = LcrStimulusFunction.Value;
            }
            if (LcrDcBiasSource.HasValue)
            {
                channelOutput.LCR.DCBiasSource = LcrDcBiasSource.Value;
            }
            if (LcrOpenCompensationEnabled.HasValue)
            {
                channelOutput.LCR.Compensation.OpenCompensationEnabled = LcrOpenCompensationEnabled.Value;
            }
            if (LcrShortCompensationEnabled.HasValue)
            {
                channelOutput.LCR.Compensation.ShortCompensationEnabled = LcrShortCompensationEnabled.Value;
            }
            if (LcrLoadCompensationEnabled.HasValue)
            {
                channelOutput.LCR.Compensation.LoadCompensationEnabled = LcrLoadCompensationEnabled.Value;
            }
            if (AutorangeThresholdMode.HasValue)
            {
                channelOutput.Measurement.AutorangeThresholdMode = AutorangeThresholdMode.Value;
            }
            if (AutorangeApertureTimeMode.HasValue)
            {
                channelOutput.Measurement.AutorangeApertureTimeMode = AutorangeApertureTimeMode.Value;
            }
            if (AutorangeBehavior.HasValue)
            {
                channelOutput.Measurement.AutorangeBehavior = AutorangeBehavior.Value;
            }
            if (AutorangeMaximumDelayAfterRangeChange.HasValue)
            {
                channelOutput.Measurement.AutorangeMaximumDelayAfterRangeChange = AutorangeMaximumDelayAfterRangeChange.Value;
            }
            if (AutorangeMinimumApertureTime.HasValue)
            {
                channelOutput.Measurement.AutorangeMinimumApertureTime = AutorangeMinimumApertureTime.Value;
            }
            if (AutorangeMinimumCurrentRange.HasValue)
            {
                channelOutput.Measurement.AutorangeMinimumCurrentRange = AutorangeMinimumCurrentRange.Value;
            }
            if (AutorangeMinimumVoltageRange.HasValue)
            {
                channelOutput.Measurement.AutorangeMinimumVoltageRange = AutorangeMinimumVoltageRange.Value;
            }
            if (AutorangeMinimumApertureTimeUnits.HasValue)
            {
                channelOutput.Measurement.AutorangeMinimumApertureTimeUnits = AutorangeMinimumApertureTimeUnits.Value;
            }
            if (VoltageLimitHigh.HasValue)
            {
                channelOutput.Source.Current.VoltageLimitHigh = VoltageLimitHigh.Value;
            }
            if (VoltageLimitLow.HasValue)
            {
                channelOutput.Source.Current.VoltageLimitLow = VoltageLimitLow.Value;
            }
            if (VoltageCompensationFrequency.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Voltage.CompensationFrequency = VoltageCompensationFrequency.Value;
            }
            if (VoltageGainBandwidth.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Voltage.GainBandwidth = VoltageGainBandwidth.Value;
            }
            if (VoltagePoleZeroRatio.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Voltage.PoleZeroRatio = VoltagePoleZeroRatio.Value;
            }
            if (CurrentLimitHigh.HasValue)
            {
                channelOutput.Source.Voltage.CurrentLimitHigh = CurrentLimitHigh.Value;
            }
            if (CurrentLimitLow.HasValue)
            {
                channelOutput.Source.Voltage.CurrentLimitLow = CurrentLimitLow.Value;
            }
            if (CurrentCompensationFrequency.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Current.CompensationFrequency = CurrentCompensationFrequency.Value;
            }
            if (CurrentGainBandwidth.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Current.GainBandwidth = CurrentGainBandwidth.Value;
            }
            if (CurrentPoleZeroRatio.HasValue)
            {
                channelOutput.Source.CustomTransientResponse.Current.PoleZeroRatio = CurrentPoleZeroRatio.Value;
            }
            if (CurrentLevelRisingSlewRate.HasValue)
            {
                channelOutput.Source.Current.CurrentLevelRisingSlewRate = CurrentLevelRisingSlewRate.Value;
            }
            if (CurrentLevelFallingSlewRate.HasValue)
            {
                channelOutput.Source.Current.CurrentLevelFallingSlewRate = CurrentLevelFallingSlewRate.Value;
            }
            if (PulseVoltageLimitHigh.HasValue)
            {
                channelOutput.Source.PulseCurrent.VoltageLimitHigh = PulseVoltageLimitHigh.Value;
            }
            if (PulseVoltageLimitLow.HasValue)
            {
                channelOutput.Source.PulseCurrent.VoltageLimitLow = PulseVoltageLimitLow.Value;
            }
            if (PulseBiasVoltageLimit.HasValue)
            {
                channelOutput.Source.PulseCurrent.BiasVoltageLimit = PulseBiasVoltageLimit.Value;
            }
            if (PulseBiasVoltageLimitHigh.HasValue)
            {
                channelOutput.Source.PulseCurrent.BiasVoltageLimitHigh = PulseBiasVoltageLimitHigh.Value;
            }
            if (PulseBiasVoltageLimitLow.HasValue)
            {
                channelOutput.Source.PulseCurrent.BiasVoltageLimitLow = PulseBiasVoltageLimitLow.Value;
            }
            if (PulseCurrentLimitHigh.HasValue)
            {
                channelOutput.Source.PulseVoltage.CurrentLimitHigh = PulseCurrentLimitHigh.Value;
            }
            if (PulseCurrentLimitLow.HasValue)
            {
                channelOutput.Source.PulseVoltage.CurrentLimitLow = PulseCurrentLimitLow.Value;
            }
            if (PulseBiasCurrentLimit.HasValue)
            {
                channelOutput.Source.PulseVoltage.BiasCurrentLimit = PulseBiasCurrentLimit.Value;
            }
            if (PulseBiasCurrentLimitHigh.HasValue)
            {
                channelOutput.Source.PulseVoltage.BiasCurrentLimitHigh = PulseBiasCurrentLimitHigh.Value;
            }
            if (PulseBiasCurrentLimitLow.HasValue)
            {
                channelOutput.Source.PulseVoltage.BiasCurrentLimitLow = PulseBiasCurrentLimitLow.Value;
            }
            if (PulseOnTime.HasValue)
            {
                channelOutput.Source.PulseOnTime = PulseOnTime.Value;
            }
            if (PulseOffTime.HasValue)
            {
                channelOutput.Source.PulseOffTime = PulseOffTime.Value;
            }
            if (PulseBiasDelay.HasValue)
            {
                channelOutput.Source.PulseBiasDelay = PrecisionTimeSpan.FromSeconds(PulseBiasDelay.Value);
            }
            if (SequenceStepDeltaTime.HasValue)
            {
                channelOutput.Source.SequenceStepDeltaTime = PrecisionTimeSpan.FromSeconds(SequenceStepDeltaTime.Value);
            }
            if (OutputResistance.HasValue)
            {
                channelOutput.Source.Output.Resistance = OutputResistance.Value;
            }
            if (ConductionVoltageMode.HasValue)
            {
                channelOutput.Source.ConductionVoltageMode = ConductionVoltageMode.Value;
            }
            if (ConductionVoltageOffThreshold.HasValue)
            {
                channelOutput.Source.ConductionVoltageOffThreshold = ConductionVoltageOffThreshold.Value;
            }
            if (ConductionVoltageOnThreshold.HasValue)
            {
                channelOutput.Source.ConductionVoltageOnThreshold = ConductionVoltageOnThreshold.Value;
            }
            if (LcrActualLoadReactance.HasValue)
            {
                channelOutput.LCR.Compensation.ActualLoadReactance = LcrActualLoadReactance.Value;
            }
            if (LcrActualLoadResistance.HasValue)
            {
                channelOutput.LCR.Compensation.ActualLoadResistance = LcrActualLoadResistance.Value;
            }
            if (LcrCurrentRange.HasValue)
            {
                channelOutput.LCR.Advanced.CurrentRange = LcrCurrentRange.Value;
            }
            if (LcrDcBiasCurrentRange.HasValue)
            {
                channelOutput.LCR.Advanced.DCBiasCurrentRange = LcrDcBiasCurrentRange.Value;
            }
            if (LcrDcBiasTransientResponse.HasValue)
            {
                channelOutput.LCR.Advanced.DCBiasTransientResponse = LcrDcBiasTransientResponse.Value;
            }
            if (LcrDcBiasVoltageRange.HasValue)
            {
                channelOutput.LCR.Advanced.DCBiasVoltageRange = LcrDcBiasVoltageRange.Value;
            }
            if (LcrImpedanceAutoRange.HasValue)
            {
                channelOutput.LCR.ImpedanceAutoRange = LcrImpedanceAutoRange.Value;
            }
            if (LcrLoadCapacitance.HasValue)
            {
                channelOutput.LCR.LoadCapacitance = LcrLoadCapacitance.Value;
            }
            if (LcrLoadInductance.HasValue)
            {
                channelOutput.LCR.LoadInductance = LcrLoadInductance.Value;
            }
            if (LcrLoadResistance.HasValue)
            {
                channelOutput.LCR.LoadResistance = LcrLoadResistance.Value;
            }
            if (LcrMeasuredLoadReactance.HasValue)
            {
                channelOutput.LCR.Compensation.MeasuredLoadReactance = LcrMeasuredLoadReactance.Value;
            }
            if (LcrMeasuredLoadResistance.HasValue)
            {
                channelOutput.LCR.Compensation.MeasuredLoadResistance = LcrMeasuredLoadResistance.Value;
            }
            if (LcrOpenConductance.HasValue)
            {
                channelOutput.LCR.Compensation.OpenConductance = LcrOpenConductance.Value;
            }
            if (LcrOpenSusceptance.HasValue)
            {
                channelOutput.LCR.Compensation.OpenSusceptance = LcrOpenSusceptance.Value;
            }
            if (LcrShortReactance.HasValue)
            {
                channelOutput.LCR.Compensation.ShortReactance = LcrShortReactance.Value;
            }
            if (LcrShortResistance.HasValue)
            {
                channelOutput.LCR.Compensation.ShortResistance = LcrShortResistance.Value;
            }
            if (LcrSourceDelayMode.HasValue)
            {
                channelOutput.LCR.SourceDelayMode = LcrSourceDelayMode.Value;
            }
            if (LcrVoltageRange.HasValue)
            {
                channelOutput.LCR.Advanced.VoltageRange = LcrVoltageRange.Value;
            }
            if (InstrumentMode.HasValue)
            {
                channelOutput.InstrumentMode = InstrumentMode.Value;
            }
        }

        #endregion
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
