using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using NationalInstruments.TestStand.SemiconductorModule.Restricted;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
    public sealed class PPMUTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager InitializeSessionsAndCreateSessionManager(string pinMapFileName, string digitalPatternProjectFileName)
        {
            return InitializeSessionsAndCreateSessionManager(pinMapFileName, digitalPatternProjectFileName, out _);
        }

        public TSMSessionManager InitializeSessionsAndCreateSessionManager(string pinMapFileName, string digitalPatternProjectFileName, out IPublishedDataReader publishedDataReader)
        {
            _tsmContext = CreateTSMContext(pinMapFileName, out publishedDataReader, digitalPatternProjectFileName);
            Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            Close(_tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ForceSameVoltage_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);

            sessionsBundle.Do(sessionInfo => Assert.Equal(3.5, sessionInfo.PinSet.Ppmu.DCVoltage.VoltageLevel, 1));
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForcePerPinVoltage_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new Dictionary<string, double>() { ["C0"] = 3.5, ["C1"] = 5 };
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCVoltage.VoltageLevel, 1);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForcePerPinVoltage_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new Dictionary<string, double>() { ["C0"] = 3.5, ["C1"] = 5 };
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCVoltage.VoltageLevel, 1);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForcePerSiteVoltage_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new SiteData<double>(new Dictionary<int, double>() { [0] = 3.5, [1] = 5 });
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForcePerSiteVoltage_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new SiteData<double>(new Dictionary<int, double>() { [0] = 3.5, [1] = 5 });
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForcePerSitePerPinVoltage_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["C0"] = new Dictionary<int, double>() { [0] = 3, [1] = 4 },
                ["C1"] = new Dictionary<int, double>() { [0] = 3.5, [1] = 4.5 }
            });
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4.5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCVoltage.VoltageLevel, 1);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForcePerSitePerPinVoltage_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var voltageLevels = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["C0"] = new Dictionary<int, double>() { [0] = 3, [1] = 4 },
                ["C1"] = new Dictionary<int, double>() { [0] = 3.5, [1] = 4.5 }
            });
            sessionsBundle.ForceVoltage(voltageLevels, currentLimitRange: 0.01);

            Assert.Equal(3, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4.5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceVoltageWithPerPinSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUSettings>()
            {
                ["C0"] = new PPMUSettings() { VoltageLevel = 3.5, ApertureTime = 0.05 },
                ["C1"] = new PPMUSettings() { VoltageLevel = 5 },
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
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceVoltageWithPerPinSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUSettings>()
            {
                ["C0"] = new PPMUSettings() { VoltageLevel = 3.5 },
                ["C1"] = new PPMUSettings() { VoltageLevel = 5, ApertureTime = 0.05 },
            };
            sessionsBundle.ForceVoltage(settings);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.ApertureTime);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime, 2);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime, 2);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceVoltageWithPerSiteSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new SiteData<PPMUSettings>(new Dictionary<int, PPMUSettings>()
            {
                [0] = new PPMUSettings() { VoltageLevel = 3.5, ApertureTime = 0.05 },
                [1] = new PPMUSettings() { VoltageLevel = 5 },
            });
            sessionsBundle.ForceVoltage(settings);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime, 2);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.ApertureTime);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceVoltageWithPerSiteSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new SiteData<PPMUSettings>(new Dictionary<int, PPMUSettings>()
            {
                [0] = new PPMUSettings() { VoltageLevel = 3.5 },
                [1] = new PPMUSettings() { VoltageLevel = 5, ApertureTime = 0.05 },
            });
            sessionsBundle.ForceVoltage(settings);

            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime, 2);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceVoltageWithPerSitePerPinSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new PinSiteData<PPMUSettings>(new Dictionary<string, IDictionary<int, PPMUSettings>>()
            {
                ["C0"] = new Dictionary<int, PPMUSettings>() { [0] = new PPMUSettings() { VoltageLevel = 3, ApertureTime = 0.05 }, [1] = new PPMUSettings() { VoltageLevel = 4 } },
                ["C1"] = new Dictionary<int, PPMUSettings>() { [0] = new PPMUSettings() { VoltageLevel = 3.5 }, [1] = new PPMUSettings() { VoltageLevel = 4.5 } }
            });
            sessionsBundle.ForceVoltage(settings);

            Assert.Equal(3, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime);
            Assert.Equal(4, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4.5, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCVoltage.VoltageLevel, 1);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceVoltageWithPerSitePerPinSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new PinSiteData<PPMUSettings>(new Dictionary<string, IDictionary<int, PPMUSettings>>()
            {
                ["C0"] = new Dictionary<int, PPMUSettings>() { [0] = new PPMUSettings() { VoltageLevel = 3, ApertureTime = 0.05 }, [1] = new PPMUSettings() { VoltageLevel = 4 } },
                ["C1"] = new Dictionary<int, PPMUSettings>() { [0] = new PPMUSettings() { VoltageLevel = 3.5 }, [1] = new PPMUSettings() { VoltageLevel = 4.5 } }
            });
            sessionsBundle.ForceVoltage(settings);

            Assert.Equal(3, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(3.5, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4.5, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCVoltage.VoltageLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ForceSameCurrent_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceCurrent(currentLevel: 0.02);

            sessionsBundle.Do(sessionInfo => Assert.Equal(0.02, sessionInfo.PinSet.Ppmu.DCCurrent.CurrentLevel, 2));
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceCurrentWithSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUSettings>()
            {
                ["C0"] = new PPMUSettings() { CurrentLevel = 0.01, ApertureTime = 0.05 },
                ["C1"] = new PPMUSettings() { CurrentLevel = 0.02 },
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
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceCurrentWithSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUSettings>()
            {
                ["C0"] = new PPMUSettings() { CurrentLevel = 0.01 },
                ["C1"] = new PPMUSettings() { CurrentLevel = 0.02, ApertureTime = 0.05 },
            };
            sessionsBundle.ForceCurrent(settings);

            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.ApertureTime);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime, 2);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime, 2);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceCurrentWithPerPinSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUSettings>()
            {
                ["C0"] = new PPMUSettings() { CurrentLevel = 0.01, ApertureTime = 0.05 },
                ["C1"] = new PPMUSettings() { CurrentLevel = 0.02 },
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
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceCurrentWithPerPinSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new Dictionary<string, PPMUSettings>()
            {
                ["C0"] = new PPMUSettings() { CurrentLevel = 0.01 },
                ["C1"] = new PPMUSettings() { CurrentLevel = 0.02, ApertureTime = 0.05 },
            };
            sessionsBundle.ForceCurrent(settings);

            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site1/C0").Ppmu.ApertureTime);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime, 2);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime, 2);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceCurrentWithPerSiteSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new SiteData<PPMUSettings>(new Dictionary<int, PPMUSettings>()
            {
                [0] = new PPMUSettings() { CurrentLevel = 0.01, ApertureTime = 0.05 },
                [1] = new PPMUSettings() { CurrentLevel = 0.02 },
            });
            sessionsBundle.ForceCurrent(settings);

            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.ApertureTime);
            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime, 2);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.ApertureTime);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceCurrentWithPerSiteSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new SiteData<PPMUSettings>(new Dictionary<int, PPMUSettings>()
            {
                [0] = new PPMUSettings() { CurrentLevel = 0.01 },
                [1] = new PPMUSettings() { CurrentLevel = 0.02, ApertureTime = 0.05 },
            });
            sessionsBundle.ForceCurrent(settings);

            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime, 2);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ForceCurrentWithPerSitePerPinSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new PinSiteData<PPMUSettings>(new Dictionary<string, IDictionary<int, PPMUSettings>>()
            {
                ["C0"] = new Dictionary<int, PPMUSettings>() { [0] = new PPMUSettings() { CurrentLevel = 0.01, ApertureTime = 0.05 }, [1] = new PPMUSettings() { CurrentLevel = 0.02 } },
                ["C1"] = new Dictionary<int, PPMUSettings>() { [0] = new PPMUSettings() { CurrentLevel = 0.03 }, [1] = new PPMUSettings() { CurrentLevel = 0.02, ApertureTime = 0.06 } }
            });
            sessionsBundle.ForceCurrent(settings);

            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.ApertureTime);
            Assert.Equal(0.03, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.06, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").Ppmu.ApertureTime, 2);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ForceCurrentWithPerSitePerPinSettingsObject_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new PinSiteData<PPMUSettings>(new Dictionary<string, IDictionary<int, PPMUSettings>>()
            {
                ["C0"] = new Dictionary<int, PPMUSettings>() { [0] = new PPMUSettings() { CurrentLevel = 0.01, ApertureTime = 0.05 }, [1] = new PPMUSettings() { CurrentLevel = 0.02 } },
                ["C1"] = new Dictionary<int, PPMUSettings>() { [0] = new PPMUSettings() { CurrentLevel = 0.03 }, [1] = new PPMUSettings() { CurrentLevel = 0.02, ApertureTime = 0.06 } }
            });
            sessionsBundle.ForceCurrent(settings);

            Assert.Equal(0.01, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.05, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").Ppmu.ApertureTime, 2);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").Ppmu.ApertureTime);
            Assert.Equal(0.03, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(4e-6, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").Ppmu.ApertureTime);
            Assert.Equal(0.02, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.DCCurrent.CurrentLevel, 1);
            Assert.Equal(0.06, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.Ppmu.ApertureTime, 2);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_MeasureVoltageAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Voltage);

            Assert.Equal(2, results.Length);
            Assert.Equal(2, results[0].Length);
            Assert.Equal(2, results[1].Length);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_MeasureCurrentAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Current);

            Assert.Equal(2, results.Length);
            Assert.Equal(2, results[0].Length);
            Assert.Equal(2, results[1].Length);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureVoltageAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Voltage);

            Assert.Equal(2, results.Length);
            Assert.Equal(3, results[0].Length);
            Assert.Single(results[1]);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureCurrentAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Current);

            Assert.Equal(2, results.Length);
            Assert.Equal(3, results[0].Length);
            Assert.Single(results[1]);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureVoltageAndReturnPerSitePerPinResults_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceCurrent(currentLevel: 0.02);
            var results = sessionsBundle.MeasureVoltage();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureCurrentAndReturnPerSitePerPinResults_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceCurrent(currentLevel: 0.02);
            var results = sessionsBundle.MeasureCurrent();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void VoltageForcedOnSessions_TurnOffOutput_ChannelsAreTurnedOff(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.Do(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.TurnOffOutput();

            sessionsBundle.Do(sessionInfo => Assert.Equal(SelectedFunction.Off, sessionInfo.PinSet.SelectedFunction));
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void VoltageForcedOnSessions_DisconnectOuput_ChannelsAreDisconnected(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.Do(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.DisconnectOutput();

            sessionsBundle.Do(sessionInfo => Assert.Equal(SelectedFunction.Disconnect, sessionInfo.PinSet.SelectedFunction));
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void VoltageForcedOnSessions_SelectDigital_ChannelsSwitchToDigitalFunction(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.Do(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.SelectDigital();

            sessionsBundle.Do(sessionInfo => Assert.Equal(SelectedFunction.Digital, sessionInfo.PinSet.SelectedFunction));
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_SelectPPMU_ChannelsSwitchToDigitalFunction(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.Do(sessionInfo => Assert.Equal(SelectedFunction.Disconnect, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.SelectPPMU();

            sessionsBundle.Do(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureAperatureTime_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureApertureTime(0.05);

            sessionsBundle.Do(sessionInfo => Assert.Equal(0.05, sessionInfo.PinSet.Ppmu.ApertureTime, 2));
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureAndPublishVoltage_MeasurementArrayNotEmptyAndCorrectPublishedDataCount(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject, out var publishDatReader);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);
            var expectedNumberOfPublishedDataPoints = (pins.Length * _tsmContext.SiteNumbers.Count);

            sessionsBundle.MeasureAndPublishVoltage("VoltageMeasurments", out var voltageMeasurements);

            Assert.NotEmpty(voltageMeasurements);
            Assert.Equal(expectedNumberOfPublishedDataPoints, publishDatReader.GetAndClearPublishedData().Length);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureAndPublishCurrent_MeasurementArrayNotEmptyAndCorrectPublishedDataCount(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject, out var publishDatReader);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);
            var expectedNumberOfPublishedDataPoints = (pins.Length * _tsmContext.SiteNumbers.Count);

            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.MeasureAndPublishCurrent("CurrentMeasurments", out var currentMeasurements);

            Assert.NotEmpty(currentMeasurements);
            Assert.Equal(expectedNumberOfPublishedDataPoints, publishDatReader.GetAndClearPublishedData().Length);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureAndPublishCurrent_ThrowsExecptionAndReturnsEmptyData(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject, out var publishDatReader);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);

            void MeasureAndPublishCurrent() => sessionsBundle.MeasureAndPublishCurrent("CurrentMeasurments");

            AggregateException aggregateException = Assert.Throws<AggregateException>(MeasureAndPublishCurrent);
            Assert.Empty(publishDatReader.GetAndClearPublishedData());
            foreach (Exception innerExeption in aggregateException.InnerExceptions)
            {
                Assert.IsType<InvalidOperationException>(innerExeption);
                Assert.Contains("PPMU cannot measure current on a channel that is not sourcing voltage or current.", innerExeption.Message);
            }
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ForceVoltageMeasureCurrent_SucceedsAndMeasurementsNotEmpty(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject, out var publishDatReader);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);

            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var currentMeasurements = sessionsBundle.MeasureCurrent();

            foreach (var pin in pins)
            {
                Assert.Contains(pin, currentMeasurements.PinNames);
            }
            Assert.Equal(_tsmContext.SiteNumbers, currentMeasurements.SiteNumbers);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureCurrentWithoutForcing_ThrowsExecption(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject, out var publishDatReader);
            var pins = new string[] { "C0", "C1" };
            var sessionsBundle = sessionManager.Digital(pins);

            void MeasureCurrent() => sessionsBundle.MeasureCurrent();

            AggregateException aggregateException = Assert.Throws<AggregateException>(MeasureCurrent);
            Assert.Empty(publishDatReader.GetAndClearPublishedData());
            foreach (Exception innerExeption in aggregateException.InnerExceptions)
            {
                Assert.IsType<InvalidOperationException>(innerExeption);
                Assert.Contains("PPMU cannot measure current on a channel that is not sourcing voltage or current.", innerExeption.Message);
            }
        }

        [Fact]
        public void SessionsInitialized_ForceSameVoltageWithoutSpecifyingPins_ValuesCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");

            var sessionsBundle = sessionManager.Digital();
            var settings = new PPMUSettings() { VoltageLevel = 3.5, CurrentLimitRange = 0.01 };
            sessionsBundle.ForceVoltage(settings);

            Assert.Equal(2, sessionsBundle.InstrumentSessions.Count());
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).AssociatedSitePinList.Count);
            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(1).AssociatedSitePinList.Count);
            sessionsBundle.Do(sessionInfo => Assert.Equal(3.5, sessionInfo.PinSet.Ppmu.DCVoltage.VoltageLevel, 1));
        }
    }
}
