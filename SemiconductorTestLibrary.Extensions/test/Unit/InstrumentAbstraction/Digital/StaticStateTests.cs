using System;
using System.Collections.Generic;
using NationalInstruments.ModularInstruments.NIDigital;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Digital
{
    [Collection("NonParallelizable")]
    public sealed class StaticStateTests : IDisposable
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
        public void SessionsInitialized_WriteTheSameStaticState_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.SelectedFunction = SelectedFunction.Digital;
            });
            sessionsBundle.WriteStatic(PinState._1);

            var results = sessionsBundle.ReadStatic();
            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
            // Writing static states is not supported in offline mode.
            // Assert.Equal(PinState.H, results[0]["C0"]);
            // Assert.Equal(PinState.H, results[0]["C1"]);
            // Assert.Equal(PinState.H, results[1]["C0"]);
            // Assert.Equal(PinState.H, results[1]["C1"]);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WritePerSiteStaticState_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var states = new Dictionary<int, PinState>()
            {
                [0] = PinState._0,
                [1] = PinState._1
            };
            sessionsBundle.WriteStatic(states);

            var results = sessionsBundle.ReadStatic();
            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
            // Writing static states is not supported in offline mode.
            // Assert.Equal(PinState.L, results[0]["C0"]);
            // Assert.Equal(PinState.L, results[0]["C1"]);
            // Assert.Equal(PinState.H, results[1]["C0"]);
            // Assert.Equal(PinState.H, results[1]["C1"]);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WritePerSitePerPinStaticState_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var sessionManager = InitializeSessionsAndCreateSessionManager(pinMap, digitalProject);

            var sessionsBundle = sessionManager.Digital(new string[] { "C0", "C1" });
            var states = new Dictionary<int, Dictionary<string, PinState>>()
            {
                [0] = new Dictionary<string, PinState>() { ["C0"] = PinState._0, ["C1"] = PinState._1 },
                [1] = new Dictionary<string, PinState>() { ["C0"] = PinState._1, ["C1"] = PinState._0 }
            };
            sessionsBundle.WriteStatic(states);

            var results = sessionsBundle.ReadStatic();
            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(2, results.ExtractSite(1).Count);
            // Writing static states is not supported in offline mode.
            // Assert.Equal(PinState.L, results[0]["C0"]);
            // Assert.Equal(PinState.H, results[0]["C1"]);
            // Assert.Equal(PinState.H, results[1]["C0"]);
            // Assert.Equal(PinState.L, results[1]["C1"]);
        }
    }
}
