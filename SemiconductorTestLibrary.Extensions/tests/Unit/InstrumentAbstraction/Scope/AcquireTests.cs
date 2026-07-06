using System;
using System.Collections.Generic;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.SemiconductorTestLibrary.Common.Utilities;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Scope
{
    [Collection("NonParallelizable")]
    public sealed class AcquireTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(bool pinMapWithChannelGroup)
        {
            string pinMapFileName = pinMapWithChannelGroup ? "ScopeWithPerChassisSession.pinmap" : "ScopeWithPerInstrumentSession.pinmap";
            return Initialize(pinMapFileName);
        }

        public TSMSessionManager Initialize(string pinMapFileName = "ScopeTests.pinmap")
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.Close(_tsmContext);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void ReadWaveform_DifferentSessionTypes_ReturnsRequestedNumberOfSamples(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var pins = new string[] { "Vosc1", "Vosc2" };
            var sessionsBundle = sessionManager.Scope(pins);
            const long numberOfSamples = 64;

            sessionsBundle.ConfigureTriggerImmediate();
            var waveformData = sessionsBundle.ReadWaveform(PrecisionTimeSpan.FromSeconds(5), numberOfSamples);

            Assert.NotNull(waveformData);
            Assert.Contains("Vosc1", waveformData.PinNames);
            Assert.NotEmpty(waveformData.SiteNumbers);

            foreach (var siteNumber in waveformData.SiteNumbers)
            {
                var waveform = waveformData.GetValue(siteNumber, "Vosc1");
                Assert.NotNull(waveform);
                Assert.Equal(numberOfSamples, waveform.Samples.Count);
            }
        }
    }
}
