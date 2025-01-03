using System;
using System.Linq;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    public sealed class DigitalInputTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;
        private TSMSessionManager _sessionManager;

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxDITasks(_tsmContext);
        }

        [Fact]
        public void ReadU32SamplesFromOneChannel_ResultsContainExpectedData()
        {
            Initialize();
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadDigitalMultiSampleU32();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["DIPin"]);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["DIPin"]);
        }

        [Fact]
        public void ReadFiveU32SamplesFromOneChannel_ResultsContainExpectedData()
        {
            Initialize();
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadDigitalMultiSampleU32(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["DIPin"].Length);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["DIPin"].Length);
        }

        [Theory]
        [InlineData("DIPin", new int[] { 1, 3 })]
        [InlineData("DIPin", new int[] { 0, 1, 3 })]

        public void ReadDigitalSamples_ChannelsForMultipleSitesPerPinInstrument_ResultsContainExpectedData(string pinName, int[] sites)
        {
            Initialize("DAQmxMultiChannelTests.pinmap");
            var tasksBundle = _sessionManager.DAQmx(pinName);
            var results = tasksBundle.FilterBySite(sites).ReadDigitalMultiSampleU32(100);

            for (int i = 0; i < results.SiteNumbers.Length; i++)
            {
                int value = (int)Math.Pow(2, results.SiteNumbers[i]);
                Assert.Equal(value, (int)results.GetValue(results.SiteNumbers[i], pinName)[value]);
            }
            Assert.Equal(sites.Length, results.SiteNumbers.Length);
        }

        [Fact]
        public void ReadDigitalWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            Initialize();
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadDigitalWaveform();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["DIPin"].Samples);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["DIPin"].Samples);
        }

        [Fact]
        public void ReadFiveDigitalWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            Initialize();
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadDigitalWaveform(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["DIPin"].Samples.Count);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["DIPin"].Samples.Count);
        }

        [Theory]
        [InlineData("DIPin", new int[] { 1, 3 })]
        [InlineData("DIPin", new int[] { 0, 1, 3 })]

        public void ReadDigitalWaveformSamples_ChannelsForMultipleSitesPerPinInstrument_ResultsContainExpectedData(string pinName, int[] sites)
        {
            Initialize("DAQmxMultiChannelTests.pinmap");
            var tasksBundle = _sessionManager.DAQmx(pinName);
            var results = tasksBundle.FilterBySite(sites).ReadDigitalWaveform(100);

            for (int i = 0; i < results.SiteNumbers.Length; i++)
            {
                int sampleCount = (int)Math.Pow(2, results.SiteNumbers[i]);
                Assert.Equal(1, ((int)results.ExtractSite(results.SiteNumbers[i])[pinName].Samples[sampleCount].States.FirstOrDefault()));
            }
            Assert.Equal(sites.Length, results.SiteNumbers.Length);
        }

        [Fact]
        public void ReadBooleanSamplesFromOneChannel_ResultsContainExpectedData()
        {
            Initialize();
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadDigitalSingleSample();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.True(results.ExtractSite(0).ContainsKey("DIPin"));
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.True(results.ExtractSite(1).ContainsKey("DIPin"));
        }

        [Theory]
        [InlineData("DIPin", new int[] { 1, 3 })]
        [InlineData("DIPin", new int[] { 0, 1, 3 })]

        public void ReadBooleanSamples_ChannelsForMultipleSitesPerPinInstrument_ResultsContainExpectedData(string pinName, int[] sites)
        {
            Initialize("DAQmxMultiChannelTests.pinmap");
            var tasksBundle = _sessionManager.DAQmx(pinName);
            var results = tasksBundle.FilterBySite(sites).ReadDigitalSingleSample();

            for (int i = 0; i < results.SiteNumbers.Length; i++)
            {
                Assert.False(results.ExtractSite(results.SiteNumbers[i])[pinName]);
            }
            Assert.Equal(sites.Length, results.SiteNumbers.Length);
        }

        private void Initialize(string pinMapFileName = "DAQmxSingleChannelTests.pinmap")
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            _sessionManager = new TSMSessionManager(_tsmContext);
            InitializeAndClose.CreateDAQmxDITasks(_tsmContext);
        }
    }
}
