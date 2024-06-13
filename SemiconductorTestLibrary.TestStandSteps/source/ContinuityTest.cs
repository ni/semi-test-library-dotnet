﻿using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.ModularInstruments.NIDigital;
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
        /// Performs a basic continuity test. It serially checks either upper or lower protection diodes DUT pins,
        /// regardless of if they are mapped to digital or SMU instruments. The test will first set 0V on all the pins
        /// and then source a small amount of current on the targeted continuity pins to validate the voltage drop
        /// across the protecting diode. After current is applied, the targeted pin(s) will be forced back to 0V
        /// before continuing on to the next pin.
        /// Note that each continuity pin will be tested one pin at a time.
        /// Pins mapped to either an NI SMU or NI PPMU(s) instrument channel are supported.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="supplyPinsOrPinGroups">The supply pins or pin groups.</param>
        /// <param name="currentLimitsPerSupplyPinOrPinGroup">The current limits for every supply pin or pin group, in amperes.</param>
        /// <param name="continuityPinsOrPinGroups">The continuity pins or pin groups.</param>
        /// <param name="currentLevelPerContinuityPinOrPinGroup">The current levels for every continuity pin or pin group, in amperes.</param>
        /// <param name="voltageLimitHighPerContinuityPinOrPinGroup">The voltage limit high for every continuity pin or pin group, in volts.</param>
        /// <param name="voltageLimitLowPerContinuityPinOrPinGroup">The voltage limit low for every continuity pin or pin group, in volts.</param>
        /// <param name="apertureTime">The measurement aperture time in seconds.</param>
        /// <param name="settlingTime">The amount of time to wait before measuring the voltage, in seconds.</param>
        public static void ContinuityTest(
            ISemiconductorModuleContext tsmContext,
            string[] supplyPinsOrPinGroups,
            double[] currentLimitsPerSupplyPinOrPinGroup,
            string[] continuityPinsOrPinGroups,
            double[] currentLevelPerContinuityPinOrPinGroup,
            double[] voltageLimitHighPerContinuityPinOrPinGroup,
            double[] voltageLimitLowPerContinuityPinOrPinGroup,
            double apertureTime,
            double settlingTime)
        {
            VerifySizeOfArrayInputs(
                $"{nameof(supplyPinsOrPinGroups)} and {nameof(currentLimitsPerSupplyPinOrPinGroup)}",
                supplyPinsOrPinGroups,
                currentLimitsPerSupplyPinOrPinGroup);
            VerifySizeOfArrayInputs(
                $"{nameof(continuityPinsOrPinGroups)}, {nameof(currentLevelPerContinuityPinOrPinGroup)}, {nameof(voltageLimitHighPerContinuityPinOrPinGroup)}, and {nameof(voltageLimitLowPerContinuityPinOrPinGroup)}",
                continuityPinsOrPinGroups,
                currentLevelPerContinuityPinOrPinGroup,
                voltageLimitHighPerContinuityPinOrPinGroup,
                voltageLimitLowPerContinuityPinOrPinGroup);

            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);

                // Force 0V on all supply pins.
                InvokeInParallel(
                    () =>
                    {
                        tsmContext.FilterPinsOrPinGroups(supplyPinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower, out var supplyPins, out var supplyPinIndexes, out var supplyPinsFlattened);
                        if (supplyPinsFlattened.Any())
                        {
                            var dcPower = sessionManager.DCPower(supplyPinsFlattened);
                            var originalSourceDelays = dcPower.GetSourceDelayInSeconds();
                            dcPower.ConfigureSourceDelay(settlingTime);
                            dcPower.ForceVoltage(BuildDCPowerSourceSettings(supplyPins, supplyPinIndexes, currentLimitsPerSupplyPinOrPinGroup));
                            dcPower.ConfigureSourceDelay(originalSourceDelays);
                        }
                    },
                    () =>
                    {
                        tsmContext.FilterPinsOrPinGroups(continuityPinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower, out var continuityPins, out var continuityPinIndexes, out var continuityPinsFlattened);
                        if (continuityPinsFlattened.Any())
                        {
                            var dcPower = sessionManager.DCPower(continuityPinsFlattened);
                            var originalSourceDelays = dcPower.GetSourceDelayInSeconds();
                            dcPower.ConfigureSourceDelay(settlingTime);
                            dcPower.ForceVoltage(BuildDCPowerSourceSettings(continuityPins, continuityPinIndexes, currentLevelPerContinuityPinOrPinGroup));
                            dcPower.ConfigureSourceDelay(originalSourceDelays);
                        }
                    },
                    () =>
                    {
                        tsmContext.FilterPinsOrPinGroups(supplyPinsOrPinGroups, InstrumentTypeIdConstants.NIDigitalPattern, out var supplyPins, out var supplyPinIndexes, out var supplyPinsFlattened);
                        if (supplyPinsFlattened.Any())
                        {
                            var digital = sessionManager.Digital(supplyPinsFlattened);
                            digital.ForceVoltage(BuildPPMUSettings(supplyPins, supplyPinIndexes, currentLimitsPerSupplyPinOrPinGroup));
                            PreciseWait(settlingTime);
                        }
                    },
                    () =>
                    {
                        tsmContext.FilterPinsOrPinGroups(continuityPinsOrPinGroups, InstrumentTypeIdConstants.NIDigitalPattern, out var continuityPins, out var continuityPinIndexes, out var continuityPinsFlattened);
                        if (continuityPinsFlattened.Any())
                        {
                            var digital = sessionManager.Digital(continuityPinsFlattened);
                            digital.ForceVoltage(BuildPPMUSettings(continuityPins, continuityPinIndexes, currentLevelPerContinuityPinOrPinGroup));
                            PreciseWait(settlingTime);
                        }
                    });

                // Source current on SMU continuity pins and measure voltage sequentially.
                tsmContext.FilterPinsOrPinGroups(continuityPinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower, out var dcPowerContinuityPins, out var dcPowerContinuityPinIndexes, out _);
                var dcPowerMeasureSettings = new DCPowerMeasureSettings() { ApertureTime = apertureTime };
                for (int i = 0; i < dcPowerContinuityPins.Count; i++)
                {
                    var dcPowerContinuityPin = sessionManager.DCPower(dcPowerContinuityPins[i]);
                    int translatedIndex = dcPowerContinuityPinIndexes[i];
                    var originalSourceDelays = dcPowerContinuityPin.GetSourceDelayInSeconds();
                    dcPowerContinuityPin.ConfigureSourceDelay(settlingTime);
                    dcPowerContinuityPin.ConfigureMeasureSettings(dcPowerMeasureSettings);
                    dcPowerContinuityPin.ForceCurrentAsymmetricLimit(
                        currentLevelPerContinuityPinOrPinGroup[translatedIndex],
                        voltageLimitHighPerContinuityPinOrPinGroup[translatedIndex],
                        voltageLimitLowPerContinuityPinOrPinGroup[translatedIndex]);
                    dcPowerContinuityPin.MeasureAndPublishVoltage("Continuity", out _);
                    dcPowerContinuityPin.ForceVoltage(0);
                    dcPowerContinuityPin.ConfigureSourceDelay(originalSourceDelays);
                }

                // Source current on PPMU continuity pins and measure voltage sequentially.
                tsmContext.FilterPinsOrPinGroups(continuityPinsOrPinGroups, InstrumentTypeIdConstants.NIDigitalPattern, out var digitalContinuityPins, out var digitalContinuityPinIndexes, out _);
                for (int i = 0; i < digitalContinuityPins.Count; i++)
                {
                    var digitalContinuityPin = sessionManager.Digital(digitalContinuityPins[i]);
                    int translatedIndex = digitalContinuityPinIndexes[i];
                    digitalContinuityPin.ConfigureApertureTime(apertureTime);
                    digitalContinuityPin.ForceCurrent(
                        currentLevelPerContinuityPinOrPinGroup[translatedIndex],
                        voltageLimitHigh: voltageLimitHighPerContinuityPinOrPinGroup[translatedIndex],
                        voltageLimitLow: voltageLimitLowPerContinuityPinOrPinGroup[translatedIndex]);
                    PreciseWait(settlingTime);
                    digitalContinuityPin.MeasureAndPublishVoltage("Continuity", out _);
                    digitalContinuityPin.ForceVoltage(0);
                    PreciseWait(settlingTime);
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }

            DutPowerDown(tsmContext, supplyPinsOrPinGroups.Concat(continuityPinsOrPinGroups).ToArray(), settlingTime);
        }

        private static void FilterPinsOrPinGroups(this ISemiconductorModuleContext tsmContext, string[] pinsOrPinGroups, string instrumentTypeId, out IList<string[]> pins, out IList<int> pinIndexes, out string[] pinsFlattened)
        {
            pins = new List<string[]>();
            pinIndexes = new List<int>();
            for (int index = 0; index < pinsOrPinGroups.Length; index++)
            {
                var filteredPins = tsmContext.FilterPinsByInstrumentType(new string[] { pinsOrPinGroups[index] }, instrumentTypeId);
                if (filteredPins.Any())
                {
                    pins.Add(filteredPins);
                    pinIndexes.Add(index);
                }
            }
            pinsFlattened = pins.SelectMany(x => x).ToArray();
        }

        private static IDictionary<string, DCPowerSourceSettings> BuildDCPowerSourceSettings(IList<string[]> pins, IList<int> pinIndexes, double[] currentLimits)
        {
            var settings = new Dictionary<string, DCPowerSourceSettings>();
            for (int i = 0; i < pins.Count; i++)
            {
                foreach (var pin in pins[i])
                {
                    settings.Add(pin, new DCPowerSourceSettings()
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        Level = 0,
                        Limit = currentLimits[pinIndexes[i]],
                    });
                }
            }
            return settings;
        }

        private static IDictionary<string, PPMUSettings> BuildPPMUSettings(IList<string[]> pins, IList<int> pinIndexes, double[] currentLimits)
        {
            var settings = new Dictionary<string, PPMUSettings>();
            for (int i = 0; i < pins.Count; i++)
            {
                foreach (var pin in pins[i])
                {
                    settings.Add(pin, new PPMUSettings()
                    {
                        OutputFunction = PpmuOutputFunction.DCVoltage,
                        VoltageLevel = 0,
                        CurrentLimitRange = currentLimits[pinIndexes[i]],
                    });
                }
            }
            return settings;
        }
    }
}
