using System;
using System.Linq;
using Ivi.Driver;
using NationalInstruments.SemiconductorTestLibrary.Common;
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
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.Lungyuan))]
    public sealed class PatternTests : IDisposable
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
        public void SessionsInitialized_BurstPatternSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.BurstPattern("TX_RF");
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstPatternAndReturnResults_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var results = sessionsBundle.BurstPatternAndPublishResults("TX_RF");

            Assert.Equal(2, results.Length);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstPatternAndGetSitePassFail_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.BurstPattern("TX_RF");
            var results = sessionsBundle.GetSitePassFail();

            Assert.Equal(2, results.SiteNumbers.Length);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstPatternAndGetFailCountAndReturnPerSitePerPinResults_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

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
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstLongPatternAndWaitUntilDone_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

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
        }

        [Theory(Skip = "Not supported in offline mode. Should run with actual hardware.")]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstLongPatternAndWaitUntilDone_Timesout(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);
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

            AggregateException aggregateException = Assert.Throws<AggregateException>(() => sessionsBundle.WaitUntilDone(0.001));
            foreach (Exception innerExeption in aggregateException.InnerExceptions)
            {
                Assert.IsType<IviCDriverException>(innerExeption);
                Assert.Contains("Specified operation did not complete, because the specified timeout expired.", innerExeption.Message);
            }
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WaitUntilDoneWithInvalidTimeout_ThrowsExeception(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital("C0");
            void WaitUntilDone() => sessionsBundle.WaitUntilDone(-2);

            AggregateException aggregateException = Assert.Throws<AggregateException>(WaitUntilDone);
            foreach (Exception innerExeption in aggregateException.InnerExceptions)
            {
                Assert.IsType<ArgumentException>(innerExeption);
            }
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WaitUntilDoneSucceeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital("C0");
            sessionsBundle.WaitUntilDone();
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstLongPatternAndAbort_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

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
        }

        [Theory(Skip = "Not supported in offline mode. Should run with actual hardware.")]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_BurstKeepAlivePatternAndAbort_Succeeds(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

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
        }

        [Fact]
        public void SessionsInitialized_BurstPatternWithoutSpecifyingPins_Succeeds()
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager("Mixed Signal Tests.pinmap", "Mixed Signal Tests.digiproj");

            var sessionsBundle = sessionManager.Digital();
            sessionsBundle.BurstPattern("TX_RF");
            sessionsBundle.WaitUntilDone();
            var results = sessionsBundle.GetSitePassFail();
            var failCountResults = sessionsBundle.GetFailCount();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, failCountResults.SiteNumbers.Length);
            Assert.Equal(5, failCountResults.ExtractSite(0).Count);
        }
    }
}
