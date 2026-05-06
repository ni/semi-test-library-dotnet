using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.Examples.SemiconductorTestLibrary.SMUGangPinGroup
{
    /// <summary>
    /// This class provides example methods to demonstrate how to use the GangPinGroup and UngangPinGroup.
    /// DCPower Instrument Abstraction methods from the Semiconductor Test Library.
    /// These methods can be used to gang DUT pins together to output higher current.
    /// These methods are only supported under the following conditions:
    /// 1. The pin map must define a pin group to contain all the pins that are to be ganged together.
    /// 2. The SMU module must support the source trigger and measure trigger feature.
    /// For example: PXIe-4137, PXIe-4139, PXIe-4147, PXIe-4150, PXIe-4162, and PXIe-4163.
    /// 3. The pins are physically connected externally on the application load board, either in a fixed configuration or via relays.
    /// The example methods of this class demonstrate how relay configurations can be applied
    /// to ensure the SMUs channels are physically connected in parallel before the GangPinGroup operation,
    /// and subsequently disconnected after the UngangPinGroup operation.
    /// </summary>
    public static partial class TestSteps
    {
        /// <summary>
        /// Forces the specified DC voltage on all pins and/or pin groups specified, waits the specified amount of settling time,
        /// and then measures the current on those pins and publishes the results to TestStand.
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
