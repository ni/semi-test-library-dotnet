using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SMUMergePinGroup
{
    public static partial class TestSteps
    {
        /// <summary>
        /// Forces the specified DC voltage on all pins and/or pin groups specified, waits the specified amount of settling time,
        /// and then measures the current on those pins and publishes the results to TestStand. Both DCPower and Digital PPMU pins are supported.
        /// Both the <paramref name="settlingTime"/> and <paramref name="apertureTime"/> inputs are expected to be provided in Seconds.
        /// By default the <paramref name="apertureTime"/> input is set to -1, which will cause this input to be ignored
        /// and the device will use any pre-configured aperture time set by a proceeding set, such as the Setup NI-DCPower Instrumentation step.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups to force DC voltage on.</param>
        /// <param name="voltageLevel">The DC voltage level to force, in volts.</param>
        /// <param name="currentLimit">The current limit in amperes.</param>
        /// <param name="settlingTime">The amount of time to wait before continuing, in seconds.</param>
        /// <param name="apertureTime">The measurement aperture time in seconds.</param>
        public static void ForceVoltageMeasureCurrent(
            ISemiconductorModuleContext tsmContext,
            string[] pinsOrPinGroups,
            double voltageLevel,
            double currentLimit,
            double settlingTime = 0,
            double apertureTime = -1)
        {
            var sessionManager = new TSMSessionManager(tsmContext);

            var dcPowerPinsOrPinGroups = tsmContext.FilterPinsByInstrumentType(pinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower);
            if (dcPowerPinsOrPinGroups.Any())
            {
                var dcPower = sessionManager.DCPower(dcPowerPinsOrPinGroups);
                var originalSourceDelays = dcPower.GetSourceDelayInSeconds();
                dcPower.ConfigureSourceDelay(settlingTime);
                if (apertureTime != -1)
                {
                    dcPower.ConfigureMeasureSettings(new DCPowerMeasureSettings { ApertureTime = apertureTime });
                }

                dcPower.ForceVoltage(voltageLevel, currentLimit, waitForSourceCompletion: true);
                dcPower.MeasureAndPublishCurrent("Current", out _);
                dcPower.ConfigureSourceDelay(originalSourceDelays);
            }
        }
    }
}
