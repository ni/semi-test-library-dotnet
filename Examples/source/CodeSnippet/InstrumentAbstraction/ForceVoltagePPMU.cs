using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to force voltage on pins mapped to Digital Pattern, using the instrument's PPMU function mode.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have been already initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class ForceVoltagePPMU
    {
        /// <summary>
        /// This example demonstrates how to force the same voltage level on the specified pins across all sites.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="ppmuPinNames">The PPMU pins to force voltage on</param>
        internal static void SameValueToAllPpmuPins(ISemiconductorModuleContext tsmContext, string[] ppmuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var voltageLevel = 3.3;
            var currentLimit = 0.1;

            var ppmuPins = sessionManager.Digital(ppmuPinNames);

            ppmuPins.ForceVoltage(voltageLevel, currentLimit);
        }

        /// <summary>
        /// This example demonstrates how to force different voltage levels for different pins across all sites.
        /// The example assumes there are exactly 2 pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        internal static void DifferentLevelsPerPpmuPin(ISemiconductorModuleContext tsmContext)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var ppmuPinNames = new[] { "PinA", "PinB" };
            var perPinVoltageLevels = new[] { 1.0, 3.3 };
            var activeSites = tsmContext.SiteNumbers.ToArray();
            // Create a PinSiteData object to map each voltage level with the appropriate pin name.
            var perPinLevelsToForce = new PinSiteData<double>(ppmuPinNames, activeSites, perPinVoltageLevels);

            var ppmuPins = sessionManager.Digital(ppmuPinNames);

            ppmuPins.ForceVoltage(perPinLevelsToForce);
        }

        /// <summary>
        /// This example demonstrates how to force different voltage levels and current limits for different pins across all sites.
        /// The example assumes there are exactly 2 pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        internal static void DifferentValuesPerPpmuPin(ISemiconductorModuleContext tsmContext)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            // This dictionary is hard coded for demonstration purposes,
            // but could otherwise be built by looping over array input parameters of the same length.
            var perPinSmuSettings = new Dictionary<string, PPMUSettings>()
            {
                ["PinA"] = new PPMUSettings { VoltageLevel = 1.0, CurrentLimitRange = 0.09 },
                ["PinB"] = new PPMUSettings { VoltageLevel = 3.3, CurrentLimitRange = 0.01 },
            };

            var ppmuPins = sessionManager.Digital(perPinSmuSettings.Keys.ToArray());

            ppmuPins.ForceVoltage(perPinSmuSettings);
        }

        /// <summary>
        /// This example demonstrates how to configure different voltage levels for each site.
        /// The example assumes there is a maximum of 4 sites.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="ppmuPinNames">The PPMU pins to force voltage on</param>
        internal static void DifferentValuesPerSiteAcrossAllPpmuPins(ISemiconductorModuleContext tsmContext, string[] ppmuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();
            // Unique per-site data, assuming a max site count of 4.
            // Each index of the array represents the corresponding site number.
            var perSiteVoltageLevelsArray = new double[] { 3.2, 3.3, 3.1, 3.8 };
            // Note the site unique values must be filtered based on the active sites.
            var activeSiteUniqueValues = new double[activeSites.Length];
            for (int i = 0; i < activeSites.Length; i++)
            {
                activeSiteUniqueValues[i] = perSiteVoltageLevelsArray[activeSites[i]];
            }
            var perSiteVoltages = new SiteData<double>(activeSites, perSiteVoltageLevelsArray);

            var ppmuPins = sessionManager.Digital(ppmuPinNames);

            ppmuPins.ForceVoltage(perSiteVoltages);
        }

        /// <summary>
        /// This is an advanced example that demonstrates how to configure different modes (voltage/current) and values for separate pins,
        /// and then start forcing on those pins in their respective mode (voltage/current).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        internal static void ConfigureDifferentModesDifferentValuesPerPpmuPin(ISemiconductorModuleContext tsmContext)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();
            var pinNames = new string[] { "PinA", "PinB" };
            var perPinPpmuSettings = new[]
            {
                new PPMUSettings
                {
                    VoltageLevel = 0.01,
                    CurrentLimitRange = 1,
                    OutputFunction = PpmuOutputFunction.DCCurrent
                },
                new PPMUSettings
                {
                    VoltageLevel = 3.3,
                    CurrentLimitRange = 0.01,
                    OutputFunction = PpmuOutputFunction.DCVoltage,
                }
            };
            var ppmuSettings = new PinSiteData<PPMUSettings>(pinNames, activeSites, perPinPpmuSettings);

            // Below is a temporary example until extensions for ConfigureSettings and Source are implemented.
            // At which point it would be replaced with the following:
            // ppmuPins = sessionManager.Digital(pinNames);
            // ppmuPins.ConfigureSettings(ppmuSettings);
            // ppmuPins.Source();
            Parallel.For(0, pinNames.Length, i =>
            {
                var setting = perPinPpmuSettings[i];
                var pin = sessionManager.Digital(pinNames);
                if (setting.OutputFunction == PpmuOutputFunction.DCVoltage)
                {
                    pin.ForceVoltage(setting);
                }
                if (setting.OutputFunction == PpmuOutputFunction.DCCurrent)
                {
                    pin.ForceCurrent(setting);
                }
            });
        }
    }
}
