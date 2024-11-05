using System;
using System.Collections.Generic;
using System.Globalization;
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
        /// Forces DC voltage on the specified DUT supply pins. Must provide voltages and current limit values for each of the DUT supply pins.
        /// If the <paramref name="powerUpSuppliesSerially"/> Boolean is set to True, the DUT supplies will be powered up sequentially in the order provided,
        /// and the <paramref name="settlingTime"/> input will be used after power up each pin. Both DCPower and Digital PPMU pins are supported.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutSupplyPinsOrPinGroups">The DUT supply pins or pin groups.</param>
        /// <param name="perSupplyPinOrPinGroupVoltages">The DC voltage level to force on each DUT supply pin or pin group, in volts.</param>
        /// <param name="perSupplyPinOrPinGroupCurrentLimits">The current limit for each DUT supply pin or pin group, in amperes.</param>
        /// <param name="settlingTime">The amount of time to wait before continuing, in seconds.</param>
        /// <param name="powerUpSuppliesSerially">Whether to power up DUT supplies sequentially.</param>
        public static void DutPowerUp(
            ISemiconductorModuleContext tsmContext,
            string[] dutSupplyPinsOrPinGroups,
            double[] perSupplyPinOrPinGroupVoltages,
            double[] perSupplyPinOrPinGroupCurrentLimits,
            double settlingTime = 0,
            bool powerUpSuppliesSerially = false)
        {
            VerifySizeOfArrayInputs(
                $"{nameof(dutSupplyPinsOrPinGroups)}, {nameof(perSupplyPinOrPinGroupVoltages)} and {nameof(perSupplyPinOrPinGroupCurrentLimits)}",
                dutSupplyPinsOrPinGroups,
                perSupplyPinOrPinGroupVoltages,
                perSupplyPinOrPinGroupCurrentLimits);

            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                if (powerUpSuppliesSerially)
                {
                    for (int i = 0; i < dutSupplyPinsOrPinGroups.Length; i++)
                    {
                        var dcPowerPins = tsmContext.FilterPinsByInstrumentType(new string[] { dutSupplyPinsOrPinGroups[i] }, InstrumentTypeIdConstants.NIDCPower);
                        foreach (var dcPowerPin in dcPowerPins)
                        {
                            var dcPower = sessionManager.DCPower(dcPowerPin);
                            var originalSourceDelays = dcPower.GetSourceDelayInSeconds();
                            dcPower.ConfigureSourceDelay(settlingTime);
                            dcPower.ForceVoltage(perSupplyPinOrPinGroupVoltages[i], perSupplyPinOrPinGroupCurrentLimits[i]);
                            dcPower.ConfigureSourceDelay(originalSourceDelays);
                        }

                        var digitalPins = tsmContext.FilterPinsByInstrumentType(new string[] { dutSupplyPinsOrPinGroups[i] }, InstrumentTypeIdConstants.NIDigitalPattern);
                        foreach (var digitalPin in digitalPins)
                        {
                            var digital = sessionManager.Digital(digitalPin);
                            digital.ForceVoltage(perSupplyPinOrPinGroupVoltages[i], perSupplyPinOrPinGroupCurrentLimits[i]);
                            PreciseWait(settlingTime);
                        }
                    }
                }
                else
                {
                    InvokeInParallel(
                        () =>
                        {
                            tsmContext.FilterPinsOrPinGroups(dutSupplyPinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower, out var supplyPins, out var supplyPinIndexes, out var supplyPinsFlattened);
                            if (supplyPinsFlattened.Any())
                            {
                                var dcPower = sessionManager.DCPower(supplyPinsFlattened);
                                var originalSourceDelays = dcPower.GetSourceDelayInSeconds();
                                dcPower.ConfigureSourceDelay(settlingTime);
                                var dcPowerSourceSettings = BuildDCPowerSourceSettings(supplyPins, supplyPinIndexes, perSupplyPinOrPinGroupVoltages, perSupplyPinOrPinGroupCurrentLimits);
                                dcPower.ForceVoltage(dcPowerSourceSettings);
                                dcPower.ConfigureSourceDelay(originalSourceDelays);
                            }
                        },
                        () =>
                        {
                            tsmContext.FilterPinsOrPinGroups(dutSupplyPinsOrPinGroups, InstrumentTypeIdConstants.NIDigitalPattern, out var supplyPins, out var supplyPinIndexes, out var supplyPinsFlattened);
                            if (supplyPinsFlattened.Any())
                            {
                                var digital = sessionManager.Digital(supplyPinsFlattened);
                                digital.ForceVoltage(BuildPPMUSettings(supplyPins, supplyPinIndexes, perSupplyPinOrPinGroupVoltages, perSupplyPinOrPinGroupCurrentLimits));
                                PreciseWait(settlingTime);
                            }
                        });
                    }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }

        private static IDictionary<string, DCPowerSourceSettings> BuildDCPowerSourceSettings(IList<string[]> pins, IList<int> pinIndexes, double[] voltageLevels, double[] currentLimits)
        {
            var settings = new Dictionary<string, DCPowerSourceSettings>();
            for (int i = 0; i < pins.Count; i++)
            {
                int translatedIndex = pinIndexes[i];
                foreach (var pin in pins[i])
                {
                    settings.Add(pin, new DCPowerSourceSettings()
                    {
                        OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                        Level = voltageLevels[translatedIndex],
                        Limit = currentLimits[translatedIndex],
                    });
                }
            }
            return settings;
        }

        private static IDictionary<string, PPMUSettings> BuildPPMUSettings(IList<string[]> pins, IList<int> pinIndexes, double[] voltageLevels, double[] currentLimits)
        {
            var settings = new Dictionary<string, PPMUSettings>();
            for (int i = 0; i < pins.Count; i++)
            {
                int translatedIndex = pinIndexes[i];
                foreach (var pin in pins[i])
                {
                    settings.Add(pin, new PPMUSettings()
                    {
                        OutputFunction = PpmuOutputFunction.DCVoltage,
                        VoltageLevel = voltageLevels[translatedIndex],
                        CurrentLimitRange = currentLimits[translatedIndex],
                    });
                }
            }
            return settings;
        }
    }
}
