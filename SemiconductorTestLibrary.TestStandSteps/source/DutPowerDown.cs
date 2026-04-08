using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
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
        /// Forces DC voltage to Zero on the specified DUT supply pins. If the <paramref name="powerDownSuppliesSerially"/> Boolean is set to True,
        /// the DUT supplies will be powered down sequentially in the order provided, and the <paramref name="settlingTime"/> input will be used after power down each pin.
        /// If <paramref name="forceLowestCurrentLimit"/> is set to false, the <paramref name="dutSupplyPinsOrPinGroups"/> will be configured to force 0V
        /// at the currently programmed current limit and range, which may not be desirable. Otherwise, it will default to true,
        /// which will ensure the pins are configured to force 0V at the lowest possible current limit to mimic high-impedance.
        /// The actual current limit selected is dependent on the mapped hardware channel.
        /// Both DCPower and Digital PPMU pins are supported.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="dutSupplyPinsOrPinGroups">The DUT supply pins or pin groups.</param>
        /// <param name="settlingTime">The amount of time to wait before continuing, in seconds.</param>
        /// <param name="powerDownSuppliesSerially">Whether to power down DUT supplies sequentially.</param>
        /// <param name="forceLowestCurrentLimit">Whether to force lowest current limit (100nA) for safety considerations.</param>
        public static void DutPowerDown(
            ISemiconductorModuleContext tsmContext,
            string[] dutSupplyPinsOrPinGroups,
            double settlingTime = 0,
            bool powerDownSuppliesSerially = false,
            bool forceLowestCurrentLimit = true)
        {
            try
            {
                var sessionManager = new TSMSessionManager(tsmContext);
                if (powerDownSuppliesSerially)
                {
                    for (int i = 0; i < dutSupplyPinsOrPinGroups.Length; i++)
                    {
                        var dcPowerPins = tsmContext.FilterPinsByInstrumentType(new string[] { dutSupplyPinsOrPinGroups[i] }, InstrumentTypeIdConstants.NIDCPower);
                        foreach (var dcPowerPin in dcPowerPins)
                        {
                            var dcPower = sessionManager.DCPower(dcPowerPin);
                            ForceZeroVoltageAndLowestCurrentLimitOnDemand(dcPower, settlingTime, forceLowestCurrentLimit);
                            dcPower.PowerDown(settlingTime);
                        }

                        var digitalPins = tsmContext.FilterPinsByInstrumentType(new string[] { dutSupplyPinsOrPinGroups[i] }, InstrumentTypeIdConstants.NIDigitalPattern);
                        foreach (var digitalPin in digitalPins)
                        {
                            var digital = sessionManager.Digital(digitalPin);
                            digital.TurnOffOutput(settlingTime);
                            PreciseWait(settlingTime);
                        }
                    }
                }
                else
                {
                    InvokeInParallel(
                        () =>
                        {
                            var dcPowerPins = tsmContext.FilterPinsByInstrumentType(dutSupplyPinsOrPinGroups, InstrumentTypeIdConstants.NIDCPower);
                            var dcPower = sessionManager.DCPower(dcPowerPins);
                            ForceZeroVoltageAndLowestCurrentLimitOnDemand(dcPower, settlingTime, forceLowestCurrentLimit);
                            dcPower.PowerDown(settlingTime);
                        },
                        () =>
                        {
                            var digitalPins = tsmContext.FilterPinsByInstrumentType(dutSupplyPinsOrPinGroups, InstrumentTypeIdConstants.NIDigitalPattern);
                            var digital = sessionManager.Digital(digitalPins);
                            digital.TurnOffOutput(settlingTime);
                        });
                }
            }
            catch (Exception e)
            {
                NISemiconductorTestException.Throw(e);
            }
        }

        private static void ForceZeroVoltageAndLowestCurrentLimitOnDemand(DCPowerSessionsBundle dcPower, double settlingTimeInSeconds, bool forceLowestCurrentLimit)
        {
            var originalSourceDelays = dcPower.GetSourceDelayInSeconds();
            dcPower.ConfigureSourceDelay(settlingTimeInSeconds);
            var containsPinMappedToPowerSupply = dcPower.ContainsPinMappedToPowerSupplyInstrument(forceLowestCurrentLimit, out var dcPowerSourceSettings);
            if (forceLowestCurrentLimit)
            {
                var originalCurrentLimit = dcPower.GetCurrentLimits();
                if (containsPinMappedToPowerSupply)
                {
                    dcPower.ForceVoltage(dcPowerSourceSettings);
                }
                else
                {
                    dcPower.ForceVoltage(voltageLevel: 0, currentLimit: 1e-7);
                }
                dcPower.ConfigureCurrentLimit(originalCurrentLimit);
            }
            else
            {
                if (containsPinMappedToPowerSupply)
                {
                    dcPower.ForceVoltage(dcPowerSourceSettings);
                }
                else
                {
                    dcPower.ForceVoltage(voltageLevel: 0);
                }
            }
            dcPower.ConfigureSourceDelay(originalSourceDelays);
        }

        private static void ConfigureCurrentLimit(this DCPowerSessionsBundle dcPower, PinSiteData<double> currentLimits)
        {
            dcPower.Do((sessionInfo, sitePinInfo) =>
            {
                var channelOutput = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                channelOutput.Control.Abort();
                channelOutput.ConfigureCurrentLimit(currentLimits.GetValue(sitePinInfo.SiteNumber, sitePinInfo.PinName));
            });
        }

        private static bool ContainsPinMappedToPowerSupplyInstrument(this DCPowerSessionsBundle dcPower, bool forceLowestCurrentLimit, out PinSiteData<DCPowerSourceSettings> dcPowerSourceSettings)
        {
            bool containsPinMappedToPowerSupply = false;
            dcPowerSourceSettings = dcPower.DoAndReturnPerSitePerPinResults((sessionInfo, sitePinInfo) =>
            {
                var output = sessionInfo.Session.Outputs[sitePinInfo.IndividualChannelString];
                bool isOverRangingEnabled = output.Source.OverrangingEnabled;
                bool isPowerSupply = PowerSupplySettings.TryGetValue(sitePinInfo.ModelString, out var powerSupplyInstrumentConfiguration);
                if (isPowerSupply)
                {
                    containsPinMappedToPowerSupply = true;
                }

                return new DCPowerSourceSettings
                {
                    Level = isPowerSupply ? powerSupplyInstrumentConfiguration.GetVoltageLevel(isOverRangingEnabled) : 0.00,
                    Limit = forceLowestCurrentLimit ? (isPowerSupply ? powerSupplyInstrumentConfiguration.GetCurrentLimit(isOverRangingEnabled) : 1e-7) : (double?)null,
                };
            });

            return containsPinMappedToPowerSupply;
        }

        internal sealed class PowerSupplyInstrumentConfiguration
        {
            private readonly double _overRangingEnabledVoltageLevel;
            private readonly double _overRangingEnabledCurrentLimit;
            private readonly double _overRangingDisabledVoltageLevel;
            private readonly double _overRangingDisabledCurrentLimit;

            public PowerSupplyInstrumentConfiguration(double overRangingEnabledVoltageLevel, double overRangingEnabledCurrentLimit, double overRangingDisabledVoltageLevel, double overRangingDisabledCurrentLimit)
            {
                _overRangingEnabledVoltageLevel = overRangingEnabledVoltageLevel;
                _overRangingEnabledCurrentLimit = overRangingEnabledCurrentLimit;
                _overRangingDisabledVoltageLevel = overRangingDisabledVoltageLevel;
                _overRangingDisabledCurrentLimit = overRangingDisabledCurrentLimit;
            }
            public double GetVoltageLevel(bool isOverRangingEnabled) => isOverRangingEnabled ? _overRangingEnabledVoltageLevel : _overRangingDisabledVoltageLevel;
            public double GetCurrentLimit(bool isOverRangingEnabled) => isOverRangingEnabled ? _overRangingEnabledCurrentLimit : _overRangingDisabledCurrentLimit;
        }

        internal static readonly IReadOnlyDictionary<string, PowerSupplyInstrumentConfiguration> PowerSupplySettings =
            new Dictionary<string, PowerSupplyInstrumentConfiguration>
            {
                [DCPowerModelStrings.PXI_4110] = new PowerSupplyInstrumentConfiguration(overRangingEnabledVoltageLevel: 0.0, overRangingEnabledCurrentLimit: 0.01, overRangingDisabledVoltageLevel: 0.0, overRangingDisabledCurrentLimit: 0.01),
                [DCPowerModelStrings.PXIe_4112] = new PowerSupplyInstrumentConfiguration(overRangingEnabledVoltageLevel: 0.07, overRangingEnabledCurrentLimit: 0.007, overRangingDisabledVoltageLevel: 0.1, overRangingDisabledCurrentLimit: 0.01),
                [DCPowerModelStrings.PXIe_4113] = new PowerSupplyInstrumentConfiguration(overRangingEnabledVoltageLevel: 0.025, overRangingEnabledCurrentLimit: 0.015, overRangingDisabledVoltageLevel: 0.03, overRangingDisabledCurrentLimit: 0.02),
                [DCPowerModelStrings.PXIe_4051] = new PowerSupplyInstrumentConfiguration(overRangingEnabledVoltageLevel: 0.0, overRangingEnabledCurrentLimit: 0.4, overRangingDisabledVoltageLevel: 0.0, overRangingDisabledCurrentLimit: 0.4),
                [DCPowerModelStrings.PXIe_4150] = new PowerSupplyInstrumentConfiguration(overRangingEnabledVoltageLevel: 0.01, overRangingEnabledCurrentLimit: 0.15, overRangingDisabledVoltageLevel: 0.01, overRangingDisabledCurrentLimit: 0.15),
                [DCPowerModelStrings.PXIe_4151] = new PowerSupplyInstrumentConfiguration(overRangingEnabledVoltageLevel: 0.01, overRangingEnabledCurrentLimit: 0.3, overRangingDisabledVoltageLevel: 0.01, overRangingDisabledCurrentLimit: 0.3),
                [DCPowerModelStrings.PXIe_4154] = new PowerSupplyInstrumentConfiguration(overRangingEnabledVoltageLevel: 0.0, overRangingEnabledCurrentLimit: 0.01, overRangingDisabledVoltageLevel: 0.0, overRangingDisabledCurrentLimit: 0.01),
            };
    }
}
