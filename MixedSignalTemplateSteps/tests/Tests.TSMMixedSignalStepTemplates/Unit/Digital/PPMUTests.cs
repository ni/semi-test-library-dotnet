using System.Collections.Generic;
using System.Linq;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.Restricted;
using Xunit;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.MixedSignalLibrary.TypeDefinitions;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.Digital
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
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
        public void TwoDevicesWorkForTwoSitesSeparately_MeasureAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(2, results.Item1.Length);
            Assert.Equal(2, results.Item1[0].Length);
            Assert.Equal(2, results.Item1[1].Length);
            Assert.Equal(2, results.Item2.Length);
            Assert.Equal(2, results.Item2[0].Length);
            Assert.Equal(2, results.Item2[1].Length);
            Close(tsmContext);
        }

        [Fact]
        public void TwoDevicesWorkForTwoSitesSeparately_MeasureVoltageAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var tsmContext = CreateTSMContext("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Voltage);

            Assert.Equal(2, results.Item1.Length);
            Assert.Equal(2, results.Item1[0].Length);
            Assert.Equal(2, results.Item1[1].Length);
            Assert.Null(results.Item2);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults();

            Assert.Equal(2, results.Item1.Length);
            Assert.Equal(3, results.Item1[0].Length);
            Assert.Single(results.Item1[1]);
            Assert.Equal(2, results.Item2.Length);
            Assert.Equal(3, results.Item2[0].Length);
            Assert.Single(results.Item2[1]);
            Close(tsmContext);
        }

        [Fact]
        public void OneDeviceWorksForOnePinOnTwoSites_MeasureCurrentAndReturnPerInstrumentPerChannelResults_ValuesCorrectlyGet()
        {
            var tsmContext = CreateTSMContext("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj");
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            var results = sessionsBundle.MeasureAndReturnPerInstrumentPerChannelResults(MeasurementType.Current);

            Assert.Equal(2, results.Item2.Length);
            Assert.Equal(3, results.Item2[0].Length);
            Assert.Single(results.Item2[1]);
            Assert.Null(results.Item1);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureAndReturnPerSitePerPinResults_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.ForceCurrent(currentLevel: 0.02);
            var results = sessionsBundle.MeasureAndReturnPerSitePerPinResults();

            Assert.Equal(2, results.Item1.Values.Count);
            Assert.Equal(2, results.Item1.Values[0].Count);
            Assert.Equal(2, results.Item1.Values[1].Count);
            Assert.Equal(2, results.Item2.Values.Count);
            Assert.Equal(2, results.Item2.Values[0].Count);
            Assert.Equal(2, results.Item2.Values[1].Count);
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

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.ForceCurrent(currentLevel: 0.02);
            var results = sessionsBundle.MeasureAndReturnPerSitePerPinResults(MeasurementType.Voltage);

            Assert.Equal(2, results.Item1.Values.Count);
            Assert.Equal(2, results.Item1.Values[0].Count);
            Assert.Equal(2, results.Item1.Values[1].Count);
            Assert.Equal(2, results.Item2.Values.Count);
            Assert.Equal(2, results.Item2.Values[0].Count);
            Assert.Equal(2, results.Item2.Values[1].Count);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void VoltageForcedOnSessions_PowerDown_ChannelsAreTurnedOff(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);
            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.ForceVoltage(voltageLevel: 3.5, currentLimitRange: 0.01);
            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Ppmu, sessionInfo.PinSet.SelectedFunction));

            sessionsBundle.PowerDown();

            sessionsBundle.InstrumentSessions.SafeForEach(sessionInfo => Assert.Equal(SelectedFunction.Off, sessionInfo.PinSet.SelectedFunction));
            Close(tsmContext);
        }
    }
}
