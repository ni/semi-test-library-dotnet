using System;
using System.Collections.Generic;
using System.Linq;
using Ivi.Driver;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    public class PPMUTests
    {
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ForceSameVoltage_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(3.5, sessionInfo.PinSet.Ppmu.DCVoltage.VoltageLevel, 1));
            Close(tsmContext);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForcePerSiteVoltage_ValuesCorrectlySet()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new Dictionary<int, double>() { [0] = 3.5, [1] = 5 };
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForcePerSiteVoltage_ValuesCorrectlySet()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new Dictionary<int, double>() { [0] = 3.5, [1] = 5 };
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
            Close(tsmContext);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForcePerSitePerPinVoltage_ValuesCorrectlySet()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new Dictionary<int, Dictionary<string, double>>()
            {
                [0] = new Dictionary<string, double>() { ["C0"] = 3, ["C1"] = 3.5 },
                [1] = new Dictionary<string, double>() { ["C0"] = 4, ["C1"] = 4.5 }
            };
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4.5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForcePerSitePerPinVoltage_ValuesCorrectlySet()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new Dictionary<int, Dictionary<string, double>>()
            {
                [0] = new Dictionary<string, double>() { ["C0"] = 3, ["C1"] = 3.5 },
                [1] = new Dictionary<string, double>() { ["C0"] = 4, ["C1"] = 4.5 }
            };
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4.5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
            Close(tsmContext);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceVoltageWithSettingsObject_ValuesCorrectlySet()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUForcingSettings>()
            {
                ["C0"] = new PPMUForcingSettings() { VoltageLevel = 3.5, ApertureTime = 0.05 },
                ["C1"] = new PPMUForcingSettings() { VoltageLevel = 5 },
            };
            sessionsBundle.ForceVoltage(settings);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.ApertureTime);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceVoltageWithSettingsObject_ValuesCorrectlySet()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUForcingSettings>()
            {
                ["C0"] = new PPMUForcingSettings() { VoltageLevel = 3.5 },
                ["C1"] = new PPMUForcingSettings() { VoltageLevel = 5, ApertureTime = 0.05 },
            };
            sessionsBundle.ForceVoltage(settings);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.ApertureTime);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime, 2);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime, 2);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ForceSameCurrent_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceCurrent(currentLevel: 0.02);

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(0.02, sessionInfo.PinSet.Ppmu.DCCurrent.CurrentLevel, 2));
            Close(tsmContext);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceCurrentWithSettingsObject_ValuesCorrectlySet()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUForcingSettings>()
            {
                ["C0"] = new PPMUForcingSettings() { CurrentLevel = 0.01, ApertureTime = 0.05 },
                ["C1"] = new PPMUForcingSettings() { CurrentLevel = 0.02 },
            };
            sessionsBundle.ForceCurrent(settings);

            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.ApertureTime);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceCurrentWithSettingsObject_ValuesCorrectlySet()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUForcingSettings>()
            {
                ["C0"] = new PPMUForcingSettings() { CurrentLevel = 0.01 },
                ["C1"] = new PPMUForcingSettings() { CurrentLevel = 0.02, ApertureTime = 0.05 },
            };
            sessionsBundle.ForceCurrent(settings);

            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.ApertureTime);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime, 2);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime, 2);
            Close(tsmContext);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_MeasureVoltageAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Voltage);

            Assert.Equal(2, results.Length);
            Assert.Equal(2, results[0].Length);
            Assert.Equal(2, results[1].Length);
            Close(tsmContext);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_MeasureCurrentAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Current);

            Assert.Equal(2, results.Length);
            Assert.Equal(2, results[0].Length);
            Assert.Equal(2, results[1].Length);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureVoltageAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Voltage);

            Assert.Equal(2, results.Length);
            Assert.Equal(3, results[0].Length);
            Assert.Single(results[1]);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureCurrentAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Current);

            Assert.Equal(2, results.Length);
            Assert.Equal(3, results[0].Length);
            Assert.Single(results[1]);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureVoltageAndReturnPerSitePerPinResults_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceCurrent(currentLevel: 0.02);
            var results = sessionsBundle.MeasureVoltage();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureCurrentAndReturnPerSitePerPinResults_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceCurrent(currentLevel: 0.02);
            var results = sessionsBundle.MeasureCurrent();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void VoltageForcedOnSessions_TurnOffOutput_ChannelsAreTurnedOff(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.TurnOffOutput();

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Off, sessionInfo.PinSet.SelectedFunction));
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void VoltageForcedOnSessions_DisconnectOuput_ChannelsAreDisconnected(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.DisconnectOutput();

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Disconnect, sessionInfo.PinSet.SelectedFunction));
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void VoltageForcedOnSessions_SelectDigital_ChannelsSwitchToDigitalFunction(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.SelectDigital();

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Digital, sessionInfo.PinSet.SelectedFunction));
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_SelectPPMU_ChannelsSwitchToDigitalFunction(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Disconnect, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.SelectPPMU();

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureAperatureTime_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureApertureTime(0.05);

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(0.05, sessionInfo.PinSet.Ppmu.ApertureTime, 2));
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureAndPublishVoltage_MeasurementArrayNotEmptyAndCorrectPublishedDataCount(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, out var publishDatReader, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);
            var expectedNumberOfPublishedDataPoints = (pins.Length * tsmContext.SiteNumbers.Count);

            sessionsBundle.MeasureAndPublishVoltage("VoltageMeasurments", out var voltageMeasurements);

            Assert.NotEmpty(voltageMeasurements);
            Assert.Equal(expectedNumberOfPublishedDataPoints, publishDatReader.GetAndClearPublishedData().Length);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureAndPublishCurrent_MeasurementArrayNotEmptyAndCorrectPublishedDataCount(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, out var publishDatReader, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);
            var expectedNumberOfPublishedDataPoints = (pins.Length * tsmContext.SiteNumbers.Count);

            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.MeasureAndPublishCurrent("CurrentMeasurments", out var currentMeasurements);

            Assert.NotEmpty(currentMeasurements);
            Assert.Equal(expectedNumberOfPublishedDataPoints, publishDatReader.GetAndClearPublishedData().Length);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureAndPublishCurrent_ThrowsExecptionAndReturnsEmptyData(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, out var publishDatReader, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);

            void MeasureAndPublishCurrent() => sessionsBundle.MeasureAndPublishCurrent("CurrentMeasurments");

            var exception = Assert.Throws<AggregateException>(MeasureAndPublishCurrent);
            Assert.Empty(publishDatReader.GetAndClearPublishedData());
            exception.IfNotNull(e =>
            {
                foreach (var innerExeption in e.InnerExceptions)
                {
                    Assert.IsType<InvalidOperationException>(innerExeption);
                    Assert.Contains("PPMU cannot measure current on a channel that is not sourcing voltage or current.", innerExeption.Message);
                }
            });
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ForceVoltageMeasureCurrent_SucceedsAndMeasurementsNotEmpty(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, out var publishDatReader, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);

            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var currentMeasurements = sessionsBundle.MeasureCurrent();

            foreach (var pin in pins)
            {
                Assert.Contains(pin, currentMeasurements.PinNames);
            }
            Assert.Equal(tsmContext.SiteNumbers, currentMeasurements.SiteNumbers);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureCurrentWithoutForcing_ThrowsExecption(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, out var publishDatReader, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);

            void MeasureCurrent() => sessionsBundle.MeasureCurrent();

            var exception = Assert.Throws<AggregateException>(MeasureCurrent);
            Assert.Empty(publishDatReader.GetAndClearPublishedData());
            exception.IfNotNull(x =>
            {
                foreach (var innerExeption in x.InnerExceptions)
                {
                    Assert.IsType<InvalidOperationException>(innerExeption);
                    Assert.Contains("PPMU cannot measure current on a channel that is not sourcing voltage or current.", innerExeption.Message);
                }
            });
            Close(tsmContext);
        }
    }
}
