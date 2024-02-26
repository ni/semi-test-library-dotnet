using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital;
using Xunit;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.Digital
{
    [Collection("NonParallelizable")]
    public class PatternTests
    {
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstPattern_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.BurstPattern("TX_RF");

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstPatternAndReturnResults_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            var results = sessionsBundle.BurstPatternAndPublishResults("TX_RF");

            Assert.Equal(2, results.Length);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstPatternAndGetSitePassFail_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.BurstPattern("TX_RF");
            var results = sessionsBundle.GetSitePassFail();

            Assert.Equal(2, results.Count);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstPatternAndGetFailCountAndReturnPerSitePerPinResults_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.BurstPattern("TX_RF");
            var results = sessionsBundle.GetFailCountAndReturnPerSitePerPinResults();

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(2, results.Values[0].Count);
            Assert.Contains("C0", results.Values[0].Keys);
            Assert.Contains("C1", results.Values[0].Keys);
            Assert.Equal(2, results.Values[1].Count);
            Assert.Contains("C0", results.Values[1].Keys);
            Assert.Contains("C1", results.Values[1].Keys);
            Close(tsmContext);
        }
    }
}
