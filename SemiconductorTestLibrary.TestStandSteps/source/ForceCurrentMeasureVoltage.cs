using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary.TestStandSteps
{
    public static partial class CommonSteps
    {
        /// <summary>
        /// Forces the specified DC current on all pins and/or pin groups specified, waits the specified amount of settling time,
        /// and then measures the voltage on those pins and publishes the results to TestStand.
        /// Note that the voltage measurements are published separately for each pin, regardless of if a pin group is provided, using the following Published Data Id: Voltage.
        /// Both DCPower and Digital PPMU pins are supported.
        /// Both the settlingTime and apertureTime inputs are expected to be provided in Seconds.
        /// By default the apertureTime input is set to -1, which will cause this input to be ignored
        /// and the device will use any pre-configured aperture time set by a proceeding set, such as the Setup NI-DCPower Instrumentation step.
        /// The absolute value of voltage limit will be applied symmetrically (i.e if voltageLimit = 1, the output voltage will be limited between -1V and +1V).
        /// An exception will be thrown if the specified limit value is outside the high-end of the voltage limit range for any of the mapped instruments.
        /// For Digital PPMU pins, since the voltage range of the digital pattern instruments is typically not symmetrically (i.e. -2V to 6V),
        /// the low-end of the applied voltage limit will be coerced to within range (i.e if voltageLimit = 3, the output voltage will be limited between -2V and +3V).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups to force DC voltage on.</param>
        /// <param name="currentLevel">The DC current level to force, in amperes.</param>
        /// <param name="voltageLimit">The voltage limit in volts.</param>
        /// <param name="apertureTime">The measurement aperture time in seconds.</param>
        /// <param name="settlingTime">The amount of time to wait before continuing, in seconds.</param>
        public static void ForceCurrentMeasureVoltage(
            ISemiconductorModuleContext tsmContext,
            string[] pinsOrPinGroups,
            double currentLevel,
            double voltageLimit,
            double settlingTime = 0,
            double apertureTime = -1)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var absoluteValueOfVoltageLimit = Math.Abs(voltageLimit);
                InvokeInParallel(
                    () =>
                    {
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
                            dcPower.ForceCurrent(currentLevel, absoluteValueOfVoltageLimit, waitForSourceCompletion: true);
                            dcPower.MeasureAndPublishVoltage("Voltage", out _);
                            dcPower.ConfigureSourceDelay(originalSourceDelays);
                        }
                    },
                    () =>
                    {
                        var digitalPinsOrPinGroups = tsmContext.FilterPinsByInstrumentType(pinsOrPinGroups, InstrumentTypeIdConstants.NIDigitalPattern);
                        if (digitalPinsOrPinGroups.Any())
                        {
                            var digital = sessionManager.Digital(digitalPinsOrPinGroups);
                            if (apertureTime != -1)
                            {
                                digital.ConfigureApertureTime(apertureTime);
                            }
                            digital.ForceCurrent(currentLevel, voltageLimitLow: Math.Max(-2, -absoluteValueOfVoltageLimit), voltageLimitHigh: absoluteValueOfVoltageLimit);
                            PreciseWait(settlingTime);
                            digital.MeasureAndPublishVoltage("Voltage", out _);
                        }
                    });
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
