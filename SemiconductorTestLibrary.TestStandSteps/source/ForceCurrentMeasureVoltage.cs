using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;

namespace NationalInstruments.SemiconductorTestLibrary
{
    public static partial class Steps
    {
        /// <summary>
        /// Forces the specified DC current on all pins and/or pin groups specified, waits the specified amount of settling time,
        /// and then measures the voltage on those pins and publishes the results to TestStand. Both DCPower and Digital PPMU pins are supported.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups to force DC voltage on.</param>
        /// <param name="currentLevel">The DC current level to force, in amperes.</param>
        /// <param name="voltageLimit">The voltage limit in volts.</param>
        /// <param name="settlingTime">The amount of time to wait before continuing, in seconds.</param>
        public static void ForceCurrentMeasureVoltage(
            ISemiconductorModuleContext tsmContext,
            string[] pinsOrPinGroups,
            double currentLevel,
            double voltageLimit,
            double settlingTime = 0)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                InvokeInParallel(
                    () =>
                    {
                        var dcPowerPinsOrPinGroups = tsmContext.FilterPinsByInstrumentType(pinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower);
                        if (dcPowerPinsOrPinGroups.Any())
                        {
                            var dcPower = sessionManager.DCPower(dcPowerPinsOrPinGroups);
                            var originalSourceDelays = dcPower.GetSourceDelayInSeconds();
                            dcPower.ConfigureSourceDelay(settlingTime);
                            dcPower.ForceCurrent(currentLevel, voltageLimit);
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
                            digital.ForceCurrent(currentLevel, voltageLimitLow: voltageLimit, voltageLimitHigh: voltageLimit);
                            PreciseWait(settlingTime);
                            digital.MeasureAndPublishVoltage("Voltage", out _);
                        }
                    });
            }
            catch (Exception e)
            {
                NIMixedSignalException.Throw(e);
            }
        }
    }
}
