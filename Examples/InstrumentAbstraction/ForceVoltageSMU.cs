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
    /// This class, and it's methods are intended for example purposes only,
    /// and are therefore intentionally marked as internal to prevent them from be directly invoked.
    /// </summary>
    internal static class ForceVoltageSMU
    {
        internal static void SameValueToAllPinsSmu(ISemiconductorModuleContext tsmContext, string[] smuPinNames)
        {
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(smuPinNames);
            var voltageLevel = 3.3;
            var currentLimit = 0.1;

            smuPins.ForceVoltage(voltageLevel, currentLimit);
        }

        internal static void DifferentValuesPerPinSmu(ISemiconductorModuleContext tsmContext)
        {
            var pinNames = new string[] { "PinA", "PinB" };
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(pinNames);

            var pinASettings = new DCPowerSourceSettings { Level = 1.0, Limit = 0.09 };
            var pinBSettings = new DCPowerSourceSettings { Level = 3.3, Limit = 0.01 };
            var smuSettings = new Dictionary<string, DCPowerSourceSettings>()
            {
                [pinNames[0]] = pinASettings,
                [pinNames[1]] = pinBSettings,
            };

            smuPins.ForceVoltage(smuSettings);
        }

        /// <summary>
        /// This example demonstrates how to configure different voltage levels for each site.
        /// </summary>
        /// <param name="tsmContext">The <see cref="ISemiconductorModuleContext"/> object.</param>
        internal static void DifferentValuesPerSiteSmu(ISemiconductorModuleContext tsmContext)
        {
            var pinNames = new string[] { "PinA", "PinB" };
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(pinNames);

            var perSiteVoltageLevelsArray = tsmContext.GetSiteData<double>("PerSiteVoltagesToSource");
            var voltageLevelsSiteData = new SiteData<double>(perSiteVoltageLevelsArray);
            // This is a temporary implementation until ForceVoltage gets an override accepting
            // an input of type SiteData<double> similar to that of the PPMU.
            var voltageLevelsSiteDataArray = Enumerable.Repeat(voltageLevelsSiteData, pinNames.Length).ToArray();
            var voltageLevelsPinSiteData = new PinSiteData<double>(pinNames, voltageLevelsSiteDataArray);

            smuPins.ForceVoltage(voltageLevelsPinSiteData, 0.1);
        }

        internal static void ConfigureDifferentModesDifferentValuesPerPinSmu(ISemiconductorModuleContext tsmContext)
        {
            var pinNames = new string[] { "PinA", "PinB" };
            var sessionManager = new TSMSessionManager(tsmContext);
            var smuPins = sessionManager.DCPower(pinNames);
            var activeSites = tsmContext.SiteNumbers.ToArray();

            var pinASettings = new DCPowerSourceSettings
            {
                Level = 0.01,
                Limit = 1,
                OutputFunction = DCPowerSourceOutputFunction.DCCurrent,
            };
            var pinBSettings = new DCPowerSourceSettings
            {
                Level = 3.3,
                Limit = 0.01,
                OutputFunction = DCPowerSourceOutputFunction.DCVoltage,
            };

            // This is a temporary implementation until ForceVoltage gets an override accepting
            // an input of type Dictionary<string pinName, DCPowerSourceSettings setting>,
            // similar to that of the PPMU. Or a constructors is added to PinSiteData
            // that allows user to declare a PinSiteData object via an input of
            // Dictionary<string pinName, T>, where the T is a scale value that will be
            // assumed to be applied to all sites in the underlaying SiteData<T> object.
            var smuSettings = new PinSiteData<DCPowerSourceSettings>(
                pinNames,
                new SiteData<DCPowerSourceSettings>[]
                {
                    new SiteData<DCPowerSourceSettings>(activeSites, pinASettings),
                    new SiteData<DCPowerSourceSettings>(activeSites, pinBSettings)
                });

            smuPins.ConfigureSourceSettings(smuSettings);
            smuPins.Initiate();
        }
    }
}
