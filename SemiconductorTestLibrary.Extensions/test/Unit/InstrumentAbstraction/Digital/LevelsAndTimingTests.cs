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
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    public sealed class LevelsAndTimingTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager InitializeSessionsAndCreateSessionManager(string pinMapFileName, string digitalPatternProjectFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName, digitalPatternProjectFileName);
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
        public void SessionsInitialized_ConfigureTheSameLevel_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureSingleLevel(LevelsAndTiming.LevelType.Vih, levelValue: 3.5);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(3.5, sessionInfo.PinSet.DigitalLevels.Vih, 1);
            });
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ConfigurePerSiteLevels_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var levels = new SiteData<double>(new Dictionary<int, double>()
            {
                [0] = 0.1,
                [1] = 0.2
            });
            sessionsBundle.ConfigureSingleLevel(LevelsAndTiming.LevelType.Vil, levels);

            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(0).PinSet.DigitalLevels.Vil, 1);
            Assert.Equal(0.2, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.DigitalLevels.Vil, 1);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ConfigurePerSiteLevels_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var levels = new SiteData<double>(new Dictionary<int, double>()
            {
                [0] = 0.1,
                [1] = 0.2
            });
            sessionsBundle.ConfigureSingleLevel(LevelsAndTiming.LevelType.Vil, levels);

            Assert.Equal(0.1, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0, site0/C1").DigitalLevels.Vil, 1);
            Assert.Equal(0.2, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").DigitalLevels.Vil, 1);
            Assert.Equal(0.2, sessionsBundle.InstrumentSessions.ElementAt(1).PinSet.DigitalLevels.Vil, 1);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureVoltageLevels_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureVoltageLevels(vil: 1, vih: 3.6, vol: 1.5, voh: 3, vterm: 2);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(1, sessionInfo.PinSet.DigitalLevels.Vil, 1);
                Assert.Equal(3.6, sessionInfo.PinSet.DigitalLevels.Vih, 1);
                Assert.Equal(1.5, sessionInfo.PinSet.DigitalLevels.Vol, 1);
                Assert.Equal(3, sessionInfo.PinSet.DigitalLevels.Voh, 1);
                Assert.Equal(2, sessionInfo.PinSet.DigitalLevels.Vterm, 1);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureTerminationMode_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureTerminationMode(TerminationMode.Vterm);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TerminationMode.Vterm, sessionInfo.PinSet.DigitalLevels.TerminationMode);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureTheSameTimeSetCompareEdgesStrobe_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureTimeSetCompareEdgesStrobe("TS_SW", compareEdge: 5e-6);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(5e-6, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            });
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ConfigurePerSiteTimeSetCompareEdgesStrobe_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var compareEdges = new SiteData<double>(new Dictionary<int, double>()
            {
                [0] = 5e-6,
                [1] = 8e-6
            });
            sessionsBundle.ConfigureTimeSetCompareEdgesStrobe("TS_SW", compareEdges);

            var sessionInfo0 = sessionsBundle.InstrumentSessions.ElementAt(0);
            Assert.Equal(5e-6, sessionInfo0.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo0.PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            var sessionInfo1 = sessionsBundle.InstrumentSessions.ElementAt(1);
            Assert.Equal(8e-6, sessionInfo1.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo1.PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ConfigurePerSiteTimeSetCompareEdgesStrobe_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var compareEdges = new SiteData<double>(new Dictionary<int, double>()
            {
                [0] = 5e-6,
                [1] = 8e-6
            });
            sessionsBundle.ConfigureTimeSetCompareEdgesStrobe("TS_SW", compareEdges);

            var sessionInfo0Session = sessionsBundle.InstrumentSessions.ElementAt(0).Session;
            var sessionInfo0Site0PinSet = sessionInfo0Session.PinAndChannelMap.GetPinSet("site0/C0, site0/C1");
            var sessionInfo0Site1PinSet = sessionInfo0Session.PinAndChannelMap.GetPinSet("site1/C0");
            Assert.Equal(5e-6, sessionInfo0Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo0Site0PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            Assert.Equal(8e-6, sessionInfo0Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo0Site1PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            var sessionInfo1 = sessionsBundle.InstrumentSessions.ElementAt(1);
            Assert.Equal(8e-6, sessionInfo1.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo1.PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ConfigurePerSitePerPinTimeSetCompareEdgesStrobe_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var compareEdges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                { "C0", new Dictionary<int, double>() { [0] = 5e-6, [1] = 7e-6 } },
                { "C1", new Dictionary<int, double>() { [0] = 6e-6, [1] = 8e-6 } }
            });
            sessionsBundle.ConfigureTimeSetCompareEdgesStrobe("TS_SW", compareEdges);

            var sessionInfo0Session = sessionsBundle.InstrumentSessions.ElementAt(0).Session;
            var site0C0PinSet = sessionInfo0Session.PinAndChannelMap.GetPinSet("site0/C0");
            var site0C1PinSet = sessionInfo0Session.PinAndChannelMap.GetPinSet("site0/C1");
            Assert.Equal(5e-6, sessionInfo0Session.Timing.GetTimeSet("TS_SW").GetEdge(site0C0PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            Assert.Equal(6e-6, sessionInfo0Session.Timing.GetTimeSet("TS_SW").GetEdge(site0C1PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            var sessionInfo1Session = sessionsBundle.InstrumentSessions.ElementAt(1).Session;
            var site1C0PinSet = sessionInfo1Session.PinAndChannelMap.GetPinSet("site1/C0");
            var site1C1PinSet = sessionInfo1Session.PinAndChannelMap.GetPinSet("site1/C1");
            Assert.Equal(7e-6, sessionInfo1Session.Timing.GetTimeSet("TS_SW").GetEdge(site1C0PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            Assert.Equal(8e-6, sessionInfo1Session.Timing.GetTimeSet("TS_SW").GetEdge(site1C1PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ConfigurePerSitePerPinTimeSetCompareEdgesStrobe_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var compareEdges = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["C0"] = new Dictionary<int, double>() { [0] = 5e-6, [1] = 7e-6 },
                ["C1"] = new Dictionary<int, double>() { [0] = 6e-6, [1] = 8e-6 }
            });
            sessionsBundle.ConfigureTimeSetCompareEdgesStrobe("TS_SW", compareEdges);

            var sessionInfo0Session = sessionsBundle.InstrumentSessions.ElementAt(0).Session;
            var site0C0PinSet = sessionInfo0Session.PinAndChannelMap.GetPinSet("site0/C0");
            var site0C1PinSet = sessionInfo0Session.PinAndChannelMap.GetPinSet("site0/C1");
            var site1C0PinSet = sessionInfo0Session.PinAndChannelMap.GetPinSet("site1/C0");
            Assert.Equal(5e-6, sessionInfo0Session.Timing.GetTimeSet("TS_SW").GetEdge(site0C0PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            Assert.Equal(6e-6, sessionInfo0Session.Timing.GetTimeSet("TS_SW").GetEdge(site0C1PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            Assert.Equal(7e-6, sessionInfo0Session.Timing.GetTimeSet("TS_SW").GetEdge(site1C0PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
            var sessionInfo1Session = sessionsBundle.InstrumentSessions.ElementAt(1).Session;
            var site1C1PinSet = sessionInfo1Session.PinAndChannelMap.GetPinSet("site1/C1");
            Assert.Equal(8e-6, sessionInfo1Session.Timing.GetTimeSet("TS_SW").GetEdge(site1C1PinSet, TimeSetEdge.CompareStrobe).TotalSeconds);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureTimeSetPeriod_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureTimeSetPeriod("TS_SW", 5e-6);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(5e-6, sessionInfo.Session.Timing.GetTimeSet("TS_SW").Period.TotalSeconds);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureTimeSetDriverEdges_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureTimeSetDriveEdges("TS_SW", DriveFormat.ReturnToHigh, driveOn: 5e-6, driveData: 5e-6, driveReturn: 1e-5, driveOff: 1e-5);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(DriveFormat.ReturnToHigh, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetDriveFormat(sessionInfo.PinSet));
                Assert.Equal(5e-6, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveOn).TotalSeconds);
                Assert.Equal(5e-6, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveData).TotalSeconds);
                Assert.Equal(1e-5, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveReturn).TotalSeconds);
                Assert.Equal(1e-5, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveOff).TotalSeconds);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureTimeSetDriverEdgesWithTwoDriveData_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureTimeSetDriveEdges("TS_SW", DriveFormat.ReturnToLow, driveOn: 5e-6, driveData: 5e-6, driveReturn: 1e-5, driveOff: 2e-5, driveData2: 1.5e-5, driveReturn2: 2e-5);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(DriveFormat.ReturnToLow, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetDriveFormat(sessionInfo.PinSet));
                Assert.Equal(5e-6, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveOn).TotalSeconds);
                Assert.Equal(5e-6, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveData).TotalSeconds);
                Assert.Equal(1e-5, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveReturn).TotalSeconds);
                Assert.Equal(2e-5, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveOff).TotalSeconds);
                Assert.Equal(1.5e-5, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveData2).TotalSeconds);
                Assert.Equal(2e-5, sessionInfo.Session.Timing.GetTimeSet("TS_SW").GetEdge(sessionInfo.PinSet, TimeSetEdge.DriveReturn2).TotalSeconds);
            });
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_MeasureTDROffsets_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var results = sessionsBundle.MeasureTDROffsets();

            Assert.Equal(2, results.Length);
            Assert.Equal(2, results[0].Length);
            Assert.Equal(2, results[1].Length);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureTDROffsets_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var results = sessionsBundle.MeasureTDROffsets();

            Assert.Equal(2, results.Length);
            Assert.Equal(3, results[0].Length);
            Assert.Single(results[1]);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ApplyPerSitePerPinTDROffsets_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["C0"] = new Dictionary<int, double>() { [0] = 1e-8, [1] = 1.2e-8 },
                ["C1"] = new Dictionary<int, double>() { [0] = 1.5e-8, [1] = 1.7e-8 }
            });
            sessionsBundle.ApplyTDROffsets(offsets);

            Assert.Equal(1e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.5e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").TdrOffset.TotalSeconds);
            Assert.Equal(1.2e-8, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.7e-8, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").TdrOffset.TotalSeconds);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ApplyPerSitePerPinTDROffsets_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>>()
            {
                ["C0"] = new Dictionary<int, double>() { [0] = 1e-8, [1] = 1.2e-8 },
                ["C1"] = new Dictionary<int, double>() { [0] = 1.5e-8, [1] = 1.7e-8 }
            });
            sessionsBundle.ApplyTDROffsets(offsets);

            Assert.Equal(1e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.5e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").TdrOffset.TotalSeconds);
            Assert.Equal(1.2e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.7e-8, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").TdrOffset.TotalSeconds);
        }
    }
}
