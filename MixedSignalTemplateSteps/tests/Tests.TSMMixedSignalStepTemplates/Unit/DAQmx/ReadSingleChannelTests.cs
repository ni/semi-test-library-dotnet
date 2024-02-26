using System;
using NationalInstruments.MixedSignalLibrary.Common;
using NationalInstruments.MixedSignalLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.Utilities.TSMContext;

namespace NationalInstruments.Tests.TSMMixedSignalStepTemplates.Unit.DAQmx
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
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("AIPin");
            var results = tasksBundle.ReadAnalogSamples();

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.Single(results.Values[0]["AIPin"]);
            Assert.Equal(1, results.Values[1].Count);
            Assert.Single(results.Values[1]["AIPin"]);
        }

        [Fact]
        public void ReadFiveAnalogSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("AIPin");
            var results = tasksBundle.ReadAnalogSamples(samplesToRead: 5);

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.Equal(5, results.Values[0]["AIPin"].Length);
            Assert.Equal(1, results.Values[1].Count);
            Assert.Equal(5, results.Values[1]["AIPin"].Length);
        }

        [Fact]
        public void ReadAnalogWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("AIPin");
            var results = tasksBundle.ReadAnalogWaveformSamples();

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.Equal(1, results.Values[0]["AIPin"].SampleCount);
            Assert.Equal(1, results.Values[1].Count);
            Assert.Equal(1, results.Values[1]["AIPin"].SampleCount);
        }

        [Fact]
        public void ReadFiveAnalogWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("AIPin");
            var results = tasksBundle.ReadAnalogWaveformSamples(samplesToRead: 5);

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.Equal(5, results.Values[0]["AIPin"].SampleCount);
            Assert.Equal(1, results.Values[1].Count);
            Assert.Equal(5, results.Values[1]["AIPin"].SampleCount);
        }

        [Fact]
        public void ReadU32SamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("DIPin");
            var results = tasksBundle.ReadU32Samples();

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.Single(results.Values[0]["DIPin"]);
            Assert.Equal(1, results.Values[1].Count);
            Assert.Single(results.Values[1]["DIPin"]);
        }

        [Fact]
        public void ReadFiveU32SamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("DIPin");
            var results = tasksBundle.ReadU32Samples(samplesToRead: 5);

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.Equal(5, results.Values[0]["DIPin"].Length);
            Assert.Equal(1, results.Values[1].Count);
            Assert.Equal(5, results.Values[1]["DIPin"].Length);
        }

        [Fact]
        public void ReadDigitalWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("DIPin");
            var results = tasksBundle.ReadDigitalWaveformSamples();

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.Single(results.Values[0]["DIPin"].Samples);
            Assert.Equal(1, results.Values[1].Count);
            Assert.Single(results.Values[1]["DIPin"].Samples);
        }

        [Fact]
        public void ReadFiveDigitalWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("DIPin");
            var results = tasksBundle.ReadDigitalWaveformSamples(samplesToRead: 5);

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.Equal(5, results.Values[0]["DIPin"].Samples.Count);
            Assert.Equal(1, results.Values[1].Count);
            Assert.Equal(5, results.Values[1]["DIPin"].Samples.Count);
        }

        [Fact]
        public void ReadBooleanSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.GetDAQmxTasksBundle("DIPin");
            var results = tasksBundle.ReadBooleanSamples();

            Assert.Equal(2, results.Values.Count);
            Assert.Equal(1, results.Values[0].Count);
            Assert.True(results.Values[0].ContainsKey("DIPin"));
            Assert.Equal(1, results.Values[1].Count);
            Assert.True(results.Values[1].ContainsKey("DIPin"));
        }
    }
}
