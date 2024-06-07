using System;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDmm;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary
{
    public static partial class Steps
    {
        /// <summary>
        /// Initializes NI DMM instrument sessions associated with the pin map.
        /// If the <paramref name="resetDevice"/> input is set True, then the instrument will be reset as the session is initialized (default = False).
        /// If the <paramref name="powerlineFrequency"/> is set to -1, the method will attempt to automatically determine the power line frequency
        /// and set the power line frequency property for the respective driver sessions.
        /// If the power line frequency cannot be determined, the property will not be set and the driver will use the default value of this property (60Hz).
        /// This is currently only supported by systems that use a PXIe-109x series of PXIe chassis or newer.
        /// If the <paramref name="initialMeasurmentSettings"/> is specified, all DMM sessions will be configured to the specified measurement function (not configured by default).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="apertureTimeUnits">The aperture time units.</param>
        /// <param name="apertureTime">The aperture time.</param>
        /// <param name="settleTime">The settle time.</param>
        /// <param name="powerlineFrequency">The power line frequency.</param>
        /// <param name="initialMeasurmentSettings">The initial measurement settings.</param>
        public static void SetupNIDMMInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            DmmApertureTimeUnits apertureTimeUnits = DmmApertureTimeUnits.PowerLineCycles,
            double apertureTime = 1,
            double settleTime = 0.01,
            double powerlineFrequency = -1,
            DMMMeasurementSettings? initialMeasurmentSettings = null)
        {
            try
            {
                InstrumentAbstraction.DMM.InitializeAndClose.Initialize(tsmContext, resetDevice, powerlineFrequency);

                Parallel.ForEach(tsmContext.GetAllNIDmmSessions(), session =>
                {
                    session.Advanced.ApertureTimeUnits = apertureTimeUnits;
                    session.Advanced.ApertureTime = apertureTime;
                    session.Advanced.SettleTime = settleTime;
                    if (initialMeasurmentSettings.HasValue)
                    {
                        session.MeasurementFunction = initialMeasurmentSettings.Value.MeasurementFunction;
                        session.Range = initialMeasurmentSettings.Value.Range;
                        session.Resolution = initialMeasurmentSettings.Value.ResolutionDigits;
                    }
                });
            }
            catch (Exception e)
            {
                NIMixedSignalException.Throw(e);
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
