using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    public sealed class ReadTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public ReadTests()
        {
            _tsmContext = CreateTSMContext("DAQmxTests.pinmap");
            _sessionManager = new TSMSessionManager(_tsmContext);
            SetupAndCleanup.CreateDAQmxTasks(_tsmContext);
        }

        public void Dispose()
        {
            SetupAndCleanup.ClearDAQmxTasks(_tsmContext);
        }

        [Fact]
        public void ReadAnalogSamplesFromOneChannelInMultipleChannelsTask_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("VCC1");
            var results = tasksBundle.ReadAnalogSamples();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["VCC1"]);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["VCC1"]);
        }

        [Fact]
        public void ReadFiveAnalogSamplesFromTwoChannels_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx(new string[] { "VCC1", "VCC2" });
            var results = tasksBundle.ReadAnalogSamples(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["VCC1"].Length);
            Assert.Equal(5, results.ExtractSite(0)["VCC2"].Length);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["VCC1"].Length);
            Assert.Equal(5, results.ExtractSite(1)["VCC2"].Length);
        }

        [Fact]
        public void ReadAnalogWaveformSamplesFromOneChannelInMultipleChannelsTask_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("VCC1");
            var results = tasksBundle.ReadAnalogWaveformSamples();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(1, results.ExtractSite(0)["VCC1"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(1, results.ExtractSite(1)["VCC1"].SampleCount);
        }

        [Fact]
        public void ReadFiveAnalogWaveformSamplesFromTwoChannels_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx(new string[] { "VCC1", "VCC2" });
            var results = tasksBundle.ReadAnalogWaveformSamples(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["VCC1"].SampleCount);
            Assert.Equal(5, results.ExtractSite(0)["VCC2"].SampleCount);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["VCC1"].SampleCount);
            Assert.Equal(5, results.ExtractSite(1)["VCC2"].SampleCount);
        }

        // Cannot test reading multiple digital channels due to AzDO Bug #2563332: [PinMapEditor] Multiple DAQmx DIO channel list assignment is not supported
    }
}
