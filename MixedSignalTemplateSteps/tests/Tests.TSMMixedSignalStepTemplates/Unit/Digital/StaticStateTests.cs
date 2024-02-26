using System.Collections.Generic;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital;
using NationalInstruments.ModularInstruments.NIDigital;
using Xunit;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.Digital
{
    [Collection("NonParallelizable")]
    public class StaticStateTests
    {
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WriteTheSameStaticState_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            sessionsBundle.Do(sessionInfo =>
            {
                sessionInfo.PinSet.SelectedFunction = SelectedFunction.Digital;
            });
            sessionsBundle.WriteStatic(PinState._1);

            var results = sessionsBundle.ReadStatic();
            Assert.Equal(2, results.Values.Count);
            Assert.Equal(2, results.Values[0].Count);
            Assert.Equal(2, results.Values[1].Count);
            // Writing static states is not supported in offline mode.
            // Assert.Equal(PinState.H, results[0]["C0"]);
            // Assert.Equal(PinState.H, results[0]["C1"]);
            // Assert.Equal(PinState.H, results[1]["C0"]);
            // Assert.Equal(PinState.H, results[1]["C1"]);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WritePerSiteStaticState_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            var states = new Dictionary<int, PinState>()
            {
                [0] = PinState._0,
                [1] = PinState._1
            };
            sessionsBundle.WriteStatic(states);

            var results = sessionsBundle.ReadStatic();
            Assert.Equal(2, results.Values.Count);
            Assert.Equal(2, results.Values[0].Count);
            Assert.Equal(2, results.Values[1].Count);
            // Writing static states is not supported in offline mode.
            // Assert.Equal(PinState.L, results[0]["C0"]);
            // Assert.Equal(PinState.L, results[0]["C1"]);
            // Assert.Equal(PinState.H, results[1]["C0"]);
            // Assert.Equal(PinState.H, results[1]["C1"]);
            Close(tsmContext);
        }

        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_WritePerSitePerPinStaticState_ValuesCanBeReadBack(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            var states = new Dictionary<int, Dictionary<string, PinState>>()
            {
                [0] = new Dictionary<string, PinState>() { ["C0"] = PinState._0, ["C1"] = PinState._1 },
                [1] = new Dictionary<string, PinState>() { ["C0"] = PinState._1, ["C1"] = PinState._0 }
            };
            sessionsBundle.WriteStatic(states);

            var results = sessionsBundle.ReadStatic();
            Assert.Equal(2, results.Values.Count);
            Assert.Equal(2, results.Values[0].Count);
            Assert.Equal(2, results.Values[1].Count);
            // Writing static states is not supported in offline mode.
            // Assert.Equal(PinState.L, results[0]["C0"]);
            // Assert.Equal(PinState.H, results[0]["C1"]);
            // Assert.Equal(PinState.H, results[1]["C0"]);
            // Assert.Equal(PinState.L, results[1]["C1"]);
            Close(tsmContext);
        }
    }
}
