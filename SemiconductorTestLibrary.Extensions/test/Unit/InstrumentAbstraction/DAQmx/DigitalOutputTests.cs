using System;
using System.Collections.Generic;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    public sealed class DigitalOutputTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public DigitalOutputTests()
        {
            _tsmContext = CreateTSMContext("DAQmxTests.pinmap");
            _sessionManager = new TSMSessionManager(_tsmContext);
            InitializeAndClose.CreateDAQmxDOTasks(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxDOTasks(_tsmContext);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleBooleanDigitalSample_WriteSucceeds(bool autoStart)
        {
            bool data = true;

            var tasksBundle = _sessionManager.DAQmx("DOPin");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteDigital(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SinglePerSitePerPinBooleanDigitalSample_WriteSucceeds(bool autoStart)
        {
            var data = new PinSiteData<bool>(new Dictionary<string, IDictionary<int, bool>> { { "DOPin", new Dictionary<int, bool> { { 0, true }, { 1, false } } } });

            var tasksBundle = _sessionManager.DAQmx("DOPin");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteDigital(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DigitalWaveformSamples_WriteSucceeds(bool autoStart)
        {
            var site0Data = DigitalWaveform.FromPort(new byte[] { 6 }, mask: 8);
            var site1Data = DigitalWaveform.FromPort(new byte[] { 8 }, mask: 8);
            var data = new PinSiteData<DigitalWaveform>(new Dictionary<string, IDictionary<int, DigitalWaveform>> { { "DOPin", new Dictionary<int, DigitalWaveform> { { 0, site0Data }, { 1, site1Data } } } });

            var tasksBundle = _sessionManager.DAQmx("DOPin");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteDigitalWaveform(data, autoStart);
        }
    }
}
