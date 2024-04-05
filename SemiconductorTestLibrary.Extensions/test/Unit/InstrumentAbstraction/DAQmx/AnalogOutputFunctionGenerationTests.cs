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
    public sealed class AnalogOutputFunctionGenerationTests : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.CreateDAQmxAOFunctionGenerationTasks(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.ClearDAQmxAOVoltageTasks(_tsmContext);
        }

        [Fact]
        public void SingleChannelSingleDevice_GenerateSineWave_Succeeds()
        {
            var sessionManager = Initialize("DAQmxTests.pinmap");
            var tasksBundle = sessionManager.DAQmx("VCC1");

            tasksBundle.Stop();
            tasksBundle.Unreserve();
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
