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
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName = "")
        {
            _tsmContext = CreateTSMContext(string.IsNullOrEmpty(pinMapFileName) ? "DAQmxTests.pinmap" : pinMapFileName);
            InitializeAndClose.CreateDAQmxAOVoltageTasks(_tsmContext);
            return new TSMSessionManager(_tsmContext);
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
            var sessionManager = Initialize();
            double data = 0.5;

            var tasksBundle = sessionManager.DAQmx("VDD");
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
            var sessionManager = Initialize();
            var data = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>> { { "VDD", new Dictionary<int, double> { { 0, 0.6 }, { 1, 0.8 } } } });

            var tasksBundle = sessionManager.DAQmx("VDD");
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
            var sessionManager = Initialize("DAQmxSharedPin.pinmap");
            var data = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>> { { "AO_PIN1", new Dictionary<int, double> { { 1, 0.8 } } } });

            var tasksBundle = sessionManager.DAQmx("AO_PIN1");
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
            var sessionManager = Initialize();
            var data = new PinSiteData<double>(new Dictionary<string, IDictionary<int, double>> { { "VDD", new Dictionary<int, double> { { 0, 0.6 }, { 1, 0.8 }, { 2, 1.0 } } } });

            var tasksBundle = sessionManager.DAQmx("VDD");
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
            var sessionManager = Initialize("DAQmxSharedPin.pinmap");
            var site0Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.6, 0.7 });
            var site1Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 });
            var data = new PinSiteData<AnalogWaveform<double>>(new Dictionary<string, IDictionary<int, AnalogWaveform<double>>> { { "AO_PIN1", new Dictionary<int, AnalogWaveform<double>> { { 0, site0Data }, { 1, site1Data } } } });

            var tasksBundle = sessionManager.DAQmx("AO_PIN1");
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
            var sessionManager = Initialize();
            var site1Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 });
            var data = new PinSiteData<AnalogWaveform<double>>(new Dictionary<string, IDictionary<int, AnalogWaveform<double>>> { { "VDD", new Dictionary<int, AnalogWaveform<double>> { { 1, site1Data } } } });

            var tasksBundle = sessionManager.DAQmx("VDD");
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
            var sessionManager = Initialize();
            var site0Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.6, 0.7 });
            var site1Data = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 });
            var site2Data = AnalogWaveform<double>.FromArray1D(new double[] { 1.0, 1.1 });
            var data = new PinSiteData<AnalogWaveform<double>>(new Dictionary<string, IDictionary<int, AnalogWaveform<double>>> { { "VDD", new Dictionary<int, AnalogWaveform<double>> { { 0, site0Data }, { 1, site1Data }, { 2, site2Data } } } });

            var tasksBundle = sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogWaveform(data, autoStart);
        }

        [Fact(Skip = "Fails without the fix in Abstractions.")]
        public void PinMapWithMismatchChannelListOrder_WriteSucceeds()
        {
            var sessionManager = Initialize("DAQmxChannelListOrderTests.pinmap");
            var data = 0.5;

            var tasksBundle = sessionManager.DAQmx("AOPin");
            tasksBundle.WriteAnalogSingleSample(data);
        }
    }
}
