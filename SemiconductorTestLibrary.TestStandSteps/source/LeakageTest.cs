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
        /// Performs a single leakage measurement using the following test method:
        /// 1. The test first configures the instruments connected to the specified pins and ensures they are forcing 0V.
        /// 2. The specified voltage is then forced on each pin.
        /// 3. After the specified settling time, the current is measured on each pin.
        /// 4. Each pin is then configured back to forcing 0V.
        /// 5. Finally, the output on each pin is disabled.
        /// Pins mapped to either an NI SMU or NI PPMU(s) instrument channel are supported.
        /// By default, steps 2-4 are preformed in parallel, across all pins simultaneously.
        /// When the serialOperationEnabled Boolean input parameter is set to true, steps 2-4 are preformed serially, for each pin or pin group, one at a time.
        /// Pins within pin groups are always operated on in parallel.
        /// The following Published Data Id is used for test results on a per-pin basis: Leakage.
        /// Note that the leakage current measurements are published separately for each pin, regardless of if a pin group is provided.
        /// Each test evaluation defined in the Test tab of the calling TestStand step must specify a pin.
        /// To perform the leakage test on some DUT pins in parallel but not others, use the serialOperationEnabled Boolean input parameter and define separate pin groups (i.e. LeakageOddPins and LeakageEvenPins).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups.</param>
        /// <param name="voltageLevel">The DC voltage level to force, in Volts.</param>
        /// <param name="currentLimit">The current limit in Amperes.</param>
        /// <param name="apertureTime">The measurement aperture time in seconds.</param>
        /// <param name="settlingTime">The amount of time to wait before measuring the current, in seconds.</param>
        /// <param name="serialOperationEnabled">Whether to enforce testing sequentially across pins or pin groups.</param>
        public static void LeakageTest(
            ISemiconductorModuleContext tsmContext,
            string[] pinsOrPinGroups,
            double voltageLevel,
            double currentLimit,
            double apertureTime,
            double settlingTime,
            bool serialOperationEnabled = false)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);

                // Get the appropriate ISessionsBundle objects associated with all pins or pin groups.
                DCPowerSessionsBundle dcPower = null;
                DigitalSessionsBundle digital = null;
                InvokeInParallel(
                    () =>
                    {
                        var dcPowerPins = tsmContext.FilterPinsByInstrumentType(pinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower);
                        if (dcPowerPins.Any())
                        {
                            dcPower = sessionManager.DCPower(dcPowerPins);
                        }
                    },
                    () =>
                    {
                        var digitalPins = tsmContext.FilterPinsByInstrumentType(pinsOrPinGroups, InstrumentTypeIdConstants.NIDigitalPattern);
                        if (digitalPins.Any())
                        {
                            digital = sessionManager.Digital(digitalPins);
                        }
                    });

                var originalSourceDelays = dcPower?.GetSourceDelayInSeconds();
                var dcPowerMeasureSettings = new DCPowerMeasureSettings() { ApertureTime = apertureTime };

                // Configure initial settings and force 0 V on all pins.
                InvokeInParallel(
                    () =>
                    {
                        if (dcPower != null)
                        {
                            dcPower.ConfigureSourceDelay(settlingTime);
                            dcPower.ConfigureMeasureSettings(dcPowerMeasureSettings);
                            dcPower.ForceVoltage(0, currentLimit, waitForSourceCompletion: true);
                        }
                    },
                    () =>
                    {
                        if (digital != null)
                        {
                            digital.ConfigureApertureTime(apertureTime);
                            digital.ForceVoltage(0, currentLimit, settlingTime: settlingTime);
                        }
                    });

                // Force specified voltage either sequentially or in parallel according to the serialOperationEnabled parameter.
                if (serialOperationEnabled)
                {
                    // Force specified voltage, measure current, then force back to 0V, one pin or pin group at a time.
                    foreach (var pinOrPinGroup in pinsOrPinGroups)
                    {
                        var dcPowerPins = tsmContext.FilterPinsByInstrumentType(new string[] { pinOrPinGroup }, InstrumentTypeIdConstants.NIDCPower);
                        if (dcPowerPins.Any())
                        {
                            var dcPowerPerPinOrPinGroup = sessionManager.DCPower(dcPowerPins);
                            dcPowerPerPinOrPinGroup.ForceVoltage(voltageLevel, waitForSourceCompletion: true);
                            dcPowerPerPinOrPinGroup.MeasureAndPublishVoltage("Leakage", out _);
                            dcPowerPerPinOrPinGroup.ForceVoltage(0, waitForSourceCompletion: true);
                        }

                        var digitalPins = tsmContext.FilterPinsByInstrumentType(new string[] { pinOrPinGroup }, InstrumentTypeIdConstants.NIDigitalPattern);
                        if (digitalPins.Any())
                        {
                            var digitalPerPinOrPinGroup = sessionManager.Digital(digitalPins);
                            digitalPerPinOrPinGroup.ForceVoltage(voltageLevel, settlingTime: settlingTime);
                            digitalPerPinOrPinGroup.MeasureAndPublishVoltage("Leakage", out _);
                            digitalPerPinOrPinGroup.ForceVoltage(0, settlingTime: settlingTime);
                        }
                    }
                }
                else
                {
                    // Force specified voltage on all pins simultaneously, and wait for settling.
                    InvokeInParallel(
                        () => dcPower?.ForceVoltage(voltageLevel, waitForSourceCompletion: true),
                        () => digital?.ForceVoltage(voltageLevel, settlingTime: settlingTime));

                    // Measure current on all pins simultaneously.
                    InvokeInParallel(
                        () => dcPower?.MeasureAndPublishCurrent("Leakage", out _),
                        () => digital?.MeasureAndPublishCurrent("Leakage", out _));

                    // Force pins back to 0V simultaneously, and wait for settling.
                    InvokeInParallel(
                        () => dcPower?.ForceVoltage(0, waitForSourceCompletion: true),
                        () => digital?.ForceVoltage(0, settlingTime: settlingTime));
                }

                // Disable output on all pins simultaneously.
                // Do not need to wait for settling, since the output of all pins is already set 0V.
                // Ensure the source delay property is restored back to its original value.
                InvokeInParallel(
                    () =>
                    {
                        if (dcPower != null)
                        {
                            dcPower.ConfigureSourceDelay(originalSourceDelays);
                            dcPower.PowerDown();
                        }
                    },
                    () => digital?.TurnOffOutput());
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
