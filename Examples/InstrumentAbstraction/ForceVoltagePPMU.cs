using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to force voltage on pins mapped to Digital Pattern, using the instrument's PPMU function mode.
    /// This class, and it's methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from be directly invoked.
    /// </summary>
    internal static class ForceVoltagePPMU
    {
        internal static void SameValueToAllPinsPpmu(ISemiconductorModuleContext tsmContext, string[] ppmuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var ppmuPins = sessionManager.Digital(ppmuPinNames);
            var voltageLevel = 3.3;
            var currentLimit = 0.1;

            ppmuPins.ForceVoltage(voltageLevel, currentLimit);
        }

        internal static void DifferentValuesPerPinPpmu(ISemiconductorModuleContext tsmContext)
        {
            var pinNames = new string[] { "PinA", "PinB" };
            var sessionManager = new TSMSessionManager(tsmContext);
            var ppmuPins = sessionManager.Digital(pinNames);

            var pinASettings = new PPMUForcingSettings { VoltageLevel = 1.0, CurrentLimitRange = 0.09 };
            var pinBSettings = new PPMUForcingSettings { VoltageLevel = 3.3, CurrentLimitRange = 0.01 };
            var smuSettings = new Dictionary<string, PPMUForcingSettings>()
            {
                [pinNames[0]] = pinASettings,
                [pinNames[1]] = pinBSettings,
            };

            ppmuPins.ForceVoltage(smuSettings);
        }

        /// <summary>
        /// This example demonstrates how to configure different voltage levels for each site.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        internal static void DifferentValuesPerSitePpmu(ISemiconductorModuleContext tsmContext)
        {
            var pinNames = new string[] { "PinA", "PinB" };
            var sessionManager = new TSMSessionManager(tsmContext);
            var ppmuPins = sessionManager.Digital(pinNames);

            var perSiteVoltageLevelsArray = tsmContext.GetSiteData<double>("PerSiteVoltagesToSource");
            var perSiteVoltageLevels = new SiteData<double>(perSiteVoltageLevelsArray);

            ppmuPins.ForceVoltage(perSiteVoltageLevels);
        }

        internal static void ConfigureDifferentModesDifferentValuesPerPinPpmu(ISemiconductorModuleContext tsmContext)
        {
            var pinNames = new string[] { "PinA", "PinB" };
            var sessionManager = new TSMSessionManager(tsmContext);
            var ppmuPins = sessionManager.Digital(pinNames);

            var pinASettings = new PPMUForcingSettings
            {
                VoltageLevel = 0.01,
                CurrentLimitRange = 1,
                OutputFunction = PpmuOutputFunction.DCCurrent
            };
            var pinBSettings = new PPMUForcingSettings
            {
                VoltageLevel = 3.3,
                CurrentLimitRange = 0.01,
                OutputFunction = PpmuOutputFunction.DCVoltage,
            };
            var ppmuSettings = new Dictionary<string, PPMUForcingSettings>()
            {
                [pinNames[0]] = pinASettings,
                [pinNames[1]] = pinBSettings,
            };

            ppmuPins.ForceVoltage(ppmuSettings);
        }
    }
}
