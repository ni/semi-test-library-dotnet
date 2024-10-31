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
        /// Forces the specified DC current on all pins and/or pin groups specified. Both DCPower and Digital PPMU pins are supported.
        /// If a value is provided to the settlingTimeInSeconds input, the method will wait the specified amount of settling time before continuing.
        /// The absolute value of voltage limit will be applied symmetrically (i.e if voltageLimit = 1, the output voltage will be limited between -1V and +1V).
        /// An exception will be thrown if the specified limit value is outside the high-end of the voltage limit range for any of the mapped instruments.
        /// For Digital PPMU pins, since the voltage range of the digital pattern instruments is typically not symmetrically (i.e. -2V to 6V),
        /// the low-end of the applied voltage limit will be coerced to within range (i.e if voltageLimit = 3, the output voltage will be limited between -2V and +3V).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups to force DC voltage on.</param>
        /// <param name="currentLevel">The DC current level to force, in amperes.</param>
        /// <param name="voltageLimit">The voltage limit in volts.</param>
        /// <param name="settlingTime">The amount of time to wait before continuing, in seconds.</param>
        public static void ForceDcCurrent(
            ISemiconductorModuleContext tsmContext,
            string[] pinsOrPinGroups,
            double currentLevel,
            double voltageLimit,
            double settlingTime = 0)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                var absoluteValueOfVoltageLimit = Math.Abs(voltageLimit);
                InvokeInParallel(
                    () =>
                    {
                        var dcPowerPins = tsmContext.FilterPinsByInstrumentType(pinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower);
                        if (dcPowerPins.Any())
                        {
                            var dcPower = sessionManager.DCPower(dcPowerPins);
                            var originalSourceDelays = dcPower.GetSourceDelayInSeconds();
                            dcPower.ConfigureSourceDelay(settlingTime);
                            dcPower.ForceCurrent(currentLevel, absoluteValueOfVoltageLimit);
                            dcPower.ConfigureSourceDelay(originalSourceDelays);
                        }
                    },
                    () =>
                    {
                        var digitalPins = tsmContext.FilterPinsByInstrumentType(pinsOrPinGroups, InstrumentTypeIdConstants.NIDigitalPattern);
                        if (digitalPins.Any())
                        {
                            var digital = sessionManager.Digital(digitalPins);
                            digital.ForceCurrent(currentLevel, voltageLimitLow: Math.Max(-2, -absoluteValueOfVoltageLimit), voltageLimitHigh: absoluteValueOfVoltageLimit);
                            PreciseWait(settlingTime);
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
