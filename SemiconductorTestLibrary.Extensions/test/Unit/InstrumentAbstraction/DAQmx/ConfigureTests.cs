using System;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    public sealed class ConfigureTests : IDisposable
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
        public void ConfigureTimingAndGetSampleClockRate_ReturnsCorrectValue()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");

            DAQmxTimingSampleClockSettings timingSettings = new DAQmxTimingSampleClockSettings
            {
                SampleClockRate = 5555,
                SampleQuantityMode = SampleQuantityMode.FiniteSamples,
                SamplesPerChannel = 1000
            };
            tasksBundle.ConfigureTiming(timingSettings);

            var sampleClockRateActual = tasksBundle.GetSampleClockRateDistinct();
            Assert.Equal(5555, sampleClockRateActual, 0);
        }
    }
}
