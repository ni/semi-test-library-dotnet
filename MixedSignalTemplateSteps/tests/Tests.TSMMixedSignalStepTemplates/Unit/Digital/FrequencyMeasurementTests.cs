using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital;
using Xunit;
using static NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.Digital.InitializeAndClose;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.Digital
{
    [Collection("NonParallelizable")]
    public class FrequencyMeasurementTests
    {
        [Theory]
        [InlineData("TwoDevicesWorkForTwoSitesSeparately.pinmap", "TwoDevicesWorkForTwoSitesSeparately.digiproj")]
        [InlineData("OneDeviceWorksForOnePinOnTwoSites.pinmap", "OneDeviceWorksForOnePinOnTwoSites.digiproj")]
        public void SessionsInitialized_MeasureFrequency_Succeeds(string pinMap, string digitalProject)
        {
            var tsmContext = CreateTSMContext(pinMap, digitalProject);
            var sessionManager = new TSMSessionManager(tsmContext);
            Initialize(tsmContext);

            var sessionsBundle = sessionManager.GetDigitalSessionsBundle(new string[] { "C0", "C1" });
            var results = sessionsBundle.MeasureFrequency();

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(2, results.Values[0].Count);
            Assert.True(results.Values[0].ContainsKey("C0"));
            Assert.True(results.Values[0].ContainsKey("C1"));
            Assert.Equal(2, results.Values[1].Count);
            Assert.True(results.Values[1].ContainsKey("C0"));
            Assert.True(results.Values[1].ContainsKey("C1"));
            Close(tsmContext);
        }
    }
}
