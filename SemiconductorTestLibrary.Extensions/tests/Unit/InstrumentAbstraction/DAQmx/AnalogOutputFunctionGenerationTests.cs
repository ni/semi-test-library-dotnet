using System;
using NationalInstruments.DAQmx;
using NationalInstruments.Restricted;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.DAQmx;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.DAQmx
{
    [Collection("NonParallelizable")]
    public sealed class AnalogOutputFunctionGenerationTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.CreateDAQmxAOFunctionGenerationTasks(_tsmContext);
            InitializeAndClose.CreateDAQmxAOVoltageTasks(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxAOFunctionGenerationTasks(_tsmContext);
            InitializeAndClose.ClearDAQmxAOVoltageTasks(_tsmContext);
        }

        [Fact]
        public void SupportedDevice_GenerateSineWaveSucceeds()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("PureTonePin");

            GenerateSineWave(tasksBundle);
        }

        [Fact]
        public void UnsupportedDevice_GenerateSineWaveThrowsException()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VDD");

            var exception = Assert.Throws<AggregateException>(() => GenerateSineWave(tasksBundle));
            exception.IfNotNull(x =>
            {
                foreach (var innerExeption in x.InnerExceptions)
                {
                    Assert.Contains("Specified property is not supported by the device or is not applicable to the task.", innerExeption.InnerException.Message);
                }
            });
        }

        private static void GenerateSineWave(DAQmxTasksBundle tasksBundle)
        {
            tasksBundle.ConfigureAOFunctionGeneration(new AOFunctionGenerationSettings
            {
                FunctionType = AOFunctionGenerationType.Sine,
                Frequency = 1000,
                Amplitude = 1,
                Offset = 0,
            });
            tasksBundle.StartAOFunctionGeneration();
        }
    }
}
