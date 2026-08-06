using System;
using NationalInstruments.SemiconductorTestLibrary.Common;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction;
using NationalInstruments.SemiconductorTestLibrary.InstrumentAbstraction.Fgen;
using NationalInstruments.TestStand.SemiconductorModule.CodeModuleAPI;
using Xunit;
using static NationalInstruments.Tests.SemiconductorTestLibrary.Utilities.TSMContext;

namespace NationalInstruments.Tests.SemiconductorTestLibrary.Unit.InstrumentAbstraction.Fgen
{
    [Collection("NonParallelizable")]
    public sealed class ControlTests : IDisposable
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

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        public void InitializeBundleWithSinglePin_PerformInitiateOperation_ThrowsExpectedException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.Initiate());

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.NIFgen.InitiateGeneration()", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074118636", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("No waveforms have been created", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformInitiateOperation_ThrowsExpectedException(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            var exception = Assert.Throws<NISemiconductorTestException>(() => sessionsBundle.Initiate());

            Assert.Contains("at NationalInstruments.ModularInstruments.NIFgen.NIFgen.InitiateGeneration()", exception.Message); // Ensure that correct driver method call is reported in the exception message.
            Assert.Contains("Error code: -1074118636", exception.Message); // Ensure correct error code is reported in the exception message.
            Assert.Contains("No waveforms have been created", exception.Message); // Ensure correct error message is reported in the exception message.
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        public void InitializeBundleWithSinglePin_PerformCommitOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.Commit();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformCommitOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            sessionsBundle.Commit();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        public void InitializeBundleWithSinglePin_PerformAbortOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.Abort();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformAbortOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            sessionsBundle.Abort();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        public void InitializeBundleWithSinglePin_PerformIsDoneOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.IsDone();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformIsDoneOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            sessionsBundle.IsDone();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        public void InitializeBundleWithSinglePin_PerformWaitUntilDoneOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen("A");

            sessionsBundle.WaitUntilDone();
        }

        [Theory]
        [InlineData("FgenSingleInstrumentPerPin.pinmap")]
        [InlineData("FgenSingleInstrumentPerSite.pinmap")]
        public void InitializeBundleWithMultiplePins_PerformWaitUntilDoneOperation_Succeeds(string pinmap)
        {
            var sessionManager = Initialize(pinmap);
            var sessionsBundle = sessionManager.Fgen(new[] { "A", "B" });

            sessionsBundle.WaitUntilDone();
        }
    }
}
