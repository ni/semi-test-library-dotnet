using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    public sealed class SetupAndCleanupTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;

        public SetupAndCleanupTests()
        {
            _tsmContext = CreateTSMContext("DAQmxTests.pinmap");
        }

        public void Dispose()
        {
            SetupAndCleanup.ClearDAQmxTasks(_tsmContext);
        }

        [Fact]
        public void CreateDAQmxAITasks_TasksCreatedWithSpecifiedSettings()
        {
            SetupAndCleanup.CreateAndConfigureDAQmxAIVoltageTasks(_tsmContext, sampleClockRate: 2000, samplesPerChannel: 2000);

            var aiTasks = _tsmContext.GetAllNIDAQmxTasks(taskType: "AI");
            Assert.Equal(2, aiTasks.Length);
            Assert.Equal(3, aiTasks[0].AIChannels.Count);
            Assert.Equal(2, aiTasks[1].AIChannels.Count);
            Assert.Equal(-1, aiTasks[0].AIChannels[0].Minimum);
            Assert.Equal(1, aiTasks[0].AIChannels[1].Maximum);
            Assert.Equal(2000, aiTasks[1].Timing.SampleClockRate);
            Assert.Equal(2000, aiTasks[1].Timing.SamplesPerChannel);
        }

        [Fact]
        public void CreateDAQmxAOTasks_TasksCreatedWithSpecifiedSettings()
        {
            SetupAndCleanup.CreateAndConfigureDAQmxAOVoltageTasks(_tsmContext, sampleClockRate: 2000, samplesPerChannel: 2000);

            var aoTasks = _tsmContext.GetAllNIDAQmxTasks(taskType: "AO");
            Assert.Equal(2, aoTasks.Length);
            Assert.Equal(1, aoTasks[0].AOChannels.Count);
            Assert.Equal(1, aoTasks[1].AOChannels.Count);
            Assert.Equal(-1, aoTasks[0].AOChannels[0].Minimum);
            Assert.Equal(1, aoTasks[0].AOChannels[0].Maximum);
            Assert.Equal(2000, aoTasks[1].Timing.SampleClockRate);
            Assert.Equal(2000, aoTasks[1].Timing.SamplesPerChannel);
        }
    }
}
