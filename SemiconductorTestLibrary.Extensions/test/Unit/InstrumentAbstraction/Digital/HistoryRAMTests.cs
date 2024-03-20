using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    public class HistoryRAMTests
    {
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_ConfigureHistoryRAM_ValuesCorrectlySet(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new HistoryRAMSettings { CyclesToAcquire = HistoryRamCycle.All };
            settings.TriggerSettings.TriggerType = HistoryRamTriggerType.CycleNumber;
            settings.TriggerSettings.PretriggerSamples = 5;
            settings.TriggerSettings.CycleNumber = 2;
            sessionsBundle.ConfigureHistoryRAM(settings);

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.Equal(HistoryRamCycle.All, sessionInfo.Session.HistoryRam.CyclesToAcquire);
                Assert.Equal(5, sessionInfo.Session.Trigger.HistoryRamTrigger.PretriggerSamples);
                Assert.Equal(HistoryRamTriggerType.CycleNumber, sessionInfo.Session.Trigger.HistoryRamTrigger.TriggerType);
                Assert.Equal(2, sessionInfo.Session.Trigger.HistoryRamTrigger.CycleNumber.Number);
            });
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_GetHistoryRAMConfiguration_ValuesCorrectlyGet(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var settings = new HistoryRAMSettings { CyclesToAcquire = HistoryRamCycle.All };
            settings.TriggerSettings.TriggerType = HistoryRamTriggerType.PatternLabel;
            settings.TriggerSettings.PretriggerSamples = 5;
            settings.TriggerSettings.PatternLabel = "DummyPatternLabel";
            sessionsBundle.ConfigureHistoryRAM(settings);
            var configuration = sessionsBundle.GetHistoryRAMConfiguration();

            Assert.Equal(HistoryRamCycle.All, configuration.CyclesToAcquire);
            Assert.Equal(5, configuration.TriggerSettings.PretriggerSamples);
            Assert.Equal(HistoryRamTriggerType.PatternLabel, configuration.TriggerSettings.TriggerType);
            Assert.Equal("DummyPatternLabel", configuration.TriggerSettings.PatternLabel);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_FetchHistoryRAMResults_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.BurstPattern("TX_RF");
            var results = sessionsBundle.FetchHistoryRAMResults();

            Assert.Equal(2, results.SiteNumbers.Length);
            Close(tsmContext);
        }
    }
}
