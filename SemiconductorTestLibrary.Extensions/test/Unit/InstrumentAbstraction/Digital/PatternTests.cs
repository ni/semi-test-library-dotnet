using System;
using System.Diagnostics;
using Ivi.Driver;
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

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
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

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.BurstPattern("TX_RF");
            var results = sessionsBundle.GetSitePassFail();

            Assert.Equal(2, results.SiteNumbers.Length);
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

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.BurstPattern("TX_RF");
            var results = sessionsBundle.GetFailCount();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Contains("C0", results.ExtractSite(0).Keys);
            Assert.Contains("C1", results.ExtractSite(0).Keys);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Assert.Contains("C0", results.ExtractSite(1).Keys);
            Assert.Contains("C1", results.ExtractSite(1).Keys);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstLongPatternAndWaitUntilDone_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.Session.PatternControl.IsDone);
            });
            sessionsBundle.BurstPattern("LongRunningPattern", waitUntilDone: false);
            // Not Supported in Offline Mode
            // sessionsBundle.Do(sessionInfo =>
            // {
            //     Assert.False(sessionInfo.Session.PatternControl.IsDone);
            // });
            sessionsBundle.WaitUntilDone();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.Session.PatternControl.IsDone);
            });

            Close(tsmContext);
        }

        [Theory(Skip = "Not supported in offline mode. Should run with actual hardware.")]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstLongPatternAndWaitUntilDone_Timesout(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital("C0");

            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.Session.PatternControl.IsDone);
            });
            sessionsBundle.BurstPattern("LongRunningPattern", waitUntilDone: false);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.False(sessionInfo.Session.PatternControl.IsDone);
            });
            var exception = Assert.Throws<AggregateException>(() => sessionsBundle.WaitUntilDone(0.001));
            exception.IfNotNull(x =>
            {
                foreach (var innerExeption in x.InnerExceptions)
                {
                    Assert.IsType<MaxTimeExceededException>(innerExeption);
                }
            });

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WaitUntilDoneBadTimeoutArguement_ThrowsExeception(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital("C0");
            var exception = Assert.Throws<AggregateException>(() => sessionsBundle.WaitUntilDone(-2));
            exception.IfNotNull(x =>
            {
                foreach (var innerExeption in x.InnerExceptions)
                {
                    Assert.IsType<ArgumentException>(innerExeption);
                }
            });

            Close(tsmContext);
        }
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WaitUntilDone_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.WaitUntilDone();

            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstLongPatternAndAbort_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.Session.PatternControl.IsDone);
            });
            sessionsBundle.BurstPattern("LongRunningPattern", waitUntilDone: false);
            // Not Supported in Offline Mode
            // sessionsBundle.Do(sessionInfo =>
            // {
            //     Assert.False(sessionInfo.Session.PatternControl.IsDone);
            // });
            sessionsBundle.AbortPattern();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.Session.PatternControl.IsDone);
            });

            Close(tsmContext);
        }

        [Theory(Skip = "Not supported in offline mode. Should run with actual hardware.")]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstKeepAlivePatternAndAbort_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.False(sessionInfo.Session.PatternControl.IsKeepAliveActive);
            });
            sessionsBundle.BurstPattern("StartKeepAlive", waitUntilDone: false);
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.True(sessionInfo.Session.PatternControl.IsKeepAliveActive);
            });
            sessionsBundle.AbortKeepAlivePattern();
            sessionsBundle.Do(sessionInfo =>
            {
                Assert.False(sessionInfo.Session.PatternControl.IsKeepAliveActive);
            });

            Close(tsmContext);
        }
    }
}
