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
    public sealed class DigitalInputTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public DigitalInputTests()
        {
            _tsmContext = CreateTSMContext("DAQmxSingleChannelTests.pinmap");
            _sessionManager = new TSMSessionManager(_tsmContext);
            InitializeAndClose.CreateDAQmxDITasks(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxDITasks(_tsmContext);
        }

        [Fact]
        public void ReadU32SamplesFromOneChannel_ResultsContainExpectedData()
        {
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
            InitializeAndClose.ClearDAQmxDITasks(_tsmContext);
            var tsm = CreateTSMContext("DAQmxSharedPin.pinmap");
            var sessionManager = new TSMSessionManager(tsm);
            InitializeAndClose.CreateDAQmxDITasks(tsm);
            var tasksBundle = sessionManager.DAQmx("DI_PIN1");
            var results = tasksBundle.ReadDigitalMultiSampleU32(samplesToRead: 5);

            Assert.Equal(3, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["DI_PIN1"].Length);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["DI_PIN1"].Length);
            Assert.Equal(5, results.ExtractSite(2)["DI_PIN1"].Length);
            InitializeAndClose.ClearDAQmxDITasks(tsm);
            _tsmContext = CreateTSMContext("DAQmxSingleChannelTests.pinmap");
            InitializeAndClose.CreateDAQmxDITasks(_tsmContext);
        }

        [Fact]
        public void ReadDigitalWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            InitializeAndClose.ClearDAQmxDITasks(_tsmContext);
            var tsm = CreateTSMContext("DAQmxSharedPin.pinmap");
            var sessionManager = new TSMSessionManager(tsm);
            InitializeAndClose.CreateDAQmxDITasks(tsm);
            var tasksBundle = sessionManager.DAQmx("DI_PIN1");
            var results = tasksBundle.ReadDigitalWaveform();

            Assert.Equal(3, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Single(results.ExtractSite(0)["DI_PIN1"].Samples);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(1, results.ExtractSite(2).Count);
            Assert.Single(results.ExtractSite(1)["DI_PIN1"].Samples);
            InitializeAndClose.ClearDAQmxDITasks(tsm);
            _tsmContext = CreateTSMContext("DAQmxSingleChannelTests.pinmap");
            InitializeAndClose.CreateDAQmxDITasks(_tsmContext);
        }

        [Fact]
        public void ReadFiveDigitalWaveformSamplesFromOneChannel_ResultsContainExpectedData()
        {
            var tasksBundle = _sessionManager.DAQmx("DIPin");
            var results = tasksBundle.ReadDigitalWaveform(samplesToRead: 5);

            Assert.Equal(2, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.Equal(5, results.ExtractSite(0)["DIPin"].Samples.Count);
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.Equal(5, results.ExtractSite(1)["DIPin"].Samples.Count);
        }

        [Fact]
        public void ReadBooleanSamplesFromOneChannel_ResultsContainExpectedData()
        {
            InitializeAndClose.ClearDAQmxDITasks(_tsmContext);
            var tsm = CreateTSMContext("DAQmxSharedPin.pinmap");
            var sessionManager = new TSMSessionManager(tsm);
            InitializeAndClose.CreateDAQmxDITasks(tsm);
            var tasksBundle = sessionManager.DAQmx("DI_PIN1");
            var results = tasksBundle.ReadDigitalSingleSample();

            Assert.Equal(3, results.SiteNumbers.Length);
            Assert.Equal(1, results.ExtractSite(0).Count);
            Assert.True(results.ExtractSite(0).ContainsKey("DI_PIN1"));
            Assert.Equal(1, results.ExtractSite(1).Count);
            Assert.True(results.ExtractSite(1).ContainsKey("DI_PIN1"));
            InitializeAndClose.ClearDAQmxDITasks(tsm);
            _tsmContext = CreateTSMContext("DAQmxSingleChannelTests.pinmap");
            InitializeAndClose.CreateDAQmxDITasks(_tsmContext);
        }
    }
}
