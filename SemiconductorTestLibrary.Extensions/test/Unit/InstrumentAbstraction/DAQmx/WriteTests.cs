using System;
using System.Collections.Generic;
using NationalInstruments.DAQmx;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    public sealed class WriteTests : IDisposable
    {
        private readonly ISemiconductorModuleContext _tsmContext;
        private readonly TSMSessionManager _sessionManager;

        public WriteTests()
        {
            _tsmContext = CreateTSMContext("DAQmxTests.pinmap");
            _sessionManager = new TSMSessionManager(_tsmContext);
            SetupAndCleanup.CreateDAQmxTasks(_tsmContext);
        }

        public void Dispose()
        {
            SetupAndCleanup.ClearDAQmxTasks(_tsmContext);
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
            tasksBundle.WriteSingleAnalogSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleAnalogSampleWithMatchingPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, double>>()
            {
                [0] = new Dictionary<string, double>() { ["VDD"] = 0.6 },
                [1] = new Dictionary<string, double>() { ["VDD"] = 0.8 }
            };

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteSingleAnalogSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleAnalogSampleWithMissingPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, double>>()
            {
                [1] = new Dictionary<string, double>() { ["VDD"] = 0.8 }
            };

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteSingleAnalogSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleAnalogSampleWithExtraPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, double>>()
            {
                [0] = new Dictionary<string, double>() { ["VDD"] = 0.6 },
                [1] = new Dictionary<string, double>() { ["VDD"] = 0.8 },
                [2] = new Dictionary<string, double>() { ["VDD"] = 1.0 },
            };

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteSingleAnalogSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AnalogWaveformsSamplesWithMatchingPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, AnalogWaveform<double>>>()
            {
                [0] = new Dictionary<string, AnalogWaveform<double>>() { ["VDD"] = AnalogWaveform<double>.FromArray1D(new double[] { 0.6, 0.7 }) },
                [1] = new Dictionary<string, AnalogWaveform<double>>() { ["VDD"] = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 }) }
            };

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogWaveformsSamples(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AnalogWaveformsSamplesWithMissingPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, AnalogWaveform<double>>>()
            {
                [1] = new Dictionary<string, AnalogWaveform<double>>() { ["VDD"] = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 }) }
            };

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogWaveformsSamples(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AnalogWaveformsSamplesWithExtraPerSitePerPinData_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, AnalogWaveform<double>>>()
            {
                [0] = new Dictionary<string, AnalogWaveform<double>>() { ["VDD"] = AnalogWaveform<double>.FromArray1D(new double[] { 0.6, 0.7 }) },
                [1] = new Dictionary<string, AnalogWaveform<double>>() { ["VDD"] = AnalogWaveform<double>.FromArray1D(new double[] { 0.8, 0.9 }) },
                [2] = new Dictionary<string, AnalogWaveform<double>>() { ["VDD"] = AnalogWaveform<double>.FromArray1D(new double[] { 1.0, 1.1 }) }
            };

            var tasksBundle = _sessionManager.DAQmx("VDD");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteAnalogWaveformsSamples(data, autoStart);
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
            tasksBundle.WriteSingleDigitalSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SingleUIntDigitalSample_WriteSucceeds(bool autoStart)
        {
            uint data = 5;

            var tasksBundle = _sessionManager.DAQmx("DOPin");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteSingleDigitalSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SinglePerSitePerPinBooleanDigitalSample_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, bool>>()
            {
                [0] = new Dictionary<string, bool>() { ["DOPin"] = true },
                [1] = new Dictionary<string, bool>() { ["DOPin"] = false }
            };

            var tasksBundle = _sessionManager.DAQmx("DOPin");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteSingleDigitalSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void SinglePerSitePerPinUIntDigitalSample_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, uint>>()
            {
                [0] = new Dictionary<string, uint>() { ["DOPin"] = 6 },
                [1] = new Dictionary<string, uint>() { ["DOPin"] = 8 }
            };

            var tasksBundle = _sessionManager.DAQmx("DOPin");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteSingleDigitalSample(data, autoStart);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void DigitalWaveformSamples_WriteSucceeds(bool autoStart)
        {
            var data = new Dictionary<int, Dictionary<string, DigitalWaveform>>()
            {
                [0] = new Dictionary<string, DigitalWaveform>() { ["DOPin"] = DigitalWaveform.FromPort(new byte[] { 6 }, mask: 8) },
                [1] = new Dictionary<string, DigitalWaveform>() { ["DOPin"] = DigitalWaveform.FromPort(new byte[] { 8 }, mask: 8) }
            };

            var tasksBundle = _sessionManager.DAQmx("DOPin");
            if (!autoStart)
            {
                tasksBundle.Do(taskInfo => taskInfo.Task.Control(TaskAction.Start));
            }
            tasksBundle.WriteDigitalWaveformSamples(data, autoStart);
        }
    }
}
