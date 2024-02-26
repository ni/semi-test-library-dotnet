using NationalInstruments.ModularInstruments.NIDCPower;
using static NationalInstruments.MixedSignalLibrary.TypeDefinitions;

namespace NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DCPower
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

        internal DCPowerSettings(MixedSignalParameters parameters, int index, DCPowerSourceOutputFunction outputFunction)
        {
            SourceSettings = new DCPowerSourceSettings(parameters, index, outputFunction);
            MeasureSettings = new DCPowerMeasureSettings(parameters, index);
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
        public DCPowerSourceOutputFunction OutputFunction { get; set; } = DCPowerSourceOutputFunction.DCVoltage;

        /// <summary>
        /// The limit symmetry.
        /// </summary>
        public DCPowerComplianceLimitSymmetry LimitSymmetry { get; set; } = DCPowerComplianceLimitSymmetry.Symmetric;

        /// <summary>
        /// The voltage or current level.
        /// </summary>
        public double Level { get; set; }

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
        /// The source delay.
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

        internal DCPowerSourceSettings(MixedSignalParameters parameters, int index, DCPowerSourceOutputFunction outputFunction)
        {
            OutputFunction = outputFunction;
            LimitSymmetry = DCPowerComplianceLimitSymmetry.Symmetric;
            Level = parameters.Supplies.ElementAtOrFirst(index);
            Limit = parameters.Limits?.ElementAtOrFirst(index);
            SourceDelayInSeconds = parameters.SettlingTimes?.ElementAtOrFirst(index);
            TransientResponse = parameters.DCPowerSourceTransientResponses?.ElementAtOrFirst(index);
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

        internal DCPowerMeasureSettings(MixedSignalParameters parameters, int index)
        {
            ApertureTime = parameters.ApertureTimes?.ElementAtOrFirst(index);
            MeasureWhen = DCPowerMeasurementWhen.OnDemand;
            Sense = parameters.DCPowerMeasurementSenses?.ElementAtOrFirst(index);
            RecordLength = 1;
        }
    }
}
