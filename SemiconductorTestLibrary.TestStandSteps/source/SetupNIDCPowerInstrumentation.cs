using System;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using ExtensionUtilities = NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower.Utilities;
using CommonUtilities = NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class SetupAndCleanupSteps
    {
        /// <summary>
        /// Initializes the NI DCPower instrument sessions associated with the pin map.
        /// If the <paramref name="resetDevice"/> input is set to True, then the instrument will be reset as the session is initialized (default = False).
        /// If the <paramref name="sourceDelay"/> is set to -1 the method will not set the source delay property, and will assume the initialized default value from the driver,
        /// which is expected to be the inverse of the power line frequency.
        /// If the <paramref name="powerLineFrequency"/> is set to -1, the method will attempt to automatically determine the power line frequency
        /// and set the power line frequency property for the respective driver sessions.
        /// If the power line frequency cannot be determined, the property will not be set and the driver will use the default value of this property (60 Hz).
        /// This is currently only supported by systems that use a PXIe-109x chassis or newer.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="resetDevice">Whether to reset device during initialization.</param>
        /// <param name="apertureTime">The aperture time.</param>
        /// <param name="apertureTimeUnits">The aperture time units.</param>
        /// <param name="measureWhen">When to do measurement.</param>
        /// <param name="measurementSense">The measurement sense to use.</param>
        /// <param name="sourceDelay">The source delay in seconds.</param>
        /// <param name="powerLineFrequency">The power line frequency.</param>
        public static void SetupNIDCPowerInstrumentation(
            ISemiconductorModuleContext tsmContext,
            bool resetDevice = false,
            double apertureTime = 1,
            DCPowerMeasureApertureTimeUnits apertureTimeUnits = DCPowerMeasureApertureTimeUnits.PowerLineCycles,
            DCPowerMeasurementWhen measureWhen = DCPowerMeasurementWhen.OnDemand,
            DCPowerMeasurementSense measurementSense = DCPowerMeasurementSense.Remote,
            double sourceDelay = -1,
            double powerLineFrequency = -1)
        {
            try
            {
                InitializeAndClose.Initialize(tsmContext, resetDevice);

                tsmContext.GetPins(InstrumentTypeIdConstants.NIDCPower, out var dutPins, out var systemPins);
                var sessionManager = new TSMSessionManager(tsmContext);
                var dcPower = sessionManager.DCPower(dutPins.Concat(systemPins).ToArray());
                if (powerLineFrequency < 0)
                {
                    CommonUtilities.TryDeterminePowerLineFrequency(ref powerLineFrequency, tsmContext.IsSemiconductorModuleInOfflineMode);
                }
                if (powerLineFrequency >= 0)
                {
                    dcPower.ConfigurePowerLineFrequency(powerLineFrequency);
                }
                if (sourceDelay >= 0)
                {
                    dcPower.ConfigureSourceDelay(sourceDelay);
                }
                var measureSettings = new DCPowerMeasureSettings
                {
                    ApertureTime = apertureTime,
                    ApertureTimeUnits = apertureTimeUnits,
                    MeasureWhen = measureWhen,
                    Sense = measurementSense,
                };
                dcPower.ConfigureMeasureSettings(measureSettings);
                ExtensionUtilities.CreateDCPowerAdvancedSequencePropertyMappingsCache();
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
