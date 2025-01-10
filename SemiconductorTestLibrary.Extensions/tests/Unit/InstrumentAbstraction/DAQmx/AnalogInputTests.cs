using System;
using System.Linq;
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
    public sealed class AnalogInputTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.CreateDAQmxAIVoltageTasks(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxAIVoltageTasks(_tsmContext);
        }

        [Fact]
        public void ReadAnalogSamplesFromOneChannelInMultipleChannelsTask_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            var results = tasksBundle.ReadAnalogMultiSample();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["VCC1"]);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["VCC1"]);
        }

        [Fact]
        public void ReadFiveAnalogSamplesFromTwoChannels_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx(new string[] { "VCC1", "VCC2" });
            var results = tasksBundle.ReadAnalogMultiSample(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["VCC1"].Length);
            Assert.Equal(5, results.ExtractSite(0)["VCC2"].Length);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["VCC1"].Length);
            Assert.Equal(5, results.ExtractSite(1)["VCC2"].Length);
        }

        [Theory]
        [InlineData("AIPin")]
        public void ReadAnalogSamplesFromTwoFilteredChannels_ResultsContainExpectedData(string pinName)
        {
            var sessionManager = Initialize("DAQmxMultiChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx(pinName);
            var filteredResult = tasksBundle.FilterBySite(new int[] { 1, 3 }).ReadAnalogMultiSample(100);
            var site1Data = filteredResult.GetValue(filteredResult.SiteNumbers[0], pinName).FirstOrDefault();
            var site2Data = filteredResult.GetValue(filteredResult.SiteNumbers[1], pinName).FirstOrDefault();

            Assert.True(0.10 > site1Data && 0.01 < site1Data);
            Assert.True(0.30 > site2Data && 0.20 < site2Data);
            Assert.Equal(2, filteredResult.SiteNumbers.Length);
        }

        [Fact]
        public void ReadAnalogWaveformSamplesFromOneChannelInMultipleChannelsTask_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");
            var results = tasksBundle.ReadAnalogWaveform();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(1, results.ExtractSite(0)["VCC1"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(1, results.ExtractSite(1)["VCC1"].SampleCount);
        }

        [Fact]
        public void ReadFiveAnalogWaveformSamplesFromTwoChannels_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx(new string[] { "VCC1", "VCC2" });
            var results = tasksBundle.ReadAnalogWaveform(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(2, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["VCC1"].SampleCount);
            Assert.Equal(5, results.ExtractSite(0)["VCC2"].SampleCount);
            Assert.Equal(2, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["VCC1"].SampleCount);
            Assert.Equal(5, results.ExtractSite(1)["VCC2"].SampleCount);
        }

        [Fact]
        public void ReadAnalogSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxSingleChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogMultiSample();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["AIPin"]);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Single(results.ExtractSite(1)["AIPin"]);
        }

        [Fact]
        public void ReadFiveAnalogSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxSingleChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogMultiSample(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["AIPin"].Length);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["AIPin"].Length);
        }

        [Fact]
        public void ReadAnalogWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxSingleChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogWaveform();

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(1, results.ExtractSite(0)["AIPin"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(1, results.ExtractSite(1)["AIPin"].SampleCount);
        }

        [Fact]
        public void ReadFiveAnalogWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var sessionManager = Initialize("DAQmxSingleChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("AIPin");
            var results = tasksBundle.ReadAnalogWaveform(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["AIPin"].SampleCount);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["AIPin"].SampleCount);
        }

        [Theory]
        [InlineData("AIPin")]
        public void ReadAnalogWaveformSamplesFromTwoFilteredChannels_ResultsContainExpectedData(string pinName)
        {
            var sessionManager = Initialize("DAQmxMultiChannelTests.pinmap");
            var tasksBundle = sessionManager.DAQmx(pinName);
            var filteredResult = tasksBundle.FilterBySite(new int[] { 1, 3 }).ReadAnalogWaveform(100);
            var site1Data = filteredResult.GetValue(filteredResult.SiteNumbers[0], pinName).Samples.FirstOrDefault().Value;
            var site2Data = filteredResult.GetValue(filteredResult.SiteNumbers[1], pinName).Samples.FirstOrDefault().Value;

            Assert.True(0.10 > site1Data && 0.01 < site1Data);
            Assert.True(0.30 > site2Data && 0.20 < site2Data);
            Assert.Equal(2, filteredResult.SiteNumbers.Length);
        }
    }
}
