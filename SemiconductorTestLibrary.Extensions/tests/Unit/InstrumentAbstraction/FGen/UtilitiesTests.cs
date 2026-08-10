using System;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class UtilitiesTests : IDisposable
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
        public void InitializeBundleWithSinglePin_PerformResetOperationSucceeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.Reset();
        }

        [Fact]
        public void InitializeBundleWithSinglePin_PerformResetDeviceOperationSucceeds()
        {
            var sessionManager = Initialize("FgenSingleInstrumentPerPin.pinmap");
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.ResetDevice();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformResetOperationSucceeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            sessionsBundle.Reset();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformResetDeviceOperationSucceeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new string[] { "A", "B" });

            sessionsBundle.ResetDevice();
        }
    }
}