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
    public sealed class AnalogOutputTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public AnalogOutputTests()
        {
            _tsmContext = CreateTSMContext("DAQmxTests.pinmap");
            _sessionManager = new TSMSessionManager(_tsmContext);
            InitializeAndClose.CreateDAQmxAOVoltageTasks(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxAOVoltageTasks(_tsmContext);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleAnalogSample_WriteSucceeds(bool autoStart)
        {
            double data = 0.5;

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogSingleSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleAnalogSampleWithMatchingPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>> { { "VDD", new Dictionary<int, double> { { 0, 0.6 }, { 1, 0.8 } } } });

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogSingleSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleAnalogSampleWithMissingPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>> { { "VDD", new Dictionary<int, double> { { 1, 0.8 } } } });

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogSingleSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleAnalogSampleWithExtraPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>> { { "VDD", new Dictionary<int, double> { { 0, 0.6 }, { 1, 0.8 }, { 2, 1.0 } } } });

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogSingleSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AnalogWaveformsSamplesWithMatchingPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var site0Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.6, 0.7 });
            var site1Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 });
            var data = new PinSiteData<AnalogWaveform<double>>(new Dictionary<string, IDictionary<int, AnalogWaveform<double>>> { { "VDD", new Dictionary<int, AnalogWaveform<double>> { { 0, site0Data }, { 1, site1Data } } } });

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogWaveform(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AnalogWaveformsSamplesWithMissingPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var site1Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 });
            var data = new PinSiteData<AnalogWaveform<double>>(new Dictionary<string, IDictionary<int, AnalogWaveform<double>>> { { "VDD", new Dictionary<int, AnalogWaveform<double>> { { 1, site1Data } } } });

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogWaveform(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AnalogWaveformsSamplesWithExtraPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var site0Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.6, 0.7 });
            var site1Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 });
            var site2Data = AnalogWaveform<double>.FromArray1D(new double[] { 1.0, 1.1 });
            var data = new PinSiteData<AnalogWaveform<double>>(new Dictionary<string, IDictionary<int, AnalogWaveform<double>>> { { "VDD", new Dictionary<int, AnalogWaveform<double>> { { 0, site0Data }, { 1, site1Data }, { 2, site2Data } } } });

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogWaveform(data, autoStart);
        }
    }
}
