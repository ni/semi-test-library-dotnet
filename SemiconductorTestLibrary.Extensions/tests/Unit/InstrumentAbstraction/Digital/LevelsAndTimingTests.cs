using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
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

            Assert.Equal(2, results.PinNames.Length);
            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureTDROffsets_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var results = sessionsBundle.MeasureTDROffsets();

            Assert.Equal(2, results.PinNames.Length);
            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ApplyPerSitePerPinTDROffsets_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new PinSiteData<Ivi.Driver.PrecisionTimeSpan>(new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>()
            {
                ["C0"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8)
                },
                ["C1"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8)
                }
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
            var offsets = new PinSiteData<Ivi.Driver.PrecisionTimeSpan>(new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>()
            {
                ["C0"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8)
                },
                ["C1"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8)
                }
            });
            sessionsBundle.ApplyTDROffsets(offsets);

            Assert.Equal(1e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.5e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").TdrOffset.TotalSeconds);
            Assert.Equal(1.2e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.7e-8, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").TdrOffset.TotalSeconds);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_SavePerSitePerPinTDROffsetsToAndFromFile_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new PinSiteData<Ivi.Driver.PrecisionTimeSpan>(new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>()
            {
                ["C0"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8)
                },
                ["C1"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8)
                }
            });
            var fileName = Path.GetTempFileName();
            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            PreciseWait(timeInSeconds: 0.1);
            var offsetsFromFile = sessionsBundle.LoadTDROffsetsFromFile(fileName);

            Assert.Equal(offsets.GetValue(0, "C0"), offsetsFromFile.GetValue(0, "C0"));
            Assert.Equal(offsets.GetValue(0, "C1"), offsetsFromFile.GetValue(0, "C1"));
            Assert.Equal(offsets.GetValue(1, "C0"), offsetsFromFile.GetValue(1, "C0"));

            RemoveTemporaryFile(fileName);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_SavePerSitePerPinTDROffsetsToAndFromFile_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new PinSiteData<Ivi.Driver.PrecisionTimeSpan>(new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>()
            {
                ["C0"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8)
                },
                ["C1"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8)
                }
            });
            var fileName = Path.GetTempFileName();
            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            PreciseWait(timeInSeconds: 0.1);
            var offsetsFromFile = sessionsBundle.LoadTDROffsetsFromFile(fileName);

            Assert.Equal(offsets.GetValue(0, "C0"), offsetsFromFile.GetValue(0, "C0"));
            Assert.Equal(offsets.GetValue(0, "C1"), offsetsFromFile.GetValue(0, "C1"));
            Assert.Equal(offsets.GetValue(1, "C0"), offsetsFromFile.GetValue(1, "C0"));

            RemoveTemporaryFile(fileName);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_SavePerSitePerPinTDROffsetsToAndFromFile_OverwritesExistingFile()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new PinSiteData<Ivi.Driver.PrecisionTimeSpan>(new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>()
            {
                ["C0"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8)
                },
                ["C1"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8)
                }
            });
            var fileName = Path.GetTempFileName();

            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            var numberofLinesBefore = File.ReadLines(fileName).Count();
            // Update the offsets value so that during debug, it is clear the file changes vs. not just getting appended to unsuccessfully.
            var updatedOffsets = new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>();
            foreach (var pinName in offsets.PinNames)
            {
                if (!updatedOffsets.ContainsKey(pinName))
                {
                    updatedOffsets.Add(pinName, new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>());
                }
                foreach (var siteNumber in offsets.SiteNumbers)
                {
                    updatedOffsets[pinName].Add(siteNumber, offsets.GetValue(siteNumber, pinName));
                }
            }
            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            var numberofLinesAfter = File.ReadLines(fileName).Count();

            Assert.True(File.Exists(fileName));
            Assert.Equal(numberofLinesBefore, numberofLinesAfter);

            RemoveTemporaryFile(fileName);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_SavePerSitePerPinTDROffsetsToAndFromFile_OverwritesExistingFile()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new PinSiteData<Ivi.Driver.PrecisionTimeSpan>(new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>()
            {
                ["C0"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8)
                },
                ["C1"] = new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>()
                {
                    [0] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8),
                    [1] = Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8)
                }
            });
            var fileName = Path.GetTempFileName();

            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            var numberofLinesBefore = File.ReadLines(fileName).Count();
            // Update the offsets value so that during debug, it is clear the file changes vs. not just getting appended to unsuccessfully.
            var updatedOffsets = new Dictionary<string, IDictionary<int, Ivi.Driver.PrecisionTimeSpan>>();
            foreach (var pinName in offsets.PinNames)
            {
                if (!updatedOffsets.ContainsKey(pinName))
                {
                    updatedOffsets.Add(pinName, new Dictionary<int, Ivi.Driver.PrecisionTimeSpan>());
                }
                foreach (var siteNumber in offsets.SiteNumbers)
                {
                    updatedOffsets[pinName].Add(siteNumber, offsets.GetValue(siteNumber, pinName));
                }
            }
            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            var numberofLinesAfter = File.ReadLines(fileName).Count();

            Assert.True(File.Exists(fileName));
            Assert.Equal(numberofLinesBefore, numberofLinesAfter);

            RemoveTemporaryFile(fileName);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_MeasureTDROffsetsPerInstrumentSession_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.MeasureTDROffsets(out var results);

            Assert.Equal(2, results.Length);
            Assert.Equal(2, results[0].Length);
            Assert.Equal(2, results[1].Length);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureTDROffsetsPerInstrumentSession_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.MeasureTDROffsets(out var results);

            Assert.Equal(2, results.Length);
            Assert.Equal(3, results[0].Length);
            Assert.Single(results[1]);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_ApplyPerInstrumentSessionTDROffsets_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new Ivi.Driver.PrecisionTimeSpan[2][]
            {
                new[] { Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8), Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8) },
                new[] { Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8), Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8) }
            };
            sessionsBundle.ApplyTDROffsets(offsets);

            Assert.Equal(1e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.5e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").TdrOffset.TotalSeconds);
            Assert.Equal(1.2e-8, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.7e-8, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").TdrOffset.TotalSeconds);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_ApplyPerInstrumentSessionTDROffsets_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new Ivi.Driver.PrecisionTimeSpan[][]
            {
                new[]
                {
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8), // site0/C0
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8), // site0/C1
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8) // site1/C0
                },
                new[]
                {
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8) // site1/C1
                }
            };
            sessionsBundle.ApplyTDROffsets(offsets);

            Assert.Equal(1e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.5e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site0/C1").TdrOffset.TotalSeconds);
            Assert.Equal(1.2e-8, sessionsBundle.InstrumentSessions.ElementAt(0).Session.PinAndChannelMap.GetPinSet("site1/C0").TdrOffset.TotalSeconds);
            Assert.Equal(1.7e-8, sessionsBundle.InstrumentSessions.ElementAt(1).Session.PinAndChannelMap.GetPinSet("site1/C1").TdrOffset.TotalSeconds);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_SaveInstrumentSessionTDROffsetsToAndFromFile_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new Ivi.Driver.PrecisionTimeSpan[2][]
            {
                new[] { Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8), Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8) },
                new[] { Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8), Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8) }
            };
            var fileName = Path.GetTempFileName();
            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            PreciseWait(timeInSeconds: 0.1);
            sessionsBundle.LoadTDROffsetsFromFile(fileName, out var offsetsFromFile);

            Assert.Equal(offsets, offsetsFromFile);

            RemoveTemporaryFile(fileName);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_SaveInstrumentSessionTDROffsetsToAndFromFile_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new Ivi.Driver.PrecisionTimeSpan[2][]
            {
                new[]
                {
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8), // site0/C0
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8), // site0/C1
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8), // site1/C0
                },
                new[]
                {
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8) // site1/C1
                }
            };
            var fileName = Path.GetTempFileName();
            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            PreciseWait(timeInSeconds: 0.1);
            sessionsBundle.LoadTDROffsetsFromFile(fileName, out var offsetsFromFile);

            Assert.Equal(offsets, offsetsFromFile);

            RemoveTemporaryFile(fileName);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_SaveInstrumentSessionTDROffsetsToAndFromFile_OverwritesExistingFile()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new Ivi.Driver.PrecisionTimeSpan[2][]
            {
                new[] { Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8), Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8) },
                new[] { Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8), Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8) }
            };
            var fileName = Path.GetTempFileName();

            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            var numberofLinesBefore = File.ReadLines(fileName).Count();
            // Update the offsets value so that during debug, it is clear the file changes vs. not just getting appended to unsuccessfully.
            for (int instrIndex = 0; instrIndex < offsets.Length; instrIndex++)
            {
                for (int chIndex = 0; chIndex < offsets[instrIndex].Length; chIndex++)
                {
                    offsets[instrIndex][chIndex] = offsets[instrIndex][chIndex] + Ivi.Driver.PrecisionTimeSpan.FromSeconds(.5);
                }
            }
            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            var numberofLinesAfter = File.ReadLines(fileName).Count();

            Assert.True(File.Exists(fileName));
            Assert.Equal(numberofLinesBefore, numberofLinesAfter);

            RemoveTemporaryFile(fileName);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_SaveInstrumentSessionTDROffsetsToAndFromFile_OverwritesExistingFile()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var offsets = new Ivi.Driver.PrecisionTimeSpan[2][]
            {
                new[]
                {
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1e-8), // site0/C0
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.5e-8), // site0/C1
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.2e-8), // site1/C0
                },
                new[]
                {
                    Ivi.Driver.PrecisionTimeSpan.FromSeconds(1.7e-8) // site1/C1
                }
            };
            var fileName = Path.GetTempFileName();

            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            var numberofLinesBefore = File.ReadLines(fileName).Count();
            // Update the offsets value so that during debug, it is clear the file changes vs. not just getting appended to unsuccessfully.
            for (int instrIndex = 0; instrIndex < offsets.Length; instrIndex++)
            {
                for (int chIndex = 0; chIndex < offsets[instrIndex].Length; chIndex++)
                {
                    offsets[instrIndex][chIndex] = offsets[instrIndex][chIndex] + Ivi.Driver.PrecisionTimeSpan.FromSeconds(.5);
                }
            }
            sessionsBundle.SaveTDROffsetsToFile(offsets, fileName);
            var numberofLinesAfter = File.ReadLines(fileName).Count();

            Assert.True(File.Exists(fileName));
            Assert.Equal(numberofLinesBefore, numberofLinesAfter);

            RemoveTemporaryFile(fileName);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureTdrEndpointTermination_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.ConfigureTdrEndpointTermination(TdrEndpointTermination.TdrToShortToGround);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(TdrEndpointTermination.TdrToShortToGround, sessionInfo.Session.Timing.TdrEndpointTermination);
            });
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_GetTimeSetPeriod_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var pins = new string[] { "C0", "C1" };

            var sessionsBundle = sessionManager.Digital();
            sessionsBundle.ConfigureTimeSetPeriod("TS_SW", 5e-6);

            for (int i = 0; i <= 1; i++)
            {
                var timeSetPeriod = sessionsBundle.GetTimeSetPeriod("TS_SW").ExtractSite(i).Select(value => value.Value.TotalSeconds);
                Assert.True(timeSetPeriod.All(value => value == 5e-6));
            }
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_GetTimeSetPeriodDistinct_ValueCorrectlySet(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital();
            sessionsBundle.ConfigureTimeSetPeriod("TS_SW", 5e-6);
            sessionsBundle.ConfigureTimeSetPeriod("TS", 5e-6);
            var timeSet1 = sessionsBundle.GetTimeSetPeriodDistinct("TS_SW").TotalSeconds;
            var timeSet2 = sessionsBundle.GetTimeSetPeriodDistinct("TS").TotalSeconds;

            Assert.Equal(5e-6, timeSet1);
            Assert.Equal(5e-6, timeSet2);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_GetTimeSetPeriodDistinct_ThrowsException(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
            var timeSetPeriod = 5e-6;

            var sessionsBundle = sessionManager.Digital();
            foreach (var session in sessionsBundle.InstrumentSessions)
            {
                session.Session.Timing.GetTimeSet("TS").ConfigurePeriod(Ivi.Driver.PrecisionTimeSpan.FromSeconds(timeSetPeriod));
                timeSetPeriod /= 10;
            }

            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.GetTimeSetPeriodDistinct("TS"));
            Assert.Contains("The value of the time set period (TS) is not the same for all underlying instrument sessions.", exception.Message);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_GetPerSiteTimeSetCompareEdgesStrobe_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var pins = new string[] { "C0", "C1" };

            var sessionsBundle = sessionManager.Digital(pins);
            var compareEdges = new SiteData<double>(new Dictionary<int, double>()
            {
                [0] = 5e-6,
                [1] = 8e-6
            });
            sessionsBundle.ConfigureTimeSetCompareEdgesStrobe("TS_SW", compareEdges);
            var timeSetEdge = sessionsBundle.GetTimeSetEdge("TS_SW", TimeSetEdge.CompareStrobe);

            for (int i = 0; i <= 1; i++)
            {
                var siteData = timeSetEdge.ExtractSite(i).Values.Select(value => value.TotalSeconds);
                Assert.True(siteData.All(value => value == compareEdges.GetValue(i)));
            }
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", 2)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", 2)]
        public void SessionsInitialized_GetTimeSetEdgeMultiplier_ValueCorrectlySet(string pinMap, string digitalProject, int edgeMultiplier)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital();
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.GetTimeSet("TS_SW").ConfigureEdgeMultiplier(sessionInfo.PinSet, edgeMultiplier);
            });
            var timeSet1 = sessionsBundle.GetTimeSetEdgeMultiplier("TS_SW");

            for (int i = 0; i <= 1; i++)
            {
                var siteData = timeSet1.ExtractSite(i).Values;
                Assert.True(siteData.All(value => value == edgeMultiplier));
            }
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj", DriveFormat.ReturnToHigh)]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj", DriveFormat.ReturnToHigh)]
        public void SessionsInitialized_GetTimeSetDriveFormat_ValueCorrectlySet(string pinMap, string digitalProject, DriveFormat driveFormat)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital();
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.Session.Timing.GetTimeSet("TS_SW").ConfigureDriveFormat(sessionInfo.PinSet, driveFormat);
            });
            var timeSet1 = sessionsBundle.GetTimeSetDriveFormat("TS_SW");

            for (int i = 0; i <= 1; i++)
            {
                var siteData = timeSet1.ExtractSite(i).Values;
                Assert.True(siteData.All(value => value == driveFormat));
            }
        }

        [Fact]
        public void SessionsInitialized_ConfigureTimeSetPeriodWithoutSpecifyingPins_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");

            var sessionsBundle = sessionManager.Digital();
            sessionsBundle.ConfigureTimeSetPeriod("TS_SW", 5e-6);

            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).AssociatedSitePinList.Count);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(5e-6, sessionInfo.Session.Timing.GetTimeSet("TS_SW").Period.TotalSeconds);
            });
        }

        [Fact]
        public void SessionsInitialized_ApplyLevelsAndTimingWithoutSpecifyingPins_ValueCorrectlySet()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");

            var sessionsBundle = sessionManager.Digital();
            sessionsBundle.ApplyLevelsAndTiming("Levels_MixedSignal", "Timing_MixedSignal");

            Assert.Equal(5, sessionsBundle.InstrumentSessions.ElementAt(0).AssociatedSitePinList.Count);
            Assert.Equal(1.8, sessionsBundle.InstrumentSessions.ElementAt(0).PinSet.DigitalLevels.Vih, 1);
        }

        /// <summary>
        /// Removes a temporary file, if it exists.
        /// This method is intended to be called at the end of the unit test that saving information to a temporary file.
        /// </summary>
        /// <param name="fileName">The name of the file.</param>
        private static void RemoveTemporaryFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }
    }
}
