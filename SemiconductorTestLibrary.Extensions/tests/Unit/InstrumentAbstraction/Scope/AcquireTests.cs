using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Scope;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;

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
        public void DifferentSessionTypes_ReadWaveform_ReturnsRequestedNumberOfSamples(bool pinMapWithChannelGroup)
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

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public void DifferentSessionTypes_ReadWaveformOnSameDevice_ThrowsException(bool pinMapWithChannelGroup)
        {
            var sessionManager = Initialize(pinMapWithChannelGroup);
            var pins = new string[] { "Vosc1", "Vosc2" };
            var sessionsBundle1 = sessionManager.Scope("Vosc1");
            var sessionsBundle2 = sessionManager.Scope("Vosc2");
            TimingSettings timingSettings1 = new TimingSettings();
            timingSettings1.SampleRateMin = 1e6;
            timingSettings1.RecordLengthMin = 10;
            TimingSettings timingSettings2 = new TimingSettings();

            const long numberOfSamples = 64;

            sessionsBundle1.ConfigureTiming(timingSettings1);
            sessionsBundle2.ConfigureTiming(timingSettings2);
            sessionsBundle1.ConfigureTriggerImmediate();
            sessionsBundle2.ConfigureTriggerImmediate();
            var waveformData1 = sessionsBundle1.ReadWaveform(PrecisionTimeSpan.FromSeconds(5), numberOfSamples);
            var waveformData2 = sessionsBundle2.ReadWaveform(PrecisionTimeSpan.FromSeconds(5), numberOfSamples);

            Assert.NotNull(waveformData1);
            Assert.Contains("Vosc1", waveformData1.PinNames);
            Assert.NotEmpty(waveformData1.SiteNumbers);
            Assert.NotNull(waveformData2);
            Assert.Contains("Vosc2", waveformData2.PinNames);
            Assert.NotEmpty(waveformData2.SiteNumbers);
            foreach (var siteNumber in waveformData1.SiteNumbers)
            {
                var waveform = waveformData1.GetValue(siteNumber, "Vosc1");
                Assert.NotNull(waveform);
                Assert.Equal(numberOfSamples, waveform.Samples.Count);
            }
            foreach (var siteNumber in waveformData2.SiteNumbers)
            {
                var waveform = waveformData2.GetValue(siteNumber, "Vosc2");
                Assert.NotNull(waveform);
                Assert.Equal(numberOfSamples, waveform.Samples.Count);
            }
        }
    }
}
