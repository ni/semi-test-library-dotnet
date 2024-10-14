﻿using System;
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
        /// 3. After the specified settling time current is measured on each pin.
        /// 4. Each pin is configured back to forcing 0V.
        /// 5. The output on each pin is disabled.
        /// By default, steps 2-4 are preformed in parallel, across all pins simultaneously.
        /// When the <paramref name="serialOperationEnabled"/> Boolean input is set to true,
        /// steps 2-4 are preformed serially, for each pin or pin group, one at a time.
        /// Pin within Pin groups are always operated on in parallel.
        /// Pins mapped to either an NI SMU or NI PPMU(s) instrument channel are supported.
        /// <para>
        /// Configure test evaluations on the Tests tab of the Step Settings pane in TestStand to evaluate the results of these measurements.
        /// Note that the leakage current measurements are published separately for each pin, regardless of if a pin group is provided.
        /// </para>
        /// <para>
        /// Use the <paramref name="serialOperationEnabled"/> Boolean when leakage can be performed on some DUT pins in parallel but not others,
        /// and define those pins within separate pin groups (i.e. LeakageOddPins and LeakageEvenPins).
        /// </para>
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups.</param>
        /// <param name="voltageLevel">The DC voltage level to force, in volts.</param>
        /// <param name="currentLimit">The current limit in amperes.</param>
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

                // Get <see cref="ISessionsBundle"/> objects associated with all <paramref name="pinsOrPinGroups"/>.
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

                // Configure initial settings and Force 0 V on all pins.
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
                            digital.ForceVoltage(0, currentLimit);
                            PreciseWait(settlingTime);
                        }
                    });

                // Force specified voltage either sequentially or in parallel according to <paramref name="serialOperationEnabled"/>.
                if (serialOperationEnabled)
                {
                    // Force specified voltage, measure current, then disable the output on each pin or pin group in serially, one pin or pin group at a time.
                    foreach (var pinOrPinGroup in pinsOrPinGroups)
                    {
                        var dcPowerPins = tsmContext.FilterPinsByInstrumentType(new string[] { pinOrPinGroup }, InstrumentTypeIdConstants.NIDCPower);
                        if (dcPowerPins.Any())
                        {
                            var dcPowerPerPinOrPinGroup = sessionManager.DCPower(dcPowerPins);
                            dcPowerPerPinOrPinGroup.ForceVoltage(voltageLevel, currentLimit, waitForSourceCompletion: true);
                            dcPowerPerPinOrPinGroup.MeasureAndPublishVoltage("Leakage", out _);
                            dcPowerPerPinOrPinGroup.PowerDown();
                        }

                        var digitalPins = tsmContext.FilterPinsByInstrumentType(new string[] { pinOrPinGroup }, InstrumentTypeIdConstants.NIDigitalPattern);
                        if (digitalPins.Any())
                        {
                            var digitalPerPinOrPinGroup = sessionManager.Digital(digitalPins);
                            digitalPerPinOrPinGroup.ForceVoltage(voltageLevel, currentLimit);
                            PreciseWait(settlingTime);
                            digitalPerPinOrPinGroup.MeasureAndPublishVoltage("Leakage", out _);
                            digitalPerPinOrPinGroup.TurnOffOutput();
                        }
                    }
                }
                else
                {
                    // Force specified voltage on all pins simultaneously.
                    InvokeInParallel(
                        () => dcPower?.ForceVoltage(voltageLevel, currentLimit, waitForSourceCompletion: true),
                        () =>
                        {
                            if (digital != null)
                            {
                                digital.ForceVoltage(voltageLevel, currentLimit);
                                PreciseWait(settlingTime);
                            }
                        });

                    // Measure current on all pins simultaneously.
                    InvokeInParallel(
                        () => dcPower?.MeasureAndPublishCurrent("Leakage", out _),
                        () => digital?.MeasureAndPublishCurrent("Leakage", out _));

                    // Disable output on all pins simultaneously.
                    InvokeInParallel(
                        () => dcPower?.PowerDown(),
                        () => digital?.TurnOffOutput());
                }

                // Restore Source Delay back to its original value.
                dcPower?.ConfigureSourceDelay(originalSourceDelays);
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }
    }
}
