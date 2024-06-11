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
        /// Performs a single leakage measurement. The test configures the instruments connected to the pins specified, sources voltage on each pin,
        /// and then measures the current on all pins simultaneously.
        /// Provide input values to the method to specify the voltage to source and the current limit for the leakage measurement.
        /// The leakage current measurements are published separately for each pin, regardless of if a pin group is provided.
        /// Configure test evaluations on the Tests tab of the Step Settings pane in TestStand to evaluate the results of these measurements.
        /// If the forced voltage cannot be applied to pins simultaneously, you can set the <paramref name="serialOperationEnabled"/> Boolean input to true (false by default).
        /// When the <paramref name="serialOperationEnabled"/> Boolean input is set to true and a pin group is provided,
        /// this will still perform parallel operations across all pins in the pin group.
        /// Take advantage of this functionality for when leakage can be performed on some DUT pins in parallel, but not others,
        /// and define those pins within separate pin groups (i.e. LeakageOddPins and LeakageEvenPins).
        /// Pins mapped to either an NI SMU or NI PPMU(s) instrument channel are supported.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="pinsOrPinGroups">The pins or pin groups.</param>
        /// <param name="voltageLevel">The DC voltage level to force, in volts.</param>
        /// <param name="currentLimit">The current limit in amperes.</param>
        /// <param name="apertureTime">The measurement aperture time in seconds.</param>
        /// <param name="settlingTime">The amount of time to wait before measuring the current, in seconds.</param>
        /// <param name="serialOperationEnabled">Whether to force voltage sequentially.</param>
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

                // Source specified voltage either sequentially or in parallel according to <paramref name="serialOperationEnabled"/>.
                if (serialOperationEnabled)
                {
                    foreach (var pinOrPinGroup in pinsOrPinGroups)
                    {
                        var dcPowerPins = tsmContext.FilterPinsByInstrumentType(new string[] { pinOrPinGroup }, InstrumentTypeIdConstants.NIDCPower);
                        if (dcPowerPins.Any())
                        {
                            var dcPowerPerPinOrPinGroup = sessionManager.DCPower(dcPowerPins);
                            var originalSourceDelays = dcPowerPerPinOrPinGroup.GetSourceDelayInSeconds();
                            dcPowerPerPinOrPinGroup.ConfigureSourceDelay(settlingTime);
                            dcPowerPerPinOrPinGroup.ForceVoltage(voltageLevel, currentLimit);
                            dcPowerPerPinOrPinGroup.ConfigureSourceDelay(originalSourceDelays);
                        }

                        var digitalPins = tsmContext.FilterPinsByInstrumentType(new string[] { pinOrPinGroup }, InstrumentTypeIdConstants.NIDigitalPattern);
                        if (digitalPins.Any())
                        {
                            var digitalPerPinOrPinGroup = sessionManager.Digital(digitalPins);
                            digitalPerPinOrPinGroup.ForceVoltage(voltageLevel, currentLimit);
                            PreciseWait(settlingTime);
                        }
                    }
                }
                else
                {
                    InvokeInParallel(
                        () =>
                        {
                            if (dcPower != null)
                            {
                                var originalSourceDelays = dcPower?.GetSourceDelayInSeconds();
                                dcPower.ConfigureSourceDelay(settlingTime);
                                dcPower.ForceVoltage(voltageLevel, currentLimit);
                                dcPower.ConfigureSourceDelay(originalSourceDelays);
                            }
                        },
                        () =>
                        {
                            if (digital != null)
                            {
                                digital.ForceVoltage(voltageLevel, currentLimit);
                                PreciseWait(settlingTime);
                            }
                        });
                }

                // Measure current on all pins simultaneously.
                InvokeInParallel(
                    () =>
                    {
                        var dcPowerMeasureSettings = new DCPowerMeasureSettings() { ApertureTime = apertureTime };
                        dcPower?.ConfigureMeasureSettings(dcPowerMeasureSettings);
                        dcPower?.MeasureAndPublishCurrent("Leakage", out _);
                    },
                    () =>
                    {
                        digital?.ConfigureApertureTime(apertureTime);
                        digital?.MeasureAndPublishCurrent("Leakage", out _);
                    });
            }
            catch (Exception e)
            {
                NIMixedSignalException.Throw(e);
            }
        }
    }
}
