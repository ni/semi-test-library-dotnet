using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    public sealed class ReadSingleChannelTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public ReadSingleChannelTests()
        {
            _tsmContext = CreateTSMContext("DAQmxSingleChannelTests.pinmap");
            _sessionManager = new TSMSessionManager(_tsmContext);
            SetupAndCleanup.CreateDAQmxTasks(_tsmContext);
        }

        public void Dispose()
        {
            SetupAndCleanup.ClearDAQmxTasks(_tsmContext);
        }

        [Fact]
        public void ReadAnalogSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogSamples();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["AIPin"]);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["AIPin"]);
        }

        [Fact]
        public void ReadFiveAnalogSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogSamples(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["AIPin"].Length);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["AIPin"].Length);
        }

        [Fact]
        public void ReadAnalogWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogWaveformSamples();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(1, results.ExtractSite(0)["AIPin"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(1, results.ExtractSite(1)["AIPin"].SampleCount);
        }

        [Fact]
        public void ReadFiveAnalogWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogWaveformSamples(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["AIPin"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["AIPin"].SampleCount);
        }

        [Fact]
        public void ReadU32SamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadU32Samples();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["DIPin"]);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["DIPin"]);
        }

        [Fact]
        public void ReadFiveU32SamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadU32Samples(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["DIPin"].Length);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["DIPin"].Length);
        }

        [Fact]
        public void ReadDigitalWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadDigitalWaveformSamples();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["DIPin"].Samples);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["DIPin"].Samples);
        }

        [Fact]
        public void ReadFiveDigitalWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadDigitalWaveformSamples(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["DIPin"].Samples.Count);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["DIPin"].Samples.Count);
        }

        [Fact]
        public void ReadBooleanSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadBooleanSamples();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.True(results.ExtractSite(0).ContainsKey("DIPin"));
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.True(results.ExtractSite(1).ContainsKey("DIPin"));
        }
    }
}
