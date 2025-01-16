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
        StartTrigger
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
