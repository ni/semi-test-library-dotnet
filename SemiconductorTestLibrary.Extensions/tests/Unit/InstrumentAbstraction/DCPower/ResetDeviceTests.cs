using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DCPower;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using Xunit.Abstractions;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DCPower
{
    // TEMPORARY hardware test for the ResetDevice investigation (WI 3815948). Remove before merging.
    [Collection("NonParallelizable")]
    public sealed class ResetDeviceTests : IDisposable
    {
        private readonly ITestOutputHelper _output;
        private ISemiconductorModuleContext _tsmContext;

        public ResetDeviceTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public void Dispose()
        {
            InitializeAndClose.Close(_tsmContext);
        }

        [Theory]
        [InlineData("ResetProbe_SessionPerChannel.pinmap")]
        [InlineData("ResetProbe_SharedSessionsWithinSameModule.pinmap")]
        [InlineData("ResetProbe_CommonSession.pinmap")]
        public void ResetDevicesOnSite0SessionA_SessionBKeepsApertureTimeAndSessionAInitiateThrows(string pinMap)
        {
            const double expectedApertureTime = 0.08;
            _tsmContext = CreateTSMContext(pinMap);
            InitializeAndClose.Initialize(_tsmContext);
            var sessionManager = new TSMSessionManager(_tsmContext);
            var sessionsBundle = sessionManager.DCPower(new[] { "VCC", "VDD" });
            var sessionA = sessionsBundle.FilterBySite(0);
            var sessionB = sessionsBundle.FilterBySite(_tsmContext.SiteNumbers.Where(site => site != 0).ToArray());

            sessionB.ConfigureApertureTimeInSeconds(expectedApertureTime);
            ResetDeviceLog.Clear();
            sessionA.ResetDevices();

            _output.WriteLine($"Pin map: {pinMap}");
            foreach (var entry in ResetDeviceLog.Snapshot())
            {
                _output.WriteLine($"  {entry}");
            }
            var apertureTimes = sessionB.GetApertureTimeInSeconds(out _);
            sessionB.Do((_, sitePinInfo) =>
            {
                _output.WriteLine($"Session B {sitePinInfo.IndividualChannelString} (Site{sitePinInfo.SiteNumber}/{sitePinInfo.PinName}) aperture time: {apertureTimes.GetValue(sitePinInfo)} s");
                Assert.Equal(expectedApertureTime, apertureTimes.GetValue(sitePinInfo), 4);
            });
            var exception = Record.Exception(() => sessionA.Initiate());
            _output.WriteLine($"Session A Initiate: {(exception == null ? "no exception" : $"{exception.GetType().Name}: {exception.Message}")}");
            Assert.NotNull(exception);
        }
    }
}
