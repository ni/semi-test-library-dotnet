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
            InitializeAndClose.ClearDAQmxAOVoltageTasks(_tsmContext);
        }

        [Fact]
        public void SupportedDevice_GenerateSineWave_Succeeds()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("PureTonePin");

            tasksBundle.ConfigureAOFunctionGeneration(new AOFunctionGenerationSettings
            {
                FunctionType = AOFunctionGenerationType.Sine,
                Frequency = 1000,
                Amplitude = 1,
                Offset = 0,
            });
            tasksBundle.StartAOFunctionGeneration();
        }

        [Fact]
        public void UnsupportedDevice_GenerateSineWave_ThrowsException()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VDD");

            void Operation()
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

            var exception = Assert.Throws<AggregateException>(Operation);
            exception.IfNotNull(x =>
            {
                foreach (var innerExeption in x.InnerExceptions)
                {
                    Assert.Contains("Specified property is not supported by the device or is not applicable to the task.", innerExeption.Message);
                }
            });
        }
    }
}
