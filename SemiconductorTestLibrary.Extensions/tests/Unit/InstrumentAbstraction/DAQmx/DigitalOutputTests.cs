using System;
using System.Collections.Generic;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.DataAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.Tests.SemiconductorTestLibrary.Utilities;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.SemiconductorTestLibrary.Common.ParallelExecution;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    [Trait(nameof(HardwareConfiguration), nameof(HardwareConfiguration.GP3))]
    public sealed class DigitalOutputTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName = "")
        {
            _tsmContext = CreateTSMContext(string.IsNullOrEmpty(pinMapFileName) ? "DAQmxTests.pinmap" : pinMapFileName);
            InitializeAndClose.CreateDAQmxDOTasks(_tsmContext);
            return new TSMSessionManager(_tsmContext);
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
            var sessionManager = Initialize();
            bool data = true;

            var tasksBundle = sessionManager.DAQmx("DOPin");
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
            var sessionManager = Initialize("DAQmxSharedPin.pinmap");
            var data = new PinSiteData<bool>(new Dictionary<string, IDictionary<int, bool>> { { "DO_PIN1", new Dictionary<int, bool> { { 0, true }, { 1, false } } } });

            var tasksBundle = sessionManager.DAQmx("DO_PIN1");
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
            var sessionManager = Initialize("DAQmxSharedPin.pinmap");
            var site0Data = DigitalWaveform.FromPort(new byte[] { 6 }, mask: 8);
            var site1Data = DigitalWaveform.FromPort(new byte[] { 8 }, mask: 8);
            var data = new PinSiteData<DigitalWaveform>(new Dictionary<string, IDictionary<int, DigitalWaveform>> { { "DO_PIN1", new Dictionary<int, DigitalWaveform> { { 0, site0Data }, { 1, site1Data } } } });

            var tasksBundle = sessionManager.DAQmx("DO_PIN1");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteDigitalWaveform(data, autoStart);
        }

        [Theory(Skip = "Fails without the fix in Abstractions.")]
        [InlineData("DOPin1")]
        [InlineData("DOPin2")]
        [InlineData("DOPin3")]
        public void PinMapWithMismatchChannelListOrder_WriteSucceeds(string pinName)
        {
            var sessionManager = Initialize("DAQmxChannelListOrderTests.pinmap");
            var data = true;

            var tasksBundle = sessionManager.DAQmx(pinName);
            tasksBundle.WriteDigital(data);
        }
    }
}
