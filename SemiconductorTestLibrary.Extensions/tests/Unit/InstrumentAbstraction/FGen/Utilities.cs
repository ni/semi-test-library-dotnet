using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.FGen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.FGen
{
    [Collection("NonParallelizable")]
    public sealed class Utilities : IDisposable
    {
        private ISemiconductorModuleContext _tsmContext;

        public TSMSessionManager Initialize(string pinMapFileName)
        {
            _tsmContext = CreateTSMContext(pinMapFileName);
            InitializeAndClose.Initialize(_tsmContext);
            return new TSMSessionManager(_tsmContext);
        }

        public void Dispose()
        {
            InitializeAndClose.Close(_tsmContext);
        }

        [Fact]
        public void InitializeBundleWithSinglePin_PerformResetOperation__Succeeds()
        {
            var sessionManager = Initialize("Fgen.pinmap");
            var sessionsBundle = sessionManager.Fgen("Pin1");
            sessionsBundle.Reset();
        }

        [Fact]
        public void InitializeBundleWithSinglePin_PerformResetDeviceOperation_Succeeds()
        {
            var sessionManager = Initialize("Fgen.pinmap");
            var sessionsBundle = sessionManager.Fgen("Pin1");
            sessionsBundle.ResetDevice();
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformResetOperation_Succeeds()
        {
            var sessionManager = Initialize("Fgen.pinmap");
            var sessionsBundle = sessionManager.Fgen("Pin1");
            sessionsBundle.Reset();
        }

        [Fact]
        public void InitializeBundleWithMultiplePin_PerformResetDeviceOperation_Succeeds()
        {
            var sessionManager = Initialize("Fgen.pinmap");
            var sessionsBundle = sessionManager.Fgen("Pin1");
            sessionsBundle.ResetDevice();
        }
    }
}
