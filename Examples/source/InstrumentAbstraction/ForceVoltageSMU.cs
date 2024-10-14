using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDCPower;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;

namespace NationalInstruments.SemiconductorTestLibrary.Examples.InstrumentAbstraction
{
    /// <summary>
    /// This class contains examples of how to use the Instrument Abstraction extensions from the Semiconductor Test Library.
    /// Specifically, how to force voltage on pins mapped to DCPower instruments.
    /// Note that DCPower Instruments include both Source Measurement Units (SMUs) and Programmable Power Supplies (PPS) devices.
    /// This class and its methods are intended for example purposes only and are not meant to be ran standalone.
    /// They are only meant to demonstrate specific coding concepts and may otherwise assume a hypothetical test program
    /// with any dependent instrument sessions have already been initiated and configured.
    /// Additionally, they are intentionally marked as internal to prevent them from being directly invoked from code outside of this project.
    /// </summary>
    internal static class ForceVoltageSMU
    {
        /// <summary>
        /// This example demonstrates how to force the same voltage level on the specified pins across all sites.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage on</param>
        internal static void SameValueToAllSmuPins(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var voltageLevel = 3.3;
            var currentLimit = 0.1;

            var smuPins = sessionManager.DCPower(smuPinNames);

            smuPins.ForceVoltage(voltageLevel, currentLimit);
        }

        /// <summary>
        /// This example demonstrates how to force different voltage levels for different pins across all sites.
        /// The example assumes there are exactly 2 pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        internal static void DifferentLevelsPerSmuPin(ISemiconductorModuleContext tsmContext)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPinNames = new[] { "PinA", "PinB" };
            var perPinVoltageLevels = new[] { 1.0, 3.3 };
            var activeSites = tsmContext.SiteNumbers.ToArray();
            // Create a PinSiteData object to map each voltage level with the appropriate pin name.
            var perPinLevelsToForce = new PinSiteData<double>(smuPinNames, activeSites, perPinVoltageLevels);

            var smuPins = sessionManager.DCPower(smuPinNames);

            smuPins.ForceVoltage(perPinLevelsToForce);
        }

        /// <summary>
        /// This example demonstrates how to force different voltage levels and current limits for different pins across all sites.
        /// The example assumes there are exactly 2 pins.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        internal static void DifferentValuesPerSmuPin(ISemiconductorModuleContext tsmContext)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            // This dictionary is hard coded for demonstration purposes,
            // but could otherwise be built by looping over array input parameters of the same length.
            var perPinSmuSettings = new Dictionary<string, DCPowerSourceSettings>()
            {
                ["PinA"] = new DCPowerSourceSettings { Level = 1.0, Limit = 0.09 },
                ["PinB"] = new DCPowerSourceSettings { Level = 3.3, Limit = 0.01 },
            };

            var smuPins = sessionManager.DCPower(perPinSmuSettings.Keys.ToArray());

            smuPins.ForceVoltage(perPinSmuSettings);
        }

        /// <summary>
        /// This example demonstrates how to configure different voltage levels for each site.
        /// The example assumes there is a maximum of 4 sites.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        /// <param name="smuPinNames">The SMU pins to force voltage on</param>
        internal static void DifferentValuesPerSiteAcrossAllSmuPins(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
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

            var smuPins = sessionManager.DCPower(smuPinNames);

            smuPins.ForceVoltage(perSiteVoltages);
        }

        /// <summary>
        /// This is an advanced example that demonstrates how to configure different modes (voltage/current) and values for separate pins,
        /// and then start forcing on those pins in their respective mode (voltage/current).
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        internal static void ConfigureDifferentModesDifferentValuesPerSmuPin(ISemiconductorModuleContext tsmContext)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var activeSites = tsmContext.SiteNumbers.ToArray();
            var pinNames = new string[] { "PinA", "PinB" };
            var perPinSettings = new[]
            {
                new DCPowerSourceSettings
                {
                    Level = 0.01,
                    Limit = 1,
                    OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
                },
                new DCPowerSourceSettings
                {
                    Level = 3.3,
                    Limit = 0.01,
                    OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
                }
            };
            var smuSettings = new PinSiteData<DCPowerSourceSettings>(pinNames, activeSites, perPinSettings);

            var smuPins = sessionManager.DCPower(pinNames);

            smuPins.ConfigureSourceSettings(smuSettings);
            smuPins.Initiate();
        }
    }
}
