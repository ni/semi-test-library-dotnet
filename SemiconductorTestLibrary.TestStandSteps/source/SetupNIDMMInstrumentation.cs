using System;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DMM;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes NI DMM instrument sessions associated with the pin map.
        /// If the <paramref name="resetDevice"/> input is set to True, then the instrument will be reset as the session is initialized (default = False).
        /// If the <paramref name="powerLineFrequency"/> is set to -1, the method will attempt to automatically determine the power line frequency
        /// and set the power line frequency property for the respective driver sessions.
        /// If the power line frequency cannot be determined, the property will not be set and the driver will use the default value of this property (60Hz).
        /// This is currently only supported by systems that use a PXIe-109x chassis or newer.
        /// If the <paramref name="initialMeasurmentSettings"/> is specified, all DMM sessions will be configured to the specified measurement function (not configured by default).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="apertureTimeUnits">The aperture time units.</param>
        /// <param name="apertureTime">The aperture time.</param>
        /// <param name="settleTime">The settle time.</param>
        /// <param name="powerLineFrequency">The power line frequency.</param>
        /// <param name="initialMeasurmentSettings">The initial measurement settings.</param>
        public static void SetupNIDMMInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            DmmApertureTimeUnits apertureTimeUnits = DmmApertureTimeUnits.PowerLineCycles,
            double apertureTime = 1,
            double settleTime = 0.01,
            double powerLineFrequency = -1,
            DMMMeasurementSettings? initialMeasurmentSettings = null)
        {
            try
            {
                InitializeAndClose.Initialize(tsmContext, resetDevice);

                tsmContext.GetPins(InstrumentTypeIdConstants.NIDmm, out var dutPins, out var systemPins);
                var sessionManager = new TSMSessionManager(tsmContext);
                var dmm = sessionManager.DMM(dutPins.Concat(systemPins).ToArray());
                if (powerLineFrequency < 0)
                {
                    Utilities.TryDeterminePowerLineFrequency(ref powerLineFrequency, tsmContext.IsSemiconductorModuleInOfflineMode);
                }
                if (powerLineFrequency >= 0)
                {
                    dmm.ConfigurePowerlineFrequency(powerLineFrequency);
                }
                dmm.ConfigureApertureTime(apertureTimeUnits, apertureTime);
                dmm.ConfigureSettleTime(settleTime);
                if (initialMeasurmentSettings.HasValue)
                {
                    dmm.ConfigureMeasurementDigits(
                        initialMeasurmentSettings.Value.MeasurementFunction,
                        initialMeasurmentSettings.Value.Range,
                        initialMeasurmentSettings.Value.ResolutionDigits);
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }

    /// <summary>
    /// Defines DMM measurement settings.
    /// </summary>
#pragma warning disable CA1815 // Override equals and operator equals on value types
    public struct DMMMeasurementSettings
#pragma warning restore CA1815 // Override equals and operator equals on value types
    {
        /// <summary>
        /// The measurement function.
        /// </summary>
        public DmmMeasurementFunction MeasurementFunction { get; }

        /// <summary>
        /// The measurement range.
        /// </summary>
        public double Range { get; }

        /// <summary>
        /// The resolution in digits.
        /// </summary>
        public double ResolutionDigits { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DMMMeasurementSettings"/> struct.
        /// </summary>
        /// <param name="measurementFunction">The measurement function.</param>
        /// <param name="range">The measurement range.</param>
        /// <param name="resolutionDigits">The resolution in digits.</param>
        public DMMMeasurementSettings(DmmMeasurementFunction measurementFunction, double range, double resolutionDigits)
        {
            MeasurementFunction = measurementFunction;
            Range = range;
            ResolutionDigits = resolutionDigits;
        }
    }
}
